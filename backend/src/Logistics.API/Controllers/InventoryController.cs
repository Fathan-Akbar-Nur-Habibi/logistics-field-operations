using Logistics.Application.DTOs.Inventory;
using Logistics.Application.Interfaces.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // GET: api/inventory/items

    [HttpGet("items")]
    [ProducesResponseType(typeof(List<ItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ItemResponse>>> GetItems(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetItemsAsync(search, isActive, cancellationToken);
        return Ok(result);
    }

    // GET: api/inventory/items/{id}

    [HttpGet("items/{id:guid}", Name = "GetInventoryItemById")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> GetItemById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetItemByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound(new
            {
                isSuccess = false,
                message = "Item Not Found."
            });
        }

        return Ok(result);
    }

    // POST: api/inventory/items

    [HttpPost("items")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ItemResponse>> CreateItem([FromBody] CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _inventoryService.CreateItemAsync(request, cancellationToken);
            return CreatedAtRoute("GetInventoryItemById", new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                isSuccess = false,
                message = ex.Message
            });
        }
    }

    // PUT: api/inventory/items/{id}

    [HttpPut("items/{id:guid}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> UpdateItem(Guid id, [FromBody] UpdateItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _inventoryService.UpdateItemAsync(id, request, cancellationToken);
            if (result == null)
            {
                return NotFound(new
                {
                    isSuccess = false,
                    message = "Item Not Found."
                });
            }
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = ex.Message
            });
        }
    }

    // GET: api/inventory/stocks

    [HttpGet("stocks")]
    [ProducesResponseType(typeof(List<InventoryStockResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<InventoryStockResponse>>> GetStocks([FromQuery] Guid? itemId,
        [FromQuery] Guid? locationId, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetStocksAsync(itemId, locationId, cancellationToken);

        return Ok(result);
    }

    // GET: api/inventory/transactions

    [HttpGet("transactions")]
    [ProducesResponseType(typeof(List<StockTransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<StockTransactionResponse>>>
        GetStockTransactions([FromQuery] Guid? itemId, [FromQuery] Guid? locationId,
            [FromQuery] string? transactionType, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _inventoryService.GetStockTransactionsAsync(
                itemId,
                locationId,
                transactionType,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                isSuccess = false,
                message = ex.Message
            });
        }
    }

    [HttpPost("stock-in")]
    public async Task<IActionResult> StockIn(
        [FromBody] CreateStockTransactionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            request.TransactionType = "IN";

            var result = await _inventoryService.ProcessStockTransactionAsync(
                request,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOut([FromBody] CreateStockTransactionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            request.TransactionType = "OUT";
            var result = await _inventoryService.ProcessStockTransactionAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

}