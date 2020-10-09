using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WDLMassage.Models
{
    public partial class Outtake
    {
        public Outtake()
        {
            Appointment = new HashSet<Appointment>();
        }

        public int SurveyPk { get; set; }

        [Required(ErrorMessage = "Professional Rating Required")]
        public int RatingProfessional { get; set; }

        [Required(ErrorMessage = "Rating of Addressed Needs Required")]
        public int RatingAddressedNeeds { get; set; }

        [Required(ErrorMessage = "Rating of Massage Quality Required")]
        public int RatingMassageQuality { get; set; }

        [Required(ErrorMessage = "Total Score Required")]
        public int TotalScore { get; set; }

        public string Comments { get; set; }

        public bool IsComplete { get; set; }
        public int Fkappointment { get; set; }
        public int? Fkstaff { get; set; }
        public int? Fkclient { get; set; }

        public string ProfessionalScore
        {
            get
            {
                return GetScore(RatingProfessional);
            }
        }
        public string NeedsScore
        {
            get
            {
                return GetScore(RatingAddressedNeeds);
            }
        }
        public string QualityScore
        {
            get
            {
                return GetScore(RatingMassageQuality);
            }
        }
        public string OverallScore
        {
            get
            {
                return GetScore(TotalScore);
            }
        }

        public virtual Appointment FkappointmentNavigation { get; set; }
        public virtual User FkclientNavigation { get; set; }
        public virtual User FkstaffNavigation { get; set; }
        public virtual ICollection<Appointment> Appointment { get; set; }

        private string GetScore(int rating)
        {
            string score = "";
            switch (rating)
            {
                case 5:
                    score = "Very Satisfied";
                    break;
                case 4:
                    score = "Satisfied";
                    break;
                case 3:
                    score = "Neutral";
                    break;
                case 2:
                    score = "Dissatisfied";
                    break;
                default:
                    score = "Very Dissatisfied";
                    break;
            }
            return score;
        }
    }
}
