using System.ComponentModel.DataAnnotations;

namespace ConcurrencyLab.DBEntities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    // 原始數量
    public int OriginalAmount { get; set; }

    //[ConcurrencyCheck]
    public int Amount { get; set; }

    //[ConcurrencyCheck]
    public Guid Version { get; set; } = Guid.NewGuid();
}
