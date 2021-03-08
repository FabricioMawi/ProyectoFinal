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

namespace prueba1.Controllers
{
    public class ComentariosController : Controller
    {
        private prueba1Entities db = new prueba1Entities();

        // GET: Comentarios
        public async Task<ActionResult> Index()
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var comentarios = db.Comentario.Include(c => c.Proyecto);
                return View(await comentarios.ToListAsync());
            }


        }

        // GET: Comentarios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comentario comentario = await db.Comentario.FindAsync(id);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            return View(comentario);
        }

        // GET: Comentarios/Create
        public ActionResult Create()
        {
            var session = (Models.Profesor)Session["User"];
            

            
            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                int idProfesor = session.idProfesor;
       

                var gen = (from d in db.Proyecto
                           where d.estadoProyecto == "Activo" && d.idProfesor == idProfesor
                           select d.nombreProyecto).ToList();



                cargarProyectos();

               

                return View();
        }
        }

        // POST: Comentarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idComentario,fecha,contenido,idProyecto")] Comentario comentario)
        {
            var session = (Models.Profesor)Session["User"];

            
            if (ModelState.IsValid)
            {
               
               
                db.Comentario.Add(comentario);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
                int idProfesor = session.idProfesor;
                var proyecto = (from d in db.Proyecto
                                where d.estadoProyecto == "Activo" && d.idProfesor == idProfesor
                                select d.idProyecto);
            
            int hola = comentario.idComentario;
            ViewBag.Error = hola;
            ViewBag.idProyecto = new SelectList(proyecto);
                
            return View(comentario);
            }
        

        // GET: Comentarios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comentario comentario = await db.Comentario.FindAsync(id);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProyecto = new SelectList(db.Proyecto, "idProyecto", "nombreProyecto", comentario.idProyecto);
            return View(comentario);
        }

        // POST: Comentarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idComentario,fecha,contenido,idProyecto")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comentario).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idProyecto = new SelectList(db.Proyecto, "idProyecto", "nombreProyecto", comentario.idProyecto);
            return View(comentario);
        }

        // GET: Comentarios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comentario comentario = await db.Comentario.FindAsync(id);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Comentario comentario = await db.Comentario.FindAsync(id);
            db.Comentario.Remove(comentario);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public void cargarProyectos()
        {
            var session = (Models.Profesor)Session["User"];
            int idProfesor = session.idProfesor;
            prueba1Entities db = new prueba1Entities();
            List<Proyecto> list = (from d in db.Proyecto
                                   where d.estadoProyecto == "Activo" && d.idProfesor == idProfesor
                                   select d).ToList();
            List<SelectListItem> lst = list.ConvertAll(d =>
            {
                return new SelectListItem
                {
                    Text = d.nombreProyecto,
                    Value = d.idProyecto.ToString()
                };

            });
            ViewBag.idProyecto = new SelectList(lst, "Value", "Text");
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
