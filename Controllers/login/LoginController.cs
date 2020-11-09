using Kurs.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kurs.Controllers.login
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }


        [HttpGet]
   
        public ActionResult ResetPassword()
        {
            return View();
        }



        [HttpPost]
        public ActionResult ResetPassword(UserModal user, string ReturnUrl)
        {
            using (KursEntities db = new KursEntities())
            {
                var myuser = db.Users.Where(e => e.Emial == user.Email).FirstOrDefault();
          
                if (myuser != null)
                {
                    ResetPassword rs = new ResetPassword();

                    rs.UserID = myuser.ID;
                    Guid CodeID = Guid.NewGuid();
                    rs.Code = CodeID.ToString();
                    db.ResetPasswords.Add(rs);
                    db.SaveChanges();

                    string Name = myuser.Name;
                    string Body = "لاستعادة كلمة المرور اضفط على الزر";
                    string BottunName = "اضغط هنا";
                    string link = System.Web.HttpContext.Current.Request.Url.Host+"/login/resetpass?code="+rs.Code;
                    string website = System.Web.HttpContext.Current.Request.Url.Host + "Web Site";

               

                    MailMessage mail = new MailMessage();
                    SmtpClient client = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress("mohammad33habbaba@gmail.com");
                    mail.To.Add(user.Email);
                    mail.Subject = "استعادة كلمة المرور ";
                    mail.Body = Functions.CreatEmailBody(Name, Body, link, BottunName, website);
                    mail.IsBodyHtml = true;
                    client.Port = 587;
                    client.Credentials = new System.Net.NetworkCredential("mohammad33habbaba@gmail.com", "Mohammad33ha");
                    client.EnableSsl = true;
                    client.Send(mail);
                    client.Dispose();

                    ModelState.AddModelError("Login Error", "تم ارسال رابط تعديل كلمة المرور الى الايميل الخاص بك بنجاح");



                    return View();
                }
                

          
            else
            {

                ModelState.AddModelError("Login Error", "يرجى التأكد من الايميل");
                return View();
            }

            }


        }

        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            Session.Clear();
            return Redirect(Url.Action("Index", "Home"));

        }
        [HttpPost]
        public ActionResult Login(UserModal user, string ReturnUrl)
        {
            if (isvalid(user))
            {

                FormsAuthentication.SetAuthCookie(user.Email, true);

                return RedirectToAction("LoginTest", "Login");


            }
            else
            {

                ModelState.AddModelError("Login Error", " الايميل او كلمة المرور غير صحيحة");
                return View();
            }
        }
        private bool isvalid(UserModal user)
        {
            using (KursEntities DB = new KursEntities())
            {
                var myuser = DB.Users.Where(e => e.Emial == user.Email && e.Password == user.Password && e.Active ==1).FirstOrDefault();
                if (myuser == null)
                {
                    UserInfo.ID = 0; 
                    return false;
                }
                else
                {
                    Session["userID"] = myuser.ID.ToString();
                     Session["UserType"] = myuser.UserTaype.ToString();
                    return (user.Email == myuser.Emial && user.Password == myuser.Password);


                }
            }

        }
        public ActionResult Header()
        {
            if (Session["userID"] == null)
                return Redirect("/login/login");
            KursEntities db = new KursEntities();
            List<User> user = new List<User>();
      
            int id = int.Parse(Session["userID"].ToString());
            user = db.Users.Where(e => e.ID == id).Select(e => e).ToList();
            return PartialView("Header", user);
        }
    
           
        
        public ActionResult LoginTest()
        {


            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("index", "Classes");
            }
            else if (User.IsInRole("Teacher"))
            {

                return RedirectToAction("index", "TeacherHome");
            }
            else if (User.IsInRole("Student"))
            {

                return RedirectToAction("index", "StdHome");
            }
            else
            {
                return RedirectToAction("login", "login");


            }


        }

        [HttpPost]
        public ActionResult ResetPass([Bind(Include = "Password")] User users, string code)
        {
            using (KursEntities db = new KursEntities())
            {
                var myuser = db.ResetPasswords.Where(e => e.Code == code).FirstOrDefault();

                if (myuser != null)
                {
                    User rs = db.Users.Find(myuser.UserID);
                    rs.Password = users.Password;
                    db.Entry(rs).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.AddModelError("Login Error", "تم التعديل بنجاح يمكنك تسجيل الدخول بكلمة السر الجديدة");

                    db.ResetPasswords.RemoveRange(db.ResetPasswords.Where(x => x.UserID == myuser.UserID));
                    db.SaveChanges();

                    return View();

                }

               else
                {
                    ModelState.AddModelError("Login Error", "حدث خطاء اثناء تحديث كلمة المرور");

                    return View();
                }

            }

        }

        public ActionResult ResetPass(string code)
        {
            if (code == null)
            {
                return RedirectToAction("login");
            }
            return View();

    

        }






        // Functions
        public static string CreateCode(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }



    }
}