using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuokaAPI.Tests.Builders
{
    public class ReseptiAinesosaBuilder
    {
        private ReseptiAinesosa _reseptiAinesosa;

        public ReseptiAinesosaBuilder()
        {
            _reseptiAinesosa = new ReseptiAinesosa
            {
                
                ReseptiId = 1,
                AinesosaId = 1,
                Maara = "2",
               
            };
        }

        public ReseptiAinesosaBuilder WithId(int id)
        {
            _reseptiAinesosa.AinesosaId= id;
            return this;
        }

        public ReseptiAinesosaBuilder WithReseptiId(int reseptiId)
        {
            _reseptiAinesosa.ReseptiId = reseptiId;
            return this;
        }

        public ReseptiAinesosaBuilder WithAinesosaId(int ainesosaId)
        {
            _reseptiAinesosa.AinesosaId = ainesosaId;
            return this;
        }

        public ReseptiAinesosaBuilder WithMaara(string maara)
        {
            _reseptiAinesosa.Maara = maara;
            return this;
        }

        

        public ReseptiAinesosa Build() => _reseptiAinesosa;
    }
}

