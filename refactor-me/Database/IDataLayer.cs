using refactor_me.Models;
using System;

namespace refactor_me.Database
{
    public interface IDataLayer
    {
        Products LoadProducts(string where);
        Products LoadProductsByName(string name);

        Product LoadProductById(string id);
        void SaveProduct(Product product);
        void DeleteProduct(Guid id);
    }
}