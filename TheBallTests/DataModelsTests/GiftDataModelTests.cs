
using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;

namespace TheBallTests.DataModelsTests;

[TestFixture]
internal class GiftDataModelTests
{
    [Test]
    public void IdIsNullOrEmptyTest()
    {
        var gift = CreateDataModel(null, "name", GiftType.Accessories, Guid.NewGuid().ToString(), 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
        gift = CreateDataModel(string.Empty, "name", GiftType.Accessories, Guid.NewGuid().ToString(), 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void IdIsNotGuidTest()
    {
        var gift = CreateDataModel("id", "name", GiftType.Accessories, Guid.NewGuid().ToString(), 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void GiftNameIsEmptyTest()
    {
        var gift = CreateDataModel(Guid.NewGuid().ToString(), null, GiftType.Accessories, Guid.NewGuid().ToString(), 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
        gift = CreateDataModel(Guid.NewGuid().ToString(), string.Empty, GiftType.Accessories, Guid.NewGuid().ToString(), 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void GiftTypeIsNoneTest()
    {
        var gift = CreateDataModel(Guid.NewGuid().ToString(), null, GiftType.None, Guid.NewGuid().ToString(), 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ManufacturerIdIsNullOrEmptyTest()
    {
        var gift = CreateDataModel(Guid.NewGuid().ToString(), "name", GiftType.Accessories, null, 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
        gift = CreateDataModel(Guid.NewGuid().ToString(), "name", GiftType.Accessories, string.Empty, 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ManufacturerIdIsNotGuidTest()
    {
        var gift = CreateDataModel(Guid.NewGuid().ToString(), "name", GiftType.Accessories, "manufacturerId", 10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void PriceIsLessOrZeroTest()
    {
        var gift = CreateDataModel(Guid.NewGuid().ToString(), "name", GiftType.Accessories, Guid.NewGuid().ToString(), 0, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
        gift = CreateDataModel(Guid.NewGuid().ToString(), "name", GiftType.Accessories, Guid.NewGuid().ToString(), -10, false);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var giftId = Guid.NewGuid().ToString();
        var giftName = "name";
        var giftType = GiftType.Accessories;
        var giftManufacturerId = Guid.NewGuid().ToString();
        var productPrice = 10;
        var productIsDelete = false;
        var gift = CreateDataModel(giftId, giftName, giftType, giftManufacturerId, productPrice, productIsDelete);
        Assert.That(() => gift.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(gift.Id, Is.EqualTo(giftId));
            Assert.That(gift.Name, Is.EqualTo(giftName));
            Assert.That(gift.GiftType, Is.EqualTo(giftType));
            Assert.That(gift.ManufacturerId, Is.EqualTo(giftManufacturerId));
            Assert.That(gift.Price, Is.EqualTo(productPrice));
            Assert.That(gift.IsDeleted, Is.EqualTo(productIsDelete));
        });
    }

    private static GiftDataModel CreateDataModel(string? id, string? giftName, GiftType giftType, string? manufacturerId, double price, bool isDeleted) =>
        new(id, giftName, giftType, manufacturerId, price, isDeleted);
}
