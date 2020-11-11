using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Electro.Models;
using Microsoft.AspNetCore.Identity;
using Electro.Data;

namespace Electro.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext context;

        public AccountController(ILogger<AccountController> logger, SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
         [Route("Identity/Account/Login")]
        public IActionResult LoginRedirect(string ReturnUrl)
        {
            return Redirect("/Account/Login?returnurl=" + ReturnUrl);
        }
         



        [Route("Identity/Account/AccessDenied")]
        public IActionResult AccessRedirect(string ReturnUrl)
        {
            return Redirect("/account/accessdenied?returnurl=" + ReturnUrl);
        }
        [Route("Account/Manage/Index")]
        public IActionResult NoReturn()
        {
            return NoContent();
        }
        [Route("account/manage/twofactorauthentication")]
        public IActionResult GetBack()
        {
            return Redirect("/account/accessdenied?returnurl");
        }
        [HttpPost]
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index","Product");
            }
        }
       public async Task<IActionResult> Login(string email,  string password, bool Remember, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(email, password,  Remember, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    // if(context.ApplicationUsers.FirstOrDefault(x=>x.Email.Equals(email)).IsEmployer)
                    // {
                    //   return RedirectToAction("Index", "Employer");
                    // }
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe =true });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                }
            }

            // If we got this far, something failed, redisplay form
              return Ok();
        }



    }
}
