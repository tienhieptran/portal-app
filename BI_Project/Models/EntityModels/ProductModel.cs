using DemoData;
using System.Collections.Generic;

namespace bicen.Models.EntityModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Department { get; set; }

        //pagination
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int PagerCount { get; set; }

        public List<Product> Products { get; set; }
    }
}