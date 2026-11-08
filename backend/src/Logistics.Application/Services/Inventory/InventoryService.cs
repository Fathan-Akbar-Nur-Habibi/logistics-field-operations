using Logistics.Application.DTOs.Inventory;
using Logistics.Application.Interfaces.Inventory;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Inventory;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;

    public InventoryService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    // GET ITEMS

    public async Task<List<ItemResponse>> GetItemsAsync(string? search, bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetItemsAsync(search, isActive, cancellationToken);
        return items.Select(MapItemToResponse).ToList();
    }

    // GET ITEM BY ID

    public async Task<ItemResponse?> GetItemByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(id, cancellationToken);
        return item == null ? null : MapItemToResponse(item);
    }

    // CREATE ITEM

    public async Task<ItemResponse> CreateItemAsync(CreateItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ArgumentException("The item code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("The item name is required.");
        }

        if (request.ItemCategoryId == Guid.Empty)
        {
            throw new ArgumentException("The item category is required.");
        }

        if (request.UnitOfMeasureId == Guid.Empty)
        {
            throw new ArgumentException("The unit of measure is required.");
        }

        if (request.MinimumStock < 0)
        {
            throw new ArgumentException("Minimum stock cannot be negative.");
        }

        if (request.UnitPrice < 0)
        {
            throw new ArgumentException("Unit price cannot be negative.");
        }

        var code = request.Code.Trim().ToUpperInvariant();
        var codeExists = await _repository.ItemCodeExistsAsync(code, null, cancellationToken);
        if (codeExists)
        {
            throw new InvalidOperationException(
                $"Item with code '{code}' already registered.");
        }
        var categoryExists = await _repository.ItemCategoryExistsAsync(request.ItemCategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException(
                "Item category not found.");
        }

        var unitExists = await _repository.UnitOfMeasureExistsAsync(request.UnitOfMeasureId, cancellationToken);

        if (!unitExists)
        {
            throw new ArgumentException("Unit of measure not found.");
        }

        var item = new Item
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            ItemCategoryId = request.ItemCategoryId,
            UnitOfMeasureId = request.UnitOfMeasureId,
            MinimumStock = request.MinimumStock,
            UnitPrice = request.UnitPrice,
            IsActive = true
        };

        await _repository.AddItemAsync(item, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        var savedItem = await _repository.GetItemByIdAsync(item.Id, cancellationToken);
        if (savedItem == null)
        {
            throw new InvalidOperationException("The item data was successfully saved but could not be retrieved.");
        }

        return MapItemToResponse(savedItem);
    }

    // UPDATE ITEM
    public async Task<ItemResponse?> UpdateItemAsync(Guid id, UpdateItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(id, cancellationToken);
        if (item == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("The “Item Name” field is required.");
        }

        if (request.ItemCategoryId == Guid.Empty)
        {
            throw new ArgumentException("The “Item Category” field is required.");
        }

        if (request.UnitOfMeasureId == Guid.Empty)
        {
            throw new ArgumentException("The “Unit of Measure” field is required.");
        }

        if (request.MinimumStock < 0)
        {
            throw new ArgumentException("The “Minimum Stock” field cannot be negative.");
        }

        if (request.UnitPrice < 0)
        {
            throw new ArgumentException("The “Unit Price” field cannot be negative.");
        }

        var categoryExists = await _repository.ItemCategoryExistsAsync(request.ItemCategoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new ArgumentException("The “Item Category” field is required.");
        }

        var unitExists = await _repository.UnitOfMeasureExistsAsync(request.UnitOfMeasureId, cancellationToken);

        if (!unitExists)
        {
            throw new ArgumentException("The “Unit of Measure” field is required.");
        }

        item.Name = request.Name.Trim();
        item.Description = request.Description?.Trim();
        item.ItemCategoryId = request.ItemCategoryId;
        item.UnitOfMeasureId = request.UnitOfMeasureId;
        item.MinimumStock = request.MinimumStock;
        item.UnitPrice = request.UnitPrice;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return MapItemToResponse(item);
    }


    // GET INVENTORY STOCKS

    public async Task<List<InventoryStockResponse>> GetStocksAsync(Guid? itemId, Guid? locationId, CancellationToken cancellationToken = default)
    {
        var stocks = await _repository.GetStocksAsync(itemId, locationId, cancellationToken);

        return stocks.Select(x => new InventoryStockResponse
        {
            Id = x.Id,
            ItemId = x.ItemId,
            ItemCode = x.Item.Code,
            ItemName = x.Item.Name,
            LocationId = x.LocationId,
            LocationCode = x.Location.Code,
            LocationName = x.Location.Name,
            Quantity = x.Quantity,
            ReservedQuantity = x.ReservedQuantity,
            AvailableQuantity = x.AvailableQuantity,
            MinimumStock = x.Item.MinimumStock,
            IsBelowMinimumStock = x.AvailableQuantity < x.Item.MinimumStock
        }).ToList();
    }

    // GET STOCK TRANSACTIONS
    public async Task<List<StockTransactionResponse>>
        GetStockTransactionsAsync(
            Guid? itemId,
            Guid? locationId,
            string? transactionType,
            CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(transactionType))
        {
            transactionType = transactionType.Trim().ToUpperInvariant();

            if (transactionType != "IN" && transactionType != "OUT")
            {
                throw new ArgumentException(
                    "The transaction type must be either IN or OUT.");
            }
        }

        var transactions = await _repository.GetStockTransactionsAsync(itemId, locationId, transactionType, cancellationToken);

        return transactions.Select(x => new StockTransactionResponse
        {
            Id = x.Id,
            TransactionNumber = x.TransactionNumber,
            ItemId = x.ItemId,
            ItemCode = x.Item.Code,
            ItemName = x.Item.Name,
            LocationId = x.LocationId,
            LocationCode = x.Location.Code,
            LocationName = x.Location.Name,
            TransactionType = x.TransactionType,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            ReferenceNumber = x.ReferenceNumber,
            Notes = x.Notes,
            PerformedByUserId = x.PerformedByUserId,
            TransactionDate = x.TransactionDate
        }).ToList();
    }

    // MAPPING
    private static ItemResponse MapItemToResponse(Item item)
    {
        return new ItemResponse
        {
            Id = item.Id,
            Code = item.Code,
            Name = item.Name,
            Description = item.Description,
            ItemCategoryId = item.ItemCategoryId,
            ItemCategoryName = item.ItemCategory.Name,
            UnitOfMeasureId = item.UnitOfMeasureId,
            UnitOfMeasureName = item.UnitOfMeasure.Name,
            MinimumStock = item.MinimumStock,
            UnitPrice = item.UnitPrice,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public async Task<StockTransactionResponse> ProcessStockTransactionAsync(CreateStockTransactionRequest request, CancellationToken cancellationToken = default)
    {

        // VALIDATION

        if (request.ItemId == Guid.Empty)
            throw new ArgumentException("ItemId is required.");

        if (request.LocationId == Guid.Empty)
            throw new ArgumentException("LocationId is required.");

        if (string.IsNullOrWhiteSpace(request.TransactionType))
            throw new ArgumentException("TransactionType is required.");

        var transactionType =
            request.TransactionType.Trim().ToUpperInvariant();

        if (transactionType != "IN" && transactionType != "OUT")
            throw new ArgumentException(
                "TransactionType must be either IN or OUT.");

        if (request.Quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than 0.");

        if (request.UnitPrice < 0)
            throw new ArgumentException(
                "UnitPrice must not be negative.");

        // EXECUTE DATABASE TRANSACTION

        return await _repository.ExecuteInTransactionAsync(
            async ct =>
            {
                // Lock parent item row first.
                // Requests for the same item are serialized.
                var item = await _repository.LockItemForStockUpdateAsync(request.ItemId, ct);

                if (item == null)
                    throw new ArgumentException("Item not found.");

                var locationExists = await _repository.LocationExistsAsync(request.LocationId, ct);

                if (!locationExists)
                    throw new ArgumentException("Location not found.");

                // Get existing stock.
                var stock = await _repository.GetStockAsync(request.ItemId, request.LocationId, ct);

                //  CREATE STOCK IF NOT EXISTS
                if (stock == null)
                {
                    if (transactionType == "OUT")
                    {
                        throw new InvalidOperationException(
                            "Stock is not available at the specified location.");
                    }

                    stock = new InventoryStock
                    {
                        Id = Guid.NewGuid(),
                        ItemId = request.ItemId,
                        LocationId = request.LocationId,
                        Quantity = 0,
                        ReservedQuantity = 0
                    };

                    await _repository.AddInventoryStockAsync(stock, ct);
                }

                // UPDATE STOCK QUANTITY

                if (transactionType == "IN")
                {
                    stock.Quantity += request.Quantity;
                }
                else
                {
                    if (request.Quantity > stock.AvailableQuantity)
                    {
                        throw new InvalidOperationException(
                            $"Available stock is only {stock.AvailableQuantity}. " +
                            "Stock Out quantity exceeds available stock.");
                    }

                    stock.Quantity -= request.Quantity;
                }

                // CREATE TRANSACTION HISTORy

                var transaction = new StockTransaction
                {
                    Id = Guid.NewGuid(),

                    // Maximum 36 characters.
                    TransactionNumber =
                        $"TRX-{Guid.NewGuid():N}",

                    ItemId = request.ItemId,
                    LocationId = request.LocationId,
                    TransactionType = transactionType,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    ReferenceNumber = request.ReferenceNumber?.Trim(),
                    Notes = request.Notes?.Trim(),
                    TransactionDate = DateTimeOffset.UtcNow
                };

                await _repository.AddStockTransactionAsync(transaction, ct);


                //  SAVE BOTH CHANGES

                await _repository.SaveChangesAsync(ct);

                // Reload stock with Location navigation.
                var savedStock = await _repository.GetStockAsync(request.ItemId, request.LocationId, ct);

                if (savedStock == null || savedStock.Location == null)
                {
                    throw new InvalidOperationException(
                        "Stock data could not be retrieved after saving.");
                }

                //  RETURN RESPONSE
                return new StockTransactionResponse
                {
                    Id = transaction.Id,
                    TransactionNumber = transaction.TransactionNumber,

                    ItemId = item.Id,
                    ItemCode = item.Code,
                    ItemName = item.Name,

                    LocationId = savedStock.LocationId,
                    LocationCode = savedStock.Location.Code,
                    LocationName = savedStock.Location.Name,

                    TransactionType = transaction.TransactionType,
                    Quantity = transaction.Quantity,
                    UnitPrice = transaction.UnitPrice,
                    ReferenceNumber = transaction.ReferenceNumber,
                    Notes = transaction.Notes,
                    PerformedByUserId = transaction.PerformedByUserId,
                    TransactionDate = transaction.TransactionDate
                };
            },
            cancellationToken);
    }


}