

namespace TheBallDatabase.Models;

internal class GiftHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string GiftId { get; set; }
    public double OldPrice { get; set; }
    public DateTime ChangeDate { get; set; }
    public Gift? Gift { get; set; }
}
