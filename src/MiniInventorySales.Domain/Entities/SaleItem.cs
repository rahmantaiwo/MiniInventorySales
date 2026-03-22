namespace MiniInventorySales.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; set; }
        public Sale? Sale { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPriceAtSale { get; set; }
        public decimal LineTotal { get; set; }
    }
}
