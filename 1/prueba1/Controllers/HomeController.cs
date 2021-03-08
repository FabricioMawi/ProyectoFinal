using Microsoft.Ajax.Utilities;
using prueba1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Net.Mail;


namespace prueba1.Controllers
{
    public class HomeController : Controller
    {
        private prueba1Entities db = new prueba1Entities();
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void sendEmail(string correo, string pass)
        {
            try
            {
                MailMessage mensaje = new MailMessage("portalAnalisis@gmail.com", correo, "Registro exitoso",
                    "Se solicito un cambio de contraseña, su nueva contraseña es " + pass +
                    ". Favor iniciar sesión en el sistema y cambiar la contraseña");

                SmtpClient server = new SmtpClient("smtp.gmail.com");
                server.EnableSsl = true;

                server.UseDefaultCredentials = false;
                server.Port = 587;
                server.Credentials = new System.Net.NetworkCredential("portalAnalisis@gmail.com", "portalanalisis123");
                server.Send(mensaje);

            }
            catch (Exception e)
            {
                throw;
            }


        }
        public ActionResult Index(String message)
        {
            var session = (Models.Profesor)Session["User"];
            

            if (session == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View("Index");
            }

            


        }
        public ActionResult Login()
        {
            return View("Login");
        }
        public ActionResult RecuperarContraseña()
        {
            return View("RecuperarContraseña");
        }
        public ActionResult CambiarContraseña()
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View("CambiarContraseña");
            }
        }
        [HttpPost]
        public ActionResult CambiarContraseña(string contrasena)
        {
            var session = (Models.Profesor)Session["User"];
            try
            {
                using (prueba1Entities db = new prueba1Entities())
                {
                    var user = (from d in db.Profesor
                                where d.idProfesor == session.idProfesor select d).FirstOrDefault();

                    if (user == null)
                    {
                     
                        return View("CambiarContraseña");
                    }

                    user.contrasena = Encripta.GetSHA256(contrasena);


                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }


            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                String pass = Encripta.GetSHA256(password);
                var user = db.Profesor.FirstOrDefault(e => e.emailProfesor == email && e.contrasena == pass);
                


                if (user != null)
                {
                    
                    Session["User"] = user;
                    Session["Nombre"] = user.nombreProfesor +" "+ user.apellidoProfesor;
                    Session["RolVista"] = user.rol;




                    return RedirectToAction("Index");

                }
                else
                {

                    ViewBag.Error = "No se encuentra un usuario";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.Error = "Debes llenar los campos para iniciar sesión";
                return View("Login");
                
            }

        }
        [HttpPost]
        public ActionResult RecuperarContraseña(string correo)
        {
            try
            {
                using (prueba1Entities db = new prueba1Entities())
                {
                    var user = (from d in db.Profesor
                                 where d.emailProfesor == correo && d.estado == "Activo"
                                 select d).FirstOrDefault();

                    if (user == null)
                    {
                        ViewBag.Error = "No se encuentra ningún usuario registrado o activo con la dirección email: " + correo;
                        return View("RecuperarContraseña");
                    }

                    string contrasena = RandomString(6);

                    MailMessage mensaje = new MailMessage("portalAnalisis@gmail.com", correo, "Cambio de Contraseña",
                   "Se solicito un cambio de contraseña, su nueva contraseña es " + contrasena +
                   ". Favor iniciar sesión en el sistema y cambiar la contraseña");

                    SmtpClient server = new SmtpClient("smtp.gmail.com");
                    server.EnableSsl = true;

                    server.UseDefaultCredentials = false;
                    server.Port = 587;
                    server.Credentials = new System.Net.NetworkCredential("portalAnalisis@gmail.com", "portalanalisis123");
                    server.Send(mensaje);


                    user.contrasena = Encripta.GetSHA256(contrasena);
                    

                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Login");

                }


            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        public ActionResult Logout()
        {
            
            Session["User"] = null;
            Session["Nombre"] = null;
            return RedirectToAction("Login", "Home");
        }

    }
}