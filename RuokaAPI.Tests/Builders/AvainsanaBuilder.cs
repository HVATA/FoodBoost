using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuokaAPI.Properties.Model;

namespace RuokaAPI.Tests.Builders
{
    public class AvainsanaBuilder
    {
        private Avainsana _avainsana;

        public AvainsanaBuilder()
        {
            _avainsana = new Avainsana
            {
                Id = 1,
                Sana = "Gluteeniton"
            };
        }

        public AvainsanaBuilder WithId(int id)
        {
            _avainsana.Id = id;
            return this;
        }

        public AvainsanaBuilder WithNimi(string sana)
        {
            _avainsana.Sana = sana;
            return this;
        }

        public Avainsana Build() => _avainsana;
    }
}
