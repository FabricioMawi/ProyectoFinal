
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prueba1.Models;
using PagedList;
using System.Security.Cryptography;
using System.Data.Entity.Core.Mapping;
using System.IO;
using System.Xml;
using System.Data.Entity.Migrations;
using static prueba1.Models.Archivo2;
using System.Data.Entity.Validation;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;

namespace prueba1.Controllers
{
    public class ProyectoesController : Controller
    {
        private prueba1Entities db = new prueba1Entities();

        // GET: Proyectoes
        public ActionResult Index(string sortOrder, string searchString)
        {
            var session = (Models.Profesor)Session["User"];
           


            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var proyectos = from s in db.Proyecto
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                proyectos = proyectos.Where(s => s.nombreProyecto.Contains(searchString)
                                       || s.idContacto.ToString().Contains(searchString)
                                       || s.idCurso.ToString().Contains(searchString)
                                       || s.tecnologia.Contains(searchString)
                                       || s.idProfesor.ToString().Contains(searchString)
                                       || s.idGrupo.ToString().Contains(searchString)
                                       || s.estadoProyecto.Contains(searchString)
                                       || s.fechaInicio.ToString().Contains(searchString)
                                       || s.fechaFinalizado.ToString().Contains(searchString));
            }
            
            return View(proyectos.ToList());
            }
        }

        // GET: Proyectoes/Details/5
        public ActionResult Details(int? id)
        {
                     
            var session = (Models.Profesor)Session["User"];

            var fase = (from c in db.Proyecto where c.idProyecto == id select c.idCurso).FirstOrDefault();

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
            int idProyecto = id.GetValueOrDefault();
            Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
                if (fase == 1)
                {
                    cargarProyectos(idProyecto);
                    return View("Details", proyecto);
                    
                }
                else if (fase == 2)
                {
                    cargarProyectos(idProyecto);
                    return View("Details2", proyecto);
                    
                }
                else
                {
                    cargarProyectos(idProyecto);
                    return View("Details3", proyecto);
                    
                }

               
            }
        }


        // GET: Proyectoes/Create
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

            
                var grupo = (from c in db.Grupo where c.idProfesor == idProfesor select c.idGrupo).ToList();
                

            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso");
            ViewBag.idContacto = new SelectList(db.Empresa, "idContacto", "nombreEmpresa");
            ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "nombreGrupo");
            cargarProfesores();
            return View();
            }
        }

        // POST: Proyectoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idProyecto,nombreProyecto,idContacto,idCurso,tecnologia,idProfesor,idGrupo,estadoProyecto,fechaInicio,fechaFinalizado")] Proyecto proyecto)
        {
            var session = (Models.Profesor)Session["User"];
            


            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {

                if (ModelState.IsValid)
            {
                db.Proyecto.Add(proyecto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          

            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso", proyecto.idCurso);
            ViewBag.idContacto = new SelectList(db.Empresa, "idContacto", "nombreEmpresa", proyecto.idContacto);
            ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "idGrupo", proyecto.idGrupo);
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor", proyecto.idProfesor);
            return View(proyecto);
            }
        }


        // GET: Proyectoes/Edit/5
        public ActionResult Edit(int? id)
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

            Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }

                int idProfesor = session.idProfesor;
                var gen = (from d in db.Profesor
                       where d.estado == "Activo"
                       select d.nombreProfesor).ToList();

            var grupo = (from c in db.Grupo where c.idProfesor == idProfesor select c.idGrupo).ToList();


            
            

            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso", proyecto.idCurso);
            ViewBag.idContacto = new SelectList(db.Empresa, "idContacto", "nombreEmpresa", proyecto.idContacto);
            ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "nombreGrupo");
            cargarProfesores();
          
            return View(proyecto);
            }
        }

        // POST: Proyectoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idProyecto,nombreProyecto,idContacto,idCurso,tecnologia,idProfesor,idGrupo,estadoProyecto,fechaInicio,fechaFinalizado")] Proyecto proyecto)
        {

            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {

                if (ModelState.IsValid)
            {
                db.Entry(proyecto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          
            ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso", proyecto.idCurso);
            ViewBag.idContacto = new SelectList(db.Empresa, "idContacto", "nombreEmpresa", proyecto.idContacto);
            ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "idGrupo", proyecto.idGrupo);
            ViewBag.idProfesor = new SelectList(db.Profesor, "idProfesor", "nombreProfesor", proyecto.idProfesor);
       
            return View(proyecto);
            }
        }

        // GET: Proyectoes/Delete/5
        public ActionResult Delete(int? id)
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
            Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
            }
        }
   
    


        // POST: Proyectoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Proyecto proyecto = db.Proyecto.Find(id);
            String proyectoActivo = (from c in db.Proyecto where c.idProyecto == id select c.estadoProyecto).FirstOrDefault();
            int documentos = (from c in db.Archivo1 where c.idProyecto == id select c.idArchivo).FirstOrDefault();
            int documentos2 = (from c in db.Archivo2 where c.idProyecto == id select c.idArchivo).FirstOrDefault();
            int documentos3 = (from c in db.Archivo3 where c.idProyecto == id select c.idArchivo).FirstOrDefault();
            int comentarioActivo = (from c in db.Comentario where c.idProyecto == id select c.idComentario).FirstOrDefault();
            if (proyectoActivo != "Desertado")
            {

                ViewBag.Error = "Este proyecto se encuentra Activo, En Proceso o Implantado. Solo se eliminara si esta inactivo";
                return View(proyecto);
            }
            else if (comentarioActivo != 0)
            {

                ViewBag.Error = "Este proyecto tiene comentarios, debe eliminarlos para poder eliminar este proyecto";
                return View(proyecto);
            }

            if(documentos != 0)
            {
                Archivo1 archivo1 = db.Archivo1.Find(documentos);
                db.Archivo1.Remove(archivo1);
            }
            if (documentos2 != 0)
            {
                Archivo2 archivo2 = db.Archivo2.Find(documentos2);
                db.Archivo2.Remove(archivo2);
            }
            if (documentos3 != 0)
            {
                Archivo3 archivo3 = db.Archivo3.Find(documentos);
                db.Archivo3.Remove(archivo3);
            }


            db.Proyecto.Remove(proyecto);
            db.SaveChanges();

           return RedirectToAction("Index");

            

           
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

        public void cargarProyectos(int id)
        {
            
            prueba1Entities db = new prueba1Entities();
            List<Proyecto> list = (from d in db.Proyecto
                                   where d.estadoProyecto == "Activo" && d.idProyecto == id
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

        [HttpPost]

        public ActionResult Guardar(HttpPostedFileBase nombreArchivo, int idProyecto, String DocumentoFinal)
        {

            int curso = (from d in db.Proyecto where d.idProyecto == idProyecto select d.idCurso).FirstOrDefault();
            if (nombreArchivo != null && nombreArchivo.ContentLength > 0)
            {
                var fileName = Path.GetFileName(nombreArchivo.FileName);

                byte[] bytes;



                using (BinaryReader br = new BinaryReader(nombreArchivo.InputStream))
                {
                    bytes = br.ReadBytes(nombreArchivo.ContentLength);
                }

                if (curso == 1)
                {
                    int edit = (from d in db.Archivo1 where d.idProyecto == idProyecto select d.idArchivo).FirstOrDefault();
                    if (edit != 0)
                    {
                        try
                        {
                            using (db)
                            {
                                int delete = (from c in db.Archivo1 where c.idProyecto == idProyecto select c.idArchivo).FirstOrDefault();
                                Archivo1 d = db.Archivo1.Find(delete);
                                db.Archivo1.Remove(d);

                                Archivo1 f = new Archivo1();
                                f.nombreArchivo = Path.GetFileName(nombreArchivo.FileName);
                                f.tipoContenido = nombreArchivo.ContentType;
                                f.contenidoArchivo = bytes;
                                f.idProyecto = idProyecto;
                                f.documentos = DocumentoFinal;



                                db.Archivo1.Add(f);

                                db.SaveChanges();
                            }
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = "El archivo supera el tamaño permitido";
                           
                        }


                    }
                    else
                    {
                        try
                        {
                            using (db)
                            {
                                Archivo1 d = new Archivo1();
                                d.nombreArchivo = Path.GetFileName(nombreArchivo.FileName);
                                d.tipoContenido = nombreArchivo.ContentType;
                                d.contenidoArchivo = bytes;
                                d.idProyecto = idProyecto;
                                d.documentos = DocumentoFinal;

                                db.Archivo1.Add(d);
                                db.SaveChanges();


                            }
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = "El archivo supera el tamaño permitido";
                            
                        }
                        return RedirectToAction("Index");
                    }

                }
                else if (curso == 2)
                {
                    int edit = (from d in db.Archivo2 where d.idProyecto == idProyecto select d.idArchivo).FirstOrDefault();
                    if (edit != 0)
                    {
                        try
                        {
                            using (db)
                            {
                                int delete = (from c in db.Archivo2 where c.idProyecto == idProyecto select c.idArchivo).FirstOrDefault();
                                Archivo2 d = db.Archivo2.Find(delete);
                                db.Archivo2.Remove(d);

                                Archivo2 f = new Archivo2();
                                f.nombreArchivo = Path.GetFileName(nombreArchivo.FileName);
                                f.tipoContenido = nombreArchivo.ContentType;
                                f.contenidoArchivo = bytes;
                                f.idProyecto = idProyecto;
                                f.documentos = DocumentoFinal;



                                db.Archivo2.Add(f);

                                db.SaveChanges();
                            }
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }


                    }
                    else
                    {
                        try
                        {
                            using (db)
                            {
                                Archivo2 d = new Archivo2();
                                d.nombreArchivo = Path.GetFileName(nombreArchivo.FileName);
                                d.tipoContenido = nombreArchivo.ContentType;
                                d.contenidoArchivo = bytes;
                                d.idProyecto = idProyecto;
                                d.documentos = DocumentoFinal;


                                db.Archivo2.Add(d);
                                db.SaveChanges();


                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        return RedirectToAction("Index");
                    }

                }
                else if (curso == 3)
                {
                    int edit = (from d in db.Archivo3 where d.idProyecto == idProyecto select d.idArchivo).FirstOrDefault();
                    if (edit != 0)
                    {
                        try
                        {
                            using (db)
                            {
                                int delete = (from c in db.Archivo3 where c.idProyecto == idProyecto select c.idArchivo).FirstOrDefault();
                                Archivo3 d = db.Archivo3.Find(delete);
                                db.Archivo3.Remove(d);

                                Archivo3 f = new Archivo3();
                                f.nombreArchivo = Path.GetFileName(nombreArchivo.FileName);
                                f.tipoContenido = nombreArchivo.ContentType;
                                f.contenidoArchivo = bytes;
                                f.idProyecto = idProyecto;
                                f.documentos = DocumentoFinal;
                                db.Archivo3.Add(f);

                                db.SaveChanges();
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }


                    }
                    else
                    {
                        try
                        {
                            using (db)
                            {
                                Archivo3 d = new Archivo3();
                                d.nombreArchivo = Path.GetFileName(nombreArchivo.FileName);
                                d.tipoContenido = nombreArchivo.ContentType;
                                d.contenidoArchivo = bytes;
                                d.idProyecto = idProyecto;
                                d.documentos = DocumentoFinal;
                                db.Archivo3.Add(d);
                                db.SaveChanges();


                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        return RedirectToAction("Index");
                    }

                }

            }
            return RedirectToAction("Index");

        }


        public FileResult DownloadFile(int? fileID)
        {
            prueba1Entities db = new prueba1Entities();
            Archivo1 file = db.Archivo1.ToList().Find(p => p.idArchivo == fileID.Value);
            return File(file.contenidoArchivo, file.tipoContenido, file.nombreArchivo);
        }

        public FileResult DownloadFile2(int? fileID)
        {
            prueba1Entities db = new prueba1Entities();
            Archivo2 file = db.Archivo2.ToList().Find(p => p.idArchivo == fileID.Value);
            return File(file.contenidoArchivo, file.tipoContenido, file.nombreArchivo);
        }

        public FileResult DownloadFile3(int? fileID)
        {
            prueba1Entities db = new prueba1Entities();
            Archivo3 file = db.Archivo3.ToList().Find(p => p.idArchivo == fileID.Value);
            return File(file.contenidoArchivo, file.tipoContenido, file.nombreArchivo);
        }

        public ActionResult Reporte(int? id)
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
                Proyecto proyecto = db.Proyecto.Find(id);
                if (proyecto == null)
                {
                    return HttpNotFound();
                }
                int idProfesor = session.idProfesor;
                var gen = (from d in db.Profesor
                           where d.estado == "Activo"
                           select d.nombreProfesor).ToList();

                var grupo = (from c in db.Grupo where c.idProfesor == idProfesor select c.idGrupo).ToList();

                ViewBag.idCurso = new SelectList(db.Curso, "idCurso", "nombreCurso", proyecto.idCurso);
                ViewBag.idContacto = new SelectList(db.Empresa, "idContacto", "nombreEmpresa", proyecto.idContacto);
                ViewBag.idGrupo = new SelectList(db.Grupo, "idGrupo", "nombreGrupo");
                cargarProfesores();

                return View(proyecto);
            }
        }

        public ActionResult createReport(string tipo, int id)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), "ReportProyecto.rdlc");
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

            SqlCommand cmdProy = new SqlCommand();
            SqlCommand cmdProf = new SqlCommand();
            SqlCommand cmdGrupo = new SqlCommand();
            SqlCommand cmdCurso = new SqlCommand();
            SqlCommand cmdComm = new SqlCommand();

            cmdProy.Connection = cnn;
            cmdProf.Connection = cnn;
            cmdGrupo.Connection = cnn;
            cmdCurso.Connection = cnn;
            cmdComm.Connection = cnn;

            cmdProy.CommandType = System.Data.CommandType.StoredProcedure;
            cmdProf.CommandType = System.Data.CommandType.StoredProcedure;
            cmdGrupo.CommandType = System.Data.CommandType.StoredProcedure;
            cmdCurso.CommandType = System.Data.CommandType.StoredProcedure;
            cmdComm.CommandType = System.Data.CommandType.StoredProcedure;

            cmdProy.CommandText = "selectProyecto";
            cmdProf.CommandText = "selectProfesor";
            cmdGrupo.CommandText = "selectGrupo";
            cmdCurso.CommandText = "selectCurso";
            cmdComm.CommandText = "selectComentarios";

            cmdProy.Parameters.Add("@idProyecto", SqlDbType.Int).Value = id;
            cmdProf.Parameters.Add("@idProyecto", SqlDbType.Int).Value = id;
            cmdGrupo.Parameters.Add("@idProyecto", SqlDbType.Int).Value = id;
            cmdCurso.Parameters.Add("@idProyecto", SqlDbType.Int).Value = id;
            cmdComm.Parameters.Add("@idProyecto", SqlDbType.Int).Value = id;

            List<Comentario> comment = new List<Comentario>();
            List<Proyecto> proyect = new List<Proyecto>();
            List<Profesor> professor = new List<Profesor>();
            List<Grupo> group = new List<Grupo>();
            List<Curso> subject = new List<Curso>();

            cnn.Open();
            using (SqlDataReader dr = cmdProy.ExecuteReader())
            {
                while (dr.Read())
                {
                    Proyecto newItem = new Proyecto();
                    newItem.nombreProyecto = dr.GetString(0);
                    newItem.tecnologia = dr.GetString(1);
                    newItem.estadoProyecto= dr.GetString(2);
                    newItem.fechaInicio = dr.GetDateTime(3);
                    proyect.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdProf.ExecuteReader())
            {
                while (dr.Read())
                {
                    Profesor newItem = new Profesor();
                    newItem.nombreProfesor = dr.GetString(0);
                    newItem.apellidoProfesor= dr.GetString(1);
                    newItem.emailProfesor = dr.GetString(2);
                    professor.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdGrupo.ExecuteReader())
            {
                while (dr.Read())
                {
                    Grupo newItem = new Grupo();
                    newItem.nombreGrupo = dr.GetString(0);
                    newItem.sede = dr.GetString(1);
                    newItem.cuatrimestre = dr.GetString(2);
                    group.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdCurso.ExecuteReader())
            {
                while (dr.Read())
                {
                    Curso newItem = new Curso();
                    newItem.nombreCurso = dr.GetString(0);
                    subject.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdComm.ExecuteReader())
            {
                while (dr.Read())
                {
                    Comentario newItem = new Comentario();
                    newItem.fecha = dr.GetDateTime(0);
                    newItem.contenido = dr.GetString(1);
                    comment.Add(newItem);
                }
            }
            cnn.Close();


            ReportDataSource rdProy = new ReportDataSource("DataSetProyecto", proyect);
            ReportDataSource rdProf = new ReportDataSource("DataSetProfesor", professor);
            ReportDataSource rdGrou = new ReportDataSource("DataSetGrupo", group);
            ReportDataSource rdSubj = new ReportDataSource("DataSetCurso", subject);
            ReportDataSource rdComm = new ReportDataSource("DataSetComentario", comment);

            lr.DataSources.Add(rdProy);
            lr.DataSources.Add(rdProf);
            lr.DataSources.Add(rdGrou);
            lr.DataSources.Add(rdSubj);
            lr.DataSources.Add(rdComm);

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


        public ActionResult ReporteTodos()
        {
            var session = (Models.Profesor)Session["User"];

            if (session == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var proyectos = from s in db.Proyecto
                                select s;
                return View(proyectos.ToList());
            }
        }

        public ActionResult createReportAll(string tipo)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reportes"), "ReporteProyectos.rdlc");
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

            SqlCommand cmdProy = new SqlCommand();
            SqlCommand cmdProf = new SqlCommand();
            SqlCommand cmdEmpr = new SqlCommand();
            SqlCommand cmdCurso = new SqlCommand();

            cmdProy.Connection = cnn;
            cmdProf.Connection = cnn;
            cmdEmpr.Connection = cnn;
            cmdCurso.Connection = cnn;

            cmdProy.CommandType = System.Data.CommandType.StoredProcedure;
            cmdProf.CommandType = System.Data.CommandType.StoredProcedure;
            cmdEmpr.CommandType = System.Data.CommandType.StoredProcedure;
            cmdCurso.CommandType = System.Data.CommandType.StoredProcedure;

            cmdProy.CommandText = "selectProyectoTodo";
            cmdProf.CommandText = "selectProfesorTodo";
            cmdEmpr.CommandText = "selectEmpresaTodo";
            cmdCurso.CommandText = "selectCursoTodo";
            
            List<Proyecto> proyect = new List<Proyecto>();
            List<Profesor> professor = new List<Profesor>();
            List<Empresa> company = new List<Empresa>();
            List<Curso> subject = new List<Curso>();

            cnn.Open();
            using (SqlDataReader dr = cmdProy.ExecuteReader())
            {
                while (dr.Read())
                {
                    Proyecto newItem = new Proyecto();
                    newItem.idProyecto = dr.GetInt32(0);
                    newItem.nombreProyecto = dr.GetString(1);
                    newItem.estadoProyecto = dr.GetString(2);
                    newItem.tecnologia = dr.GetString(3);
                    newItem.fechaInicio = dr.GetDateTime(4);
                    newItem.idContacto = dr.GetInt32(5);
                    newItem.idCurso = dr.GetInt32(6);
                    newItem.idProfesor = dr.GetInt32(7);
                    proyect.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdEmpr.ExecuteReader())
            {
                while (dr.Read())
                {
                    Empresa newItem = new Empresa();
                    newItem.idContacto = dr.GetInt32(0);
                    newItem.nombreEmpresa = dr.GetString(1);
                    newItem.nombreContacto = dr.GetString(2);
                    newItem.email = dr.GetString(3);
                    newItem.telefono = dr.GetInt32(4);
                    newItem.tipoEmpresa = dr.GetString(5);
                    company.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdProf.ExecuteReader())
            {
                while (dr.Read())
                {
                    Profesor newItem = new Profesor();
                    newItem.idProfesor = dr.GetInt32(0);
                    newItem.nombreProfesor = dr.GetString(1);
                    newItem.apellidoProfesor = dr.GetString(2);
                    newItem.nombreUsuario = dr.GetString(3);
                    newItem.emailProfesor = dr.GetString(4);
                    professor.Add(newItem);
                }
            }
            cnn.Close();

            cnn.Open();
            using (SqlDataReader dr = cmdCurso.ExecuteReader())
            {
                while (dr.Read())
                {
                    Curso newItem = new Curso();
                    newItem.idCurso = dr.GetInt32(0);
                    newItem.nombreCurso = dr.GetString(1);
                    subject.Add(newItem);
                }
            }
            cnn.Close();


            ReportDataSource rdProy = new ReportDataSource("DataSetProyecto", proyect);
            ReportDataSource rdProf = new ReportDataSource("DataSetProfesor", professor);
            ReportDataSource rdEmpr = new ReportDataSource("DataSetEmpresa", company);
            ReportDataSource rdSubj = new ReportDataSource("DataSetCurso", subject);

            lr.DataSources.Add(rdProy);
            lr.DataSources.Add(rdProf);
            lr.DataSources.Add(rdEmpr);
            lr.DataSources.Add(rdSubj);

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
