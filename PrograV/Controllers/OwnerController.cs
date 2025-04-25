using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PrograV.Miselanius;
using PrograV.Models;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

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
        public async Task<ActionResult> CreateVisit()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                var userProperties = await OwnerHelper.SearchOwnerByEmail(user.email);

                CondominiumHelper condominiumHelper = new CondominiumHelper();

                ViewBag.Condominium = condominiumHelper.getCondominiumsbycondoName(userProperties.condodetail).Result;

                return View();
            }
            ModelState.AddModelError("", "Error saving the visit.");
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
                    correo = data["Correo"].ToString(),
                    marcavehiculo = data["Marca"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime()
                };

                bool success = OwnerHelper.Savevisit(owner,user).Result;

                if (success)
                {

                    EmailHelper.SendEmailtoVisit(owner);

                    TempData["SuccessMessage"] = "¡Su Visita ha sido creada!";

                    return RedirectToAction("CreateVisit", "Owner");
                }
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();

            
        }

        public async Task<ActionResult> ViewVisit()
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
            UserModel user = IsSessionValid();

            if (user != null)
            {


                Visits datatoupadate = new Visits
                {
                    nombre = data["Nombre"].ToString(),
                    cedula = Convert.ToInt32(data["Cedula"]),
                    marcavehiculo = data["Marcavehiculo"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),
                    id = data["Id"].ToString(),
                };

                return View(datatoupadate);
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();
        }

        public ActionResult SaveUpdateToVisit(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {
                Visits datatoupadate = new Visits
                {
                    nombre = data["Nombre"].ToString(),
                    cedula = Convert.ToInt32(data["Cedula"]),
                    marcavehiculo = data["Marcavehiculo"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),
                    id = data["Id"].ToString(),
                    correo = data["Correo"].ToString()
                };


                bool result = OwnerHelper.UpdateVisitinfo(datatoupadate).Result;

                if (result)
                {

                    EmailHelper.SendEmailtoVisit(datatoupadate);

                    TempData["SuccessMessage"] = "¡Datos actualizados correctamente!";

                    return RedirectToAction("ViewVisit", "Owner");
                }
                
            }
            ModelState.AddModelError("", "Error saving the visit.");
            return View();
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

        public ActionResult DeleteVisit(string id)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {
                bool result = OwnerHelper.RemoveVisit(id).Result;

                if (result)
                {
                    TempData["SuccessMessage"] = "¡Datos eliminados exitosamente!";

                    return RedirectToAction("ViewVisit", "Owner");
                }
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();


        }
          
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

        public ActionResult Savefavorito(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                Visits owner = new Visits
                {
                    nombre = data["Nombre"].ToString(),
                    cedula = Convert.ToInt32(data["Cedula"]),
                    marcavehiculo = data["Marca"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime()
                };

                bool success = OwnerHelper.Savefavorito(owner, user).Result;

                if (success)
                {
                    TempData["SuccessMessage"] = "¡Tu Favorito ha sido creado exitosamente!";
                    return RedirectToAction("CreateFavorito", "Owner");
                }
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();


        }

        public ActionResult CreateFavorito()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            { 

                return View();
            
            }

            return RedirectToAction("Index", "Error");
        }

        public ActionResult ViewFavorito()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                ViewBag.VisitList = OwnerHelper.ViewFavorito(user).Result;

                return View();
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();

        }
        public ActionResult EditFavorito(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {
                Visits datatoupadate = new Visits
                {
                    nombre = data["Nombre"].ToString(),
                    cedula = Convert.ToInt32(data["Cedula"]),
                    marcavehiculo = data["Marcavehiculo"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),
                    id = data["Id"].ToString(),
                };

                return View(datatoupadate);
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();
        }

        public ActionResult SaveUpdateToFavorito(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {
                Visits datatoupadate = new Visits
                {
                    nombre = data["Nombre"].ToString(),
                    cedula = Convert.ToInt32(data["Cedula"]),
                    marcavehiculo = data["Marcavehiculo"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),
                    id = data["Id"].ToString(),
                };


                bool result = OwnerHelper.UpdateFavoritoinfo(datatoupadate).Result;

                if (result)
                {
                    TempData["SuccessMessage"] = "¡Tu favovorito ha sido actualizado exitosamente!";
                    return RedirectToAction("ViewFavorito", "Owner");
                }

                ModelState.AddModelError("", "Error saving the visit.");
                return View();
            }
            ModelState.AddModelError("", "Error saving the visit.");
            return View();

        }
        //quede por aqui con la validacion
        public ActionResult DeleteFavorito(string id)
        {

            bool result = OwnerHelper.RemoveFavorito(id).Result;

            if (result)
            {
                return RedirectToAction("ViewFavorito", "Owner");
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();

        }

        public ActionResult CreateQuickDelivery()
        {

            UserModel user = IsSessionValid();

            if (user != null)
            {

                return View();

            }

            return RedirectToAction("Index", "Error");
        }

        public ActionResult SaveQuickDelivery(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {
                var fivelettercode = UserHelper.PasswordGenerator.GenerateRandomPassword(5);

                QuickDelivery delivery = new QuickDelivery
                {
                    nombre = data["nombre"].ToString(),
                    //cedula = Convert.ToInt32(data["Cedula"]),
                   descripcion = data["descripcion"].ToString(),
                    correo = data["correo"].ToString(),
                    //color = data["Color"].ToString(),
                    code = fivelettercode,
                    fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime()

                };

                bool success = OwnerHelper.SaveQuickDelivery(delivery, user).Result;

                if (success)
                {
                    EmailHelper.SendEmailtoQuickPass(delivery);
                    TempData["SuccessMessage"] = "¡Tu Delivery ha sido creado!";

                    return RedirectToAction("CreateQuickDelivery", "Owner");
                }
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();


        }

        public ActionResult CreateMisVehiculos()
        {

            return View();
        }

        public ActionResult SaveMisVehiculos(IFormCollection data)
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                Vehiculo vehiculo = new Vehiculo
                {
                    marca = data["marca"].ToString(),
                    modelo = data["Modelo"].ToString(),
                    color = data["Color"].ToString(),
                    placa = data["Placa"].ToString(),
                    
                };

                bool success = OwnerHelper.SaveMiVehiculo(vehiculo, user).Result;

                if (success)
                {
                    TempData["SuccessMessage"] = "¡Tu vehiculo ha sido agregado!";

                    return RedirectToAction("CreateMisVehiculos", "Owner");
                }
            }

            ModelState.AddModelError("", "Error al salvar mi vehiculo");
            return View();


        }

        public ActionResult ViewMisVehiculos()
        {
            UserModel user = IsSessionValid();

            if (user != null)
            {

                ViewBag.VisitList = OwnerHelper.ViewMiVehiculo(user).Result;

                return View();
            }

            ModelState.AddModelError("", "Error al guardar mi vehiculo");
            return View();
        }

        public ActionResult EditMiVehiculo(IFormCollection data)
        {
            Vehiculo datatoupadate = new Vehiculo
            {
                //nombre = data["Nombre"].ToString(),
                //cedula = Convert.ToInt32(data["Cedula"]),
                marca = data["Marca"].ToString(),
                modelo = data["Modelo"].ToString(),
                color = data["Color"].ToString(),
                placa = data["Placa"].ToString(),
                //fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),
                id = data["Id"].ToString(),
            };

            return View(datatoupadate);

        }

        public ActionResult SaveUpdateToMiVehiculo(IFormCollection data)
        {
            Vehiculo datatoupadate = new Vehiculo
            {
                //nombre = data["Nombre"].ToString(),
                //cedula = Convert.ToInt32(data["Cedula"]),
                marca = data["Marca"].ToString(),
                modelo = data["Modelo"].ToString(),
                color = data["Color"].ToString(),
                placa = data["Placa"].ToString(),
                /*fecha = Convert.ToDateTime(data["Fecha"]).ToUniversalTime(),*/
                id = data["Id"].ToString(),
            };


            bool result = OwnerHelper.UpdateMiVehiculoinfo(datatoupadate).Result;

            if (result)
            {
                TempData["SuccessMessage"] = "¡Tu vehiculo ha sido actualizado!";
                return RedirectToAction("ViewMisVehiculos", "Owner");

            }
            ModelState.AddModelError("", "Error saving the visit.");
            return View();


        }

        public ActionResult DeleteVehiculo(string id)
        {
            bool result = OwnerHelper.RemoveVehiculo(id).Result;

            if (result)
            {
                TempData["SuccessMessage"] = "¡Tu vehiculo ha sido eliminado!";
                return RedirectToAction("ViewMisVehiculos", "Owner");
            }

            ModelState.AddModelError("", "Error saving the visit.");
            return View();

        }

    }
}
