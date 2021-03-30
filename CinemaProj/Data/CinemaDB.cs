using CinemaProj.Entityes;
using Microsoft.EntityFrameworkCore;

namespace CinemaProj.Data
{
    public class CinemaDB: DbContext
    {
        public DbSet<CinemaSession> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }


        public CinemaDB(DbContextOptions options) : base(options)
        {

        }
    }
}
