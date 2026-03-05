using Azure;
using CrudMVC.Models;
using CrudMVC.Models.Entities;
using CrudMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace CrudMVC.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Services.IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, Services.IEmailSender emailSender, IWebHostEnvironment hostEnvironment)
        {

            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._emailSender = emailSender;
            this._hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check email
            var chkEmail = await _userManager.FindByEmailAsync(model.Email);
            if (chkEmail != null)
            {
                ModelState.AddModelError("", "Email already exists");
                return View(model);
            }

            string? imagePath = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                imagePath = "/uploads/" + uniqueFileName;
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                ImageUrl = imagePath
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("FullName", user.FirstName + " " + user.LastName);
                HttpContext.Session.SetString("UserImage", user.ImageUrl ?? "");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account",new {area = ""});
        }
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Token");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, Token = token }, protocol: Request.Scheme);
                bool isSendEmail = await _emailSender.SendEmailAsync(model.Email, "Reset Password", $@"Please reset your password by clicking <a style='background-color:#04aa6d; border:none; color:white; padding:10px; text-align:center; text-decoration:none; display:inline-block; font-size:16px; margin:4px 2px; cursor:pointer; border-radius:10px;' href='{callbackUrl}'>Click Here</a>");
                if (isSendEmail)
                {
                    ResponseVM response = new ResponseVM();
                    response.Message = "Reset password link has been sent to your email.";
                    return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
                }
            }
            return View(model);
        }
        public IActionResult ForgetPasswordConfirmation(ResponseVM response)
        {
            return View(response);
        }
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ForgetPasswordVM 
            { 
                UserId = userId, 
                Token = token
            };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgetPasswordVM model)
        {
            ResponseVM response = new ResponseVM();
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                response.Message = "Your password has been reset successfully.";
                return RedirectToAction("Login", "Account", response);
            }
            return View(model);
        }
    }   
}
