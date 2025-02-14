using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace AutoService
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string Title { get; set; }
        public decimal Cost { get; set; }
        public int DurationInMinutes { get; set; }
        public string Description { get; set; }
        public float Discount { get; set; }
        public string MainImage { get; set; }

        [NotMapped]
        public string MainImagePath => string.IsNullOrEmpty(MainImage) ? "Images/default.png" : MainImage;

        [NotMapped]
        public decimal FinalCost => Cost * (1 - (decimal)Discount);

        [NotMapped]
        public bool HasDiscount => Discount > 0;

        [NotMapped]
        public Brush DiscountBackground => HasDiscount ? Brushes.LightGreen : Brushes.White;

        [NotMapped]
        public decimal OriginalCost => Cost;
    }

    public class Client
    {
        public int ClientID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int ServiceID { get; set; }
        public int ClientID { get; set; }
        public DateTime StartTime { get; set; }

        [NotMapped]
        public DateTime EndTime => StartTime.AddMinutes(Service.DurationInMinutes);

        public virtual Service Service { get; set; }
        public virtual Client Client { get; set; }

        [NotMapped]
        public TimeSpan TimeRemaining => StartTime - DateTime.Now;
    }

    public class ServiceImage
    {
        public int ServiceImageID { get; set; }
        public int ServiceID { get; set; }
        public string ImagePath { get; set; }

        public virtual Service Service { get; set; }
    }
}
