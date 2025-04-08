using Microsoft.EntityFrameworkCore;
using FoodAPI.Data;
using FoodAPI.Dtos;
using FoodAPI.Properties.Model;

namespace FoodAPI.Repositories
{   // This repository handles all the database operations related to recipes (Resepti),
    // including fetching, adding, updating, deleting recipes, and adding reviews (Arvostelu) to recipes.
    public class ReseptiRepository
    {
        private readonly FoodContext _konteksti;

        public ReseptiRepository(FoodContext konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<ReseptiResponse>> HaeReseptitAsync(string[]? ainesosat, string[]? avainsanat, bool haeVainJulkiset)
        {
            // Start building the query to fetch recipes from the database
            var query = _konteksti.Reseptit
                // Include related AinesosanMaara and then include related Ainesosa
                .Include(r => r.AinesosanMaara).ThenInclude(ra => ra.Ainesosa)
                // Include related Avainsanat
                .Include(r => r.Avainsanat)
                // Convert to IQueryable to enable further query modifications
                .AsQueryable();


            // If ainesosat (ingredients) are provided, filter recipes that contain any of the specified ingredients
            if (ainesosat != null && ainesosat.Length > 0)
            {
                var haettavatAinesosat = ainesosat.ToList();
                query = query.Where(r => r.AinesosanMaara
                    .Any(a => haettavatAinesosat.Contains(a.Ainesosa.Nimi)));
            }

            // If avainsanat (keywords) are provided, filter recipes that contain any of the specified keywords
            if (avainsanat != null && avainsanat.Length > 0)
            {
                var haettavatAvainsanat = avainsanat.ToList();
                query = query.Where(r => r.Avainsanat
                    .Any(a => haettavatAvainsanat.Contains(a.Sana)));
            }

            if (haeVainJulkiset)
            {
                query = query.Where(r => r.Katseluoikeus == "julkinen");
            }
            // Project the filtered recipes into ReseptiRequest DTOs and execute the query asynchronously

            return await query
            .Select(r => new ReseptiResponse
            {
                Id = r.Id,
                Nimi = r.Nimi,
                Valmistuskuvaus = r.Valmistuskuvaus,
                Kuva1 = r.Kuva1,
                Katseluoikeus = r.Katseluoikeus,
                Ainesosat = r.AinesosanMaara.Select(am => new AinesosanMaaraDto
                {
                    Ainesosa = am.Ainesosa.Nimi,
                    Maara = am.Maara
                }).ToArray(),
                Avainsanat = r.Avainsanat.Select(a => a.Sana).ToArray(),
                Arvostelut = r.Arvostelut.ToArray(),
            })
            .ToListAsync();
        }

        public async Task<ReseptiResponse?> HaeReseptiAsync(int id)
        {
            return await _konteksti.Reseptit
                .Include(r => r.Arvostelut)
                .Include(r => r.AinesosanMaara)
                .Include(r => r.Avainsanat)
                .Where(r => r.Id == id)
                .Select(r => new ReseptiResponse
                {
                    Id = r.Id,
                    Nimi = r.Nimi,
                    Valmistuskuvaus = r.Valmistuskuvaus,
                    Kuva1 = r.Kuva1,
                    Katseluoikeus = r.Katseluoikeus,
                    Ainesosat = r.AinesosanMaara.Select(am => new AinesosanMaaraDto
                    {
                        Ainesosa = am.Ainesosa.Nimi,
                        Maara = am.Maara
                    }).ToArray(),
                    Avainsanat = r.Avainsanat.Select(a => a.Sana).ToArray(),
                    Arvostelut = r.Arvostelut.ToArray(),
                    TekijaId = r.Tekijäid

                })
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<ReseptiResponse>> HaeReseptitKayttajalleAsync(int userId)
        {
            return await _konteksti.Reseptit
                .Where(r => r.Tekijäid == userId)
                .Select(r => new ReseptiResponse
                {
                    Id = r.Id,
                    Nimi = r.Nimi,
                    Valmistuskuvaus = r.Valmistuskuvaus,
                    Kuva1 = r.Kuva1,
                    Katseluoikeus = r.Katseluoikeus,
                    Ainesosat = r.AinesosanMaara.Select(am => new AinesosanMaaraDto
                    {
                        Ainesosa = am.Ainesosa.Nimi,
                        Maara = am.Maara
                    }).ToArray(),
                    Avainsanat = r.Avainsanat.Select(a => a.Sana).ToArray(),
                    Arvostelut = r.Arvostelut.ToArray(),
                    TekijaId = r.Tekijäid
                })
                .ToListAsync();
        }


        // A helper function that processes a list of new names.
        // Removes duplicates from the provided list of new keyword names by checking against existing entities in the
        // dictionary. If a keyword name exists in the dictionary, it uses the existing entity;
        // otherwise, it creates a new entity.
        private async Task<List<Ainesosa>> HaeTaiLuoUusiAinesosa(List<string> ainesosaNimet, Dictionary<string, Ainesosa> olemassaOlevatAinesosat)
        {
            var uusiAinesosalista = new List<Ainesosa>();
            foreach (var nimi in ainesosaNimet)
            {
                if (olemassaOlevatAinesosat.TryGetValue(nimi, out var olemassaOleva))
                {
                    uusiAinesosalista.Add(olemassaOleva);
                }
                else
                {
                    uusiAinesosalista.Add(new Ainesosa { Nimi = nimi });
                }
            }
            return uusiAinesosalista;
        }



        // This method processes the ingredients of a given recipe request to ensure there are no duplicate ingredients.
        // It converts the ingredient names to lowercase, fetches existing ingredients from the database, and then calls PoistaDuplikaatit to handle duplicates.
        private async Task<List<Ainesosa>> PoistaDuplikaattiAinesosat(ReseptiRequest resepti)
        {
            // Convert the ingredient names in the recipe request to lowercase
            var uudetAinesosat = resepti.Ainesosat.Select(a => a.Ainesosa.ToLower()).ToList();

            // Fetch existing ingredients from the database that match the names in the recipe request
            var olemassaOlevatAinesosat = await _konteksti.Ainesosat
                .Where(x => uudetAinesosat.Contains(x.Nimi.ToLower()))
                .ToDictionaryAsync(x => x.Nimi.ToLower());

            // Call PoistaDuplikaatit to process the list of ingredient names and ensure there are no duplicates
            return await HaeTaiLuoUusiAinesosa(uudetAinesosat, olemassaOlevatAinesosat);
        }


        // Converts the provided array of ingredient names to lowercase, fetches existing ingredients from the database,
        // and ensures there are no duplicates by calling PoistaDuplikaatit.
        private async Task<List<Ainesosa>> MuunnaAinesosat(string[] ainesosatNimet)
        {
            // Convert the ingredient names to lowercase
            var uudetNimet = ainesosatNimet.Select(a => a.ToLower()).ToList();

            // Fetch existing ingredients from the database that match the names
            var olemassaOlevat = await _konteksti.Ainesosat
                .Where(x => uudetNimet.Contains(x.Nimi.ToLower()))
                .ToDictionaryAsync(x => x.Nimi.ToLower());

            // Call PoistaDuplikaatit to process the list of ingredient names and ensure there are no duplicates
            return await HaeTaiLuoUusiAinesosa(uudetNimet, olemassaOlevat);
        }

        public async Task<Resepti> LisaaAsync(ReseptiRequest reseptiRequest)
        {
            var ainesosat = await MuunnaAinesosat(reseptiRequest.Ainesosat.Select(a => a.Ainesosa).ToArray());
            var avainsanat = _konteksti.Avainsanat
                .Where(a => reseptiRequest.Avainsanat.Contains(a.Sana))
                .ToList();
            var resepti = new Resepti
            {
                Tekijäid = reseptiRequest.TekijaId,
                Nimi = reseptiRequest.Nimi,
                Valmistuskuvaus = reseptiRequest.Valmistuskuvaus,
                Kuva1 = reseptiRequest.Kuva1,
                Katseluoikeus = reseptiRequest.Katseluoikeus,
                Avainsanat = avainsanat
            };

            foreach (var ainesosa in reseptiRequest.Ainesosat)
            {
                resepti.AinesosanMaara.Add(new ReseptiAinesosa
                {
                    Ainesosa = ainesosat.First(a => a.Nimi is not null && a.Nimi.Equals(ainesosa.Ainesosa, StringComparison.OrdinalIgnoreCase)),
                    Maara = ainesosa.Maara
                });
            }

            _konteksti.Reseptit.Add(resepti);
            await _konteksti.SaveChangesAsync();
            return resepti;
        }

        public async Task PaivitaAsync(int id, ReseptiRequest reseptiRequest)
        {
            var resepti = _konteksti.Reseptit
                .Include(r => r.AinesosanMaara).ThenInclude(r => r.Ainesosa)
                .Include(r => r.Avainsanat)
                .FirstOrDefault(r => r.Id == id);
            if (resepti == null) return;

            var ainesosat = await PoistaDuplikaattiAinesosat(reseptiRequest);
            resepti.Avainsanat = _konteksti.Avainsanat
                .Where(a => reseptiRequest.Avainsanat.Contains(a.Sana))
                .ToList();
            resepti.Katseluoikeus = reseptiRequest.Katseluoikeus;
            resepti.Valmistuskuvaus = reseptiRequest?.Valmistuskuvaus;
            resepti.Tekijäid = reseptiRequest.TekijaId;
            resepti.Kuva1 = reseptiRequest.Kuva1;
            resepti.Nimi = reseptiRequest.Nimi;
            PoistaAinesosatJoitaEiEnaaKayteta(reseptiRequest, resepti);
            PaivitaAinesosa(reseptiRequest, resepti, ainesosat);
            await _konteksti.SaveChangesAsync();
        }

        private static void PaivitaAinesosa(ReseptiRequest reseptiRequest, Resepti? resepti, List<Ainesosa> ainesosat)
        {
            foreach (var ainesosa in reseptiRequest.Ainesosat)
            {
                var onkoAineosaValmiiksiOlemassa = resepti.AinesosanMaara
                    .FirstOrDefault(ra => ra.Ainesosa.Nimi is not null
                            && ra.Ainesosa.Nimi.Equals(ainesosa.Ainesosa, StringComparison.OrdinalIgnoreCase));
                if (onkoAineosaValmiiksiOlemassa != null)
                {
                    onkoAineosaValmiiksiOlemassa.Maara = ainesosa.Maara;
                }
                else
                {
                    
                    var existingAinesosa = ainesosat.FirstOrDefault(a => a.Nimi.Equals(ainesosa.Ainesosa, StringComparison.OrdinalIgnoreCase));
                    if (existingAinesosa != null)
                    {
                        resepti.AinesosanMaara.Add(new ReseptiAinesosa
                        {
                            Ainesosa = existingAinesosa,
                            AinesosaId = existingAinesosa.Id,  // Asetetaan eksplisiittisesti vierasavain
                            Maara = ainesosa.Maara
                        });
                    }

                }
            }
        }

        private void PoistaAinesosatJoitaEiEnaaKayteta(ReseptiRequest reseptiRequest, Resepti? resepti)
        {
            var ainesosatPyynnossa = reseptiRequest.Ainesosat.Select(a => a.Ainesosa).ToArray();
            var poistettavatAinesosat = resepti.AinesosanMaara //reseptin kautta haetaan reseptin ainesosamäärät
                .Select(am => am.Ainesosa)//haetaan pelkät ainesosat ainesosataulusta
                .Where(a => a.AinesosanMaara.Count == 1 && !ainesosatPyynnossa.Contains(a.Nimi))//suodattaa listaa ehtojen perusteella
                .ToList();
            poistettavatAinesosat.ForEach(ainesosa => _konteksti.Ainesosat.Remove(ainesosa));
        }

        public async Task PoistaAsync(int id)
        {
            var resepti = await _konteksti.Reseptit.FindAsync(id);
            if (resepti != null)
            {
                _konteksti.Reseptit.Remove(resepti);
                await _konteksti.SaveChangesAsync();
            }
        }

        public async Task<bool> OnOlemassaAsync(int id)
        {
            return await _konteksti.Reseptit.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> LisaaArvosteluAsync(int reseptiId, ArvosteluRequest request)
        {
            var resepti = _konteksti.Find<Resepti>(reseptiId);
            if (resepti == null) return false;

            var arvostelu = new Arvostelu
            {
                ArvostelijanNimimerkki = request.ArvostelijanNimimerkki,
                ArvostelijanId = request.ArvostelijanId,
                Numeroarvostelu = request.Numeroarvostelu,
                Vapaateksti = request.Vapaateksti
            };
            resepti.Arvostelut.Add(arvostelu);
            await _konteksti.SaveChangesAsync();
            return true;
        }

        public async Task<List<Ainesosa>> HaeAinesosatAsync()
        {
            return await _konteksti.Ainesosat.ToListAsync();
        }
        public async Task<List<Avainsana>> HaeAvainsanatAsync()
        {
            return await _konteksti.Avainsanat.ToListAsync();
        }
    }
}
