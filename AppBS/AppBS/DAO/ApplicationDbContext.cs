using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AppBS.Shared;


namespace AppBS.DAO
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }

     
        public DbSet<Usuario> Usuario { get; set; }

        //      public DbSet<SolicitudBienServicio> SolicitudBienServicio { get; set; }



        public DbSet<Producto> Producto { get; set; }

        public DbSet<Presupuesto> Presupuesto { get; set; }

        public DbSet<Documento> Documento { get; set; }

         public DbSet<SolicitudBienServicio> SolicitudBienServicio { get; set; }


        public DbSet<ControlAprobaciones> ControlAprobaciones { get; set; }




    }
}