using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PrograV.Miselanius;
using PrograV.Models;

namespace PrograV.Controllers
{
    public class OwnerController : Controller
    {
        public UserModel IsSessionValid()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                {
                    UserModel? user = JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("userSession"));

                    if (user.type.Equals("owner"))
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

        // GET: OwnerController
        public async Task<ActionResult> Index()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {
                ViewBag.User = user;

               var userProperties = await OwnerHelper.SearchOwnerByEmail(user.email);

               CondominiumHelper condominiumHelper = new CondominiumHelper();

               ViewBag.Condominium = condominiumHelper.getCondominiumsbycondoName(userProperties.condodetail).Result;

               return View();
            }

            return RedirectToAction("Index", "Error");
        }

              

        // GET: OwnerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OwnerController/Create
        public ActionResult CreateVisit()
        {

            return View();
        }

        public ActionResult SaveVisit(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                Visits owner = new Visits
                {
                    nombre = data["Nombre"].ToString(),
                    cedula = Convert.ToInt32(data["Cedula"]),
                    marcavehiculo = data["Marca"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime()
                };

                bool success = OwnerHelper.Savevisit(owner,user).Result;

                if (success)
                {
                    return RedirectToAction("CreateVisit", "Owner");
                }
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();

            
        }

        public ActionResult ViewVisit()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                ViewBag.VisitList = OwnerHelper.ViewVisits(user).Result;

                return View();
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();

        }



        // POST: OwnerController/Create
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

        // GET: OwnerController/Edit/5
        public ActionResult EditVisit(IFormCollection data)
        {
            Visits datatoupadate = new Visits
            {
                nombre = data["Nombre"].ToString(),
                cedula = Convert.ToInt32(data["Cedula"]),
                marcavehiculo = data["Marcavehiculo"].ToString(),
                color = data["Color"].ToString(),
                placa = data["Placa"].ToString(),
                fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),
                id = data["Id"].ToString(),
            };

            return View(datatoupadate);

        }

        public ActionResult SaveUpdateToVisit(IFormCollection data)
        {
            Visits datatoupadate = new Visits
            {
                nombre = data["Nombre"].ToString(),
                cedula = Convert.ToInt32(data["Cedula"]),
                marcavehiculo = data["Marcavehiculo"].ToString(),
                color = data["Color"].ToString(),
                placa = data["Placa"].ToString(),
                fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),
                id = data["Id"].ToString(),
            };


            bool result = OwnerHelper.UpdateVisitinfo(datatoupadate).Result;
            return RedirectToAction("ViewVisit","Owner");
        }


        // POST: OwnerController/Edit/5
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

        // GET: OwnerController/Delete/5
        public ActionResult DeleteVisit(string id)
        {
            bool result = OwnerHelper.RemoveVisit(id).Result;

            if (result)
            {
                return RedirectToAction("ViewVisit", "Owner");
            }
        

            ModelState.AddModelError("", "Error saving the visit.");
            return View();


        }

        // POST: OwnerController/Delete/5
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
