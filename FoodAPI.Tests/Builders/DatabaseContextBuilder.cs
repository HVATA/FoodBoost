using Microsoft.EntityFrameworkCore;
using FoodAPI.Data;
using FoodAPI.Properties.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAPI.Tests.Builders
{
    internal class DatabaseContextBuilder
    {
        private readonly FoodContext _context;

        private static Ainesosa Kukkakaali = new AinesosaBuilder().WithId(2).WithNimi("Kukkakaali").Build();
        private static Ainesosa Palsternakka = new AinesosaBuilder().WithId(3).WithNimi("Palsternakka").Build();
        private static Avainsana Aamupala = new AvainsanaBuilder().WithId(3).WithNimi("Aamupala").Build();
        private static Avainsana Nopea = new AvainsanaBuilder().WithId(4).WithNimi("Nopea").Build();
        private static Avainsana Herkku = new AvainsanaBuilder().WithId(5).WithNimi("Herkku").Build();
        
        public static readonly Resepti JulkinenReseptiTietokannassa = new ReseptiBuilder()
            .WithId(2)
            .WithNimi("Testi Resepti 1")
            .WithKatseluoikeus("julkinen")
            .WithAinesosanMaara(Kukkakaali, "1 kpl")
            .WithAvainsanat([Aamupala, Nopea])
            .Build();
        public static readonly Resepti YksityinenReseptiTietokannassa = new ReseptiBuilder()
            .WithId(3)
            .WithNimi("Testi Resepti 2")
            .WithKatseluoikeus("Yksityinen")
            .WithAinesosanMaara(Kukkakaali, "1 kpl")
            .WithAinesosanMaara(Palsternakka, "2 kpl")
            .WithAvainsanat([Aamupala, Herkku])
            .Build();

        public DatabaseContextBuilder()
        {
            var options = new DbContextOptionsBuilder<FoodContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")                
                .Options;
            _context = new FoodContext(options);//luodaan konteksti, joka käyttää muistissa olevaa tietokantaa
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public DatabaseContextBuilder SeedDatabase()
        {
            _context.Reseptit.Add(JulkinenReseptiTietokannassa);
            _context.Reseptit.Add(YksityinenReseptiTietokannassa);
            _context.Ainesosat.Add(Kukkakaali);
            _context.Ainesosat.Add(Palsternakka);
            _context.Avainsanat.Add(Aamupala);
            _context.Avainsanat.Add(Nopea);
            _context.Avainsanat.Add(Herkku);
            _context.SaveChanges();

            return this;
        }

        public FoodContext Build() => _context;
    }
}
