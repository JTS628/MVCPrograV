using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PrograV.Firebase;
using PrograV.Miselanius;
using PrograV.Models;

namespace PrograV.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        public IActionResult Index()
        {
            return View();
        }

        public async Task< IActionResult> Login(string email, string password)
        {
            try
            {
                

                UserHelper userHelper = new UserHelper();

                UserCredential userCredential = await FirebaseAuthHelper.setFirebaseAuthClient().SignInWithEmailAndPasswordAsync(email, password);
                UserModel user = await userHelper.getUser(email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario o Contraseña incorrectos");
                    //TempData["SuccessMessage"] = "Usuario o Contraseña incorrectos"; 
                    return View(); // Return to the Login view
                }

                HttpContext.Session.SetString("userSession",JsonConvert.SerializeObject(user));

                if (user.type.Equals("admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }

                if (user.type.Equals("owner"))
                {
                    return RedirectToAction("Index", "Owner");
                }

                if (user.type.Equals("officer"))
                {
                    return RedirectToAction("Index", "Security");
                }

                return RedirectToAction("Error", "home");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Usuario o Contraseña incorrectos");
                return View("Index");
            }
        }

        public IActionResult LogOut(int id)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Welcome");
        }


        // GET: LoginController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: LoginController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoginController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Delete/5
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoginController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
