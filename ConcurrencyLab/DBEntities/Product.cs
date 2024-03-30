using System.ComponentModel.DataAnnotations;

namespace ConcurrencyLab.DBEntities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [ConcurrencyCheck]
    public int Amount { get; set; }
}
