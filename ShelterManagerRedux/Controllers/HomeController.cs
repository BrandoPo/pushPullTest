using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Versioning;
using ShelterManagerRedux.DataAccess;
using ShelterManagerRedux.Models;
using System.Data.Entity.Core.Common.EntitySql;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;

namespace ShelterManagerRedux.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public int Shelter1Cots = 25, Shelter2Cots = 30, Shelter3Cots = 19;

        //self note
        private ManagerContext _context;

        private IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            //self note 


            _context = new ManagerContext();


        }
        public IActionResult Index()
        {


            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;


            if (isAuthenticated == 1)
            {
                // user is logged in
                ViewData["ShowElements"] = true;

            }
            else
            {
                // user is not logged in
                ViewData["ShowElements"] = false;
            }

            // check if the logout message is set in TempData
            var logoutMessage = TempData["LogoutMessage"] as string;
            if (!string.IsNullOrEmpty(logoutMessage))
            {
                ViewData["LogoutMessage"] = logoutMessage;
                ViewData["ShowElements"] = false;
            }

            // check if the login message is set in TempData
            var loginMessage = TempData["LoginMessage"] as string;
            if (!string.IsNullOrEmpty(loginMessage))
            {
                ViewData["LoginMessage"] = loginMessage;
            }

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            //for client manager button to appear 
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;

            //for client manager button to appear 
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;

            // manager information on user
            int managerId = HttpContext.Session.GetInt32("ManagerID") ?? 0;
            Manager manager = null;

            // manager information from user from database
            using (var context = new ManagerContext(connStr))
            {
                manager = context.Managers.FirstOrDefault(m => m.ManagerID == managerId);
            }

            if (manager != null && manager.Password == model.CurrentPassword)
            {
                if (model.NewPassword == model.ConfirmPassword) 
                {
                    manager.Password = model.NewPassword;
                    using (var context = new ManagerContext(connStr))
                    {
                        context.Entry(manager).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    TempData["ChangePasswordSuccess"] = "Password changed successfully.";
                }
                else
                {
                    TempData["ChangePasswordError"] = "New password and confirm password do not match.";
                }
            }
            else
            {
                TempData["ChangePasswordError"] = "Current password is incorrect.";
            }

            return RedirectToAction("ChangePassword");
        }

        public ActionResult ClientView()
        {
            return this.RedirectToAction("ClientView", "ClientView");
        }
        public ActionResult ShowInterest()
        {
            return this.RedirectToAction("ShowInterest", "ClientView");
        }




        public IActionResult Help()
        {
            //for client manager button to appear when logged in
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;
            //
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;

            FAQContext ff = new FAQContext(connStr);


            var query = from f in ff.FrequentlyAskedQuestions
                        orderby f.QuestionID
                        select f;

            List<FrequentlyAskedQuestions> myData = query.ToList();
            return View(myData);

        }

        public IActionResult ContactPage()
        {
            return View();
        }

        public IActionResult ShelterProfile()
        {
            //for edit profile button and client manager button
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;
            //
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;

            ShelterProfileContext dw = new ShelterProfileContext(connStr);

            var query = from w in dw.ShelterProfile
                        orderby w.ShelterID
                        select w;

            List<ShelterProfile> myData = query.ToList();

            return View(myData);
        }

        public IActionResult EditShelterProfile()
        {
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string connectionString = config.GetSection("Connnectionstrings:MyConnection").Value;
            ViewBag.ErrorMessage = "";

            using (var context = new ShelterProfileContext(connectionString))
            {
                var profile = context.ShelterProfile.FirstOrDefault(p => p.Shelter_name == "Trinity Rescue Mission");

                if (profile == null)
                {
                    return NotFound();
                }

                return View(profile);
            }
        }

        [HttpPost]
        public IActionResult SaveChanges(int id, ShelterProfile updatedProfile)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string connectionString = config.GetSection("ConnectionStrings:MyConnection").Value;
            ViewBag.ErrorMessage = "";

            try
            {
                using (var shelterProfileContext = new ShelterProfileContext(connectionString))
                {
                    var existingProfile = shelterProfileContext.ShelterProfile.FirstOrDefault(p => p.ShelterID == id);

                    if (existingProfile != null)
                    {
                        existingProfile.Shelter_name = updatedProfile.Shelter_name;
                        existingProfile.Contact_name = updatedProfile.Contact_name;
                        existingProfile.Phone_number = updatedProfile.Phone_number;
                        existingProfile.Email = updatedProfile.Email;
                        existingProfile.Operation_hours = updatedProfile.Operation_hours;
                        existingProfile.Website_Link = updatedProfile.Website_Link;

                        shelterProfileContext.SaveChanges();

                        int trinityRescueMissionId = 2;
                        return RedirectToAction("ShelterProfile", new { id = trinityRescueMissionId });
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Shelter profile not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while saving changes: " + ex.Message;
            }

            return View("EditShelterProfile", updatedProfile);
        }

        [Route("ClientManager")]
        public IActionResult ClientManager()
        {
            return ClientManagerMaster("", "", "All");
        }
        [Route("ClientManager/{ActiveFilter}")]
        public IActionResult ClientManager(string ActiveFilter)
        {
            return ClientManagerMaster("", "", ActiveFilter);
        }
        [Route("ClientManager/{FilterType}/{Filter}/{ActiveFilter}")]
        public IActionResult ClientManager(string Filtertype, string Filter, string ActiveFilter)
        {
            return ClientManagerMaster(Filtertype, Filter, ActiveFilter);
        }


        public IActionResult ClientManagerMaster(string Filtertype, string Filter, string ActiveFilter)
        {
            //for client manager button to appear when logged in
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;

            List<Client> Clients = new List<Client>();

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;

            ClientContext cc = new ClientContext(connStr);


            ShelterLocationContext slc = new ShelterLocationContext(connStr);
            var shelterLocations = from c in slc.ShelterLocations orderby c.Shelter_Location_Description select c;
            SelectList sl = new SelectList(shelterLocations, "Shelter_Location_ID", "Shelter_Location_Description");
            ViewBag.ShelterLocations = shelterLocations;
            ViewBag.ShelterLocationsFilters = sl;



            List<Client> myData = null;
            
            if (Filter.Length > 0)
            {
                if (Filtertype == "FirstName")
                {
                    var query = from c in cc.Clients
                                join s in cc.ShelterLocations on c.Shelter_Location_ID equals s.Shelter_Location_ID
                                where c.F_Name.ToUpper() == Filter.ToUpper() //&& c.Inactive_Date == null
                                orderby c.L_Name
                                select c;
                    myData = query.ToList();
                }
                else if (Filtertype == "LastName")
                {
                    var query = from c in cc.Clients
                                join s in cc.ShelterLocations on c.Shelter_Location_ID equals s.Shelter_Location_ID
                                where c.L_Name.ToUpper() == Filter.ToUpper()
                                orderby c.L_Name
                                select c;
                    myData = query.ToList();
                }
                else if (Filtertype == "ShelterLocation")
                {
                    int selectedID = int.Parse(Filter);
                    var query = from c in cc.Clients
                                join s in cc.ShelterLocations on c.Shelter_Location_ID equals s.Shelter_Location_ID
                                where c.Shelter_Location_ID == selectedID
                                orderby c.L_Name
                                select c;
                    myData = query.ToList();
                }
            }
            else
            {
                var query = from c in cc.Clients
                        join s in cc.ShelterLocations on c.Shelter_Location_ID equals s.Shelter_Location_ID
                        orderby c.L_Name
                        select c;
                myData = query.ToList();
            }
            //calculate number of rooms available by reading who is currently residing where.

            //var locCount = slc.ShelterLocations
            //.GroupBy(cGrp => new { cGrp.Shelter_Location_Description })
            //.Select(g => new
            //{
            //    ShelterLocation = g.Key,
            //    Available = g.Count(s => s.Shelter_Location_Available_Room >= 0),
            //    Total = g.Count(s => s.Shelter_Location_Total_Room >= 0)
            //})
            //.ToList();

            //var result = from c in cc.Clients
            //             join s in cc.ShelterLocations on c.Shelter_Location_ID equals s.Shelter_Location_ID
            //             group new { c, s } by s.Shelter_Location_Description into c
            //             select new ShelterLocation
            //             {
            //                 Shelter_Location_Description = c.Key.ToString(),
            //                 Shelter_Location_Available_Room = c.Count(),
            //                 Shelter_Location_Total_Room = c.Count()
            //             };

            //string s2 = "";

            if (ActiveFilter != "All")
            {
                if (ActiveFilter == "Active")
                {
                    myData = myData.FindAll(d => d.Inactive_Date == null).ToList();
                }
                else
                {
                    myData = myData.FindAll(d => d.Inactive_Date != null).ToList();
                }
            }

            return View(myData);
        }


        [Route("ClientDetailManager/{mode}/{client_ID}")]
        public IActionResult ClientDetailManager(string mode, int Client_ID)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;
            ClientContext cc = new ClientContext(connStr);
            ShelterLocationContext slc = new ShelterLocationContext(connStr);
            var shelterLocations = from c in slc.ShelterLocations orderby c.Shelter_Location_Description select c;
            ViewBag.ShelterLocations = shelterLocations;
            Client theClient = null;

            if (mode == "approve")
            {
                //integer being passed in this use case is actually the queueID from the client interest manager
                int queueID = Client_ID;
                ShownInterestContext sicc = new ShownInterestContext(connStr);
                List<ShownInterest> myData = null;
                HttpContext.Session.SetInt32("queueID", queueID);

                var query = from c in sicc.ShownInterests
                            where c.QueueID == queueID
                            select c;
                myData = query.ToList();

                if(myData != null && myData.Count == 1)
                {
                    theClient = new Client();
                    theClient.F_Name = myData[0].Fname;
                    theClient.L_Name = myData[0].Lname;
                    theClient.Shelter_Location_ID = myData[0].ShelterID;
                }
            }
            else
            {
                theClient = cc.Clients.Find(Client_ID);
            }

            if (theClient == null)
            {
                //in brand new insert use case, still need to create blank object.
                theClient = new Client();
            }

            return View(theClient);
        }

        [HttpPost]
        [Route("ClientDetailManager/{mode}/{client_ID}")]
        public IActionResult ClientDetailManager([FromForm] Client c)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;
            ViewBag.ErrorMessage = "";

            //first check if there is room for given shelter
            using (ShelterLocationContext db = new ShelterLocationContext(connStr))
            {
                var shelterLocations = from c2 in db.ShelterLocations where c2.Shelter_Location_ID == c.Shelter_Location_ID select c2;
                ShelterLocation sl = shelterLocations.FirstOrDefault();
                if (sl.Shelter_Location_Available_Room == 0)
                {
                    ViewBag.ErrorMessage = "Selected Shelter does not have room. Please select another shelter.";
                    
                    ClientContext cc3 = new ClientContext(connStr);
                    ShelterLocationContext slc = new ShelterLocationContext(connStr);
                    shelterLocations = from c3 in slc.ShelterLocations orderby c3.Shelter_Location_Description select c3;
                    ViewBag.ShelterLocations = shelterLocations;
                    return View(c);
                }
            }
            if (ViewBag.ErrorMessage == "")
            {
                if (c.ClientID == 0)
                {
                    //no client id, therefore insert
                    c.Active_Date = DateTime.Now;
                    using (ClientContext cc = new ClientContext(connStr))
                    {
                        cc.Clients.Add(c);
                        cc.SaveChanges();
                    }
                    //each time a new person added, decrement the shelter location available room for that location.
                    UpdateAvailableSlots(c.Shelter_Location_ID, true);
                }
                else
                {
                    //have client id, so update
                    using (ClientContext db = new ClientContext(connStr))
                    {
                        db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (HttpContext.Session.Keys.Contains("queueID"))
                {
                    try
                    {
                        int queueID = HttpContext.Session.GetInt32("queueID").Value;
                        if (queueID > 0)
                        {
                            ShownInterest si = null;
                            using (ShownInterestContext cc2 = new ShownInterestContext(connStr))
                            {
                                si = cc2.ShownInterests.Find(queueID);
                            }
                            using (ShownInterestContext cc = new ShownInterestContext(connStr))
                            {
                                cc.Entry(si).State = System.Data.Entity.EntityState.Deleted;
                                cc.SaveChanges();
                            }
                        }
                        HttpContext.Session.Remove("queueID");
                    }
                    catch (Exception e)
                    {
                        string erro = e.ToString();
                    }
                }
            }
            return View(null);
        }
        

        private void UpdateAvailableSlots(int shelterlocation_id, bool isOccupySlot)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;




            if (isOccupySlot == true)
            {
                //decrement available room
                using (ShelterLocationContext db = new ShelterLocationContext(connStr))
                {
                    var shelterLocations = from c in db.ShelterLocations where c.Shelter_Location_ID == shelterlocation_id select c;
                    ShelterLocation sl = shelterLocations.FirstOrDefault();
                    sl.Shelter_Location_Available_Room = sl.Shelter_Location_Available_Room - 1;
                    db.Entry(sl).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                //increment available room
                using (ShelterLocationContext db = new ShelterLocationContext(connStr))
                {
                    var shelterLocations = from c in db.ShelterLocations where c.Shelter_Location_ID == shelterlocation_id select c;
                    ShelterLocation sl = shelterLocations.FirstOrDefault();
                    sl.Shelter_Location_Available_Room = sl.Shelter_Location_Available_Room + 1;
                    db.Entry(sl).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }


        [HttpPost]
        [Route("DeleteClient")]
        public JsonResult DeleteClient(int clientIDVal)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;
            Client c = null;

            using (ClientContext cc = new ClientContext(connStr))
            {
                c = cc.Clients.Find(clientIDVal);
            }

            using (ClientContext cc = new ClientContext(connStr))
            {
               // cc.Entry(c).State = System.Data.Entity.EntityState.Deleted;
                c.Inactive_Date = DateTime.Now;
                cc.Entry(c).State = System.Data.Entity.EntityState.Modified;
                cc.SaveChanges();
            }

            this.UpdateAvailableSlots(c.Shelter_Location_ID, false);

            return Json(new { Success = false, Message = "Delete failed." });

        }

        [Route("ClientInterestManager")]
        public IActionResult ClientInterestManager()
        {
            //for client manager button to appear when logged in
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;

            List<ShownInterest> ClientsInterest = new List<ShownInterest>();

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;

            ShownInterestContext cc = new ShownInterestContext(connStr);


            ShelterLocationContext slc = new ShelterLocationContext(connStr);
            var shelterLocations = from c in slc.ShelterLocations orderby c.Shelter_Location_Description select c;
            SelectList sl = new SelectList(shelterLocations, "Shelter_Location_ID", "Shelter_Location_Description");
            ViewBag.ShelterLocations = shelterLocations;
            ViewBag.ShelterLocationsFilters = sl;



            List<ShownInterest> myData = null;


                var query = from c in cc.ShownInterests
                            join s in cc.ShelterLocations on c.ShelterID equals s.Shelter_Location_ID
                            orderby c.Lname
                            select c;
                myData = query.ToList();
            


            return View(myData);
        }


        [HttpPost]
        [Route("DenyClient")]
        public JsonResult DenyClient(int queueIDVal)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connStr = config.GetSection("Connnectionstrings:MyConnection").Value;
            ShownInterest c = null;

            using (ShownInterestContext cc = new ShownInterestContext(connStr))
            {
                c = cc.ShownInterests.Find(queueIDVal);
            }
            if(c != null && c.QueueID > 0)
            {
                using (ShownInterestContext cc = new ShownInterestContext(connStr))
                {
                    cc.Entry(c).State = System.Data.Entity.EntityState.Deleted;
                    cc.SaveChanges();
                }
            }



            return Json(new { Success = false, Message = "Deny failed." });

        }

        [Route("ClientCountPopup")]
        public IActionResult ClientCountPopup()
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




        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Manager m)
        {


            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connectionString = config.GetSection("Connnectionstrings:MyConnection").Value;
            ViewBag.ErrorMessage = "";
            if (m.ManagerID == 0)
            {
                using (ManagerContext mm = new ManagerContext(connectionString))
                {
                    mm.Managers.Add(m);
                    mm.SaveChanges();
                }
                TempData["AccountCreationMessage"] = "Account created successfully!";

            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult LoginView()
        {
            if (User.Identity.IsAuthenticated)
            {
                // if user is already logged in
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginView(LoginViewModel model)
        {
            _logger.LogInformation($"Attempting to authenticate user: {model.Username}");
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connectionString = config.GetSection("Connnectionstrings:MyConnection").Value;

            using (var context = new ManagerContext(connectionString))
            {
                try
                {
                    var manager = context.AuthenticateManager(model.Username, model.Password);

                    if (manager != null)
                    {
                        _logger.LogInformation($"User {model.Username} authenticated successfully.");
                        HttpContext.Session.SetInt32("IsAuthenticated", 1);
                        SetManagerInSession(manager.ManagerID);
                        TempData["LoginMessage"] = "Login successful!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["LoginMessage"] = "Invalid username or password";
                        _logger.LogWarning($"Invalid login attempt for user {model.Username}");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during login: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred during login");
                    return View(model);
                }
            }

        }
        private void SetManagerInSession(int managerId)
        {
            HttpContext.Session.SetInt32("ManagerID", managerId);
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("IsAuthenticated");

            await HttpContext.SignOutAsync();

            TempData["LogoutMessage"] = "You have been logged out successfully.";

            return RedirectToAction("Index");
        }

        public IActionResult AccountInformation()
        {
            //for client manager button to appear
            var isAuthenticated = HttpContext.Session.GetInt32("IsAuthenticated");
            ViewBag.IsAuthenticated = isAuthenticated == 1;
            //
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            string connectionString = config.GetSection("Connnectionstrings:MyConnection").Value;

            int managerId = HttpContext.Session.GetInt32("ManagerID") ?? 0;
            Manager manager = null;

                using (var context = new ManagerContext(connectionString))
                {
                    manager = context.Managers.FirstOrDefault(m => m.ManagerID == managerId);
                }

            return View(manager);
        }

        public IActionResult AuthenticatedIndex()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}