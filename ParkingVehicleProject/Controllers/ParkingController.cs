using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using ParkingVehicleProject.Models;

namespace ParkingVehicleProject.Controllers
{
    public class ParkingController : Controller
    {
        private ParkingContext db = new ParkingContext();

        // GET: Parking
        public ActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = 1;
            try
            {
                 var allocatedParkings = db.AllocatedParkings
                    .OrderByDescending(p => p.StartDateTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                if (Request.IsAjaxRequest())
                {
                    return Json(allocatedParkings, JsonRequestBehavior.AllowGet);
                }

                return View(allocatedParkings);
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    Console.WriteLine($"Entity: {validationError.Entry.Entity.GetType().Name}");
                    foreach (var error in validationError.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                }
                throw;
            }

        }

        [HttpPost]
        public ActionResult Allocate(string carPlateNumber)
        {
            var availableSlot = db.ParkingSlots.FirstOrDefault(slot => !slot.IsAllocated);
            if (availableSlot == null)
            {
                return Json(new { message = "Parking Full" });
            }

            var startDateTime = DateTime.Now;
            var allocatedParking = new AllocatedParking
            {
                CarPlateNumber = carPlateNumber,
                StartDateTime = startDateTime,
                ParkingNumber = availableSlot.ParkingNumber
            };

            availableSlot.IsAllocated = true;

            db.AllocatedParkings.Add(allocatedParking);
            db.SaveChanges();

            return Json(new
            {
                ParkingNumber = allocatedParking.ParkingNumber,
                StartDateTime = allocatedParking.StartDateTime
            });
        }

        [HttpPost]
        public ActionResult Release(int id)
        {
            var allocatedParking = db.AllocatedParkings.Find(id);
            if (allocatedParking == null)
            {
                return HttpNotFound();
            }

            allocatedParking.EndDateTime = DateTime.Now;
            allocatedParking.TotalAmount = CalculateTotalAmount(allocatedParking.StartDateTime, 
                allocatedParking.EndDateTime.Value);

            var parkingSlot = db.ParkingSlots.Find(allocatedParking.ParkingNumber);
            parkingSlot.IsAllocated = false;

            db.SaveChanges();

            return Json(new
            {
                EndDateTime = allocatedParking.EndDateTime,
                TotalAmount = allocatedParking.TotalAmount
            });
        }

        private decimal CalculateTotalAmount(DateTime startDateTime, DateTime endDateTime)
        {
            decimal totalAmount = 0;
            for (var date = startDateTime; date < endDateTime; date = date.AddHours(1))
            {
                if (date.Hour >= 6 && date.Hour < 22)
                {
                    totalAmount += 30;
                }
                else
                {
                    totalAmount += 50;
                }
            }
            return totalAmount;
        }
    }
}
