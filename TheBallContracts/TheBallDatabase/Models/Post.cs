
using TheBallContracts.Enums;

namespace TheBallDatabase.Models;

internal class Post
{
    public required string Id { get; set; }
    public required string PostId { get; set; }
    public required string Name { get; set; }
    public PostType PostType { get; set; }
    public double Salary { get; set; }
    public bool IsActual { get; set; }
    public DateTime ChangeDate { get; set; }
}
