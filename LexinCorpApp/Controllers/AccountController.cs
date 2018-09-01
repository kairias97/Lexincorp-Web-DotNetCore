using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LexincorpApp.Models.ViewModels;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _usersRepo;
        private readonly ICryptoManager _cryptoManager;
        public AccountController(IUserRepository _usersRepo, ICryptoManager _cryptoManager)
        {
            this._usersRepo = _usersRepo;
            this._cryptoManager = _cryptoManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login(bool? wrongCredentials)
        {
            LoginViewModel viewModel = new LoginViewModel
            {
                Credentials = new Credentials()
            };
            ViewBag.WrongCredentials = wrongCredentials??false;
            return View(viewModel);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Credentials credentials)
        {
            if (!ModelState.IsValid)
            {
                LoginViewModel viewModel = new LoginViewModel
                {
                    Credentials = credentials
                };
                return View(viewModel);
            }
            else
            {
                User user = _usersRepo.GetUserByUsername(credentials.Username);
                if(user == null)
                {
                    ViewBag.WrongCredentials = true;
                    LoginViewModel viewModel = new LoginViewModel
                    {
                        Credentials = credentials
                    };
                    return View(viewModel);
                }
                else
                {
                    if(_cryptoManager.VerifyHash(credentials.Password, user.Password))
                    {
                        //bool saved = await SaveCookies(user);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.IsAdmin ? "Administrador" : "Regular")
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            //AllowRefresh = <bool>,
                            // Refreshing the authentication session should be allowed.

                            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                            // The time at which the authentication ticket expires. A 
                            // value set here overrides the ExpireTimeSpan option of 
                            // CookieAuthenticationOptions set with AddCookie.

                            //IsPersistent = true,
                            // Whether the authentication session is persisted across 
                            // multiple requests. Required when setting the 
                            // ExpireTimeSpan option of CookieAuthenticationOptions 
                            // set with AddCookie. Also required when setting 
                            // ExpiresUtc.

                            //IssuedUtc = <DateTimeOffset>,
                            // The time at which the authentication ticket was issued.

                            //RedirectUri = <string>
                            // The full path or absolute URI to be used as an http 
                            // redirect response value.
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    else
                    {
                        ViewBag.WrongCredentials = true;
                        LoginViewModel viewModel = new LoginViewModel
                        {
                            Credentials = credentials
                        };
                        return View(viewModel);
                    }
                }
            }
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
