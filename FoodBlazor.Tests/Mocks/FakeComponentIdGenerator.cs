using System;
using AntDesign;

namespace FoodBlazor.Tests.Mocks
    {
    public class FakeComponentIdGenerator : IComponentIdGenerator
        {
        public string CreateComponentId ()
            {
            // Tämä palauttaa satunnaisen GUID:n merkkijonona
            return Guid.NewGuid().ToString();
            }

        public string Generate ( AntDomComponentBase component )
            {
            // Palautetaan yksinkertaisesti uusi GUID, voidaan käyttää CreateComponentId-metodia
            return CreateComponentId();
            }
        }
    }
