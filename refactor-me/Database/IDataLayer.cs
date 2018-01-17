using refactor_me.Models;

namespace refactor_me.Database
{
    public interface IDataLayer
    {
        Products LoadProducts(string where);
        Products LoadProductsByName(string name);
    }
}