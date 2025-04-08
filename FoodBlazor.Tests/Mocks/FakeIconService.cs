using AntDesign;
using Microsoft.JSInterop;

namespace FoodBlazor.Tests.Mocks
    {
    // Tämä on yksinkertainen fake-versio IconService:stä, joka tarjoaa tarvittavan palvelun AntDesign-komponenteille.
    public class FakeIconService : IconService
        {
        // Voit lisätä tarvittaessa toteutuksia, mutta useimmissa testeissä tämä tyhjä toteutus riittää.
        public FakeIconService ( IJSRuntime js ) : base(js)
            {
            }
        }
    }
