using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces;

public interface IViewProductByIdUseCase
{
    Task<Product> ExecuteAsync(int productId);
}