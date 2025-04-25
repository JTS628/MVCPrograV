using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrograV.Firebase;
using PrograV.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.Reflection.Metadata.Ecma335;
using PrograV.Miselanius;

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
        public ActionResult CreateCondominium(string txtName, string txtAddress, int txtCount, string txtPhoto)
        {

            UserModel? user = IsSessionValid();

            if (user != null)
            {
                CondominiumHelper condominiumHelper = new CondominiumHelper();

                bool result = condominiumHelper.saveCondominium(new Condominium
                {
                    Name = txtName,
                    Address = txtAddress,
                    Count = txtCount,
                    Photo = txtPhoto,


                }).Result;

                TempData["SuccessMessage"] = "¡El condominio ha sido creado exitosamente!";

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
     
        public ActionResult EditCondo(IFormCollection data)
        {
            Condominium datatoupadate = new Condominium
            {
                Name = data["Name"].ToString(),
                Count = Convert.ToInt32(data["count"]),
                Address = data["Address"].ToString(),
                Photo = data["Photo"].ToString(),
                Id = data["ID"].ToString(),
             
            };

            return View(datatoupadate);

        }

        public ActionResult SaveUpdateToCondo(IFormCollection data)
        {
            Condominium datatoupadate = new Condominium
            {
                Name = data["Name"].ToString(),
                Count = Convert.ToInt32(data["Count"]),
                Address = data["Address"].ToString(),
                Photo = data["Photo"].ToString(),
                Id = data["ID"].ToString(),
            };


            bool result = CondominiumHelper.UpdateCondotinfo(datatoupadate).Result;

            if (result)
            {
                TempData["SuccessMessage"] = "¡Condominio actualizado correctamente!";
                return RedirectToAction("Index", "Condo");

            }
            ModelState.AddModelError("", "Error saving the visit.");
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
        public ActionResult DeleteCondo(string id)
        {
            bool result = CondominiumHelper.RemoveCondo(id).Result;

            if (result)
            {

                TempData["SuccessMessage"] = "¡Condomino eliminado exitosamente!";


                return RedirectToAction("Index", "Condo");
            }


            ModelState.AddModelError("", "Error saving the visit.");
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
