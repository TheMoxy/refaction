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
        #region Products
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
                    items.Add(LoadProductById(rdr["id"].ToString()));
                }

                conn.Close();
            }
            return new Products(items);
        }

        public Products LoadProductsByName(string name)
        {
            return LoadProducts($"where lower(name) like '%{name.ToLower()}%'");
        }
        #endregion

        #region Product
        public Product LoadProductById(string id)
        {
            var product = new Product();

            using (var conn = Helpers.NewConnection())
            {
                var cmd = new SqlCommand($"select * from product where id = '{id}'", conn);
                conn.Open();

                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    product = new Product(Guid.Parse(rdr["Id"].ToString()));
                    product.Name = rdr["Name"].ToString();
                    product.Description = (DBNull.Value == rdr["Description"]) ? null : rdr["Description"].ToString();
                    product.Price = decimal.Parse(rdr["Price"].ToString());
                    product.DeliveryPrice = decimal.Parse(rdr["DeliveryPrice"].ToString());

                }

                conn.Close();
            }

            return product;
        }

        public void SaveProduct(Product product)
        {
            using (var conn = Helpers.NewConnection())
            {
                var cmd = product.IsNew ?
                    new SqlCommand($"insert into product (id, name, description, price, deliveryprice) values ('{product.Id}', '{product.Name}', '{product.Description}', {product.Price}, {product.DeliveryPrice})", conn) :
                    new SqlCommand($"update product set name = '{product.Name}', description = '{product.Description}', price = {product.Price}, deliveryprice = {product.DeliveryPrice} where id = '{product.Id}'", conn);

                conn.Open();
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
        
        public void DeleteProduct(Guid id)
        {
            foreach (var option in new ProductOptions(id).Items)
                option.Delete();

            using (var conn = Helpers.NewConnection())
            {
                conn.Open();
                var cmd = new SqlCommand($"delete from product where id = '{id}'", conn);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
        #endregion
    }
}