using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace WDLMassage.Models
{
    public partial class User
    {
        public User()
        {
            AppointmentFkclientNavigation = new HashSet<Appointment>();
            AppointmentFkstaffNavigation = new HashSet<Appointment>();
            IntakeFkclientNavigation = new HashSet<Intake>();
            IntakeFkstaffNavigation = new HashSet<Intake>();
            OuttakeFkclientNavigation = new HashSet<Outtake>();
            OuttakeFkstaffNavigation = new HashSet<Outtake>();
        }

        public int UserPk { get; set; }

        [Required(ErrorMessage = "First Name required")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Za-z\s'-]+$", ErrorMessage = "Please only include letters, a space, apostrophe(') or hyphen (-)")]
        public string NameFirst { get; set; }

        [Required(ErrorMessage = "Last Name required")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Za-z\s'-]+$", ErrorMessage = "Please only include letters, a space, apostrophe(') or hyphen (-)")]
        public string NameLast { get; set; }

        [Required(ErrorMessage = "Email Address required")]
        [MaxLength(50)]
        [RegularExpression(@"^\w+[\w-\.]*\@\w+((-\w+)|(\w*))\.[a-z]{2,3}$", ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number Required")]
        //[MaxLength(50)] - OLESYA commented out, can only use this function on string
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please provide your 10-digit phone number including area code")]
        //[UIHint("PhoneNumber")] TODO not working
        public long Phone { get; set; }

        [Required(ErrorMessage = "Username required")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Must include at least one letter or number")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password required")]
        [MaxLength(50)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Must include at least 8 characters, one number, and one letter")]
        [UIHint("password")]
        public string Password { get; set; }

        public bool? IsAdmin { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<Appointment> AppointmentFkclientNavigation { get; set; }
        public virtual ICollection<Appointment> AppointmentFkstaffNavigation { get; set; }
        public virtual ICollection<Intake> IntakeFkclientNavigation { get; set; }
        public virtual ICollection<Intake> IntakeFkstaffNavigation { get; set; }
        public virtual ICollection<Outtake> OuttakeFkclientNavigation { get; set; }
        public virtual ICollection<Outtake> OuttakeFkstaffNavigation { get; set; }

        public string NameFull
        {
            get
            {
                return $"{this.NameFirst} {this.NameLast}";
            }
        }

        public string ActiveStatus
        {
            get
            {
                return GetStatus(IsActive);
            }
        }

        public string FormattedPhone
        {
            get
            {
                return String.Format("{0:(###) ###-####}", Phone);
            }
        }

        public string Role
        {
            get
            {
                return GetRole(IsAdmin);
            }
        }

        private string GetStatus(bool? isActive)
        {
            if (!isActive.HasValue)
            {
                return "Inactive";
            }
            bool isActiveBool = isActive.HasValue && isActive.Value;
            if (isActiveBool)
            {
                return "Active";
            }
            else
            {
                return "Inactive";
            }
        }

        private string GetRole(bool? isAdmin)
        {
            if (!isAdmin.HasValue)
            {
                return "Client";
            }
            bool isActiveBool = isAdmin.HasValue && isAdmin.Value;
            if (isActiveBool)
            {
                return "Admin";
            }
            else
            {
                return "Client";
            }
        }
    }
}
