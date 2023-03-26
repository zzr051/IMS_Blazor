using IMS.CoreBusiness;

namespace IMS.UseCases.Interfaces;

public interface ISearchProductTransactionsUseCase
{
    Task<IEnumerable<ProductTransaction>> ExecuteAsync(
        string productName,
        DateTime? dateForm,
        DateTime? dateTo,
        ProductTransactionType? transactionType
    );
}