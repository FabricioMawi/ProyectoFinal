using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prueba1.Models;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Web.Services.Description;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.IO;

namespace prueba1.Controllers
{
    public class ProfesorsController : Controller
    {
       
        private static Random random = new Random();
        private prueba1Entities db = new prueba1Entities();

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
                    "¡Su cuenta ha sido registrada exitosamente!, su contraseña temporal es: " + pass +
                    ". Favor iniciar sesión en nuestra página y cambiar la contraseña");

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


        // GET: Profesors
        public ActionResult Index(string sortOrder, string searchString)
        {

            var session = (Models.Profesor)Session["User"];
            var rol = (Models.Profesor)Session["Rol"];

            if (session == null )
            {
                return RedirectToAction("Login", "Home");
            }
            else if (session.rol != "Administrador")
            {
                return RedirectToAction("Index", "Home"); 
            }

            else
            {
                    var profesores = from s in db.Profesor
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                profesores = profesores.Where(s =>
                                       s.nombreProfesor.Contains(searchString)
                                       || s.apellidoProfesor.Contains(searchString)
                                       || s.rol.Contains(searchString)
                                       || s.nombreUsuario.Contains(searchString)
                                       || s.emailProfesor.Contains(searchString)
                                       || s.estado.Contains(searchString));



            }
            

            return View(profesores.ToList());
            }
        }
    

        // GET: Profesors/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var session = (Models.Profesor)Session["User"];



            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (session.rol != "Administrador")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesor profesor = await db.Profesor.FindAsync(id);
            if (profesor == null)
            {
                return HttpNotFound();
            }
            return View(profesor);
            }
        }

        // GET: Profesors/Create
        public ActionResult Create()
        {
            var session = (Models.Profesor)Session["User"];
            var rol = (Models.Profesor)Session["Rol"];



            if (session == null && rol == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (session.rol != "Administrador")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        // POST: Profesors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idProfesor,nombreProfesor,apellidoProfesor,rol,contrasena,nombreUsuario,emailProfesor,estado")] Profesor profesor)
        {
            int profesorActivo = (from d in db.Profesor where d.emailProfesor == profesor.emailProfesor select d.idProfesor).FirstOrDefault();
            if(profesorActivo != 0)
            {
                ViewBag.Error = "Este correo ya se encuentra registrado";
                return View(profesor);
            }
            ViewBag.Error = profesorActivo;

            String pass = RandomString(5);
            String passEncrypt = Encripta.GetSHA256(pass);
            profesor.contrasena = passEncrypt;
            if (ModelState.IsValid)
            {
                sendEmail(profesor.emailProfesor, pass);
                //encriptar aqui 
                db.Profesor.Add(profesor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           

            return View(profesor);
        }

        // GET: Profesors/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var session = (Models.Profesor)Session["User"];



            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (session.rol != "Administrador")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesor profesor = await db.Profesor.FindAsync(id);
            if (profesor == null)
            {
                return HttpNotFound();
            }
            return View(profesor);
            }
        }

        // POST: Profesors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idProfesor,nombreProfesor,apellidoProfesor,rol,contrasena,nombreUsuario,emailProfesor,estado")] Profesor profesor)
        {

            int profesorActivo = (from d in db.Grupo where d.idProfesor == profesor.idProfesor select d.idProfesor).FirstOrDefault();
            if (profesor.estado == "Inactivo")
            {
                if (profesorActivo != 0)
                {
                    ViewBag.Error = "Este profesor tiene cursos asignados";
                    return View(profesor);
                }

            }
           
            if (ModelState.IsValid)
            {
               
                db.Entry(profesor).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(profesor);
         }
        

        // GET: Profesors/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var session = (Models.Profesor)Session["User"];



            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (session.rol != "Administrador")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesor profesor = await db.Profesor.FindAsync(id);
            if (profesor == null)
            {
                return HttpNotFound();
            }
            return View(profesor);
            }
        }

        // POST: Profesors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
           
           
            int profesorActivo = (from d in db.Grupo where d.idProfesor == id select d.idGrupo).FirstOrDefault();

            if ( profesorActivo != 0)
            {
                Profesor profesor = await db.Profesor.FindAsync(id);
                ViewBag.Error = "Este Profesor tiene grupos asignados no puede ser eliminado";
                return View(profesor);
            }
            else
            {
                
                Profesor profesor = await db.Profesor.FindAsync(id);
                db.Profesor.Remove(profesor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");

            }
        }


        public ActionResult Reporte(int? id)
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (session.rol != "Administrador")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Profesor profesor = db.Profesor.Find(id);

                if (profesor == null)
                {
                    return HttpNotFound();
                }
                int idProfesor = session.idProfesor;
                var gen = (from d in db.Profesor
                           where d.estado == "Activo"
                           select d.nombreProfesor).ToList();


                return View(profesor);
            }
        }

        public ActionResult createReport(string tipo, int id)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), "ReportProfesor.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }

            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);

            SqlCommand cmdProf = new SqlCommand();
            SqlCommand cmdProy = new SqlCommand();
            SqlCommand cmdGrupo = new SqlCommand();
            SqlCommand cmdEstud = new SqlCommand();

            cmdProf.Connection = cnn;
            cmdProy.Connection = cnn;
            cmdGrupo.Connection = cnn;
            cmdEstud.Connection = cnn;

            cmdProf.CommandType = System.Data.CommandType.StoredProcedure;
            cmdProy.CommandType = System.Data.CommandType.StoredProcedure;
            cmdGrupo.CommandType = System.Data.CommandType.StoredProcedure;
            cmdEstud.CommandType = System.Data.CommandType.StoredProcedure;


            cmdProf.CommandText = "selectProfesor1";
            cmdProy.CommandText = "selectProyecto1";
            cmdGrupo.CommandText = "selectGrupo1";
            cmdEstud.CommandText = "selectEstudiante";

            cmdProf.Parameters.Add("@idProfesor", SqlDbType.Int).Value = id;
            cmdProy.Parameters.Add("@idProfesor", SqlDbType.Int).Value = id;
            cmdGrupo.Parameters.Add("@idProfesor", SqlDbType.Int).Value = id;
            cmdEstud.Parameters.Add("@idProfesor", SqlDbType.Int).Value = id;

            List<Proyecto> proyect = new List<Proyecto>();
            List<Profesor> professor = new List<Profesor>();
            List<Grupo> group = new List<Grupo>();
            List<Estudiante> student = new List<Estudiante>();

            cnn.Open();
            using (SqlDataReader dr = cmdProf.ExecuteReader())
            {
                while (dr.Read())
                {
                    Profesor newItem = new Profesor();
                    newItem.nombreProfesor = dr.GetString(0);
                    newItem.apellidoProfesor = dr.GetString(1);
                    newItem.rol = dr.GetString(2);
                    newItem.nombreUsuario = dr.GetString(3);
                    newItem.emailProfesor = dr.GetString(4);
                    newItem.estado = dr.GetString(5);
                    professor.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdProy.ExecuteReader())
            {
                while (dr.Read())
                {
                    Proyecto newItem = new Proyecto();
                    newItem.nombreProyecto = dr.GetString(0);
                    newItem.tecnologia = dr.GetString(1);
                    newItem.fechaInicio = dr.GetDateTime(2);
                    newItem.idCurso = dr.GetInt32(3);
                    newItem.idGrupo = dr.GetInt32(4);
                    proyect.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdGrupo.ExecuteReader())
            {
                while (dr.Read())
                {
                    Grupo newItem = new Grupo();
                    newItem.idGrupo = dr.GetInt32(0);
                    newItem.nombreGrupo = dr.GetString(1);
                    newItem.sede = dr.GetString(2);
                    newItem.cuatrimestre = dr.GetString(3);
                    group.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdEstud.ExecuteReader())
            {
                while (dr.Read())
                {
                    Estudiante newItem = new Estudiante();
                    newItem.nombreEstudiante = dr.GetString(0);
                    student.Add(newItem);
                }
            }
            cnn.Close();


            ReportDataSource rdProf = new ReportDataSource("DataSetProfesor", professor);
            ReportDataSource rdProy = new ReportDataSource("DataSetProyecto", proyect);
            ReportDataSource rdGrou = new ReportDataSource("DataSetGrupo", group);
            ReportDataSource rdStud = new ReportDataSource("DataSetEstudiante", student);

            lr.DataSources.Add(rdProf);
            lr.DataSources.Add(rdProy);
            lr.DataSources.Add(rdGrou);
            lr.DataSources.Add(rdStud);

            string reportType = tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType);
        }
    }
}



