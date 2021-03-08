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

namespace prueba1.Controllers
{
    public class GrupoController : Controller
    {
        private prueba1Entities db = new prueba1Entities();

        // GET: Grupo
        public async Task<ActionResult> Index(string sortOrder, string searchString)
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
                var grupoes = db.Grupo.Include(g => g.Curso).Include(g => g.Profesor);
                return View(await grupoes.ToListAsync());

               
            }
        }

        // GET: Grupo/Details/5
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
            Grupo grupo = await db.Grupo.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            return View(grupo);
            }

        }

        // GET: Grupo/Create
        public ActionResult Create()
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

            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso");
            cargarProfesores();
            return View();
            }
        }

        // POST: Grupo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "nombreGrupo,idGrupo,idProfesor,sede,idCurso,cuatrimestre")] Grupo grupo)
        {
            if (ModelState.IsValid)
            {
                db.Grupo.Add(grupo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso", grupo.idCurso);
            cargarProfesores();
            return View(grupo);
        }

        // GET: Grupo/Edit/5
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
            Grupo grupo = await db.Grupo.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso", grupo.idCurso);
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor", grupo.idProfesor);
            return View(grupo);
            }
        }

        // POST: Grupo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "nombreGrupo,idGrupo,idProfesor,sede,idCurso,cuatrimestre")] Grupo grupo)
        {
            var session = (Models.Profesor)Session["User"];

            //agregue el nombreGrupo ver si se esta mandando

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
                if (ModelState.IsValid)
            {
               
                    db.Entry(grupo).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                
            }
            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso", grupo.idCurso);
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor", grupo.idProfesor);
            return View(grupo);
            }
        }

        // GET: Grupo/Delete/5
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
            Grupo grupo = await db.Grupo.FindAsync(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            return View(grupo);
            }
        }

        // POST: Grupo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            int grupoActivo = (from d in db.Proyecto where d.idGrupo == id select d.idGrupo).FirstOrDefault();

            if (grupoActivo != 0)
            {
                Grupo grupo = await db.Grupo.FindAsync(id);
                ViewBag.Error = "Este Grupo tiene proyectos asignados no puede ser eliminado";
                return View(grupo);
            }
            else
            {

                Grupo grupo = await db.Grupo.FindAsync(id);
                db.Grupo.Remove(grupo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");

            }
           
        }

        public void cargarProfesores()
        {
            var session = (Models.Profesor)Session["User"];
            int idProfesor = session.idProfesor;
            prueba1Entities db = new prueba1Entities();
            List<Profesor> list = (from d in db.Profesor
                                   where d.estado == "Activo"
                                   select d).ToList();

            List<SelectListItem> lst = list.ConvertAll(d =>
            {
                return new SelectListItem
                {
                    Text = d.nombreProfesor,
                    Value = d.idProfesor.ToString()
                };

            });
            ViewBag.idProfesor = new SelectList(lst, "Value", "Text");
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
