using Microsoft.EntityFrameworkCore;
using RuokaAPI.Data;
using RuokaAPI.Dtos;
using RuokaAPI.Properties.Model;
using System.Collections.Generic;

namespace RuokaAPI.Repositories
{
    public class ReseptiRepository
    {
        private readonly ruokaContext _konteksti;

        public ReseptiRepository(ruokaContext konteksti)
        {
            _konteksti = konteksti;
        }

        public async Task<IEnumerable<Resepti>> HaeReseptitAsync(string[]? ainesosat, string[]? avainsanat)
        {
            var query = _konteksti.Reseptit
                .Include(r => r.Ainesosat)
                .Include(r => r.Avainsanat)
                .AsQueryable();

            if (ainesosat != null && ainesosat.Length > 0)
            {
                var haettavatAinesosat = ainesosat.ToList();
                query = query.Where(r => r.Ainesosat
                    .Any(a => haettavatAinesosat.Contains(a.Nimi)));
            }

            if (avainsanat != null && avainsanat.Length > 0)
            {
                var haettavatAvainsanat = avainsanat.ToList();
                query = query.Where(r => r.Avainsanat
                    .Any(a => haettavatAvainsanat.Contains(a.Sana)));
            }

            return await query.ToListAsync();
        }

       
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
            var uudetAinesosat = resepti.Ainesosat.Select(a => a.ToLower()).ToList();
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
            var resepti = new Resepti
            {
                Tekijäid = reseptiDto.TekijaId,
                Nimi = reseptiDto.Nimi,
                Valmistuskuvaus = reseptiDto.Valmistuskuvaus,
                Kuva1 = reseptiDto.Kuva1,
                Katseluoikeus = reseptiDto.Katseluoikeus,
                Ainesosat = await MuunnaAinesosat(reseptiDto.Ainesosat),
                Avainsanat = await MuunnaAvainsanat(reseptiDto.Avainsanat)
            };

            _konteksti.Reseptit.Add(resepti);
            await _konteksti.SaveChangesAsync();
            return resepti;
        }

        public async Task PaivitaAsync(int id, ReseptiDto reseptiRequest)
        {
            var resepti = _konteksti.Reseptit
                .Include(r => r.Ainesosat)
                .Include(r => r.Avainsanat)
                .FirstOrDefault(r => r.Id == id);
            if (resepti == null) return;

            resepti.Avainsanat = await PoistaDuplikaattiAvainsanat(reseptiRequest);
            resepti.Ainesosat = await PoistaDuplikaattiAinesosat(reseptiRequest);
            resepti.Katseluoikeus = reseptiRequest.Katseluoikeus;
            resepti.Valmistuskuvaus = reseptiRequest?.Valmistuskuvaus;
            resepti.Tekijäid = reseptiRequest.TekijaId;
            resepti.Kuva1 = reseptiRequest.Kuva1;
            resepti.Nimi = reseptiRequest.Nimi;
            
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
