using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrograV.Firebase;
using PrograV.Miselanius;
using PrograV.Models;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace PrograV.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }
        
       public ActionResult Createowner()
        {
            CondominiumHelper condominiumHelper = new CondominiumHelper();

            ViewBag.condo = condominiumHelper.getCondominiums().Result.ToList();



            return View();
        }

        public ActionResult Createofficer()
        {
            CondominiumHelper condominiumHelper = new CondominiumHelper();

            ViewBag.condo = condominiumHelper.getCondominiums().Result.ToList();



            return View();
        }

        public ActionResult Registerowner(string txtname, string txtemail, string txtcondo, int inthouse)
        {

            string generatedPassword = UserHelper.PasswordGenerator.GenerateRandomPassword(10);
            
           UserHelper.postUserWithEmailAndPassword(new UserModel
            {
                email = txtemail,
                name = txtname,
                type = "owner",
               

            },new CondoDetails
                { 
                  condoname = txtcondo,
                  houseID = inthouse,

                }, generatedPassword);


            return RedirectToAction("Createowner", "admin");
        }

        public ActionResult Registerofficer(string txtname, string txtemail, string txtcondo, int inthouse)
        {

            string generatedPassword = UserHelper.PasswordGenerator.GenerateRandomPassword(10);

            UserHelper.postofficerWithEmailAndPassword(new UserModel
            {
                email = txtemail,
                name = txtname,
                type = "officer",


            }, new CondoDetails
            {
                condoname = txtcondo,
                houseID = inthouse,

            }, generatedPassword);


            return RedirectToAction("Createofficer", "admin");
        }

        public async Task<ActionResult> ManageOwner()
        {
            
            var ownerList = await OwnerHelper.SearchOwner();

            ViewBag.owners = ownerList;

            return View();
        }

        public async Task<ActionResult> ManageOfficer()
        {

            var officerList = await OfficerHelper.SearchOfficers();

            ViewBag.officers = officerList;

            return View();
        }

        public async Task<ActionResult> UpdateOwner(string OwnerName,string Email, string CondoName, int houseID,string uuid)
        {

             await OwnerHelper.UpdateUserDetails(OwnerName, Email, CondoName, houseID, uuid);

            return RedirectToAction("ManageOwner","Admin");
        }

        public async Task<ActionResult> UpdateOfficer(string officerName, string email, string condo, string uuid)
        {

            await OfficerHelper.UpdateOfficerDetails(officerName, email, condo, uuid);

            return RedirectToAction("ManageOfficer", "Admin");

        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
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

        // GET: AdminController/Edit/5
        public async Task<ActionResult> EditOwner(string email, string uuid)
        {
          var owner = await OwnerHelper.SearchOwnerByEmail(email);

            //ViewBag.uuid = uuid;

            return View(owner);
        }

        public async Task<ActionResult> EditOfficer(string email, string uuid)
        {
            var officer = await OfficerHelper.SearchOfficerByEmail(email);

            CondominiumHelper condominiumHelper = new CondominiumHelper();

            List<Condominium> listacondominiums = condominiumHelper.getCondominiums().Result;

            ViewBag.listaCondo = listacondominiums;

            return View(officer);
        }


        // POST: AdminController/Edit/5
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

        // GET: AdminController/Delete/5
        public ActionResult Delete(string Name, string Email, string CondoName, int houseID, string uuid)
        {

            try
            {
               OwnerHelper.RemoveProperty(Name,Email,CondoName,houseID,uuid) ;

               //OwnerHelper.RemoveOwnerIfnoproperties(Email);


                return RedirectToAction("ManageOwner", "admin");
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
                       
        }

        public async Task<ActionResult> DeleteOfficer(string Email,string uuid)
        {

            try
            {
               await OfficerHelper.RemoveOfficer(Email, uuid);

                return RedirectToAction("ManageOfficer", "admin");
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }

        }

        public async Task<ActionResult> AddPropertytoOwner(string email, string uuid)
        {
            var owner = await OwnerHelper.SearchOwnerByEmail(email);

            CondominiumHelper condominiumHelper = new CondominiumHelper();


            List < Condominium > listacondominiums = condominiumHelper.getCondominiums().Result;

            ViewBag.listaCondo = listacondominiums;


            return View(owner); 
        }

        public async Task<ActionResult> SaveNewProperty(string OwnerName, string Email, string condoname, int houseID, string uuid)
        {

           await OwnerHelper.AddNewProperty(OwnerName, Email, condoname, houseID, uuid);

            return RedirectToAction("ManageOwner", "Admin");
        }

        // POST: AdminController/Delete/5
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
