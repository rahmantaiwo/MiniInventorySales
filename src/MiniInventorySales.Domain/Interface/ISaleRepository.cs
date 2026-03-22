using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Domain.Interface
{
    public interface ISaleRepository
    {
        Task AddAsync(Sale sale, CancellationToken ct = default);
        Task<bool> CancelSale(Guid saleId, CancellationToken ct = default);

        IQueryable<Sale> Query();
    }
}
