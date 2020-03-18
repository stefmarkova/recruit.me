﻿namespace RecruitMe.Web.ViewModels.Employers
{
    using System.ComponentModel.DataAnnotations;

    using RecruitMe.Data.Models;
    using RecruitMe.Services.Mapping;

    public class CreateEmployerProfileInputModel : IMapFrom<Employer>, IMapTo<Employer>
    {
        public string ApplicationUserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Unique Identification Code")]
        public string UniqueIdentificationCode { get; set; }

        [MaxLength(12)]
        [RegularExpression("[0-9]+")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [MaxLength(80)]
        public string Address { get; set; }

        // TODO: split it into a mini section, partial view possibly? or with the weird line
        [Required]
        [MaxLength(100)]
        [Display(Name = "Contact Person Names")]
        public string ContactPersonNames { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Contact Person Email")]
        public string ContactPersonEmail { get; set; }

        [MaxLength(12)]
        [RegularExpression("[0-9]+")]
        [Display(Name = "Contact Person Phone Number")]
        public string ContactPersonPhoneNumber { get; set; }

        [Display(Name = "Contact Person Position")]
        public string ContactPersonPosition { get; set; }

        public string Logo { get; set; }

        [Url]
        [Display(Name = "Website Address")]
        public string WebsiteAddress { get; set; }

        [Required]
        [Display(Name = "Public Sector")]
        public bool IsPublicSector { get; set; }

        [Required]
        [Display(Name = "Is Hiring Agency")]
        public bool IsHiringAgency { get; set; }

        [Display(Name = "Job Sector")]
        public int JobSectorId { get; set; }
    }
}