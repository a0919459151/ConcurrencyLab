namespace ConcurrencyLab.Dtos.Product;

public class CreateProductRequestDto
{
    public string Name { get; set; } = null!;

    public int Amount { get; set; }
}
