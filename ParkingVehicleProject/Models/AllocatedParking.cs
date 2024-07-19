using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkingVehicleProject.Models
{
    public class AllocatedParking
    {
        public int ID { get; set; }
        public string CarPlateNumber { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? ParkingNumber { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}