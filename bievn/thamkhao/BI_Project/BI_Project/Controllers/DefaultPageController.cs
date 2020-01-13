using BI_Project.Controllers;
using BI_SUN.Services.SetDefaultPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bicen.Controllers
{
    public class DefaultPageController : BaseController
    {
        // GET: DefaultPage
        //[CheckUserMenus]
        public ActionResult SetDefaultPage(int id)
        {
            ViewData["pagename"] = "SetDefaultPage";
            ViewData["action_block"] = "DefaultPage/SetDefaultPage";
            SetCommonData();
            GetLanguage();
            SetConnectionDB();
            SetDefaultPageService _setDefaultPage = new SetDefaultPageService(DBConnection);

            ViewData["usermenu"] = _setDefaultPage.GetListDefaultPage(id);
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        //[Route("/SetDefaultPage/{id?}")]
        public ActionResult SetDefaultPage(int id, int menuId)
        {
            SetConnectionDB();
            SetDefaultPageService _setDefaultPage = new SetDefaultPageService(DBConnection);
            ViewData["usermenu"] = _setDefaultPage.GetListDefaultPage(id);
            _setDefaultPage.UpdatePageDefault(id, menuId);
            return Redirect("SetDefaultPage?id=" + id);
        }
    }
}
