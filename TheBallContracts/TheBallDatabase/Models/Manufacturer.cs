
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBallDatabase.Models;

internal class Manufacturer
{
    public required string Id { get; set; }
    public required string ManufacturerName { get; set; }
    public string? PrevManufacturerName { get; set; }
    public string? PrevPrevManufacturerName { get; set; }

    [ForeignKey("ManufacturerId")]
    public List<Gift>? Gifts { get; set; }
}
