using Microsoft.EntityFrameworkCore;

using RuokaAPI.Properties.Model;

namespace RuokaAPI.Data; 

public class ruokaContext : DbContext
{
    public ruokaContext(DbContextOptions<ruokaContext> options) : base(options) 
    {
    }



   public  DbSet<Avsanat> Avainsanat { get; set; }

   public DbSet<Kayttaja> Kayttajat {  get; set; }   
    
   public DbSet<Resepti> Reseptit { get; set; }

   public DbSet<Ruokaaineet> Ruokaaineet { get; set; }  

   public DbSet<Suosikit> Suosikit { get; set; }
    



    

}
