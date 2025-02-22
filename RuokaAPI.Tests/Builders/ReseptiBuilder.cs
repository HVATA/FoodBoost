using RuokaAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuokaAPI.Properties.Model;
using RuokaAPI.Tests.Builders;


namespace ToDoListDemo.Tests.Builders
{
    public class ReseptiBuilder
    {
        private Resepti _resepti;

        public ReseptiBuilder()
        {
            _resepti = new Resepti
            {
                Id = 1,
                Tekijäid = 1,
                Nimi = "Testi Resepti",
                Valmistuskuvaus = "Tämä on testiresepti",
                Katseluoikeus = "Julkinen",
                AinesosanMaara = new List<ReseptiAinesosa>
                {
                    new ReseptiAinesosaBuilder().Build(),
                    new ReseptiAinesosaBuilder().WithId(2).WithAinesosaId(2).WithMaara("100").Build()
                },
                Avainsanat = new List<Avainsana>
                {
                    new AvainsanaBuilder().Build(),
                    new AvainsanaBuilder().WithId(2).WithNimi("Nopea").Build()
                }
            };
        }

        public ReseptiBuilder WithId(int id)
        {
            _resepti.Id = id;
            return this;
        }

        public ReseptiBuilder WithTekijaId(int tekijaId)
        {
            _resepti.Tekijäid = tekijaId;
            return this;
        }

        public ReseptiBuilder WithNimi(string nimi)
        {
            _resepti.Nimi = nimi;
            return this;
        }

        public ReseptiBuilder WithValmistuskuvaus(string? valmistuskuvaus)
        {
            _resepti.Valmistuskuvaus = valmistuskuvaus;
            return this;
        }

        public ReseptiBuilder WithKatseluoikeus(string? katseluoikeus)
        {
            _resepti.Katseluoikeus = katseluoikeus;
            return this;
        }

        public ReseptiBuilder WithAinesosanMaara(List<ReseptiAinesosa> ainesosat)
        {
            _resepti.AinesosanMaara = ainesosat;
            return this;
        }

        public ReseptiBuilder WithAvainsanat(List<Avainsana> avainsanat)
        {
            _resepti.Avainsanat = avainsanat;
            return this;
        }

        public Resepti Build() => _resepti;
    }
}
