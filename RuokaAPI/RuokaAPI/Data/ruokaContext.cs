using Microsoft.EntityFrameworkCore;

using RuokaAPI.Properties.Model;

namespace RuokaAPI.Data; 

public class ruokaContext : DbContext
{
    public ruokaContext(DbContextOptions<ruokaContext> options) : base(options) 
    {
    }



   public  DbSet<Avsanat> Avainsanatset { get; set; }

   public DbSet<Kayttaja> Kayttajaset {  get; set; }   
    
   public DbSet<Resepti> Reseptiset { get; set; }

   public DbSet<Ruokaaineet> Ruokaaineetset { get; set; }  

   public DbSet<Suosikit> Suosikitset { get; set; }
    



    

}
