using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodAPI.Properties.Model;

namespace FoodAPI.Tests.Builders
{
    public class KayttajaBuilder
    {
        private Kayttaja _kayttaja;

        public KayttajaBuilder()
        {
            _kayttaja = new Kayttaja
            {
                Id = 1,
                Etunimi = "Testi",
                Sukunimi = "Kayttaja",
                Nimimerkki = "TestiNimi",
                Sahkopostiosoite = "test@example.com",
                Salasana = "salasana123",
                Kayttajataso = "Peruskäyttäjä"
            };
        }

        public KayttajaBuilder WithId(int id)
        {
            _kayttaja.Id = id;
            return this;
        }

        public KayttajaBuilder WithEtunimi(string etunimi)
        {
            _kayttaja.Etunimi = etunimi;
            return this;
        }

        public KayttajaBuilder WithSukunimi(string sukunimi)
        {
            _kayttaja.Sukunimi = sukunimi;
            return this;
        }

        public KayttajaBuilder WithNimimerkki(string nimimerkki)
        {
            _kayttaja.Nimimerkki = nimimerkki;
            return this;
        }

        public KayttajaBuilder WithSahkopostiosoite(string sahkopostiosoite)
        {
            _kayttaja.Sahkopostiosoite = sahkopostiosoite;
            return this;
        }

        public KayttajaBuilder WithSalasana(string salasana)
        {
            _kayttaja.Salasana = salasana;
            return this;
        }

        public KayttajaBuilder WithKayttajataso(string kayttajataso)
        {
            _kayttaja.Kayttajataso = kayttajataso;
            return this;
        }

        public Kayttaja Build() => _kayttaja;
    }
}
