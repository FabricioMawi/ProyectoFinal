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
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;


namespace prueba1.Controllers
{
    public class EstudiantesController : Controller
    {
        private prueba1Entities db = new prueba1Entities();

        OleDbConnection Econ;
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        // GET: Estudiantes
        public async Task<ActionResult> Index()
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
            var estudiante = db.Estudiante.Include(e => e.Grupo).Include(e => e.Profesor);
            return View(await estudiante.ToListAsync());
            }
        }

        // GET: Estudiantes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = await db.Estudiante.FindAsync(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            return View(estudiante);
            }
        }

        //GET: Estudiantes/Create
        public ActionResult Create()
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "nombreGrupo");
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor");
            return View();
            }
        }

        public ActionResult CreateExcel()
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateExcel(HttpPostedFileBase file)
        {
        
                try
            {
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/ExcelFolder/" + filename;
                file.SaveAs(Path.Combine(Server.MapPath("/ExcelFolder"), filename));
                InsertExcelData(filepath, filename);
                return View();
            } 
            catch (Exception e)
            {
                return View("Error");
            }
            
        }

        private void ExcelConn(string filepath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", filepath);
            Econ = new OleDbConnection(constr);
        }

        private void InsertExcelData(string filepath, string filename)
        {
            string fullpath = Server.MapPath("/ExcelFolder/") + filename;
            ExcelConn(fullpath);
            string query = string.Format("Select * from [{0}]", "Hoja1$");
            OleDbCommand Ecommand = new OleDbCommand(query, Econ);

            Econ.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter odbadapter = new OleDbDataAdapter(query, Econ);
            Econ.Close();

            odbadapter.Fill(ds);
            DataTable datata = ds.Tables[0];
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.DestinationTableName = "Estudiante";

            objbulk.ColumnMappings.Add("Estudiante", "nombreEstudiante");

            con.Open();
            objbulk.WriteToServer(datata);
            con.Close();

        }


        // POST: Estudiantes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idEstudiante,nombreEstudiante,idGrupo,idProfesor,nota")] Estudiante estudiante)
        {
       

                if (ModelState.IsValid)
            {
                db.Estudiante.Add(estudiante);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "nombreGrupo", estudiante.idGrupo);
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor", estudiante.idProfesor);
            return View(estudiante);
            
        }

        // GET: Estudiantes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = await db.Estudiante.FindAsync(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "nombreGrupo", estudiante.idGrupo);
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor", estudiante.idProfesor);
            return View(estudiante);
            }
        }

        // POST: Estudiantes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idEstudiante,nombreEstudiante,idGrupo,idProfesor,nota")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estudiante).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "nombreGrupo", estudiante.idGrupo);
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor", estudiante.idProfesor);
            return View(estudiante);
        }

        // GET: Estudiantes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = await db.Estudiante.FindAsync(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            return View(estudiante);
            }
        }

        // POST: Estudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Estudiante estudiante = await db.Estudiante.FindAsync(id);
            db.Estudiante.Remove(estudiante);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
