using Logistics.Application.DTOs.Inventory;

namespace Logistics.Application.Interfaces.Inventory;

public interface IInventoryService
{
    // ITEM
    Task<List<ItemResponse>> GetItemsAsync(string? search,bool? isActive,
        CancellationToken cancellationToken = default);
    Task<ItemResponse?> GetItemByIdAsync(Guid id,CancellationToken cancellationToken = default);
    Task<ItemResponse> CreateItemAsync(CreateItemRequest request,CancellationToken cancellationToken = default);

    Task<ItemResponse?> UpdateItemAsync(Guid id,UpdateItemRequest request,
        CancellationToken cancellationToken = default);

    // INVENTORY STOCK
    Task<List<InventoryStockResponse>> GetStocksAsync(Guid? itemId,Guid? locationId,
        CancellationToken cancellationToken = default);

    // STOCK TRANSACTION
    Task<List<StockTransactionResponse>> GetStockTransactionsAsync(Guid? itemId,Guid? locationId,
        string? transactionType,CancellationToken cancellationToken = default);
    
    Task<StockTransactionResponse> ProcessStockTransactionAsync(CreateStockTransactionRequest request,
    CancellationToken cancellationToken = default);
}