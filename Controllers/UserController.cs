using CST_350_MilestoneProject.Filters;
using CST_350_MilestoneProject.Models;
using CST_350_MilestoneProject.Services;
using GroupProjectMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CST_350_MilestoneProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessLogin(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", loginViewModel);
            }

            var user = _userManager.ValidateUser(loginViewModel.Username, loginViewModel.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View("Login", loginViewModel);
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("IsLoggedIn", "true");

            return RedirectToAction("LoginSuccess");
        }

        [HttpGet]
        public IActionResult LoginSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessRegister(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", viewModel);
            }

            if (_userManager.EmailExists(viewModel.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", "Email address is already in use.");
                return View("Register", viewModel);
            }

            if (_userManager.UsernameExists(viewModel.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View("Register", viewModel);
            }

            var userModel = new UserModel
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Sex = viewModel.Sex,
                Age = viewModel.Age,
                State = viewModel.State,
                EmailAddress = viewModel.EmailAddress,
                Username = viewModel.Username,
                Password = viewModel.Password
            };

            var success = _userManager.RegisterUser(userModel);

            if (!success)
            {
                return View("Register", viewModel);
            }

            return RedirectToAction("RegisterSuccess");
        }

        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [SessionCheckFilter]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
