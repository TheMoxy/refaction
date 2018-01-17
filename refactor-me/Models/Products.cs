using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace refactor_me.Models
{
    public class Products
    {
        public List<Product> Items { get; private set; }
        
        public Products(IEnumerable<Product> items)
        {
            Items = items.ToList();
        }
    }
}