using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces.Inventory;

public interface IInventoryRepository
{

    // ITEM
    Task<List<Item>> GetItemsAsync(string? search,bool? isActive,CancellationToken cancellationToken = default);
    Task<Item?> GetItemByIdAsync( Guid id,CancellationToken cancellationToken = default);
    Task<bool> ItemCodeExistsAsync(string code,Guid? excludeId = null,CancellationToken cancellationToken = default);
    Task<bool> ItemCategoryExistsAsync(Guid categoryId,CancellationToken cancellationToken = default);
    Task<bool> UnitOfMeasureExistsAsync(Guid unitOfMeasureId,CancellationToken cancellationToken = default);
    Task AddItemAsync( Item item,CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    
    // INVENTORY STOCK
    Task<List<InventoryStock>> GetStocksAsync(Guid? itemId,Guid? locationId,
        CancellationToken cancellationToken = default);

    Task<InventoryStock?> GetStockAsync(Guid itemId,Guid locationId,
        CancellationToken cancellationToken = default);

    Task<bool> LocationExistsAsync( Guid locationId,
        CancellationToken cancellationToken = default);

    // STOCK TRANSACTION
    Task<List<StockTransaction>> GetStockTransactionsAsync(Guid? itemId,Guid? locationId,
        string? transactionType,CancellationToken cancellationToken = default);
    Task<bool> TransactionNumberExistsAsync(string transactionNumber,CancellationToken cancellationToken = default);
    Task AddStockTransactionAsync(StockTransaction transaction,CancellationToken cancellationToken = default);
    Task AddInventoryStockAsync(InventoryStock stock,CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation,
    CancellationToken cancellationToken = default);
    Task<Item?> LockItemForStockUpdateAsync(Guid itemId,CancellationToken cancellationToken = default);
}