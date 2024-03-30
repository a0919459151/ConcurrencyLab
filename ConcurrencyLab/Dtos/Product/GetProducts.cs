namespace ConcurrencyLab.Dtos.Product;

public class GetProductsResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Amount { get; set; }
}
