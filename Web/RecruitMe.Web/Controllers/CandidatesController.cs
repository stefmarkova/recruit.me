﻿namespace RecruitMe.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using RecruitMe.Common;
    using RecruitMe.Data.Models;
    using RecruitMe.Services.Data;
    using RecruitMe.Web.ViewModels.Candidates;

    public class CandidatesController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICandidatesService candidatesService;

        public CandidatesController(UserManager<ApplicationUser> userManager, ICandidatesService candidatesService)
        {
            this.userManager = userManager;
            this.candidatesService = candidatesService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            this.HttpContext.Session.SetString("UserRole", GlobalConstants.CandidateRoleName);

            return this.View();
        }

        [Authorize(Roles = GlobalConstants.CandidateRoleName)]
        [HttpGet]
        public IActionResult CreateProfile()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreateCandidateProfileInputModel input)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            input.ApplicationUserId = user.Id;

            string candidateId = await this.candidatesService.CreateProfile(input);

            if (candidateId != null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View("Error");
        }
    }
}