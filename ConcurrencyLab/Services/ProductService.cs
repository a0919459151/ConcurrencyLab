using RedLockNet;
using System.Text;

namespace ConcurrencyLab.Services;

public class ProductService
{
    static int _apicallSuccessCount = 0;  // 商品數量修改成功的次數
    static int _apicallFailCount = 0;  // 商品數量修改的次數
    static int _apiConcurrencyOccurTimes = 0;  // EF Core 發生 Concurrency Exception 的次數

    // Static object lock
    private static readonly object _lock = new object();

    private readonly ConcurrencyLabDbContext _context;
    private readonly IDistributedLockFactory _redlockFactory;

    public ProductService(ConcurrencyLabDbContext context, IDistributedLockFactory redlockFactory)
    {
        _context = context;
        _redlockFactory = redlockFactory;
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

        // Add
        _context.Products.Add(product);

        // Save
        _context.SaveChanges();

        return product.Id;
    }

    // 扣商品數量 1 (no lock & concurrency token)
    public void DecreaseProductAmount(DecreaseProductAmountRequestDto request)
    {
        try
        {
            // Query
            var product = _context.Products.Find(request.Id);

            // Not found    
            if (product == null)
            {
                throw new AppException("Product not found");
            }

            // sleep
            Thread.Sleep(300);

            product.Amount -= 1;

            // 數量不足
            if (product.Amount < 0)
            {
                throw new AppException("Product amount is not enough");
            }

            // Save
            _context.SaveChanges();

            PrintLog(product);
        }
        // Concurrency count
        catch (DbUpdateConcurrencyException)
        {
            _apiConcurrencyOccurTimes++;
            _apicallFailCount++;
            throw new AppException("EF Core occurs Concurrency Exception");
        }
        // Fail count
        catch (Exception)
        {
            _apicallFailCount++;
            throw;
        }
    }

    // 扣商品數量 1 (row version)
    public void DecreaseProductAmountWithRowVersion(DecreaseProductAmountRequestDto request)
    {
        try
        {
            // Query
            var product = _context.Products.Find(request.Id);

            // Not found
            if (product == null)
            {
                throw new AppException("Product not found");
            }

            // sleep
            Thread.Sleep(300);

            product.Amount -= 1;
            product.Version = Guid.NewGuid();  // update version

            // 數量不足
            if (product.Amount < 0)
            {
                throw new AppException("Product amount is not enough");
            }

            // Save
            _context.SaveChanges();

            PrintLog(product);
        }
        // Concurrency count
        catch (DbUpdateConcurrencyException)
        {
            _apiConcurrencyOccurTimes++;
            _apicallFailCount++;
            throw new AppException("EF Core occurs Concurrency Exception");
        }
        // Fail count
        catch (Exception)
        {
            _apicallFailCount++;
            throw;
        }
    }

    // 扣商品數量 1 (with object lock)
    public void DecreaseProductAmountWithObjectLock(DecreaseProductAmountRequestDto request)
    {
        try
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

                // sleep
                Thread.Sleep(300);

                product.Amount -= 1;

                // 數量不足
                if (product.Amount < 0)
                {
                    throw new AppException("Product amount is not enough");
                }

                // Save
                _context.SaveChanges();

                PrintLog(product);
            }
        }
        catch (Exception)
        {
            _apicallFailCount++;
            throw;
        }
    }

    // 扣商品數量 1 (with redis lock)
    public void DecreaseProductAmountWithRedisLock(DecreaseProductAmountRequestDto request)
    {
        try
        {
            string lockKey = $"{nameof(DecreaseProductAmount)}:{request.Id}";

            using var redLock = _redlockFactory.CreateLock(
                lockKey,
                TimeSpan.FromMinutes(30),  // lock expire time
                TimeSpan.FromMinutes(1),  // acquire lock timeout
                TimeSpan.FromSeconds(1));  // retry every 1 second

            if (!redLock.IsAcquired)
            {
                throw new AppException("Failed to acquire lock");
            }

            var product = _context.Products.Find(request.Id);

            if (product == null)
            {
                throw new AppException("Product not found");
            }

            // Sleep
            Thread.Sleep(300);

            product.Amount -= 1;

            if (product.Amount < 0)
            {
                throw new AppException("Product amount is not enough");
            }

            // Save
            _context.SaveChanges();

            PrintLog(product);
        }
        catch (Exception)
        {
            _apicallFailCount++;
            throw;
        }
    }

    // GetReport/id
    public string GetReport(int id)
    {
        var product = _context.Products
            .Where(p => p.Id == id)
            .First();

        var report = GetReportText(product);

        return report;
    }

    // Print log
    public void PrintLog(Product product)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"API Call Success Count: {++_apicallSuccessCount}");
        sb.AppendLine($"API Call Fail Count: {_apicallFailCount}");
        sb.AppendLine($"Concurrency Occur Times: {_apiConcurrencyOccurTimes}");
        sb.AppendLine($"{{ product Id: {product.Id}, 商品原始數量: {product.OriginalAmount}, 商品剩餘數量: {product.Amount} }}");

        Console.WriteLine(sb.ToString());
    }

    // Report text
    public string GetReportText(Product product)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"API Call Success Count: {_apicallSuccessCount}");
        sb.AppendLine($"API Call Fail Count: {_apicallFailCount}");
        sb.AppendLine($"Concurrency Occur Times: {_apiConcurrencyOccurTimes}");
        sb.AppendLine($"商品原始數量: {product.OriginalAmount}");
        sb.AppendLine($"商品剩餘數量: {product.Amount}");

        return sb.ToString();
    }

}
