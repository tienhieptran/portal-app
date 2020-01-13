﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BI_Project.Services.Roles;
using BI_Project.Models.EntityModels;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using BI_Project.Models.UI;

namespace BI_Project.Controllers
{
    public class RolesController : BaseController
    {
        //public ActionResult List()
        //{
        //    if (null == Session[this.SESSION_NAME_USERID])
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //    if (Session["IsAdmin"] is false)
        //    {
        //        return RedirectToAction("Logout", "Home");
        //    }
        //    this.SetCommonData();
        //    ViewData["pagename"] = "role_list";
        //    ViewData["action_block"] = "Roles/block_role_list";


        //    this.GetLanguage();


        //    this.SetConnectionDB();
        //    RoleServices services = new RoleServices(this.DBConnection);

        //    BlockLangRoleListModel blockLang = new BlockLangRoleListModel();
        //    BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_role_list", this.LANGUAGE_OBJECT, blockLang);
        //    //blockModel.DataModel = ViewData["block_menu_left_data"];
        //    blockModel.DataModel = services.GetList();
        //    blockModel.Hidden = 0;

        //    Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

        //    ViewData["departments"] = departmentServices.GetList();

        //    if (services.ERROR != null) FileHelper.SaveFile(services.ERROR, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
        //    this.SaveAccessLog("list");
        //    ViewData["BlockData"] = blockModel;
        //    return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        //}

