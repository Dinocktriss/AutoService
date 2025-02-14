using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace AutoService
{
    public class AutoServiceContext : DbContext
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ServiceImage> ServiceImages { get; set; }

        
    }
}
