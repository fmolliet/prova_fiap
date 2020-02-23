using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using teste_fiap.Models;

namespace teste_fiap.Controllers

{
    public class AlunoController : Controller
    {
        private ProvaDesenvolvimentoEntities db = new ProvaDesenvolvimentoEntities();

        private Boolean empty( string variable)
        {
            if (String.IsNullOrEmpty(variable))
                return true;
            else
                return false;
        }

        // GET: Aluno
        public ActionResult Index(string SearchRM, string searchNome, string SearchSegmento, string salvo)
        {
            // busca alunos para filtrar 
            var alunos = from a in db.Aluno
                         // where a.Segmento.Nome == SearchSegmento || SearchSegmento.Equals(null) || SearchSegmento.Equals("")
                         select a;
            // Traz todos alunos incluido o curso e segmento
            var aluno = db.Aluno.Include(a => a.Curso).Include(a => a.Segmento);

            /**
             * Sistema de filtro
             */
            if (!empty(SearchRM))
            {
                aluno = alunos.Where(a => a.RM.ToString() == SearchRM);
            }
            if (!empty(searchNome) && empty(SearchRM) && empty(SearchSegmento))
            {
                aluno = alunos.Where(a => a.Nome.Contains(searchNome));
            }
            else if(!empty(searchNome) && !empty(SearchRM) && empty(SearchSegmento))
            {
                aluno = alunos.Where(a => a.RM.ToString() == SearchRM || a.Nome.Contains(searchNome));
            }
            else if (empty(searchNome) && empty(SearchRM) && !empty(SearchSegmento))
            {
                aluno = alunos.Where(a => a.CodigoSegmento.ToString().Contains(SearchSegmento));
            }
            else if (empty(searchNome) && !empty(SearchRM) && !empty(SearchSegmento))
            {
                aluno = alunos.Where(a => a.CodigoSegmento.ToString().Contains(SearchSegmento) || a.RM.ToString() == SearchRM);
            }
            else if (!empty(searchNome) && empty(SearchRM) && !empty(SearchSegmento))
            {
                aluno = alunos.Where(a => a.CodigoSegmento.ToString().Contains(SearchSegmento) || a.Nome.Contains(searchNome));
            }
            else if (!empty(searchNome) && !empty(SearchRM) && !empty(SearchSegmento))
            {
                aluno = alunos.Where(a => a.CodigoSegmento.ToString().Contains(SearchSegmento) || a.RM.ToString() == SearchRM || a.Nome.Contains(searchNome));
            }


            return View(aluno.ToList());
        }

        // GET: Aluno/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aluno aluno = db.Aluno.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            return View(aluno);
        }

        // GET: Aluno/Create
        public ActionResult Create()
        {
            ViewBag.CodigoCurso = new SelectList(db.Curso, "Codigo", "Nome");
            ViewBag.CodigoSegmento = new SelectList(db.Segmento, "Codigo", "Nome");
            return View();
        }

        // POST: Aluno/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Codigo,CodigoSegmento,CodigoCurso,RM,Nome,DataNascimento,DDD,Telefone")] Aluno aluno)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    db.Aluno.Add(aluno);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Aluno", new { salvo = "OK" });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    
                }

                return RedirectToAction("Index");
            }

            ViewBag.CodigoCurso = new SelectList(db.Curso, "Codigo", "Nome", aluno.CodigoCurso);
            ViewBag.CodigoSegmento = new SelectList(db.Segmento, "Codigo", "Nome", aluno.CodigoSegmento);
            return View(aluno);
        }

        // GET: Aluno/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aluno aluno = db.Aluno.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            ViewBag.CodigoCurso = new SelectList(db.Curso, "Codigo", "Nome", aluno.CodigoCurso);
            ViewBag.CodigoSegmento = new SelectList(db.Segmento, "Codigo", "Nome", aluno.CodigoSegmento);
            return View(aluno);
        }

        // POST: Aluno/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Codigo,CodigoSegmento,CodigoCurso,RM,Nome,DataNascimento,DDD,Telefone")] Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aluno).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CodigoCurso = new SelectList(db.Curso, "Codigo", "Nome", aluno.CodigoCurso);
            ViewBag.CodigoSegmento = new SelectList(db.Segmento, "Codigo", "Nome", aluno.CodigoSegmento);
            return View(aluno);
        }

        // GET: Aluno/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aluno aluno = db.Aluno.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            return View(aluno);
        }

        // POST: Aluno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aluno aluno = db.Aluno.Find(id);
            db.Aluno.Remove(aluno);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Aluno/Cursos/1
        [HttpGet]
        public ActionResult Cursos(int id)
        {
            // Trás todos cursos
            var cursos = from c in db.Curso select c;
            // Filtra por qual foi recebido
            var cursos_disponiveis = cursos.Where(c => c.CodigoSegmento == id);
            // Serializa e passa pra JSOn
            JsonSerializerSettings jsSettings = new JsonSerializerSettings();
            jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            // Cria variavel para retornar para o front
            var dado_convertido = JsonConvert.SerializeObject(cursos_disponiveis, null, jsSettings);

            return Content(dado_convertido, "application/json");
        }

        // GET: Aluno/NextRM/1
        [HttpGet]
        public ActionResult NextRM(int id)
        {
            // Trás todos cursos
            var lastAlunos = from a in db.Aluno 
                         select a;
            // Filtra por qual foi recebido
            int max = 0;
            int min = 0;
            switch (id)
            {
                case 1:
                    max = 29999;
                    min = 1000;
                break;
                case 2:
                    max = 79999;
                    min = 50000;
                break;
                case 3:
                    max = 49999;
                    min = 30000;
               break;
            }
            var alunosSegmentados = lastAlunos.Where(a => a.CodigoSegmento == id && (a.RM >= min  && a.RM <= max)).Max(a => a.RM + 1);
            // Serializa e passa pra JSOn
            JsonSerializerSettings jsSettings = new JsonSerializerSettings();
            jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            // Cria variavel para retornar para o front
            var dado_convertido = JsonConvert.SerializeObject(alunosSegmentados, null, jsSettings);

            return Content(dado_convertido , "application/json");
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
