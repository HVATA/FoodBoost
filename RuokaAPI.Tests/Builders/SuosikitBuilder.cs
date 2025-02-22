using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuokaAPI.Properties.Model;

namespace RuokaAPI.Tests.Builders
{
    public class SuosikitBuilder
    {
        private Suosikit _suosikki;

        public SuosikitBuilder()
        {
            _suosikki = new Suosikit
            {
                Id = 1,
                kayttajaID = 1,
                reseptiID = 1
            };
        }

        public SuosikitBuilder WithId(int id)
        {
            _suosikki.Id = id;
            return this;
        }

        public SuosikitBuilder WithKayttajaId(int kayttajaId)
        {
            _suosikki.kayttajaID = kayttajaId;
            return this;
        }

        public SuosikitBuilder WithReseptiId(int reseptiId)
        {
            _suosikki.reseptiID = reseptiId;
            return this;
        }

        public Suosikit Build() => _suosikki;
    }
}
