

namespace TheBallDatabase.Models;

internal class SaleGift
{
    public required string SaleId { get; set; }
    public required string GiftId { get; set; }
    public int Count { get; set; }
    public Sale? Sale { get; set; }
    public Gift? Gift { get; set; }
}
