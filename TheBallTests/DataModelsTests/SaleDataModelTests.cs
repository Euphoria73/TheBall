

using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;

namespace TheBallTests.DataModelsTests;

[TestFixture]
internal class SaleDataModelTests
{
    [Test]
    public void IdIsNullOrEmptyTest()
    {
        var sale = CreateDataModel(null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
        sale = CreateDataModel(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void IdIsNotGuidTest()
    {
        var sale = CreateDataModel("id", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }
    [Test]
    public void WorkerIdIsNullOrEmptyTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), null, Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
        sale = CreateDataModel(Guid.NewGuid().ToString(), string.Empty, Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void WorkerIdIsNotGuidTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), "workerId", Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void BuyerIdIsNotGuidTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "buyerId", 10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void SumIsLessOrZeroTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 0, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
        sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), -10, DiscountType.OnSale, 10, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductsIsNullOrEmptyTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, null);
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
        sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 10, DiscountType.OnSale, 10, false, []);
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var saleId = Guid.NewGuid().ToString();
        var workerId = Guid.NewGuid().ToString();
        var buyerId = Guid.NewGuid().ToString();
        var sum = 10;
        var discountType = DiscountType.Certificate;
        var discount = 1;
        var isCancel = true;
        var gifts = CreateSubDataModel();
        var sale = CreateDataModel(saleId, workerId, buyerId, sum, discountType, discount, isCancel, gifts);
        Assert.That(() => sale.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(sale.Id, Is.EqualTo(saleId));
            Assert.That(sale.WorkerId, Is.EqualTo(workerId));
            Assert.That(sale.BuyerId, Is.EqualTo(buyerId));
            Assert.That(sale.Sum, Is.EqualTo(sum));
            Assert.That(sale.DiscountType, Is.EqualTo(discountType));
            Assert.That(sale.Discount, Is.EqualTo(discount));
            Assert.That(sale.IsCancel, Is.EqualTo(isCancel));
            Assert.That(sale.Gifts, Is.EquivalentTo(gifts));
        });
    }

    private static SaleDataModel CreateDataModel(string? id, string? workerId, string? buyerId, double sum, DiscountType discountType, double discount, bool isCancel, List<SaleGiftDataModel>? gifts) =>
        new(id, workerId, buyerId, sum, discountType, discount, isCancel, gifts);

    private static List<SaleGiftDataModel> CreateSubDataModel()
        => [new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1)];
}
