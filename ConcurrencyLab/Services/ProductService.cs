using ConcurrencyLab.Exceptions;

namespace ConcurrencyLab.Services;

public class ProductService
{
    static int _apicallTotalCount = 0;
    static int _apiConcurrencyOccurTimes = 0;

    // static object lock
    private static readonly object _lock = new object();

    private readonly ConcurrencyLabDbContext _context;

    public ProductService(ConcurrencyLabDbContext context)
    {
        _context = context;
    }

    //  GetProducts
    public List<GetProductsResponseDto>? GetProducts()
    {
        // Query
        var products = _context.Products.ToList();

        // Map to dto
        var response = products
            .Select(p => new GetProductsResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Amount = p.Amount
            })
            .ToList();

        return response;
    }

    // CreateProduct
    public int CreateProduct(CreateProductRequestDto request)
    {
        // Init
        var product = new Product
        {
            Name = request.Name,
            Amount = request.Amount
        };

        // Save
        _context.Products.Add(product);
        // Save
        _context.SaveChanges();

        return product.Id;
    }

    // 扣商品數量 1 (no lock)
    // DecreaseProductAmount
    public void DecreaseProductAmount(DecreaseProductAmountRequestDto request)
    {
        // Query
        var product = _context.Products.Find(request.Id);

        // Not found    
        if (product == null)
        {
            throw new AppException("Product not found");
        }

        product.Amount -= 1;

        // 數量不足
        if (product.Amount < 0)
        {
            throw new AppException("Product amount is not enough");
        }

        Console.WriteLine($"=========================================== \n API Call Success Count: {++_apicallTotalCount} \n product Id: {product.Id}, 商品剩餘數量: {product.Amount}");

        try
        {
            // Save
            _context.SaveChanges();
        }
        // Concurrency count
        catch (DbUpdateConcurrencyException ex)
        {
            _apiConcurrencyOccurTimes++;
            throw;
        }

    }

    // 扣商品數量 1 (with object lock)
    public void DecreaseProductAmountWithObjectLock(DecreaseProductAmountRequestDto request)
    {
        lock (_lock)
        {
            // Query
            var product = _context.Products.Find(request.Id);

            // Not found    
            if (product == null)
            {
                throw new AppException("Product not found");
            }

            product.Amount -= 1;

            // 數量不足
            if (product.Amount < 0)
            {
                throw new AppException("Product amount is not enough");
            }

            Console.WriteLine($"=========================================== \n API Call Success Count: {++_apicallTotalCount} \n product Id: {product.Id}, 商品剩餘數量: {product.Amount}");

            // Save
            _context.SaveChanges();
        }
    }


    // GetReport
    public string GetReport()
    {
        var productAmount = _context.Products
            .Where(p => p.Id == 1)
            .Select(p => p.Amount)
            .First();

        string response = $"API Call Success Count: {_apicallTotalCount} \nConcurrency Occur Times: {_apiConcurrencyOccurTimes} \n商品原始數量: 100\n商品剩餘數量: {productAmount}";

        return response;
    }
}
