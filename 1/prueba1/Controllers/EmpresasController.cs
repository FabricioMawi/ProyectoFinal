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
using Microsoft.Reporting.WebForms;
using System.IO;

namespace prueba1.Controllers
{
    public class EmpresasController : Controller
    {
        private prueba1Entities db = new prueba1Entities();

        // GET: Empresas
        public ActionResult Index(string sortOrder, string searchString)
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var empresas = from s in db.Empresa
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                empresas = empresas.Where(s => s.nombreEmpresa.Contains(searchString)
                                       || s.nombreContacto.Contains(searchString)
                                       || s.telefono.ToString().Contains(searchString)
                                       || s.email.Contains(searchString)
                                       || s.tipoEmpresa.ToString().Contains(searchString));
            }

            return View(empresas.ToList());
            }
        }

        // GET: Empresas/Details/5
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
            Empresa empresa = await db.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
            }
        }

        // GET: Empresas/Create
        public ActionResult Create()
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

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idContacto,nombreEmpresa,nombreContacto,telefono,email,tipoEmpresa")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                db.Empresa.Add(empresa);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(empresa);
        }

        // GET: Empresas/Edit/5
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
            Empresa empresa = await db.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
            }
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idContacto,nombreEmpresa,nombreContacto,telefono,email,tipoEmpresa")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(empresa).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(empresa);
        }

        // GET: Empresas/Delete/5
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
            Empresa empresa = await db.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
            }
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            int empresaActiva = (from d in db.Proyecto where d.idContacto == id select d.idProyecto).FirstOrDefault();

            if (empresaActiva != 0)
            {
                Empresa empresa = await db.Empresa.FindAsync(id);
                ViewBag.Error = "Esta Empresa tiene proyectos asignados no puede ser eliminada";
                return View(empresa);
            }
            else
            {

                Empresa empresa = await db.Empresa.FindAsync(id);
                db.Empresa.Remove(empresa);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");

            }
        
        }

        public ActionResult Reporte()
        {
            var session = (Models.Profesor)Session["User"];
            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var empresa = from s in db.Empresa
                                select s;

                return View(empresa.ToList());
            }
        }

        public ActionResult Report(string id)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), "ReportEmpresa.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }
            List<Empresa> cm = new List<Empresa>();
            using (prueba1Entities dc = new prueba1Entities())
            {
                cm = dc.Empresa.ToList();
            }
            ReportDataSource rd = new ReportDataSource("DataSetEmpresa", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
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
