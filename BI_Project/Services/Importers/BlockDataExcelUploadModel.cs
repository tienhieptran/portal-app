using BI_Project.Models.EntityModels;
using bicen.Services.Importers;
using System.Collections.Generic;

namespace BI_Project.Services.Importers
{
    public class BlockDataExcelUploadModel
    {



        public BlockDataExcelUploadModel() : base()
        {

        }

        public List<EntityUploadHistoryModel> ListHistory { set; get; }
        public List<DNT_QMKLTN_HA1820> ListDNT { set; get; }
        public string PermissionID { set; get; }


        public int NumberRecords { set; get; }

        public int NumberPages { set; get; }



        public string FolderUpload { set; get; }

        public string HelpDoc { set; get; }

        public string Note { set; get; }
        public int CurrentPage { set; get; }

        public int PerPage { set; get; }

        public int Month { set; get; }

        public int Year { set; get; }

        public bool Export { get; set; }
        public bool ExportBaseMonth { set; get; }
        public string Url { get; set; }
        public bool ExportOff { get; set; }
        public int currentyear { set; get; }
        public int currentMonth { set; get; }

    }
}