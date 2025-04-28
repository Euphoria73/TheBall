
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBallDatabase.Models;

internal class Manufacturer
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? PrevName { get; set; }
    public string? PrevPrevName { get; set; }

    [ForeignKey("ManufacturerId")]
    public List<Gift>? Gifts { get; set; }
}
