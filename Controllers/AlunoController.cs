using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        public IEnumerable<SelectListItem> Segmentos { get; set; }

        private IEnumerable<SelectListItem> GetAllStates()
        {
            IEnumerable<SelectListItem> list = from s in db.Segmento
                                               select new SelectListItem
                                               {
                                                   Selected = false,
                                                   Text = s.Nome,
                                                   Value = s.Codigo.ToString()
                                               };
            return list;
        }

        // GET: Aluno
        public ActionResult Index(string SearchRM, string searchNome, string SearchSegmento)
        {
            var alunos = from a in db.Aluno
            select a;
            var aluno = db.Aluno.Include(a => a.Curso).Include(a => a.Segmento);

            var segmentos = GetAllStates();
           
            ViewBag.segmentosList = new SelectList(segmentos);
            Segmentos = new SelectList(segmentos);

            if (!String.IsNullOrEmpty(SearchRM))
            {
                aluno = alunos.Where(a => a.RM.ToString() == SearchRM);
            }
            if (!String.IsNullOrEmpty(searchNome) && String.IsNullOrEmpty(SearchRM))
            {
                aluno = alunos.Where(a => a.Nome.Contains(searchNome));
            }
            else if(!String.IsNullOrEmpty(searchNome) && !String.IsNullOrEmpty(SearchRM))
            {
                aluno = alunos.Where(a => a.RM.ToString() == SearchRM || a.Nome.Contains(searchNome));
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
                db.Aluno.Add(aluno);
                db.SaveChanges();
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
