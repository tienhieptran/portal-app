using BI_Project.Models.EntityModels;
using System.Collections.Generic;
namespace BI_Project.Services.User
{
    public class BlockDataUserCreateModel : BI_Project.Models.EntityModels.EntityUserModel
    {

        public List<EntityRoleModel> ListAllRoles { set; get; }

        public string StrAllowedMenus { set; get; }
        public BlockDataUserCreateModel() : base()
        {
            ListAllRoles = new List<EntityRoleModel>();
        }



    }
}