

using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;

namespace TheBallTests.DataModelsTests;

[TestFixture]
internal class GiftHistoryDataModelTests
{
    [Test]
    public void ProductIdIsNullOrEmptyTest()
    {
        var gift = CreateDataModel(null, 10);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
        gift = CreateDataModel(string.Empty, 10);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductIdIsNotGuidTest()
    {
        var gift = CreateDataModel("id", 10);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void OldPriceIsLessOrZeroTest()
    {
        var gift = CreateDataModel(Guid.NewGuid().ToString(), 0);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
        gift = CreateDataModel(Guid.NewGuid().ToString(), -10);
        Assert.That(() => gift.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var giftId = Guid.NewGuid().ToString();
        var oldPrice = 10;
        var giftHistory = CreateDataModel(giftId, oldPrice);
        Assert.That(() => giftHistory.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(giftHistory.GiftId, Is.EqualTo(giftId));
            Assert.That(giftHistory.OldPrice, Is.EqualTo(oldPrice));
            Assert.That(giftHistory.ChangeDate, Is.LessThan(DateTime.UtcNow));
            Assert.That(giftHistory.ChangeDate, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        });
    }

    private static GiftHistoryDataModel CreateDataModel(string? giftId, double oldPrice) =>
        new(giftId, oldPrice);
}
