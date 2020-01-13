﻿using BI_Project.Models.EntityModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using OracleInternal.SqlAndPlsqlParser;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using BI_Project.Helpers;
using BI_Project.Helpers.Utility;
using System.IO;
using OfficeOpenXml;
using bicen.Services.Importers;

namespace BI_Project.Services.Importers
{
    public class EVNImporterServices : DBBaseService
    {
        //ConnectOracleDB _dbDwhConnection;
        public string ERROR_USER { set; get; }
        public EVNImporterServices(ConnectOracleDB connection, DBConnection dBConnection) : base(connection, dBConnection)
        {
        }

        public string GetOleDbConnectionString(string filePath)
        {
            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
        }

        public ExcelModel GetXMLConfig(string xmlFilePath)
        {
            ExcelModel output = new ExcelModel();
            JObject jObject = BI_Project.Helpers.Utility.JTokenHelper.GetXML2Jobject(xmlFilePath);

            output.FolderUploadedDirectory = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.UploadedDirectory");
            output.DBTableName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.database.tablename");
            string strstartRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.@started_row");
            output.ExcelStartRow = Int32.Parse(strstartRow);
            string strnumberRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_row");
            //number_row = Int32.Parse(strnumberRow);

            string strnumberCell = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_cell");
            //number_cell = Int32.Parse(strnumberCell);
            output.ExcelSheetName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.#text");
            output.FolderHelpDocumentPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.HelpDocumentPath");
            output.LangNote = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.Note.#cdata-section");
            output.FolderFileNativeName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.FileNativeName");

            //LAY CAC COT TUONG UNG
            IEnumerable<JToken> columns = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, "..mapping.column");
            int index = 0;
            foreach (JToken column in columns)
            {
                MappingExcelDB mappingExcel = new MappingExcelDB();
                mappingExcel.ColumnName = column.SelectToken("..name").Value<string>();
                mappingExcel.ExcelColumn = index;
                mappingExcel.Datatype = column.SelectToken("..datatype").Value<string>();
                output.DBLstColumns.Add(mappingExcel);
                index++;
            }

            //LAY CAC THAM SO PARAMETERS
            IEnumerable<JToken> paras = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, "..paras.para");
            index = 0;
            foreach (JToken para in paras)
            {
                //XMLParaModel 
                XMLParaModel paraXMLModel = new XMLParaModel();
                paraXMLModel.Name = para.SelectToken("..name").Value<string>();
                paraXMLModel.Row = para.SelectToken("..row").Value<Int32>();
                //paraXMLModel.Column = para.SelectToken("..column").Value<Int32>();
                paraXMLModel.DataType = para.SelectToken("..datatype").Value<string>();
                paraXMLModel.Active = para.SelectToken("..active").Value<string>();
                paraXMLModel.Value = para.SelectToken("..value").Value<string>();

                output.XmlParas.Add(paraXMLModel.Name, paraXMLModel);
            }

