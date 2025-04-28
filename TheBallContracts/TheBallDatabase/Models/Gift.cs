
using System.ComponentModel.DataAnnotations.Schema;
using TheBallContracts.Enums;

namespace TheBallDatabase.Models;

internal class Gift
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public GiftType GiftType { get; set; }
    public required string ManufacturerId { get; set; }
    public double Price { get; set; }
    public bool IsDeleted { get; set; }
    public Manufacturer? Manufacturer { get; set; }

    [ForeignKey("GiftId")]
    public List<SaleGift>? SaleGifts { get; set; }

    [ForeignKey("GiftId")]
    public List<GiftHistory>? GiftHistories { get; set; }
}
