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

    [HttpGet]
    [Route("GetProducts")]
    public IActionResult GetProducts()
    {
        var response = _productServices.GetProducts();
        return Ok(response);
    }

    [HttpPost]
    [Route("CreateProduct")]
    public IActionResult CreateProduct(CreateProductRequestDto request)
    {
        var response = _productServices.CreateProduct(request);
        return Ok(response);
    }

    // 扣商品數量 1
    [HttpPost]
    [Route("DecreaseProductAmount")]
    public IActionResult DecreaseProductAmount(DecreaseProductAmountRequestDto resuest)
    {
        // 1. no lock
        // 2. concurrency token
        //_productServices.DecreaseProductAmount(resuest);

        // 3. object lock
        _productServices.DecreaseProductAmountWithObjectLock(resuest);

        // 4. database lock

        // 5. redis lock

        return Ok("OK");
    }

    // GetReport
    [HttpGet]
    [Route("GetReport")]
    public IActionResult GetReport()
    {
        var response = _productServices.GetReport();
        return Ok(response);
    }
}
