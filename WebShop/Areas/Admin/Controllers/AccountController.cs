using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Areas.Admin.Models;
using WebShop.Extension;
using WebShop.Helpper;
using WebShop.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly dbMarketsContext _context;
        public INotyfService _notyfService { get; }
        public AccountController(dbMarketsContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public IActionResult AdminLogin(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public async Task<IActionResult> AdminLogin(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    Account kh = _context.Accounts
                    .Include(p => p.Role)
                    .SingleOrDefault(p => p.Email.ToLower() == model.UserName.ToLower().Trim());

                    if (kh == null)
                    {
                        ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                    }
                    string pass = (model.Password.Trim());
                    // + kh.Salt.Trim()
                    if (kh.Password.Trim() != pass)
                    {
                        ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                        return View(model);
                    }
                    //đăng nhập thành công

                    //ghi nhận thời gian đăng nhập
                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();


                    var taikhoanID = HttpContext.Session.GetString("AccountId");
                    //identity
                    //luuw seccion Makh
                    HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());

                    //identity
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.FullName),
                        new Claim(ClaimTypes.Email, kh.Email),
                        new Claim("AccountId", kh.AccountId.ToString()),
                        new Claim("RoleId", kh.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, kh.Role.RoleName)
                    };
                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);



                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
            }
            catch
            {
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
            return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
        }
        [Route("logout.html", Name = "Logout")]
        public IActionResult AdminLogout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
            catch
            {
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
        }

    }
}
