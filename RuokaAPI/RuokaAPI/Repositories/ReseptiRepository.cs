using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Dtos;
using RuokaAPI.Properties.Model;

namespace RuokaAPI.Repositories
{
    public class ReseptiRepository
    {
        private readonly ruokaContext _konteksti;

        public ReseptiRepository(ruokaContext konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<ReseptiDto>> HaeReseptitAsync(string[]? ainesosat, string[]? avainsanat)
        {
            var query = _konteksti.Reseptit
                .Include(r => r.AinesosanMaara).ThenInclude(ra => ra.Ainesosa)
                .Include(r => r.Avainsanat)
                .AsQueryable();

            if (ainesosat != null && ainesosat.Length > 0)
            {
                var haettavatAinesosat = ainesosat.ToList();
                query = query.Where(r => r.AinesosanMaara
                    .Any(a => haettavatAinesosat.Contains(a.Ainesosa.Nimi)));
            }

            if (avainsanat != null && avainsanat.Length > 0)
            {
                var haettavatAvainsanat = avainsanat.ToList();
                query = query.Where(r => r.Avainsanat
                    .Any(a => haettavatAvainsanat.Contains(a.Sana)));
            }

            return await query
                .Select(r => new ReseptiDto
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

        public async Task<ReseptiDto?> HaeReseptiAsync(int id)
        {
            return await _konteksti.Reseptit
                .Include(r => r.Arvostelut)
                .Include(r => r.AinesosanMaara)
                .Include(r => r.Avainsanat)
                .Where(r => r.Id == id)
                .Select(r => new ReseptiDto
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
                .FirstOrDefaultAsync();
        }


        // hakee kannasta olemassa olevat ainesosat ja luo uudet ainesosat listaan
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

        private async Task<List<Ainesosa>> PoistaDuplikaattiAinesosat(ReseptiDto resepti)
        {
            var uudetAinesosat = resepti.Ainesosat.Select(a => a.Ainesosa.ToLower()).ToList();
            var olemassaOlevatAinesosat = await _konteksti.Ainesosat
                .Where(x => uudetAinesosat.Contains(x.Nimi.ToLower()))
                .ToDictionaryAsync(x => x.Nimi.ToLower());
            return await PoistaDuplikaatit(uudetAinesosat, olemassaOlevatAinesosat);
        }

        private async Task<List<Avainsana>> PoistaDuplikaattiAvainsanat(ReseptiDto resepti)
        {
            var uudetAvainsanat = resepti.Avainsanat.Select(a => a.ToLower()).ToList();
            var olemassaOlevatAvainsanat = await _konteksti.Avainsanat
                .Where(x => uudetAvainsanat.Contains(x.Sana.ToLower()))
                .ToDictionaryAsync(x => x.Sana.ToLower());
            return await PoistaDuplikaatit(uudetAvainsanat, olemassaOlevatAvainsanat);
        }

        private async Task<List<Ainesosa>> MuunnaAinesosat(string[] ainesosatNimet)
        {
            var uudetNimet = ainesosatNimet.Select(a => a.ToLower()).ToList();
            var olemassaOlevat = await _konteksti.Ainesosat
                .Where(x => uudetNimet.Contains(x.Nimi.ToLower()))
                .ToDictionaryAsync(x => x.Nimi.ToLower());

            return await PoistaDuplikaatit(uudetNimet, olemassaOlevat);
        }

        private async Task<List<Avainsana>> MuunnaAvainsanat(string[] avainsanaNimet)
        {
            var uudetSanat = avainsanaNimet.Select(a => a.ToLower()).ToList();
            var olemassaOlevat = await _konteksti.Avainsanat
                .Where(x => uudetSanat.Contains(x.Sana.ToLower()))
                .ToDictionaryAsync(x => x.Sana.ToLower());

            return await PoistaDuplikaatit(uudetSanat, olemassaOlevat);
        }

        public async Task<Resepti> LisaaAsync(ReseptiDto reseptiDto)
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

        public async Task PaivitaAsync(int id, ReseptiDto reseptiRequest)
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
    }
}