            return output;
        }
        public List<MappingExcelDB> GetColumnList(string xmlFilePath, ref string uploadFolder, ref string tablename, ref int startRow, ref string sheetActive,
            ref string helpDocumentPath, ref string note, ref string fileNativeName) //, ref int number_row, ref int number_cell, ref List<string> lstCellName
        {
            List<MappingExcelDB> output = new List<MappingExcelDB>();
            JObject jObject = BI_Project.Helpers.Utility.JTokenHelper.GetXML2Jobject(xmlFilePath);

            uploadFolder = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.UploadedDirectory");
            tablename = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.database.tablename");
            string strstartRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.@started_row");
            startRow = Int32.Parse(strstartRow);
            string strnumberRow = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_row");
            //number_row = Int32.Parse(strnumberRow);

            string strnumberCell = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell.@number_cell");
            //number_cell = Int32.Parse(strnumberCell);
            sheetActive = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.sheet_active.#text");
            helpDocumentPath = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.HelpDocumentPath");
            note = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.Note.#cdata-section");
            fileNativeName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.FileNativeName");

            //get list cell name
            //string cellName = BI_Project.Helpers.Utility.JTokenHelper.GetElementLanguage(jObject, "excel_source.ExchangingData.excel.cell_name");
            //string[] lstName = null;
            //if (cellName != null && cellName != "")
            //{
            //    lstName = cellName.Split(',');
            //    for (int i = 0; i < lstName.Length; i++)
            //    {
            //        lstCellName.Add(lstName[i].ToString());
            //    }
            //}

            IEnumerable<JToken> columns = BI_Project.Helpers.Utility.JTokenHelper.GetList(jObject, "..mapping.column");


            int index = 0;

            foreach (JToken column in columns)
            {
                MappingExcelDB mappingExcel = new MappingExcelDB();
                mappingExcel.ColumnName = column.SelectToken("..name").Value<string>();
                mappingExcel.ExcelColumn = index;
                mappingExcel.Datatype = column.SelectToken("..datatype").Value<string>();
                output.Add(mappingExcel);
                index++;
            }

            return output;

        }



        public int ImportToDatabase(int currentYear, int userid, string excelFilePath, string tablename, int startRow, string sheetActive,
            string helpDocumentPath, string note, string fileNativeName, string navetiveFile, int uploadRoleId, List<MappingExcelDB> lstMapping,
            string FileUploadedName) //, int number_row, int number_cell, List<string> lstCellName
        {
            //List<int> data = GetDataInLine(excelFilePath, sheetActive, number_cell, fileNativeName, number_row);
            int output = 0;
            int index = 0, columnIndex = 0;
            Char charRange = ':';
            int Year = 0;
            int Month = 0;
            //Dictionary<string, int> line = new Dictionary<string, int>();
            //if (data != null)
            //{
            //    for (int i = 0; i < lstCellName.Count; i++)
            //    {
            //        line.Add(lstCellName[i], data[i]);
            //    }
            //}


            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            OracleTransaction transaction = null;
            try
            {


                //GET EXCEL DATA
                string sqlOle = "select * from [" + sheetActive + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;


                int idHistory = 0;
                //******************INSERT INTO HISTORY*********************************************************
                //this.ConnectOracleDB.OpenDBConnect();6
                this.InsertHistory(userid, uploadRoleId, helpDocumentPath, note, navetiveFile, FileUploadedName, ref idHistory, Year, Month);




                ConnectOracleDB.OpenDBConnect();
                transaction = ConnectOracleDB.OracleDBConnect.BeginTransaction();
                ConnectOracleDB.command.Transaction = transaction;
                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        index++;
                        string sqlInsertDW = "insert into " + tablename + "(";
                        string sqlInsertDWValues = " values (";
                        string _sqlCheck = "Delete from  " + tablename + " WHERE  Year = " + Year;

                        bool isCommplateGenerateSQL = false;
                        if (index == 1)
                        {
                            string time = dataReader[0].ToString();
                            int startIndex = time.IndexOf(charRange);
                            if (time.ToString() == "")
                            {
                                throw new Exception("Time is null or not correct format.");
                            }
                            int endIndex = time.Length;
                            int length = endIndex - startIndex - 1;
                            int number = Int32.Parse(time.Substring(startIndex + 1, length));
                            if (number > 12) Year = number;
                            else
                                Month = number;


                            //Delete data before insert with condition year
                            //_sqlCheck = Year > 0 ? "Delete from  " + tablename + " WHERE  Year = " + Year :
                            //    "Delete from  " + tablename + " WHERE  MONTH = " + Month;                          
                            //this.ConnectOracleDB.command.CommandText = _sqlCheck;
                            //this.ConnectOracleDB.command.ExecuteNonQuery();
                        }


                        if (index < startRow) continue;

                        ConnectOracleDB.command.Parameters.Clear();
                        columnIndex = 0;

                        int countNullColumn = 0;
                        foreach (MappingExcelDB column in lstMapping)
                        {
                            //var test = dataReader.
                            columnIndex++;
                            column.Value = dataReader[column.ExcelColumn];
                            if (!isCommplateGenerateSQL)
                            {
                                sqlInsertDW += column.ColumnName + ",";
                                sqlInsertDWValues += ":" + column.ColumnName + ",";
                            }

                            try
                            {
                                if (column.Value.ToString().Trim() == "")
                                {
                                    countNullColumn++;
                                }
                            }
                            catch (Exception)
                            {
                                countNullColumn++;
                            }
                            ConnectOracleDB.command.Parameters.Add(new OracleParameter(":" + column.ColumnName, column.Value ?? (object)DBNull.Value));
                        }

                        //if (line != null)
                        //{
                        //    foreach (var cell in line)
                        //    {
                        //        if (!isCommplateGenerateSQL)
                        //        {
                        //            sqlInsertDW += cell.Key + ",";
                        //            sqlInsertDWValues += "@" + cell.Key + ",";
                        //        }
                        //        this._dbDwhConnection.command.Parameters.Add(new OracleParameter("@" + cell.Key, cell.Value));
                        //    }
                        //}

                        if (!isCommplateGenerateSQL)
                        {
                            sqlInsertDW = Year > 0 ? sqlInsertDW.Trim(',') + ",Year)" : sqlInsertDW.Trim(',') + ",Month)";
                            sqlInsertDWValues = Year > 0 ? sqlInsertDWValues.Trim(',') + ",:Year)" : sqlInsertDWValues.Trim(',') + ", :Month)";
                            if (Year > 0)
                                ConnectOracleDB.command.Parameters.Add(new OracleParameter(":Year", Year));
                            else
                                ConnectOracleDB.command.Parameters.Add(new OracleParameter(":Month", Month));
                            isCommplateGenerateSQL = true;
                        }

                        //*****************************INSERT INTO DATABASE******************************************************
                        if (countNullColumn == lstMapping.Count) break;
                        else
                        {
                            this.ConnectOracleDB.command.CommandText = sqlInsertDW + sqlInsertDWValues;
                            this.ConnectOracleDB.command.CommandType = CommandType.Text;
                            this.ConnectOracleDB.command.ExecuteNonQuery();
                            ConnectOracleDB.command.Parameters.Clear();
                        }
                    }
                }
                transaction.Commit();
                //******************UPDATE ENDDATE HISTORY***************************************************************
                //UpdateHistory(idHistory, index);

            }
            catch (Exception ex)
            {
                if (null != transaction) transaction.Rollback();
                this.ERROR = "ERROR in line=" + index.ToString() + ", column=" + columnIndex + ":::::" + ex.ToString();
                this.ERROR_USER = "line=" + index.ToString() + ", column=" + columnIndex;
            }

            finally
            {
                ConnectOracleDB.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();

                ConnectOracleDB.CloseDBConnect();
            }
            return output;
        }

        public void Import2Database(int userid, string excelFilePath, int uploadRoleId, ExcelModel lstMapping, string excelFileName)
        {

            int index = 0, columnIndex = 0, count = 0;
            //int year = 0;
            //int index = 0, columnIndex = 0, count = 0; int year = 0;




            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            OracleTransaction transaction = null;
            try
            {

                //int output = 0;

                //string sqlInserPara2tDWValues = null;
                //string sqlInsertPara2DW = null;

                //string oleConnString = GetOleDbConnectionString(excelFilePath);
                //OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
                //OleDbCommand oleReadCommand = new OleDbCommand();
                //OracleTransaction transaction = null;

                //GET EXCEL DATA
                string sqlOle = "select * from [" + lstMapping.ExcelSheetName + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;

                ConnectOracleDB.OpenDBConnect();
                transaction = ConnectOracleDB.OracleDBConnect.BeginTransaction();
                ConnectOracleDB.command.Transaction = transaction;
                ConnectOracleDB.command.Parameters.Clear();



                /**************************************************INSERT HISTORY**************************************/
                this.DBConnection.OpenDBConnect();
                int idHistory = 0;
                this.InsertHistory(userid, uploadRoleId, lstMapping.FolderHelpDocumentPath, lstMapping.LangNote, lstMapping.FolderFileNativeName, excelFileName, ref idHistory, 0,0);

                this.DBConnection.CloseDBConnect();
                string deleteRecordQuery = "DELETE FROM " + lstMapping.DBTableName + " WHERE ";
                string sqlInsertDW = "insert into " + lstMapping.DBTableName + "(";
                string sqlInsertDWValues = " values (";



                bool isCommplateGenerateSQL = false;

                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        index++;
                        if (index == 1) continue;
                        if (index == 2)
                        {
                            var madviqly = dataReader[0].ToString().Substring(0,2);
                            var _month = dataReader[2].ToString();
                            var _year = dataReader[3].ToString();
                            deleteRecordQuery += "ma_dviqly like '%" + madviqly +"%'" + " "+ "AND " +  "THANG_BC " + "= " + _month + " " + "AND " + "NAM_BC " + "= " + _year;
                            this.ConnectOracleDB.command.CommandText = deleteRecordQuery;
                            this.ConnectOracleDB.command.ExecuteNonQuery();
                        }
                        if (index >= lstMapping.ExcelStartRow)
                        {
                            columnIndex = 0;

                            int countNullColumn = 0;
                            foreach (MappingExcelDB column in lstMapping.DBLstColumns)
                            {
                                //var test = dataReader.
                                columnIndex++;
                                column.Value = dataReader[column.ExcelColumn];
                                if (!isCommplateGenerateSQL)
                                {
                                    sqlInsertDW += column.ColumnName + ",";
                                    sqlInsertDWValues += ":" + column.ColumnName + ",";
                                }

                                try
                                {
                                    if (column.Value.ToString().Trim() == "")
                                    {
                                        countNullColumn++;
                                    }
                                }
                                catch (Exception)
                                {
                                    countNullColumn++;
                                }
                                ConnectOracleDB.command.Parameters.Add(new OracleParameter(":" + column.ColumnName, column.Value ?? (object)DBNull.Value));
                            }


                            if (!isCommplateGenerateSQL)
                            {
                                sqlInsertDW = sqlInsertDW.Trim(',') + ")";
                                sqlInsertDWValues = sqlInsertDWValues.Trim(',') + ")";
                                isCommplateGenerateSQL = true;
                            }

                            //*****************************INSERT INTO DATABASE******************************************************
                            if (countNullColumn == lstMapping.DBLstColumns.Count()) break;
                            else
                            {
                                this.ConnectOracleDB.command.CommandText = sqlInsertDW + sqlInsertDWValues;
                                this.ConnectOracleDB.command.CommandType = CommandType.Text;
                                this.ConnectOracleDB.command.ExecuteNonQuery();
                                ConnectOracleDB.command.Parameters.Clear();
                            }
                        }

                    }

                }
                transaction.Commit();
                UpdateHistory(idHistory, index);

            }
            catch (Exception ex)
            {
                this.ERROR = "tại dòng số :  " + (index + 1).ToString() + ", Cột số :  " + (columnIndex) + "  " + ex.Message; ;
            }
            finally
            {
                ConnectOracleDB.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();
                oleDbConnection.Dispose();
            }
            //return output;            
        }


        public void ImportPLDatabase(int userid, string excelFilePath, int uploadRoleId, ExcelModel lstMapping, string excelFileName, int month, int year)
        {
            int rowindex = 0;
            int index = 0, columnIndex = 0;
            //int index = 0, columnIndex = 0, count = 0; int year = 0;

            DataTable dt = new DataTable();
            DataSet dts = new DataSet();
            dts.Tables.Add("Customers");

            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            OracleTransaction transaction = null;
            try
            {
                //GET EXCEL DATA
                string sqlOle = "select * from [" + lstMapping.ExcelSheetName + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;

                ConnectOracleDB.OpenDBConnect();
                transaction = ConnectOracleDB.OracleDBConnect.BeginTransaction();
                ConnectOracleDB.command.Transaction = transaction;
                ConnectOracleDB.command.Parameters.Clear();



                /**************************************************INSERT HISTORY**************************************/
                this.DBConnection.OpenDBConnect();
                int idHistory = 0;
                this.InsertHistory(userid, uploadRoleId, lstMapping.FolderHelpDocumentPath, lstMapping.LangNote, lstMapping.FolderFileNativeName, excelFileName, ref idHistory, year, month);

                this.DBConnection.CloseDBConnect();
                string deleteRecordQuery = "DELETE FROM " + lstMapping.DBTableName + " WHERE ";
                string sqlInsertDW = "insert into " + lstMapping.DBTableName + "(";
                string sqlInsertDWValues = " values (";



                bool isCommplateGenerateSQL = false;

                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    dts.Tables[0].Load(dataReader);
                    dt = dts.Tables[0];
                    var results = dt.AsEnumerable().Where(myRow => myRow.Field<double>("THANG_BC") == month && myRow.Field<double>("NAM_BC") == year);
                    DataRow querydelete = results.AsEnumerable().FirstOrDefault();
                    var madviqly = querydelete.Field<string>("MA_DVIQLY").Substring(0, 2);
                    deleteRecordQuery += "ma_dviqly like '%" + madviqly + "%'" + " " + "AND " + "THANG_BC " + "= " + month + " " + "AND " + "NAM_BC " + "= " + year;
                    this.ConnectOracleDB.command.CommandText = deleteRecordQuery;
                    this.ConnectOracleDB.command.ExecuteNonQuery();
                    List<string> lstColumns = new List<string>();
                    foreach (DataColumn colu in dt.Columns)
                    {
                        lstColumns.Add(colu.ColumnName);
                        sqlInsertDW += colu.ColumnName + ",";
                        sqlInsertDWValues += ":" + colu.ColumnName + ",";

                    }
                    sqlInsertDW = sqlInsertDW.Trim(',') + ")";
                    sqlInsertDWValues = sqlInsertDWValues.Trim(',') + ")";

                    foreach (DataRow result in results)
                    {
                        columnIndex = 0;
                        
                        foreach(object obj in result.ItemArray)
                        {


                            //string colvalue = result.Field<string>(columnIndex);
                            string colname = lstColumns[columnIndex];
                           
                            
                            ConnectOracleDB.command.Parameters.Add(new OracleParameter(":" + colname, obj ?? (object)DBNull.Value));
                            columnIndex++;
                        }
                        
                        this.ConnectOracleDB.command.CommandText = sqlInsertDW + sqlInsertDWValues;
                        this.ConnectOracleDB.command.CommandType = CommandType.Text;
                        this.ConnectOracleDB.command.ExecuteNonQuery();
                        ConnectOracleDB.command.Parameters.Clear();
                        rowindex++;
                    }
                }
                transaction.Commit();
                UpdateHistory(idHistory, rowindex);

            }
            catch (Exception ex)
            {
                this.ERROR = "tại dòng số :  " + (rowindex + 1).ToString() + ", Cột số :  " + (columnIndex) + "  " + ex.Message; ;
            }
            finally
            {
                ConnectOracleDB.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();
                oleDbConnection.Dispose();
            }
            //return output;            
        }

        public List<int> GetDataInLine(string excelFilePath, string sheetActive, int number_cell, string FileUploadedName, int number_row)
        {
            List<int> output = new List<int>();
            int index = 0, columnIndex = 0;

            string oleConnString = GetOleDbConnectionString(excelFilePath);
            OleDbConnection oleDbConnection = new OleDbConnection(oleConnString);
            OleDbCommand oleReadCommand = new OleDbCommand();
            OracleTransaction transaction = null;
            try
            {

                //GET EXCEL DATA
                string sqlOle = "select * from [" + sheetActive + "$]";
                oleDbConnection.Open();
                oleReadCommand.Connection = oleDbConnection;
                oleReadCommand.CommandText = sqlOle;


                using (OleDbDataReader dataReader = oleReadCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        index++;
                        if (index < number_row) continue;
                        columnIndex = 1;

                        for (int i = 0; i < number_cell; i++)
                        {
                            MappingExcelDB column = new MappingExcelDB();
                            column.Value = dataReader[column.ExcelColumn + columnIndex];
                            output.Add(Int32.Parse(column.Value.ToString()));
                            columnIndex += 2;
                        }
                        break;
                    }

                }

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }

            finally
            {
                this.ConnectOracleDB.CloseDBConnect();
                oleReadCommand.Dispose();
                oleDbConnection.Close();
                oleDbConnection.Dispose();
            }
            return output;
        }


        public int InsertHistory(int userId, int uploadRoleId, string helpDocPath, string note, string navetiveFile, string FileUploadedName, ref int id, int year, int month)
        {
            int output = 0;
            Dictionary<string, object> dicInputParas = new Dictionary<string, object>();
            Dictionary<string, object> dicOutputParas = new Dictionary<string, object>();

            dicInputParas.Add("UserId", userId);
            dicInputParas.Add("UploadRoleId", uploadRoleId);
            dicInputParas.Add("HelpDocumentPath", helpDocPath);
            dicInputParas.Add("Note", note);
            dicInputParas.Add("FileNativeName", navetiveFile);
            dicInputParas.Add("FileUploadedName", FileUploadedName);
            dicInputParas.Add("Year", year);
            dicInputParas.Add("Month", month);
            dicOutputParas.Add("Id", id);

            output = this.DBConnection.ExecSPNonQuery("SP_UPLOAD_HISTORY_INSERT", dicInputParas, ref dicOutputParas, false);

            id = (int)dicOutputParas["Id"];

            return output;
        }

        public int UpdateHistory(int id, int numberInsertedRow)
        {
            int output = 0;
            Dictionary<string, object> dicInputParas = new Dictionary<string, object>();
            Dictionary<string, object> dicOutputParas = new Dictionary<string, object>();
            dicInputParas.Add("ID", id);
            dicInputParas.Add("NumberInsertedRow", numberInsertedRow);
            output = this.DBConnection.ExecSPNonQuery("SP_UPLOAD_HISTORY_UPDATE", dicInputParas, ref dicOutputParas, false);
            return output;
        }


        public List<EntityUploadHistoryModel> GetHistoryList(int currentpage, int noRecordPerPage, string permissionID, ref int noPages, ref int noRecord, int month, int year)
        {

            // DBConnection.OpenDBConnect();
            //STEP1:  ***************************************************************/
            Dictionary<string, object> dicParas = new Dictionary<string, object>();
            Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<EntityUploadHistoryModel> output = new List<EntityUploadHistoryModel>();
            int intpermissionID = 0;
            try
            {
                try
                {
                    intpermissionID = Int32.Parse(permissionID);
                }
                catch (Exception)
                {
                    intpermissionID = 0;
                }
                dicParas.Add("NO_RECORD_PERPAGE", noRecordPerPage);
                dicParas.Add("CURRENT_PAGE", currentpage);
                dicParas.Add("PERMISSION_ID", intpermissionID);
                dicParas.Add("YEAR", year);
                dicParas.Add("MONTH", month);

                dicParaOuts.Add("NO_PAGES", noPages);
                dicParaOuts.Add("NO_RECORDS", noRecord);
                DataSet ds = this.DBConnection.ExecSelectSP("SP_UPLOAD_HISTORY_PAGING", dicParas, ref dicParaOuts, true);

                if (this.DBConnection.ERROR != null) throw new Exception(this.DBConnection.ERROR);

                noPages = (int)dicParaOuts["NO_PAGES"];
                noRecord = (int)dicParaOuts["NO_RECORDS"];

                DataTable table = ds.Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    EntityUploadHistoryModel model = (EntityUploadHistoryModel)dr;

                    output.Add(model);
                }
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }


            return output;
        }

        public List<DNT_QMKLTN_HA1820> GetList_DNT_QMKLTN_HA1820()
        {
            List < DNT_QMKLTN_HA1820 > output= new List<DNT_QMKLTN_HA1820>();
            OracleTransaction transaction = null;
            ConnectOracleDB.OpenDBConnect();
            transaction = ConnectOracleDB.OracleDBConnect.BeginTransaction();
            ConnectOracleDB.command.Transaction = transaction;
            ConnectOracleDB.command.Parameters.Clear();

            string sqlSelectMenu = "select * from DNT_QMKLTN_HA1820 where THANG_BC = 3 and NAM_BC = 2019";
            this.ConnectOracleDB.command.CommandText = sqlSelectMenu;
            this.ConnectOracleDB.command.CommandType = CommandType.Text;

            OracleDataReader objReader = this.ConnectOracleDB.command.ExecuteReader();
            while(objReader.Read())
            {
                DNT_QMKLTN_HA1820 model = new DNT_QMKLTN_HA1820();
                model.MA_DVIQLY = objReader.GetString(objReader.GetOrdinal("MA_DVIQLY"));
                model.TEN_CTRINH = objReader.GetString(objReader.GetOrdinal("TEN_CTRINH"));
                model.THANG_BC = objReader.GetInt32(objReader.GetOrdinal("THANG_BC"));
                model.NAM_BC = objReader.GetInt32(objReader.GetOrdinal("NAM_BC"));
                model.SL_XA = objReader.GetInt32(objReader.GetOrdinal("SL_XA"));
                model.SL_TCBD = objReader.GetInt32(objReader.GetOrdinal("SL_TCBD"));
                model.DZ_HTHE = objReader.GetInt32(objReader.GetOrdinal("DZ_HTHE"));
                model.SO_HOTN = objReader.GetInt32(objReader.GetOrdinal("SO_HOTN"));
                model.GTCL_VVNSNN = objReader.GetInt32(objReader.GetOrdinal("GTCL_VVNSNN"));
                model.GTCL_VV = objReader.GetInt32(objReader.GetOrdinal("GTCL_VV"));
                model.GTCL_VDTHTX = objReader.GetInt32(objReader.GetOrdinal("GTCL_VDTHTX"));
                model.GTCL_VDAN = objReader.GetInt32(objReader.GetOrdinal("GTCL_VDAN"));
                model.GTCL_VKHAC = objReader.GetInt32(objReader.GetOrdinal("GTCL_VKHAC"));
                model.CPHI_TNCT = objReader.GetInt32(objReader.GetOrdinal("CPHI_TNCT"));
                output.Add(model);
            }

            return output;
        }

        public List<Dictionary<string, object>> GetStoreData(ExcelXmlModel model)
        {
            List<Dictionary<string, object>> output = new List<Dictionary<string, object>>();
            try
            {
                string storeName = "shop_name";
                string proceName = null;
                //get store name to choose store procedure
                List<ExcelXMLColumn> lstColumn = new List<ExcelXMLColumn>();
                lstColumn = model.LstColumn;
                foreach (var column in lstColumn)
                {
                    if (column.Name.Contains(storeName)) { proceName = "pws_getmetadata_excel"; break; }
                    else proceName = "pws_getdata_excel";
                }

                output = GetStoreData(model, "report_name_para", "p_recordset", proceName);

            }
            catch (Exception ex)
            {

            }
            return output;
        }


        public List<Dictionary<string, object>> GetStoreData(ExcelXmlModel model, string paraInput, string paraOutput, string procedureName)
        {
            //Dictionary<string, object> dicParas = new Dictionary<string, object>();
            //Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<Dictionary<string, object>> lst_store = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> lst_kpi = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> ouput = new List<Dictionary<string, object>>();



            try
            {

                this.ConnectOracleDB.command.Parameters.Clear();

                var op = new OracleParameter(paraInput, OracleDbType.Varchar2);
                op.Direction = ParameterDirection.Input;
                op.Value = model.ExcelXmlCommon.ReportName;
                this.ConnectOracleDB.command.Parameters.Add(op);

                this.ConnectOracleDB.command.Parameters.Add(paraOutput, OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                this.ConnectOracleDB.command.CommandText = procedureName;
                this.ConnectOracleDB.command.CommandType = CommandType.StoredProcedure;

                OracleDataReader objReader = this.ConnectOracleDB.command.ExecuteReader();
                while (objReader.Read())

                {
                    var columns = Enumerable.Range(0, objReader.FieldCount).Select(objReader.GetName).ToList();


                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    for (int i = 0; i < columns.Count; i++)
                    {
                        foreach (ExcelXMLColumn xmlColumn in model.LstColumn)
                        {
                            if (!xmlColumn.IsGetData) continue;

                            if (xmlColumn.Name != columns[i].ToLower()) continue;
                            if ((objReader[xmlColumn.Name] != null) && (xmlColumn.IsGetData))
                            {
                                dic.Add(xmlColumn.Name, objReader[xmlColumn.Name]);
                            }
                        }
                    }
                    ouput.Add(dic);
                    //objReader.NextResult();

                }

                objReader.Close();
                objReader.Dispose();

            }
            catch (Exception ex)
            {


            }
            finally
            {
                this.ConnectOracleDB.CloseDBConnect();
            }
            return ouput;
        }


        public List<Dictionary<string, object>> GetStoreDataInventory(ExcelXmlModel model, int month, int year)
        {
            //Dictionary<string, object> dicParas = new Dictionary<string, object>();
            //Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<Dictionary<string, object>> lst_store = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> lst_kpi = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> ouput = new List<Dictionary<string, object>>();




            try
            {

                this.ConnectOracleDB.command.Parameters.Clear();

                var time = month.ToString() + "-" + year.ToString().Substring(2, 2);
                if (month < 10) time = '0' + time;


                var dateTime = new OracleParameter("dateTime", OracleDbType.Varchar2);

                dateTime.Direction = ParameterDirection.Input;
                dateTime.Value = time;
                this.ConnectOracleDB.command.Parameters.Add(dateTime);

                this.ConnectOracleDB.command.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                this.ConnectOracleDB.command.CommandText = "PWS_GETINVENTORYDATA_EXCEL";
                this.ConnectOracleDB.command.CommandType = CommandType.StoredProcedure;

                OracleDataReader objReader = this.ConnectOracleDB.command.ExecuteReader();
                while (objReader.Read())

                {
                    var columns = Enumerable.Range(0, objReader.FieldCount).Select(objReader.GetName).ToList();


                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    for (int i = 0; i < columns.Count; i++)
                    {

                        foreach (ExcelXMLColumn xmlColumn in model.LstColumn)
                        {
                            if (!xmlColumn.IsGetData) continue;

                            if (xmlColumn.Name.ToLower() != columns[i].ToLower()) continue;
                            if ((objReader[xmlColumn.Name] != null) && (xmlColumn.IsGetData))
                            {
                                dic.Add(xmlColumn.Name, objReader[xmlColumn.Name]);
                            }
                        }

                    }



                    ouput.Add(dic);
                    //objReader.NextResult();

                }

                objReader.Close();
                objReader.Dispose();

            }
            catch (Exception ex)
            {


            }
            finally
            {
                this.ConnectOracleDB.CloseDBConnect();
            }
            return ouput;
        }

        /// <summary>
        /// Get Store Name, Indicator
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetExcelConfigData(ExcelXmlModel model)
        {
            //Dictionary<string, object> dicParas = new Dictionary<string, object>();
            //Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
            List<Dictionary<string, object>> lst_store = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> lst_kpi = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> ouput = new List<Dictionary<string, object>>();




            try
            {
                string procedureName = null;
                if (model.ExcelXmlCommon.exportDirectory.Equals("PL_CongTy")) procedureName = "pws_getreporsetupcompany_excel";
                else procedureName = "pws_getreporsetupstore_excel";

                this.ConnectOracleDB.command.Parameters.Clear();

                var op = new OracleParameter("PK_SRPTNBR_para", OracleDbType.Varchar2);
                op.Direction = ParameterDirection.Input;
                op.Value = model.ExcelXmlCommon.ReportName;
                this.ConnectOracleDB.command.Parameters.Add(op);

                this.ConnectOracleDB.command.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                this.ConnectOracleDB.command.CommandText = procedureName;
                this.ConnectOracleDB.command.CommandType = CommandType.StoredProcedure;

                OracleDataReader objReader = this.ConnectOracleDB.command.ExecuteReader();
                while (objReader.Read())

                {
                    var columns = Enumerable.Range(0, objReader.FieldCount).Select(objReader.GetName).ToList();


                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    for (int i = 0; i < columns.Count; i++)
                    {
                        foreach (ExcelXMLColumn xmlColumn in model.LstColumn)
                        {
                            if (!xmlColumn.IsGetData) continue;

                            if (xmlColumn.Name != columns[i].ToLower()) continue;
                            if ((objReader[xmlColumn.Name] != null) && (xmlColumn.IsGetData))
                            {
                                dic.Add(xmlColumn.Name, objReader[xmlColumn.Name]);
                            }
                        }

                    }



                    ouput.Add(dic);
                    //objReader.NextResult();

                }

                objReader.Close();
                objReader.Dispose();

            }
            catch (Exception ex)
            {


            }
            finally
            {
                this.ConnectOracleDB.CloseDBConnect();
            }
            return ouput;
        }

        public MemoryStream ExportExcelWithConfigReport(string id, string filePath, ExcelXmlModel model)
        {

            List<Dictionary<string, object>> data = GetExcelConfigData(model);

            ExcelHelper excelHelper = new ExcelHelper();
            string excelFileName = null;

            MemoryStream memoryStream = excelHelper.ExportExcel(model, data, ref excelFileName);
            //save file excel exported          
            var package = new ExcelPackage(memoryStream);

            FileInfo Path = new FileInfo(filePath);
            package.SaveAs(Path);
            return memoryStream;
        }

        public int Delete(string filename, int id)
        {
            int output = 0;
            try
            {
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("filename", filename);
                dicParas.Add("userId", id);

                //dicParas.Add("")
                output = this.DBConnection.ExecSPNonQuery("SP_EXCEL_FILE_DELETE", dicParas, ref dicParaOuts, false);
            }
            catch(Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            
            return output;
        }
    }
}