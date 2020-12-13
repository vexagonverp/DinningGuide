using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using Dinning_Guide.Models.User;
using Dinning_Guide.Models.Restaurant;
using PagedList;

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
                if (search!=null)return RedirectToAction("Index1",new {option="Name",search=search,pageNumber=1,sort= "descending name" });
                return View("../Home/Index");
                
            }
            else
            {
                //return RedirectToAction("Login");
                if (search != null)return RedirectToAction("Index1", new { option = "Name", search = search, pageNumber = 1, sort = "descending name" });
                return View("../Home/Index");
                
            }
        }

        public ActionResult About()
        {
            return View("../Home/About");
        }

        public ActionResult Contact()
        {
            return View("../Home/Contact");
        }

        //GET: Register
        public ActionResult Register()
        {
            return View("../Home/Register");
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
                    _user.idUser++ ;
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Users.Add(_user);
                    _db.SaveChanges();
                    return RedirectToAction("../Home/Index");
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
            return View("../Home/Login");
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
                    return RedirectToAction("../Home/Index");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("../Home/Login");
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("../Home/Login");
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
        public ActionResult Index1(string option, string search, int? pageNumber, string sort)
        {
            //if the sort parameter is null or empty then we are initializing the value as descending name  
            ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";
            //if the sort value is gender then we are initializing the value as descending gender  
            ViewBag.SortByDescription = sort == "Description" ? "descending description" : "Description";

            //here we are converting the Db1 Restaurant to AsQueryable => we can invoke all the extension methods on variable records.  
            var records = db1.Restaurants.AsQueryable();

            //if a user choose the radio button option as Description  
            if (option == "Description")
            {
                records = records.Where(x => x.Description == search || search == null);
            }
            else if (option == "Address")
            {
                records = records.Where(x => x.Address == search || search == null);
            }
            else if (option == "Rate")
            {
                records = records.Where(x => x.Rate == search || search == null);
            }
            else if (option == "Review")
            {
                records = records.Where(x => x.Review == search || search == null);
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

                case "descending description":
                    records = records.OrderByDescending(x => x.Description);
                    break;

                case "descending rate":
                    records = records.OrderByDescending(x => x.Rate);
                    break;

                case "Description":
                    records = records.OrderBy(x => x.Description);
                    break;

                default:
                    records = records.OrderBy(x => x.Name);
                    break;

            }

            return View(records.ToPagedList(pageNumber ?? 1, 3));
        }

        //GET: Detail
        public ActionResult Details(int ID)
        {
            return View();
        }
    }
}