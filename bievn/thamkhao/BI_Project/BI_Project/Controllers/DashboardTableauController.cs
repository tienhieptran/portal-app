using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.IO;

using BI_Project.Services.Importers;
using BI_Project.Models.EntityModels;
using BI_Project.Models.UI;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;

using BI_Project.Services.Tableau;
using BI_Project.Services.Menus;
using System.Configuration;
using bicen.Services;
using BI_Project.Services.User;
using BI_Project.Services.Departments;
using System.Text;

namespace BI_Project.Controllers
{
    public class DashboardTableauController : BaseController
    {
        public string EXCEL_UPLOAD_FOLDER { set; get; }


        public DashboardTableauController() : base()
        {
            this.EXCEL_UPLOAD_FOLDER = WebConfigurationManager.AppSettings["EXCEL_UPLOAD_FOLDER"];
            //this.DBConnection.ConnectString = this.CONNECT_STRING_STAGING;
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Index(int id)
        {
            //lay url tu menu voi id

            ViewData["pagename"] = "Embed_Tableau";
            ViewData["action_block"] = "Tableau/TableauView";

            SetCommonData();
            GetLanguage();
            SetConnectionDB();


            BI_Project.Models.UI.PageModel pageModel = new Models.UI.PageModel("Embed_Tableau");
            // BI_Project.Models.UI.BlockModel blockModel = new BlockModel("TableauView");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            //pageModel.H1Title = pageModel.GetElementByPath("page_excel.menu" + id + ".h1");
            pageModel.Title = pageModel.GetElementByPath("title");
            ViewData["page_model"] = pageModel;

            TableauModel param = new TableauModel();
            ViewData["BlockData"] = param;
            MenuServices _menuServices = new MenuServices(DBConnection);


            EntityMenuModel _entityMenuModel = _menuServices.GetMenuModel(id.ToString());

            UserServices _userServices = new UserServices(DBConnection);

            DepartmentServices _departmentServices = new DepartmentServices(DBConnection);

            EntityDepartmentModel _entityDepartmentModel = new EntityDepartmentModel();

            //param.Site_Root = _entityMenuModel.Site_Root;
            param.Ticket = Helpers.TableauHelper.GetTicket("");
            param.TableauUrl = _entityMenuModel.TableauUrl;
            param.Hidden = 1;
            param.username = Session["UserName"].ToString();
            ViewBag.Id = id;

            var listFilter01 = _departmentServices.GetList().Select(x => x.Filter01).ToArray();

            StringBuilder builderOrganization = new StringBuilder();
            foreach (var _list in listFilter01)
            {
                builderOrganization.Append(_list).Append(',');
            }


            string _resultListOrganization = builderOrganization.ToString().TrimEnd(',');
            ViewBag.ListDepartment = _resultListOrganization;



            var getUser = _userServices.GetList();
            var getUserCheck = getUser.FirstOrDefault(x => x.UserName == param.username);
            var getDepartment = _departmentServices.GetEntityById(getUserCheck.DeptId);


            if (getUserCheck.IsAdmin == false && (getDepartment.Filter01 != "PE" || getDepartment.Filter01 != "PA" || getDepartment.Filter01 != "PB" || getDepartment.Filter01 != "PC" || getDepartment.Filter01 != "PD"))
            {
                param.GetFilter(id);
            }
            //param.GetFilter(id);
            Random rd = new Random();
            int item = rd.Next(100, 999);
            string log = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) + "_" + item;
            if (_menuServices.ERROR != null) FileHelper.SaveFile(new { ERROR = _menuServices.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            FileHelper.SaveFile(_entityMenuModel, this.LOG_FOLDER + "/MenuModel_" + log + ".txt");
            FileHelper.SaveFile(param.Ticket, this.LOG_FOLDER + "/Ticket_" + log + ".txt");
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");


        }


    }
}