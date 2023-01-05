using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WeCountRecrutementWebsite.Models;

namespace WeCountRecrutementWebsite.Controllers
{
    public class Admins : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment hosting;
        private readonly IHttpContextAccessor accessor;

        public Admins(AppDbContext _db, IWebHostEnvironment hosting, IHttpContextAccessor accessor)
        {
            db = _db;
            this.hosting = hosting;
            this.accessor = accessor;
        }

        public IActionResult Index(string findBy, string findValue)
        {
            if (accessor.HttpContext.Session.GetInt32("LoggedIn") == 0)
            {
                return RedirectToAction("Login");
            }

            var candidats = db.Candidates.ToList();
            if(findBy != null)
            {
                switch (findBy)
                {
                    case "firstName":
                        candidats = candidats.Where(c => c.firstName == findValue).ToList();
                        break;
                    case "lastName":
                        candidats = candidats.Where(c => c.lastName == findValue).ToList();
                        break;
                    case "email":
                        candidats = candidats.Where(c => c.email == findValue).ToList();
                        break;
                    case "phoneNumber":
                        candidats = candidats.Where(c => c.phoneNumber.ToString() == findValue).ToList();
                        break;
                }
            }
            return View(candidats);
        }

        public IActionResult Login()
        {
            if(accessor.HttpContext.Session.GetInt32("LoggedIn") == 1)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Admin admin)
        {
            try
            {
                if(admin.username == String.Empty)
                {
                    ViewBag.usernameError = "Username Required";
                    return View();
                }
                if(admin.password == String.Empty)
                {
                    ViewBag.passwordError = "Password Required";
                    return View();
                }
                var admins = db.Admins.ToList();
                foreach(Admin singleAdmin in admins)
                {
                    if(singleAdmin.username == admin.username)
                    {
                        if(singleAdmin.password == admin.password)
                        {
                            accessor.HttpContext.Session.SetInt32("LoggedIn", 1);
                        }
                        else
                        {
                            ViewBag.credentialError = "Password Incorrect";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.credentialError = "Username Incorrect";
                        return View();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public IActionResult DeleteCandidate(int candidateId)
        {
            if (accessor.HttpContext.Session.GetInt32("LoggedIn") == 0)
            {
                return RedirectToAction("Login");
            }
            var candidates = db.Candidates.ToList();
            foreach (Candidate singleCandidates in candidates)
            {
                if (singleCandidates.id == candidateId) {
                    //Temporary disabled the code below (File under use exception)

                    /*string path = Path.Combine(Path.Combine(hosting.WebRootPath, "uploads"), singleCandidates.lastName + "_" + singleCandidates.firstName);
                    if (Directory.Exists(path)) 
                    {
                        Directory.Delete(path, true);
                    }*/
                    db.Candidates.Remove(singleCandidates);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            accessor.HttpContext.Session.SetInt32("LoggedIn", 0);
            return RedirectToAction("Login");
        }
    }
}
