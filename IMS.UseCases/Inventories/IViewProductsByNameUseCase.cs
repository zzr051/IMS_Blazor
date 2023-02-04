using IMS.CoreBusiness;

namespace IMS.UseCases.Inventories;

public interface IViewProductsByNameUseCase
{
    Task<List<Product>> ExecuteAsync(string name = "");
}