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
                    _user.Type = 0;
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
                    Session["Type"] = data.FirstOrDefault().Type;
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
                records = records.Where(x => x.Address.Contains(search) || search == null);
            }
            else if (option == "Description")
            {
                records = records.Where(x => x.Decription.Contains(search)|| search == null);
            }
            else if (option == "Rate")
            {
                records = records.Where(x => x.Rate==rate || search == null);
            }

            else
            {
                records = records.Where(x => x.Name.Contains(search) || search == null);
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

            return View(records.ToPagedList(pageNumber ?? 1, 100));
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
                    if ((int)id==null||(int)checkId==null) return RedirectToAction("Index1", "Home");
                    //Should catch the exception if object is null or it will always be true 
                }
                catch (System.NullReferenceException)
                {
                    return RedirectToAction("Index", "Home");
                }
                catch (System.InvalidOperationException){
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
                    if ((int)id == null || (int)checkId == null) return RedirectToAction("Index1", "Home");
                    //Should catch the exception if object is null or it will always be true 
                }
                catch (System.NullReferenceException)
                {
                    return RedirectToAction("Index", "Home");
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

        ///View Detail-------------------
        public ActionResult Details(int? id)
        {
            Db_Restaurants db1 = new Db_Restaurants();
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

        public ActionResult ReviewDetail(int? id)
        {
            Db_Rates db2 = new Db_Rates();
            if (id == null)
            {
                return RedirectToAction("Index1", "Home");
            }
            Rate rate = db2.Rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            ViewBag.rateId = (int)id;

            return View(rate);
        }

        public ActionResult ORestaurantManage(int? pageNumber)
        {
            try
            {
                var checkId = Session["idUser"];
                var typeId = Session["Type"];
                if ((int)typeId == null || (int)checkId == null || (int)typeId != 1) return RedirectToAction("Index", "Home");
                //Should catch the exception if object is null or it will always be true 
            }
            catch(System.NullReferenceException)
            {
                return RedirectToAction("Index", "Home");
            }
            catch (System.InvalidOperationException)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = Session["idUser"];
            var records = db1.Restaurants.AsQueryable();
            records = records.OrderBy(x => x.IDUser == (int)userId);
            records = records.Where(x => x.IDUser == (int)userId);
            return View(records.ToPagedList(pageNumber ?? 1, 100));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ORestaurantCreate(Models.Restaurant.Restaurant restaurant)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var checkId = Session["idUser"];
                    var typeId = Session["Type"];
                    if ((int)typeId == null || (int)checkId == null || (int)typeId != 1) return RedirectToAction("Index", "Home");
                    //Should catch the exception if object is null or it will always be true 
                }
                catch (System.NullReferenceException)
                {
                    return RedirectToAction("Index", "Home");
                }
                catch (System.InvalidOperationException)
                {
                    return RedirectToAction("Index", "Home");
                }
                var userId = Session["idUser"];
                restaurant.IDUser = (int)userId;
                restaurant.Rate = 0;
                restaurant.ID++;
                db1.Configuration.ValidateOnSaveEnabled = true;
                db1.Restaurants.Add(restaurant);
                db1.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes.Try again.");
                return View();
            }
            return View();
        }
        public ActionResult ORestaurantCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ORestaurantDelete(int? id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkId = Session["idUser"];
                    var typeId = Session["Type"];
                    if ((int)typeId == null || (int)checkId == null || (int)typeId != 1 || (int) id == null) return RedirectToAction("Index", "Home");
                    //Should catch the exception if object is null or it will always be true 
                }
                catch (System.NullReferenceException)
                {
                    return RedirectToAction("Index", "Home");
                }
                catch (System.InvalidOperationException)
                {
                    return RedirectToAction("Index", "Home");
                }
                var userId = Session["idUser"];
                var restaurant = db1.Restaurants.AsQueryable();
                var rate = db2.Rates.AsQueryable();
                if ((int)userId != null)
                {
                    restaurant = restaurant.Where(s => s.IDUser == (int)userId);
                }
                if((int)id != null)
                {
                    restaurant = restaurant.Where(s => s.ID == (int)id);
                }
                foreach (var item in restaurant)
                {
                    //db2.Rates.Find(rate);
                    db1.Configuration.ValidateOnSaveEnabled = true;
                    db1.Restaurants.Remove(item);
                }
                if ((int)id != null)
                {
                    rate = rate.Where(s => s.IDRestaurant == (int)id);
                }
                foreach (var item in rate)
                {
                    //db2.Rates.Find(rate);
                    db2.Configuration.ValidateOnSaveEnabled = true;
                    db2.Rates.Remove(item);
                }
                db2.SaveChanges();
                db1.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
            return View();
        }
        public ActionResult ORestaurantDelete()
        {
            return View();
        }

        public ActionResult ARestaurantManage(string option, string search, double? rate, int? pageNumber, string sort)
        {
            try
            {
                var checkId = Session["idUser"];
                var typeId = Session["Type"];
                if ((int)typeId == null || (int)checkId == null ||(int)typeId != 2) return RedirectToAction("Index", "Home");
                //Should catch the exception if object is null or it will always be true 
            }
            catch (System.NullReferenceException)
            {
                return RedirectToAction("Index", "Home");
            }
            catch (System.InvalidOperationException)
            {
                return RedirectToAction("Index", "Home");
            }
            //if the sort parameter is null or empty then we are initializing the value as descending name  
            ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";
            //if the sort value is gender then we are initializing the value as descending gender  
            ViewBag.SortByDescription = sort == "Description" ? "descending description" : "Description";

            //here we are converting the Db1 Restaurant to AsQueryable => we can invoke all the extension methods on variable records.  
            var records = db1.Restaurants.AsQueryable();

            //if a user choose the radio button option as Description  

            if (option == "Address")
            {
                records = records.Where(x => x.Address.Contains(search) || search == null);
            }
            else if (option == "Description")
            {
                records = records.Where(x => x.Decription.Contains(search) || search == null);
            }
            else if (option == "Rate")
            {
                records = records.Where(x => x.Rate == rate || search == null);
            }

            else
            {
                records = records.Where(x => x.Name.Contains(search) || search == null);
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

            return View(records.ToPagedList(pageNumber ?? 1, 100));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ARestaurantDelete(int? id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkId = Session["idUser"];
                    if ((int)id == null || (int)checkId == null) return RedirectToAction("Index", "Home");
                    //Should catch the exception if object is null or it will always be true
                }
                catch (System.NullReferenceException)
                {
                    return RedirectToAction("Index", "Home");
                }
                catch (System.InvalidOperationException)
                {
                    return RedirectToAction("Index", "Home");
                }
                var userId = Session["idUser"];
                var restaurant = db1.Restaurants.AsQueryable();
                if ((int)id != null)
                {
                    restaurant = restaurant.Where(s => s.ID == (int)id);
                }
                foreach (var item in restaurant)
                {
                    //db2.Rates.Find(rate);
                    db1.Configuration.ValidateOnSaveEnabled = true;
                    db1.Restaurants.Remove(item);
                }
                var rates = db2.Rates.AsQueryable();
                if ((int)id != null)
                {
                    rates = rates.Where(s => s.IDRestaurant == (int)id);
                }
                foreach (var item in rates)
                {
                    //db2.Rates.Find(rate);
                    db2.Configuration.ValidateOnSaveEnabled = true;
                    db2.Rates.Remove(item);
                }
                db2.SaveChanges();
                db1.SaveChanges();
                return RedirectToAction("ARestaurantManage", "Home");
            }
            else
            {
                return View();
            }
            return View();
        }
        public ActionResult ARestaurantDelete()
        {
            return View();
        }

        ///CREATE RESTAURANT--------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRestaurant(Models.Restaurant.Restaurant restaurant, int? id)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var checkId = Session["idUser"];
                    if ((int)id == null || (int)checkId == null) return RedirectToAction("Index1", "Home");
                    //Should catch the exception if object is null or it will always be true 
                }
                catch (System.InvalidOperationException)
                {
                    return RedirectToAction("Index1", "Home");
                }
                var userId = Session["idUser"];
                restaurant.ID = (int)id;
                restaurant.IDUser = (int)userId;
                restaurant.ID++;
                db1.Configuration.ValidateOnSaveEnabled = true;
                db1.Restaurants.Add(restaurant);
                db1.SaveChanges();
                return RedirectToAction("Index1", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes.Try again.");
                return View();
            }
            return View();
        }
        public ActionResult CreateRestaurant()
        {
            return View();
        }
    }
}