using System.Collections.Generic;

namespace BI_Project.Models.EntityModels
{
    public class EntityRoleModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<EntityUserRoleModel> LstUserRole { get; set; }

        public List<EntityMenuModel> LstMenus { get; set; }

        public int DepartId { get; set; }
        public string DepartmentName { get; set; }
    }
}