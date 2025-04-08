using FoodAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodAPI.Properties.Model;

namespace FoodAPI.Tests.Builders
{
    public class AinesosaBuilder
    {
        public Ainesosa _ainesosa;

        public AinesosaBuilder()
        {
            _ainesosa = new Ainesosa
            {
                Id = 1,
                Nimi = "Suola",
                AinesosanMaara = new List<ReseptiAinesosa>()
            };
        }

        public AinesosaBuilder WithId(int id)
        {
            _ainesosa.Id = id;
            return this;
        }

        public AinesosaBuilder WithNimi(string nimi)
        {
            _ainesosa.Nimi = nimi;
            return this;
        }

        public AinesosaBuilder WithAinesosanMaara(List<ReseptiAinesosa> maara)
        {
            _ainesosa.AinesosanMaara = maara;
            return this;
        }

        public Ainesosa Build() => _ainesosa;
    }
}
