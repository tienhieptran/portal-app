using System.Web;
namespace BI_Project.Services.Importers
{
    public class FileUploadModel
    {

        public HttpPostedFileBase FileObj { get; set; }
        public int currentyear { get; set; }
        public int currentMonth { set; get; }
        public bool ExportOff { set; get; }
        public int PermissionId { set; get; }

    }
}