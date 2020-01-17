using BI_Project.Controllers;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using BI_Project.Models.UI;
using BI_Project.Services.Importers;
using bicen.Services.Importers;
using DemoData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace bicen.Controllers
{
    public class EVNImportExcelPLController : BaseController
    {
        // GET: EVNImportExcelPL
        // GET: EVNImportExcel
        public string EXCEL_UPLOAD_FOLDER { set; get; }
        public string EXCEL_EXPORT_FOLDER { get; set; }


        public EVNImportExcelPLController() : base()
        {
            this.EXCEL_UPLOAD_FOLDER = WebConfigurationManager.AppSettings["EXCEL_UPLOAD_FOLDER"];
            this.EXCEL_EXPORT_FOLDER = WebConfigurationManager.AppSettings["EXCEL_EXPORT_FOLDER"];
        }

        [HttpGet]
        //[CheckUserMenus]
        public ActionResult Index(string id)
        {
            int noPages = 0, noRecords = 0;
            BlockUIExcelUploadModel uiModel = new BlockUIExcelUploadModel();
            try
            {
                uiModel.CurrentPage = Int32.Parse(Request.Params["page"]);

            }
            catch (Exception e)
            {
                uiModel.CurrentPage = 1;
            }


            this.CheckPermission();

            ViewData["pagename"] = "upload_excel";
            ViewData["action_block"] = "Excels/EVNPL_block_upload_excel";
            this.SetCommonData();
            this.GetLanguage();
            this.SetConnectionDB();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = pageModel.GetElementByPath("page_excel.menu" + id + ".h1");
            pageModel.Title = pageModel.GetElementByPath("page_excel.menu" + id + ".title");
            ViewData["page_model"] = pageModel;


            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            BlockDataExcelUploadModel blockData = new BlockDataExcelUploadModel();
            blockData.PermissionID = id;

            //BI_Project.Helpers.Utility.JTokenHelper.GetLanguage("~/" + this.LANGUAGE_FOLDER, this.LANGUAGE);
            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + blockData.PermissionID.ToString() + ".xml";
            blockData.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");
            blockData.Note = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Note.#cdata-section");
            blockData.Export = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExport"));
            blockData.ExportBaseMonth = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExportBaseMonth"));
            blockData.Url = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.url");

            blockData.ExportOff = Convert.ToBoolean(BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.Export.isExportOff"));


            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
            //ImporterServices _services = new ImporterServices(DBConnection);

            blockData.ListHistory = services.GetHistoryList(uiModel.CurrentPage, uiModel.PerPage, id, ref noPages, ref noRecords, uiModel.Month, uiModel.Year);
            blockData.ListDNT = services.GetList_DNT_QMKLTN_HA1820();
            blockData.NumberPages = noPages;
            blockData.NumberRecords = noPages;
            blockData.CurrentPage = uiModel.CurrentPage;
            blockData.FolderUpload = pageModel.GetElementByPath("page_excel.menu" + id + ".UploadedDirectory");
            blockModel.DataModel = blockData;
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");

            ViewData["BlockData"] = blockModel;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        public ActionResult Index(string id, BlockDataExcelUploadModel uiModel)
        {
            int noPages = 0, noRecords = 0;

            try
            {
                uiModel.CurrentPage = Int32.Parse(Request.Params["page"]);

            }
            catch (Exception)
            {
                uiModel.CurrentPage = 1;
            }
            this.CheckPermission();

            ViewData["pagename"] = "upload_excel";
            ViewData["action_block"] = "Excels/EVNPL_block_upload_excel";
            SetCommonData();
            GetLanguage();

            //DECLARE PAGE MODEL
            BI_Project.Models.UI.PageModel pageModel = new PageModel("upload_excel");
            pageModel.SetLanguage(this.LANGUAGE_OBJECT);
            pageModel.H1Title = pageModel.GetElementByPath("page_excel.menu" + id + ".h1");
            pageModel.Title = pageModel.GetElementByPath("page_excel.menu" + id + ".title");
            ViewData["page_model"] = pageModel;


            BlockLangExcelUpload blockLang = new BlockLangExcelUpload();
            BI_Project.Models.UI.BlockModel blockModel = new BlockModel("block_upload_excel", this.LANGUAGE_OBJECT, blockLang);
            //BlockDataExcelUploadModel blockData = new BlockDataExcelUploadModel();
            uiModel.PermissionID = id;
            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);

            string xmlConfigFilePath = this.CONFIG_FOLDER + "\\excel_format_" + uiModel.PermissionID.ToString() + ".xml";
            uiModel.HelpDoc = BI_Project.Helpers.Utility.JTokenHelper.GetElementValue(xmlConfigFilePath, "excel_source.HelpDocumentPath");


            uiModel.ListHistory = services.GetHistoryList(uiModel.CurrentPage, uiModel.PerPage, id, ref noPages, ref noRecords, uiModel.Month, uiModel.Year);
            uiModel.NumberPages = noPages;
            uiModel.NumberRecords = noPages;
            uiModel.CurrentPage = uiModel.CurrentPage;
            uiModel.FolderUpload = pageModel.GetElementByPath("page_excel.menu" + id + ".UploadedDirectory");
            blockModel.DataModel = uiModel;
            //blockModel.DataModel = ViewData["block_menu_left_data"];
            if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            ViewData["BlockData"] = blockModel;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        /// <summary>
        /// Use with the xml file contains paras elements
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public ActionResult ImportExcel(FileUploadModel model)
        {
            //Test run time upload excel
            //DateTime timeFinish = DateTime.UtcNow;
            //DateTime timeStart = DateTime.UtcNow;
            //TimeSpan timeSpan = new TimeSpan();
            //timeStart = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));
            this.GetLanguage();
            try
            {

                //--------------------GET XML CONFIG -----------------------------------------

                this.SetConnectionDB();
                EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
                List<string> lstCellName = new List<string>();
                string xmlFilePath = this.CONFIG_FOLDER + "\\excel_format_" + model.PermissionId.ToString() + ".xml";
                ExcelModel lstColumns = services.GetXMLConfig(xmlFilePath);

                //************************** UPLOAD FILE TO THE UPLOAD FOLDER***********************************************

                string excelfilename = BI_Project.Helpers.Utility.APIStringHelper.GenerateId(model.currentyear, model.currentMonth) + Path.GetExtension(model.FileObj.FileName);

                //string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "/" + excelfilename;
                string excelFilePath = System.Web.Hosting.HostingEnvironment.MapPath(this.EXCEL_UPLOAD_FOLDER) + lstColumns.FolderUploadedDirectory + "\\" + excelfilename;
                model.FileObj.SaveAs(excelFilePath);

                //************************ INSERT DATA TO DATABASE ********************
                int userid = (int)Session[this.SESSION_NAME_USERID];
                int uploadRoleId = model.PermissionId;

                services.ImportPLDatabase(userid, excelFilePath, uploadRoleId, lstColumns, excelfilename, model.currentMonth, model.currentyear);

                //services.Import2Database(userid, excelFilePath)
                if (services.ERROR != null) throw new Exception(services.ERROR);

                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.success");

                Session["msg_code"] = 1;

                //timeFinish = DateTime.Parse(DateTime.Now.ToString("h:mm:ss tt"));

                //timeSpan = timeFinish.Subtract(timeStart);

                //FileHelper.SaveFile(timeSpan, this.LOG_FOLDER + "/TIMESPAN" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            }

            catch (Exception ex)
            {
                //FileHelper.SaveFile(new { ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                ERRORS = ex.Message;
                Session["msg_text"] = BlockLanguageModel.GetElementLang(this.LANGUAGE_OBJECT, "messages.block_upload_excel.error") + " Lỗi " + ex.Message;

                Session["msg_code"] = -1;
            }

            return RedirectToAction("Index/" + model.PermissionId);
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Delete()
        {
            if (Session["IsAdmin"] is false)
            {
                return RedirectToAction("Logout", "Home");
            }
            string id = this.HttpContext.Request["treeId"];
            string fileName = this.HttpContext.Request["fileUpload"];
            Logging.WriteToLog(this.GetType().ToString() + "-delete(), id=" + id + ", file name = " + fileName, LogType.Access);
            int output = 0;
            try
            {
                this.SetConnectionDB();
                EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
                int userid = (int)Session[this.SESSION_NAME_USERID];
                output = services.Delete(fileName, userid);
                if (services.ERROR != null) throw new Exception(services.ERROR);
            }
            catch (Exception ex)
            {
                this.ERRORS = ex.ToString();
                FileHelper.SaveFile(ex, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                output = -2;
            }
            ////*************************************XU LY VAN DE THONG BAO THANH CONG HAY THAT BAI********************
            this.GetLanguage();
            return RedirectToAction("Index/" + id);

        }

        [HttpGet]
        public ActionResult jqGrid()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetDataRows(string sidx, string sord, int page, int rows, int year, int month, int file)
        {
            var products = Product.GetSampleProducts();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = products.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            int uploadYear = year;
            int uploadMonth = month;
            int uploadFile = file;

            //var data = products.OrderBy(x => x.Id)
            //             .Skip(pageSize * (page - 1))
            //             .Take(pageSize).ToList();

            this.SetConnectionDB();
            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
            var dataTable = null;
            switch (file)
            {
                case 1:
                    dataTable = services.GetList_DNT_QMKLTN_HA1820(month, year); break;
                case 2:
                    dataTable = services.GetList_DNT_QMKLTN_NVK(month, year); break;
                case 3:
                    dataTable = services.GetList_DNT_QMKL_QD41(month, year); break;
                case 4:
                    dataTable = services.GetList_DNT_QMKLTN_QD2081(month, year); break;
                case 5:
                    dataTable = services.GetList_DNT_THCDIEN_PL71(month, year); break;
                case 6:
                    dataTable = services.GetList_DNT_THCDIEN_PL72(month, year); break;
                default:
                    dataTable = null;
            }
            // var data2 = services.GetList_DNT_QMKLTN_HA1820();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = dataTable
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataRowById(string id)
        {

            this.SetConnectionDB();
            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
            var rows = services.GetList_DNT_QMKLTN_HA1820().Where(x => x.MA_DVIQLY == id);

            if (rows != null)
            {
                DNT_QMKLTN_HA1820 model = new DNT_QMKLTN_HA1820();

                foreach (var item in rows)
                {
                    model.MA_DVIQLY = item.MA_DVIQLY;
                    model.TEN_CTRINH = item.TEN_CTRINH;
                    model.THANG_BC = item.THANG_BC;
                    model.NAM_BC = item.NAM_BC;
                    model.SL_XA = item.SL_XA;
                    model.SL_TCBD = item.SL_TCBD;
                    model.MA_DVIQLY = item.MA_DVIQLY;
                    model.DZ_HTHE = item.DZ_HTHE;
                    model.SO_HOTN = item.SO_HOTN;
                    model.GTCL_VVNSNN = item.GTCL_VVNSNN;
                    model.GTCL_VV = item.GTCL_VV;
                    model.GTCL_VDTHTX = item.GTCL_VDTHTX;
                    model.GTCL_VDAN = item.GTCL_VDAN;
                    model.GTCL_VKHAC = item.GTCL_VKHAC;
                    model.CPHI_TNCT = item.CPHI_TNCT;
                }

                return PartialView("_GridEditPartial", model);
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddDNTData()
        {
            if (Session["IsAdmin"] == null || (bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SetCommonData();
            this.GetLanguage();

            ViewData["pagename"] = "dnt_upload";
            ViewData["action_block"] = "EVNImportExcelPL/EVNPL_block_upload_dnt";

            BlockLangRowUploadModel blockLang = new BlockLangRowUploadModel();
            BI_Project.Models.UI.BlockModel blockModel = new BlockModel("block_dnt_upload", this.LANGUAGE_OBJECT, blockLang);
            BlockDataRowUploadModel blockData = new BlockDataRowUploadModel();
            blockModel.DataModel = blockData;

            ViewData["BlockData"] = blockModel;
            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpPost]
        [CheckUserMenus]
        public ActionResult UploadTableData(BlockDataRowUploadModel model)
        {
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["IsAdmin"] == null || (bool)Session["IsAdmin"] == false)
            {
                return RedirectToAction("Logout", "Home");
            }

            this.SetConnectionDB();
            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);

            int file = model.File;
            var lst =  null;
            int output = 0;

            switch(file) {
                case 1:
                    lst = new List<DNT_QMKLTN_HA1820>();
                    break;
                case 2:
                    lst = new List<DNT_QMKLTN_NVK>();
                    break;
                case 3:
                    lst = new List<DNT_QMKL_QD41>();
                    break;
                case 4:
                    lst = new List<DNT_QMKLTN_QD2081>();
                    break;
                case 5:
                    lst = new List<DNT_THCDIEN_PL71>();
                    break;
                case 6:
                    lst = new List<DNT_THCDIEN_PL72>();
                    break;
                default:
                    break;
            }     

            lst = JsonConvert.DeserializeAnonymousType(model.DataString, lst);
            output = services.ExecuteDataTable(model.Month, model.Year, model.File, lst);

            return RedirectToAction("AddDNTData");
        }
    }
}