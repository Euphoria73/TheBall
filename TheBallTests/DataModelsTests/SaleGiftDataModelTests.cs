

using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;

namespace TheBallTests.DataModelsTests;

[TestFixture]
internal class SaleGiftDataModelTests
{
    [Test]
    public void SaleIdIsNullOrEmptyTest()
    {
        var saleGift = CreateDataModel(null, Guid.NewGuid().ToString(), 10);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
        saleGift = CreateDataModel(string.Empty, Guid.NewGuid().ToString(), 10);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void SaleIdIsNotGuidTest()
    {
        var saleGift = CreateDataModel("saleId", Guid.NewGuid().ToString(), 10);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductIdIsNullOrEmptyTest()
    {
        var saleGift = CreateDataModel(Guid.NewGuid().ToString(), null, 10);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
        saleGift = CreateDataModel(string.Empty, Guid.NewGuid().ToString(), 10);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductIdIsNotGuidTest()
    {
        var saleGift = CreateDataModel(Guid.NewGuid().ToString(), "giftId", 10);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void CountIsLessOrZeroTest()
    {
        var saleGift = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 0);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
        saleGift = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), -10);
        Assert.That(() => saleGift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var saleId = Guid.NewGuid().ToString();
        var giftId = Guid.NewGuid().ToString();
        var count = 10;
        var saleGift = CreateDataModel(saleId, giftId, count);
        Assert.That(() => saleGift.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(saleGift.SaleId, Is.EqualTo(saleId));
            Assert.That(saleGift.GiftId, Is.EqualTo(giftId));
            Assert.That(saleGift.Count, Is.EqualTo(count));
        });
    }

    private static SaleGiftDataModel CreateDataModel(string? saleId, string? giftId, int count) =>
        new(saleId, giftId, count);
}
