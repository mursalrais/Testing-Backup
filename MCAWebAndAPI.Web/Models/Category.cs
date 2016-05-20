using System;
using System.Collections.Generic;
using System.Linq;

namespace GridInForm.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}