        public ActionResult List(int? DepartID = null)
        {
            if (Session["IsAdmin"] is false)
            {
                return RedirectToAction("Logout", "Home");
            }
            if (!Request.IsAjaxRequest())
            {
                if (null == Session[SESSION_NAME_USERID])
                {
                    return RedirectToAction("Login", "Home");
                }
                if (Session["IsAdmin"] is false)
                {
                    return RedirectToAction("Logout", "Home");
                }
                SetCommonData();
                ViewData["pagename"] = "role_list";
                ViewData["action_block"] = "Roles/block_role_list";


                GetLanguage();


                SetConnectionDB();
                RoleServices services = new RoleServices(DBConnection);

                BlockLangRoleListModel blockLang = new BlockLangRoleListModel();
                BlockModel blockModel = new Models.UI.BlockModel("block_role_list", LANGUAGE_OBJECT, blockLang);
                //blockModel.DataModel = ViewData["block_menu_left_data"];
                if ((bool)Session["IsSuperAdmin"])
                {
                    blockModel.DataModel = services.GetList();
                }
                else
                {
                    blockModel.DataModel = services.GetList((int)Session["DepartIdUserLogin"]);
                }
                ViewData["BlockData"] = blockModel;
                return View("~/" + THEME_FOLDER + "/" + THEME_ACTIVE + "/index.cshtml");
            }
            else
            {
                SetConnectionDB();
                RoleServices services = new RoleServices(DBConnection);
                var roleList = services.GetListDept(DepartID);
                return Json(roleList, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult Create(int? roleId)
        {
            this.SaveAccessLog("create");
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["IsAdmin"] is false)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SetCommonData();


            ViewData["pagename"] = "role_create";
            ViewData["action_block"] = "Roles/block_role_create";
            ViewData["data-form"] = TempData["data"];

            string roleid = (Request.QueryString["roleid"] == null ? "0" : Request.QueryString["roleid"].ToString());
            this.SetConnectionDB();
            RoleServices services = new RoleServices(this.DBConnection);

            BlockDataRoleCreateModel model = new BlockDataRoleCreateModel();

            if (ViewData["data-form"] != null)
            {
                model = (BlockDataRoleCreateModel)ViewData["data-form"];
            }
            else
            {
                model = services.GetEntityById(Int32.Parse(roleid));
            }

            //BlockDataRoleCreateModel model = services.GetEntityById(Int32.Parse(roleid));
            this.GetLanguage();
            if(model.RoleId>0 ) ViewData["pagename"] = "role_edit";

            Services.Departments.DepartmentServices departmentServices = new Services.Departments.DepartmentServices(this.DBConnection);

            ViewData["departments"] = departmentServices.GetList();
            ViewData["listdepartmentsadmin"] = departmentServices.GetListAdminLogin((string)Session["CodeIsAdmin"]);

            if (services.ERROR != null) FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            BlockLangRoleCreateModel blockLang = new BlockLangRoleCreateModel();
            BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_role_create", this.LANGUAGE_OBJECT, blockLang);
            blockModel.DataModel = model;
            ViewData["BlockData"] = blockModel;
            ViewData["CurrentRoleId"] = roleId;
            ViewData["CurrentOrgId"] = model.DeptID;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");
        }

        [HttpGet]
        [CheckUserMenus]
        public ActionResult Delete()
        {
            this.SaveAccessLog("delete");
            if (Session["IsAdmin"] is false)
            {
                return RedirectToAction("Logout", "Home");
            }
            string id = this.HttpContext.Request["roleid"];
            int output = 0;
            try
            {
                this.SetConnectionDB();


                
                RoleServices services = new RoleServices(this.DBConnection);
                if (services.ERROR != null) throw new Exception(services.ERROR);
                output = services.Delete(Int32.Parse(id));
            }
            catch (Exception ex)
            {
                 FileHelper.SaveFile(new { data = id, ERROR = ex }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
                output = -2;
            }
            //*************************************XU LY VAN DE THONG BAO THANH CONG HAY THAT BAI********************
            this.GetLanguage();
            BlockLangRoleCreateModel blockLang = new BlockLangRoleCreateModel();
            blockLang.BlockName = "block_role_list";
            blockLang.SetLanguage(this.LANGUAGE_OBJECT);
            
            Session["msg_code"] = output;
            if (output > 1)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_role_list.success_delete", this.LANGUAGE_OBJECT);
            }


            return RedirectToAction("List");
        }

        [HttpPost]
        [CheckUserMenus]
        public ActionResult Create(BlockDataRoleCreateModel model)
        {
            if (Session["IsAdmin"] is false)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SaveAccessLog("create");
            if (null == Session[this.SESSION_NAME_USERID])
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["IsAdmin"] is false)
            {
                return RedirectToAction("Logout", "Home");
            }
            this.SetConnectionDB();

            RoleServices services = new RoleServices(this.DBConnection);

            int result = services.Create(model);
            
            if(services.ERROR != null)
            {
                FileHelper.SaveFile(new { data = model, ERROR = services.ERROR }, this.LOG_FOLDER + "/ERROR_" + this.GetType().ToString() + APIStringHelper.GenerateFileId() + ".txt");
            }           
            //*************************************XU LY VAN DE THONG BAO THANH CONG HAY THAT BAI********************
            this.GetLanguage();
            BlockLangRoleCreateModel blockLang = new BlockLangRoleCreateModel();
            blockLang.BlockName = "block_role_create";
             blockLang.SetLanguage(this.LANGUAGE_OBJECT);
            Session["msg_text"] = blockLang.GetMessage(result);
            Session["msg_code"] = result;
            if (model.RoleId>0 && result>1)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_role_create.success_edit", this.LANGUAGE_OBJECT);
            }
            if(result == 0)
            {
                Session["msg_text"] = blockLang.GetLangByPath("messages.block_role_create.error_business_1", this.LANGUAGE_OBJECT);
                //return RedirectToAction("Create?roleid=" + model.RoleId);
            }
            //*********************** INSERT OR EDIT SUCCESSFULLY**************************************************
            if (result>0)
            {
                return RedirectToAction("List");
            }

            //*************************************************************************
            //BlockLangRoleCreateModel blockLang = new BlockLangRoleCreateModel();
            //BI_Project.Models.UI.BlockModel blockModel = new Models.UI.BlockModel("block_role_create", this.LANGUAGE_OBJECT, blockLang);
            //blockModel.DataModel = model;
            //ViewData["BlockData"] = blockModel;
            //return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/index.cshtml");

            TempData["data"] = model;
            return RedirectToAction("Create");


        }
    }
}