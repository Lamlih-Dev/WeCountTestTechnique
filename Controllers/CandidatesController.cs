using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using WeCountRecrutementWebsite.Models;
using System.Net.Mail;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;

namespace WeCountRecrutementWebsite.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment hosting;

        public CandidatesController(AppDbContext _db, IWebHostEnvironment hosting)
        {
            db = _db;
            this.hosting = hosting;
        }
        public IActionResult Inscription()
        {
            fillViewBagLevels();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Inscription(CandidateModel candidateModel)
        {
            fillViewBagLevels();

            try
            {
                var candidates = db.Candidates.ToList();
                foreach(Candidate singleCandidate in candidates)
                {
                    if(singleCandidate.email == candidateModel.email)
                    {
                        ViewBag.Message = "Email Already used";
                        return View();
                    }
                }
                string fullPath = string.Empty;
                if(candidateModel.CV != null)
                {
                    string uploads = Path.Combine(hosting.WebRootPath, "uploads");
                    string uploadsFolder = Path.Combine(uploads, candidateModel.lastName + "_" + candidateModel.firstName);
                    if(!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    string fileName = candidateModel.lastName + "_" + candidateModel.firstName + System.IO.Path.GetExtension(candidateModel.CV.FileName);
                    fullPath = Path.Combine(uploadsFolder, fileName);
                    candidateModel.CV.CopyTo(new FileStream(fullPath, FileMode.Create));
                }
                        
                Regex validatePhoneNumberRegex = new Regex("^0[6-7][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$");

                if (!validatePhoneNumberRegex.IsMatch("0"+candidateModel.phoneNumber.ToString()))
                {
                    if(candidateModel.phoneNumber.ToString().Length != 9)
                    {
                        ViewBag.Message = "Phone number should be composed of 10 numbers";
                    }
                    else {
                        ViewBag.Message = "Phone number format is incorrect";
                    }
                    return View();
                }
                Candidate candidate = new Candidate();
                candidate.firstName = candidateModel.firstName;
                candidate.lastName = candidateModel.lastName;
                candidate.email = candidateModel.email;
                candidate.phoneNumber = candidateModel.phoneNumber;
                candidate.studiesLevel = candidateModel.studiesLevel;
                candidate.yearsOfExperience = candidateModel.yearsOfExperience;
                candidate.lastEmploye = candidateModel.lastEmploye;
                candidate.inscriptionDate = DateTime.Today;
                candidate.cvUrl = "/uploads/" + candidateModel.lastName + "_" + candidateModel.firstName + "/" + candidateModel.lastName + "_" + candidateModel.firstName + System.IO.Path.GetExtension(candidateModel.CV.FileName); ;
                db.Candidates.Add(candidate);
                db.SaveChanges();
                TempData["email"] = candidateModel.email;
                TempData["fullname"] = candidateModel.lastName + " " + candidateModel.firstName;
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex;
                return View();
            }
        }

        public IActionResult Success()
        {
            if(TempData["email"] != null)
            {
                string to = TempData["email"].ToString();
                string subject = "We Count Recruitment - Successfully registred !";
                string body = "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <style> @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700;800;900&display=swap'); body{ margin: 0; background-color: #ccc; } table{ border-spacing: 0; } td{ padding: 0; } img{ border: 0; } .wrapper{ width: 100%; table-layout: fixed; background-color: #ccc; padding-bottom: 60px; } .main{ background-color: white; margin: 0auto; width: 100%; max-width: 600px; border-spacing: 0; font-family: 'Poppins', sans-serif; } </style></head><body><center class=wrapper><table class=main width=100% style='margin-top:50px;'><tr><td style='padding: 10px 20px;background-image: linear-gradient(to top right, #146FAE, #64CAEF);'> <img src=https://i.ibb.co/dmFPMMB/logo.png alt=logo> </td></tr><tr><td style='padding: 50px 20px;'> Hello " + TempData["fullname"].ToString() + ",<br> You Are Successfully Registred ! <strong>so what should you do now ?</strong><br> Our team will process your response and we will get in touch with you in the few coming days, please check your email regularly !</td></tr><tr><td style='padding: 10px 20px;font-size: 14px;font-weight: 600; text-align: center;background-image: linear-gradient(to top right, #146FAE, #64CAEF);color: white;'> We Count &copy;2023 Copyright.</td></tr></table></center></body></html>";

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("wecountrecruitment@gmail.com"));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = body };
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("wecountrecruitment@gmail.com", "aaeguufvtfabtquy");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
            }
            else
            {
                return RedirectToAction("Inscription");
            }
                
            return View();
        }

        private void fillViewBagLevels()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "High School", Value = "High School" });
            items.Add(new SelectListItem { Text = "Some College", Value = "Some College" });
            items.Add(new SelectListItem { Text = "College Degree", Value = "College Degree", Selected = true });
            items.Add(new SelectListItem { Text = "Master's / Advanced Degree", Value = "Master's / Advanced Degree" });
            items.Add(new SelectListItem { Text = "PhD Degree", Value = "PhD Degree" });

            ViewBag.levels = items;
        }
    }
}
