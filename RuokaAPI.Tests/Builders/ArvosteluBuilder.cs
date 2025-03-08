using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuokaAPI.Properties.Model;

namespace RuokaAPI.Tests.Builders
{
    class ArvosteluBuilder
    {
        private ArvosteluRequest _arvostelu;
        public ArvosteluBuilder()
        {
            _arvostelu = new ArvosteluRequest
            {
                ArvostelijanId = 1,
                ArvostelijanNimimerkki = "Testi",
                Numeroarvostelu = 5,
                Vapaateksti = "Tämä on testiarvostelu"
            };
        }

        public ArvosteluBuilder WithArvostelijanId(int arvostelijanId)
        {
            _arvostelu.ArvostelijanId = arvostelijanId;
            return this;
        }

        public ArvosteluBuilder WithArvostelijanNimimerkki(string arvostelijanNimimerkki)
        {
            _arvostelu.ArvostelijanNimimerkki = arvostelijanNimimerkki;
            return this;
        }

        public ArvosteluBuilder WithNumeroarvostelu(int numeroarvostelu)
        {
            _arvostelu.Numeroarvostelu = numeroarvostelu;
            return this;
        }

        public ArvosteluBuilder WithVapaateksti(string vapaateksti)
        {
            _arvostelu.Vapaateksti = vapaateksti;
            return this;
        }
        public ArvosteluRequest Build() => _arvostelu;

    }
}
