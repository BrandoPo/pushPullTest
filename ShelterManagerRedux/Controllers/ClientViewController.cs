using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Azure;
using NuGet.Versioning;
using ShelterManagerRedux.DataAccess;
using ShelterManagerRedux.Models;
using System.Data.Entity.Core.Common.EntitySql;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core;
namespace ShelterManagerRedux.Controllers
{
    public class ClientViewController : Controller
    {
        public IActionResult ClientView()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;

            ShelterLocationContext cv = new ShelterLocationContext(connStr);

            var query = from v in cv.ShelterLocations
                        orderby v.Shelter_Location_Available_Room descending
                        select v;

            List<ShelterLocation> myData = query.ToList();

            return View(myData);
        }
        [HttpPost]
        public async Task<IActionResult> ShowInterest(string fName, string lName, int shelterID)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;
            int index = 0;

            using (ShelterLocationContext db = new ShelterLocationContext(connStr))
            {
                if (fName != null && lName != null)
                {
                    var shelter = db.ShelterLocations.FirstOrDefault(s => s.Shelter_Location_ID == shelterID);
                    if (shelter != null)
                    {
                        if (shelter.Shelter_Location_Available_Room > 0)
                        {
                            shelter.Shelter_Location_ShownInterest += 1;
                            ViewData["ShownInterestIn"] = "Thank you for showing interest in " + shelter.Shelter_Location_Description;
                            db.SaveChanges();

                            // Call method to save changes to ShownInterestContext
                            await SaveInterestAsync(connStr, index, fName, lName, shelterID);

                            return View("ClientView", db.ShelterLocations.ToList());
                        }
                        else
                        {
                            ViewData["ErrorMessage"] = "This shelter has no available space";
                            return View("ClientView", db.ShelterLocations.ToList());
                        }
                    }
                }
                else
                {
                    ViewData["ErrorMessage"] = "Please Enter All Required Information.";
                    return View("ClientView", db.ShelterLocations.ToList());
                }
            }

            // Return whatever view you want after the interest is shown
            return RedirectToAction("ClientView");
        }

        private async Task SaveInterestAsync(string connStr, int index, string fName, string lName, int shelterID)
        {
            using (ShownInterestContext db1 = new ShownInterestContext(connStr))
            {
                var client = new ShownInterest();
                
                client.Fname = fName;
                client.Lname = lName;
                client.ShelterID = shelterID;

                db1.ShownInterests.Add(client);
                await db1.SaveChangesAsync();
            }

        }







    }
}
