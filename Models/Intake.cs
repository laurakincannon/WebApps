using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WDLMassage.Models
{
    public partial class Intake
    {
        public Intake()
        {
            Appointment = new HashSet<Appointment>();
        }

        public int IntakePk { get; set; }

        [Required(ErrorMessage = "Please check this box")]
        public bool FeelingWell { get; set; }

        public string Surgeries { get; set; }

        public string Medications { get; set; }

        public string Sensitives { get; set; }

        public string FocusAreas { get; set; }

        public bool IsComplete { get; set; }
        public int Fkappointment { get; set; }
        public int? Fkclient { get; set; }
        public int? Fkstaff { get; set; }

        public virtual Appointment FkappointmentNavigation { get; set; }
        public virtual User FkclientNavigation { get; set; }
        public virtual User FkstaffNavigation { get; set; }
        public virtual ICollection<Appointment> Appointment { get; set; }
    }
}
