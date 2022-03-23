using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InsuranceAgency;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    public class PolicyholdersController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Policyholders
        public ActionResult Index()
        {
            return View(db.Policyholders.ToList());
        }

        // GET: Policyholders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policyholder policyholder = db.Policyholders.Find(id);
            if (policyholder == null)
            {
                return HttpNotFound();
            }
            return View(policyholder);
        }

        // GET: Policyholders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Policyholders/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FullName,Birthday,Telephone,Passport")] Policyholder policyholder)
        {
            if (ModelState.IsValid)
            {
                db.Policyholders.Add(policyholder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(policyholder);
        }

        // GET: Policyholders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policyholder policyholder = db.Policyholders.Find(id);
            if (policyholder == null)
            {
                return HttpNotFound();
            }
            return View(policyholder);
        }

        // POST: Policyholders/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FullName,Birthday,Telephone,Passport")] Policyholder policyholder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(policyholder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(policyholder);
        }

        // GET: Policyholders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policyholder policyholder = db.Policyholders.Find(id);
            if (policyholder == null)
            {
                return HttpNotFound();
            }
            return View(policyholder);
        }

        // POST: Policyholders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Policyholder policyholder = db.Policyholders.Find(id);
            db.Policyholders.Remove(policyholder);
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
