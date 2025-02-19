using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Dtos;
using RuokaAPI.Properties.Model;

namespace RuokaAPI.Repositories
{   // This repository handles all the database operations related to recipes (Resepti),
    // including fetching, adding, updating, deleting recipes, and adding reviews (Arvostelu) to recipes.
    public class ReseptiRepository
    {
        private readonly ruokaContext _konteksti;

        public ReseptiRepository(ruokaContext konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<ReseptiRequest>> HaeReseptitAsync(string[]? ainesosat, string[]? avainsanat)
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

            // Project the filtered recipes into ReseptiRequest DTOs and execute the query asynchronously
            return await query
                .Select(r => new ReseptiRequest
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
                    Avainsanat = r.Avainsanat.Select(a => a.Sana).ToArray()
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
                    Arvostelut = r.Arvostelut.ToArray()

                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ReseptiResponse>> HaeReseptitKayttajalleAsync ( int userId )
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
                    Arvostelut = r.Arvostelut.ToArray()
                    })
                .ToListAsync();
            }


        // A helper function that processes a list of new names.
        // Removes duplicates from the provided list of new keyword names by checking against existing entities in the
        // dictionary. If a keyword name exists in the dictionary, it uses the existing entity;
        // otherwise, it creates a new entity.
        private async Task<List<Ainesosa>> PoistaDuplikaatit(List<string> uudetNimet, Dictionary<string, Ainesosa> olemassaOlevat)
        {
            var kasitellyt = new List<Ainesosa>();
            foreach (var nimi in uudetNimet)
            {
                if (olemassaOlevat.TryGetValue(nimi, out var olemassaOleva))
                {
                    kasitellyt.Add(olemassaOleva);
                }
                else
                {
                    kasitellyt.Add(new Ainesosa { Nimi = nimi });
                }
            }
            return kasitellyt;
        }

        private async Task<List<Avainsana>> PoistaDuplikaatit(List<string> uudetSanat, Dictionary<string, Avainsana> olemassaOlevat)
        {
            var kasitellyt = new List<Avainsana>();
            foreach (var sana in uudetSanat)
            {
                if (olemassaOlevat.TryGetValue(sana, out var olemassaOleva))
                {
                    kasitellyt.Add(olemassaOleva);
                }
                else
                {
                    kasitellyt.Add(new Avainsana { Sana = sana });
                }
            }
            return kasitellyt;
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
            return await PoistaDuplikaatit(uudetAinesosat, olemassaOlevatAinesosat);
        }

        private async Task<List<Avainsana>> PoistaDuplikaattiAvainsanat(ReseptiRequest resepti)
        {
            var uudetAvainsanat = resepti.Avainsanat.Select(a => a.ToLower()).ToList();
            var olemassaOlevatAvainsanat = await _konteksti.Avainsanat
                .Where(x => uudetAvainsanat.Contains(x.Sana.ToLower()))
                .ToDictionaryAsync(x => x.Sana.ToLower());
            return await PoistaDuplikaatit(uudetAvainsanat, olemassaOlevatAvainsanat);
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
            return await PoistaDuplikaatit(uudetNimet, olemassaOlevat);
        }

        // Converts the provided array of keyword names to lowercase, fetches existing keywords from the database,
        // and ensures there are no duplicates by calling PoistaDuplikaatit.
        private async Task<List<Avainsana>> MuunnaAvainsanat(string[] avainsanaNimet)
        {
            // Convert the keyword names to lowercase
            var uudetSanat = avainsanaNimet.Select(a => a.ToLower()).ToList();

            // Fetch existing keywords from the database that match the names
            var olemassaOlevat = await _konteksti.Avainsanat
                .Where(x => uudetSanat.Contains(x.Sana.ToLower()))
                .ToDictionaryAsync(x => x.Sana.ToLower());

            // Call PoistaDuplikaatit to process the list of keyword names and ensure there are no duplicates
            return await PoistaDuplikaatit(uudetSanat, olemassaOlevat);
        }

        public async Task<Resepti> LisaaAsync(ReseptiRequest reseptiDto)
        {
            var ainesosat = await MuunnaAinesosat(reseptiDto.Ainesosat.Select(a => a.Ainesosa).ToArray());
            var resepti = new Resepti
            {
                Tekijäid = reseptiDto.TekijaId,
                Nimi = reseptiDto.Nimi,
                Valmistuskuvaus = reseptiDto.Valmistuskuvaus,
                Kuva1 = reseptiDto.Kuva1,
                Katseluoikeus = reseptiDto.Katseluoikeus,                
                Avainsanat = await MuunnaAvainsanat(reseptiDto.Avainsanat)
            };

            foreach (var ainesosa in reseptiDto.Ainesosat)
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
                .Include(r => r.AinesosanMaara)
                .Include(r => r.Avainsanat)
                .FirstOrDefault(r => r.Id == id);
            if (resepti == null) return;

            var ainesosat = await PoistaDuplikaattiAinesosat(reseptiRequest);
            resepti.Avainsanat = await PoistaDuplikaattiAvainsanat(reseptiRequest);            
            resepti.Katseluoikeus = reseptiRequest.Katseluoikeus;
            resepti.Valmistuskuvaus = reseptiRequest?.Valmistuskuvaus;
            resepti.Tekijäid = reseptiRequest.TekijaId;
            resepti.Kuva1 = reseptiRequest.Kuva1;
            resepti.Nimi = reseptiRequest.Nimi;

            foreach (var ainesosaDto in reseptiRequest.Ainesosat)
            {
                var reseptiAinesosa = resepti.AinesosanMaara
                    .FirstOrDefault(ra => ra.Ainesosa.Nimi is not null 
                            && ra.Ainesosa.Nimi.Equals(ainesosaDto.Ainesosa, StringComparison.OrdinalIgnoreCase));
                if (reseptiAinesosa != null)
                {
                    reseptiAinesosa.Maara = ainesosaDto.Maara;
                }
                else
                {
                    resepti.AinesosanMaara.Add(new ReseptiAinesosa
                    {
                        Ainesosa = ainesosat.First(a => a.Nimi == ainesosaDto.Ainesosa),
                        Maara = ainesosaDto.Maara
                    });
                }
            }

            await _konteksti.SaveChangesAsync();
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
    }
}
