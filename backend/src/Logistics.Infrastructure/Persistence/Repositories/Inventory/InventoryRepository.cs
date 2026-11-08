using Logistics.Application.Interfaces.Inventory;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Inventory;

public class InventoryRepository : IInventoryRepository
{
    private readonly LogisticsDbContext _context;

    public InventoryRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    // ITEM

    public async Task<List<Item>> GetItemsAsync(
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Items
            .AsNoTracking()
            .Include(x => x.ItemCategory)
            .Include(x => x.UnitOfMeasure)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Code, $"%{keyword}%") ||
                EF.Functions.ILike(x.Name, $"%{keyword}%"));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Item?> GetItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.ItemCategory)
            .Include(x => x.UnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ItemCodeExistsAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Items
            .AsNoTracking()
            .Where(x => x.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ItemCategoryExistsAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ItemCategories
            .AsNoTracking()
            .AnyAsync(x => x.Id == categoryId, cancellationToken);
    }

    public async Task<bool> UnitOfMeasureExistsAsync(
        Guid unitOfMeasureId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UnitsOfMeasure
            .AsNoTracking()
            .AnyAsync(x => x.Id == unitOfMeasureId, cancellationToken);
    }

    public async Task AddItemAsync(
        Item item,
        CancellationToken cancellationToken = default)
    {
        await _context.Items.AddAsync(item, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    // INVENTORY STOCK
    public async Task<List<InventoryStock>> GetStocksAsync(
        Guid? itemId,
        Guid? locationId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryStocks
            .AsNoTracking()
            .Include(x => x.Item)
            .Include(x => x.Location)
            .AsQueryable();

        if (itemId.HasValue)
        {
            query = query.Where(x => x.ItemId == itemId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(x => x.LocationId == locationId.Value);
        }

        return await query
            .OrderBy(x => x.Item.Name)
            .ThenBy(x => x.Location.Name)
            .ToListAsync(cancellationToken);
    }
    public async Task<InventoryStock?> GetStockAsync(Guid itemId,Guid locationId,
    CancellationToken cancellationToken = default)
    {
        return await _context.InventoryStocks
            .Include(x => x.Location)
            .FirstOrDefaultAsync(
                x => x.ItemId == itemId &&
                     x.LocationId == locationId,
                cancellationToken);
    }

    public async Task<bool> LocationExistsAsync(
        Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .AnyAsync(x => x.Id == locationId, cancellationToken);
    }


    // =====================================================
    // STOCK TRANSACTION
    // =====================================================

    public async Task<List<StockTransaction>> GetStockTransactionsAsync(
        Guid? itemId,
        Guid? locationId,
        string? transactionType,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StockTransactions
            .AsNoTracking()
            .Include(x => x.Item)
            .Include(x => x.Location)
            .AsQueryable();

        if (itemId.HasValue)
        {
            query = query.Where(x => x.ItemId == itemId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(x => x.LocationId == locationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(transactionType))
        {
            var type = transactionType.Trim().ToUpperInvariant();

            query = query.Where(x => x.TransactionType == type);
        }

        return await query
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TransactionNumberExistsAsync(string transactionNumber, CancellationToken cancellationToken = default)
    {
        return await _context.StockTransactions
            .AsNoTracking()
            .AnyAsync(
                x => x.TransactionNumber == transactionNumber,
                cancellationToken);
    }

    public async Task AddStockTransactionAsync(StockTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.StockTransactions.AddAsync(
            transaction,
            cancellationToken);
    }
    public async Task AddInventoryStockAsync(InventoryStock stock, CancellationToken cancellationToken = default)
    {
        await _context.InventoryStocks.AddAsync(stock, cancellationToken);
    }
    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation,
    CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var result = await operation(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
    public async Task<Item?> LockItemForStockUpdateAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .FromSqlInterpolated(
                $"SELECT * FROM items WHERE \"Id\" = {itemId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
    }
}