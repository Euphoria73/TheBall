
using System.ComponentModel.DataAnnotations.Schema;
using TheBallContracts.DataModels;
using TheBallContracts.Enums;

namespace TheBallDatabase.Models;

internal class Sale
{
    public required string Id { get; set; }

    public required string WorkerId { get; set; }

    public string? BuyerId { get; set; }

    public DateTime SaleDate { get; set; }
    public double Sum { get; set; }

    public DiscountType DiscountType { get; set; }

    public double Discount { get; set; }

    public bool IsCancel { get; set; }
    public Worker? Worker { get; set; }
    public Buyer? Buyer { get; set; }

    [ForeignKey("SaleId")]
    public List<SaleGift>? SaleGifts { get; set; }
}
