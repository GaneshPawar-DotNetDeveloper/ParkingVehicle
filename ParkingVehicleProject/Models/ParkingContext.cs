using System.Data.Entity;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace ParkingVehicleProject.Models
{
    public class ParkingContext : DbContext
    {
        public DbSet<AllocatedParking> AllocatedParkings { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        public ParkingContext() : base("ParkingContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ParkingContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllocatedParking>()
                .HasKey(p => p.ID);

            modelBuilder.Entity<ParkingSlot>()
                .HasKey(s => s.ParkingNumber);

            base.OnModelCreating(modelBuilder);
        }

        public void SaveChangesToFile()
        {
            var dataFilePath = HttpContext.Current.Server.MapPath("~/App_Data/ParkingData.json");
            var data = new ParkingData
            {
                AllocatedParkings = AllocatedParkings.ToList(),
                ParkingSlots = ParkingSlots.ToList()
            };
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(dataFilePath, jsonData);
        }
    }

    public class ParkingData
    {
        public List<AllocatedParking> AllocatedParkings { get; set; }
        public List<ParkingSlot> ParkingSlots { get; set; }
    }
}
