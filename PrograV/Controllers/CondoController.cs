using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrograV.Firebase;
using PrograV.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.Reflection.Metadata.Ecma335;

namespace PrograV.Controllers
{
    public class CondoController : Controller
    {

        public UserModel IsSessionValid()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                {
                    UserModel? user = JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("userSession"));

                    if (user.type.Equals("admin"))
                    {
                        return user;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }

        }


        // GET: CondoController
        public ActionResult Index()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {
                ViewBag.User = user;

                CondominiumHelper condominiumHelper = new CondominiumHelper();

                ViewBag.Condominium = condominiumHelper.getCondominiums().Result;


                return View();


            }


            return RedirectToAction("Index", "Error");
        }

        public ActionResult Create()
        {

            UserModel? user = IsSessionValid();

            if (user != null)
            {
                return View();
            }

            return RedirectToAction("Index", "Error");
        }
          

        public ActionResult Main()
        {
            UserModel? user = IsSessionValid();

            if (user != null)
            {
                return View();
            }

            return RedirectToAction("Index", "Error");

        }

        // GET: CondoController/Create
        public ActionResult CreateCondominium(string txtName, string txtAddress, int count, string txtPhoto)
        {

            UserModel? user = IsSessionValid();

            if (user != null)
            {
                CondominiumHelper condominiumHelper = new CondominiumHelper();

                bool result = condominiumHelper.saveCondominium(new Condominium
                {
                    Name = txtName,
                    Address = txtAddress,
                    Count = count,
                    Photo = txtPhoto,


                }).Result;

                return RedirectToAction("Index");


            }

            return RedirectToAction("Index", "Error");
                       
                        
        }


        // GET: CondoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

      

        // POST: CondoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: CondoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CondoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: CondoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CondoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
