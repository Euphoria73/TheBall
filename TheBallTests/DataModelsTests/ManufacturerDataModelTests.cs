

using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;

namespace TheBallTests.DataModelsTests;

[TestFixture]
internal class ManufacturerDataModelTests
{
    [Test]
    public void IdIsNullEmptyTest()
    {
        var manufacturer = CreateDataModel(null, "name");
        Assert.That(() => manufacturer.Validate(), Throws.TypeOf<ValidationException>());
        manufacturer = CreateDataModel(string.Empty, "name");
        Assert.That(() => manufacturer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void IdIsNotGuidTest()
    {
        var manufacturer = CreateDataModel("id", "name");
        Assert.That(() => manufacturer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ManufacturerNameIsNullOrEmptyTest()
    {
        var manufacturer = CreateDataModel(Guid.NewGuid().ToString(), null);
        Assert.That(() => manufacturer.Validate(), Throws.TypeOf<ValidationException>());
        manufacturer = CreateDataModel(Guid.NewGuid().ToString(), string.Empty);
        Assert.That(() => manufacturer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var manufacturerId = Guid.NewGuid().ToString();
        var manufacturerName = "name";
        var prevManufacturerName = "prevManufacturerName";
        var prevPrevManufacturerName = "prevPrevManufacturerName";
        var manufacturer = CreateDataModel(manufacturerId, manufacturerName, prevManufacturerName, prevPrevManufacturerName);
        Assert.That(() => manufacturer.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(manufacturer.Id, Is.EqualTo(manufacturerId));
            Assert.That(manufacturer.Name, Is.EqualTo(manufacturerName));
            Assert.That(manufacturer.PrevName, Is.EqualTo(prevManufacturerName));
            Assert.That(manufacturer.PrevPrevName, Is.EqualTo(prevPrevManufacturerName));
        });
    }

    private static ManufacturerDataModel CreateDataModel(string? id, string? manufacturerName, string? prevManufacturerName = null, string? prevPrevManufacturerName = null) =>
        new(id, manufacturerName, prevManufacturerName, prevPrevManufacturerName);
}
