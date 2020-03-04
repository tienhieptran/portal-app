using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bicen.Models.EntityModels
{
    public class MotaDNT
    {

        public string Col_name { get; set; }
        public string Data_type { get; set; }
        public string Description { get; set; }
        public int Table_id { get; set; }

        public MotaDNT(string col_name, string data_type, string description, int table_id)
        {
            Col_name = col_name;
            Data_type = data_type;
            Description = description;
            Table_id = table_id;
        }

        public List<MoTaDNTModel> moTaDNTs { get; set; } = new List<MoTaDNTModel>();
    }
}