using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using Dinning_Guide.Models.User;
using Dinning_Guide.Models.Restaurant;
using Dinning_Guide.Models.Rate;
using PagedList;
using System.Web.Security;
using System.Data;


namespace Dinning_Guide.Controllers
{
    public class HomeController : Controller
    {
        private DB_Entities _db = new DB_Entities();
        // GET: Home
        public ActionResult Index(string search)
        {
            if (Session["idUser"] != null)
            {
                if (search != null) return RedirectToAction("Index1", new { option = "Name", search = search, pageNumber = 1, sort = "descending name" });
                return View();

            }
            else
            {
                //return RedirectToAction("Login");
                if (search != null) return RedirectToAction("Index1", new { option = "Name", search = search, pageNumber = 1, sort = "descending name" });
                return View();

            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        //GET: Register
        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Models.User.User _user)
        {
            if (ModelState.IsValid)
            {
                var check = _db.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    _user.Password = GetMD5(_user.Password);
                    _user.idUser++;
                    _db.Configuration.ValidateOnSaveEnabled = true;
                    _db.Users.Add(_user);
                    _db.SaveChanges();
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {

                var f_password = GetMD5(password);
                var data = _db.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().idUser;
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login","Home");
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();//remove session
            return RedirectToAction("Login","Home");
        }

        public ActionResult Profile()
        {
            return View();
        }


        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        /// ---------------------------------------------------------
        Db_Restaurants db1 = new Db_Restaurants();
        public ActionResult Index1(string option, string search, double? rate, int? pageNumber, string sort)
        {
            //if the sort parameter is null or empty then we are initializing the value as descending name  
            ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";
            //if the sort value is gender then we are initializing the value as descending gender  
            ViewBag.SortByDescription = sort == "Description" ? "descending description" : "Description";

            //here we are converting the Db1 Restaurant to AsQueryable => we can invoke all the extension methods on variable records.  
            var records = db1.Restaurants.AsQueryable();

            //if a user choose the radio button option as Description  

            if (option == "Address")
            {
                records = records.Where(x => x.Address == search || search == null);
            }
            else if (option == "Description")
            {
                records = records.Where(x => x.Decription == search|| search == null);
            }
            else if (option == "Rate")
            {
                records = records.Where(x => x.Rate == rate || search == null);
            }

            else
            {
                records = records.Where(x => x.Name.StartsWith(search) || search == null);
            }

            switch (sort)
            {

                case "descending name":
                    records = records.OrderByDescending(x => x.Name);
                    break;

                case "descending rate":
                    records = records.OrderByDescending(x => x.Rate);
                    break;

                case "Address":
                    records = records.OrderBy(x => x.Address);
                    break;

                default:
                    records = records.OrderBy(x => x.Name);
                    break;

            }

            return View(records.ToPagedList(pageNumber ?? 1, 3));
        }

        ///LOGOUT///---------------------------------------------
        public ActionResult ViewProfile()
        {
            return RedirectToAction("Index1","Home");
        }

        ///ADD REVIEW ///-----------------------------------------
        private Db_Rates db2 = new Db_Rates();
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReview(Models.Rate.Rate rate, int? id)
        {

            if (ModelState.IsValid)
            {    
                try
                {
                    var checkId = Session["idUser"];
                    if ((int)id==null||(int)checkId==null) return RedirectToAction("Index1", "Home");//Should catch the exception if object is null or it will always be true this is a hacky way to check thing but too bad im depressed and sleepy and it's 3am HAHAHAHHAHAHAHA
                }
                catch(System.InvalidOperationException){
                    return RedirectToAction("Index1", "Home");
                }
                var userId = Session["idUser"];
                rate.IDRestaurant = (int)id;
                rate.IDUser = (int)userId;
                rate.IDReview++;
                db2.Configuration.ValidateOnSaveEnabled = true;
                db2.Rates.Add(rate);
                db2.SaveChanges();
                return RedirectToAction("Index1", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes.Try again.");
                return View();
            }
            return View();
        }
        public ActionResult CreateReview()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkId = Session["idUser"];
                    if ((int)id == null || (int)checkId == null) return RedirectToAction("Index1", "Home");//Should catch the exception if object is null or it will always be true this is a hacky way to check thing but too bad im depressed and sleepy and it's 3am HAHAHAHHAHAHAHA
                }
                catch (System.InvalidOperationException)
                {
                    return RedirectToAction("Index1", "Home");
                }
                var userId = Session["idUser"];
                var rates = db2.Rates.AsQueryable();
                if((int)id != null)
{
                    rates = rates.Where(s => s.IDRestaurant == (int)id);
                }
                if ((int)userId != null)
                {
                    rates = rates.Where(s => s.IDUser == (int)userId);
                }
                foreach (var item in rates)
                {
                    //db2.Rates.Find(rate);
                    db2.Configuration.ValidateOnSaveEnabled = true;
                    db2.Rates.Remove(item);
                }
                db2.SaveChanges();
                return RedirectToAction("Index1","Home");
            }
            else
            {
                return View();
            }
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index1","Home");
            }
            Restaurant restaurant = db1.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            ViewBag.restaurantId = (int)id;
            return View(restaurant);
        }
    }
}