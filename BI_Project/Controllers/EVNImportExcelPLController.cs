﻿using System;
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
using BI_Project.Controllers;
using DemoData;

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
            Logging.WriteToLog(this.GetType().ToString() + "-delete(), id=" + id+", file name = "+ fileName, LogType.Access);
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

        [HttpGet]
        public JsonResult GetDataRow(string sidx, string sord, int page, int rows)
        {
            this.SetConnectionDB();
            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
            var data = services.GetList_DNT_QMKLTN_HA1820();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = data.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var json = data.OrderBy(x => x.MA_DVIQLY)
                         .Skip(pageSize * (page - 1))
                         .Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = json
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRows()
        {
            this.SetConnectionDB();
            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
            var data = services.GetList_DNT_QMKLTN_HA1820();


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProducts(string sidx, string sord, int page, int rows)
        {
            var products = Product.GetSampleProducts();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = products.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var data = products.OrderBy(x => x.Id)
                         .Skip(pageSize * (page - 1))
                         .Take(pageSize).ToList();

            this.SetConnectionDB();
            EVNImporterServices services = new EVNImporterServices(oracleConnection, DBConnection);
            var data2 = services.GetList_DNT_QMKLTN_HA1820();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = data2
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}