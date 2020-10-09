using System;
using System.Collections.Generic;

namespace WDLMassage.Models
{
    public partial class Massage
    {
        public Massage()
        {
            Appointment = new HashSet<Appointment>();
        }

        public int MassagePk { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Imagename { get; set; }

        public virtual ICollection<Appointment> Appointment { get; set; }
    }
}
