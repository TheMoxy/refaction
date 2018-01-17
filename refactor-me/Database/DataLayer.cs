using refactor_me.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace refactor_me.Database
{
    public class DataLayer : IDataLayer
    {
        public Products LoadProducts(string where)
        {
            var items = new List<Product>();
            using (var conn = Helpers.NewConnection())
            {
                var cmd = new SqlCommand($"select id from product {where}", conn);
                conn.Open();

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var id = Guid.Parse(rdr["id"].ToString());
                    items.Add(new Product(id));
                }

                conn.Close();
            }
            return new Products(items);
        }

        public Products LoadProductsByName(string name)
        {
            return LoadProducts($"where lower(name) like '%{name.ToLower()}%'");
        }
    }
}