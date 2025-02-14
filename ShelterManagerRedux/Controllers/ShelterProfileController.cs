//using System.Linq;
//using Microsoft.AspNetCore.Mvc;
//using ShelterManagerRedux.DataAccess;
//using ShelterManagerRedux.Models;

//namespace ShelterManagerRedux.Controllers
//{
//    public class ShelterProfileController : Controller
//    {
//        public IActionResult EditShelterProfile()
//        {
//            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
//            string connectionString = config.GetSection("Connnectionstrings:MyConnection").Value;
//            ViewBag.ErrorMessage = "";

//            using (var context = new ManagerContext())
//            {
//                int shelterId = 2;
//                ShelterProfile profile = context.ShelterProfiles.FirstOrDefault(sp => sp.ShelterID == shelterId);

//                if (profile == null)
//                {
//                    return NotFound();
//                }
//                return View(profile);
//            }
//        }
//    }
//}
