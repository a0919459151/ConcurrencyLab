using Microsoft.AspNetCore.Mvc;

namespace ConcurrencyLab.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productServices;

    public ProductController(ProductService productServices)
    {
        _productServices = productServices;
    }

    /// <summary>
    /// 取得商品列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("GetProducts")]
    public IActionResult GetProducts()
    {
        var response = _productServices.GetProducts();
        return Ok(response);
    }

    /// <summary>
    /// 新增商品
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("CreateProduct")]
    public IActionResult CreateProduct(CreateProductRequestDto request)
    {
        var response = _productServices.CreateProduct(request);
        return Ok(response);
    }

    /// <summary>
    /// 扣商品數量 1
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("DecreaseProductAmount")]
    public IActionResult DecreaseProductAmount(DecreaseProductAmountRequestDto request)
    {
        // 1. No lock
        // 2. Concurrency token
        //_productServices.DecreaseProductAmount(request);

        // 3. Row version
        //_productServices.DecreaseProductAmountWithRowVersion(request);

        // 4. Object lock
        //_productServices.DecreaseProductAmountWithObjectLock(request);

        // 5. redis lock
        _productServices.DecreaseProductAmountWithRedisLock(request);

        return Ok("OK");
    }

    /// <summary>
    /// 取得測試結果
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetReport/{id}")]
    public IActionResult GetReport(int id)
    {
        var response = _productServices.GetReport(id);
        return Ok(response);
    }
}
