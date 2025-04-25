using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PrograV.Firebase;
using PrograV.Miselanius;
using PrograV.Models;

namespace PrograV.Controllers
{
    public class SecurityController : Controller
    {
        public UserModel IsSessionValid()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                {
                    UserModel? user = JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("userSession"));

                    if (user.type.Equals("officer"))
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

              
        public async Task<ActionResult> Index()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                var VisitList  = Security.OfficerHelper.OfficerViewVisits() .Result;
                var combinedList = new List<CombinedList.VisitWithOwnerViewModel>();


                foreach (var visit in VisitList)
                {
                    var owner = await OwnerHelper.SearchOwnerByEmail(visit.correoOrigen);

                    string condoName = owner?.condodetail != null && owner.condodetail.Any()
                    ? owner.condodetail.First().condoname
                    : "N/A";

                    combinedList.Add(new CombinedList.VisitWithOwnerViewModel
                    {
                        Visit = visit,
                        Owner = owner,
                        CondoName = condoName
                    });
                }

                CondominiumHelper condominiumHelper = new CondominiumHelper();

                ViewBag.condo = condominiumHelper.getCondominiums().Result.ToList();

                return View(combinedList);
            }

            ModelState.AddModelError("", "No se han encontrado visitas");
            return View();
        }




        // GET: SecurityController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SecurityController/Create
        public ActionResult OfficerCreateVisit()
        {
            return View();
        }

        public ActionResult OfficerSaveVisit(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                Visits owner = new Visits
                {
                    nombre = data["Nombre"].ToString(),
                    cedula = Convert.ToInt32(data["Cedula"]),
                    correo = data["Correo"].ToString(),
                    marcavehiculo = data["Marca"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime()
                };

                bool success = Security.OfficerHelper.officerSavevisit(owner, user).Result;

                if (success)
                {

                    return RedirectToAction("OfficerCreateVisit", "Security");
                }
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();


        }


        public ActionResult officerDeleteVisit(string id)
        {
            bool result = Security.OfficerHelper.OfficerRemoveVisit(id).Result;

            if (result)
            {
                return RedirectToAction("Index", "Security");
            }


            ModelState.AddModelError("", "Error saving the visit.");
            return View();


        }


        // POST: SecurityController/Create
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

        // GET: SecurityController/Edit/5
        public async Task<ActionResult> CheckQuickpass(int id)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

               var codeList = await QRCodeHelper.Getcode();

                return View(codeList);
            }
            ModelState.AddModelError("", "Error al optener la lista");
            return View();
        }

        

        // POST: SecurityController/Edit/5
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

        // GET: SecurityController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SecurityController/Delete/5
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
