using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WDLMassage.Models
{
    public partial class Appointment
    {
        public Appointment()
        {
            Intake = new HashSet<Intake>();
            Outtake = new HashSet<Outtake>();
        }

        //automatically assigned
        public int AppointmentPk { get; set; }

        [Required(ErrorMessage = "Appointment duration required")]
        [MaxLength(3)]
        [Range(15, 120, ErrorMessage = "Appointment must be between 15 and 120 minutes")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Enter only numbers.")]
        public string Duration { get; set; }

        [Required(ErrorMessage = "Appointment Date Required")]
        public TimeSpan Time { get; set; }

        [Required(ErrorMessage = "Appointment Time Required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Appointment duration required")]
        // [MaxLength(3)] Olesya - commented out, it's giving an error since it's an integer
        [Range(15, 120, ErrorMessage = "Price must be between $15 and $120")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Enter only numbers.")]
        [UIHint("Currency")]
        public int Price { get; set; }

        public int Fkmassage { get; set; }

        [Required(ErrorMessage = "You must select a staff member")]
        public int? Fkstaff { get; set; }

        public int? Fkclient { get; set; }

        public int? Fkintake { get; set; }

        public int? Fkouttake { get; set; }

        public virtual User FkclientNavigation { get; set; }
        public virtual Intake FkintakeNavigation { get; set; }
        public virtual Massage FkmassageNavigation { get; set; }
        public virtual Outtake FkouttakeNavigation { get; set; }
        public virtual User FkstaffNavigation { get; set; }
        public virtual ICollection<Intake> Intake { get; set; }
        public virtual ICollection<Outtake> Outtake { get; set; }

        public string FormattedDate
        {
            get
            {
                return this.Date.ToString("MM/dd/yyyy");
            }
        }

        public string FormattedTime
        {
            get
            {
                return GetFormattedTime(this.Time);
            }
        }

        private string GetFormattedTime(TimeSpan timeToFormat)
        {
            DateTime dateToFormat;
            string formattedTime;

            dateToFormat = Convert.ToDateTime(timeToFormat.ToString());

            formattedTime = dateToFormat.ToString("h:mm tt");

            return formattedTime;
        }
    }
}
