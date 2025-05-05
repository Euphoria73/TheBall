
using Microsoft.EntityFrameworkCore;
using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallDatabase.Implementations;
using TheBallDatabase.Models;

namespace TheBallTests.StoragesContracts;

[TestFixture]
internal class BuyerStorageContractTests : BaseStorageContractTest
{

    private BuyerStorageContract _buyerStorageContract;

    [SetUp]
    public void SetUp()
    {
        _buyerStorageContract = new BuyerStorageContract(TheBallDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Sales\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Workers\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Buyers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var buyer = InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString(), phoneNumber: "+5-555-555-55-55");
        InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString(), phoneNumber: "+6-666-666-66-66");
        InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString(), phoneNumber: "+7-777-777-77-77");
        var list = _buyerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == buyer.Id), buyer);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _buyerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var buyer = InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_buyerStorageContract.GetElementById(buyer.Id), buyer);
    }

    [Test]
    public void Try_GetElementById_WhenNoRecord_Test()
    {
        InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _buyerStorageContract.GetElementById(Guid.NewGuid().ToString()), Is.Null);
    }

    [Test]
    public void Try_GetElementByFIO_WhenHaveRecord_Test()
    {
        var buyer = InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_buyerStorageContract.GetElementByFIO(buyer.FIO), buyer);
    }

    [Test]
    public void Try_GetElementByFIO_WhenNoRecord_Test()
    {
        InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _buyerStorageContract.GetElementByFIO("New Fio"), Is.Null);
    }

    [Test]
    public void Try_GetElementByPhoneNumber_WhenHaveRecord_Test()
    {
        var buyer = InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_buyerStorageContract.GetElementByPhoneNumber(buyer.PhoneNumber), buyer);
    }

    [Test]
    public void Try_GetElementByPhoneNumber_WhenNoRecord_Test()
    {
        InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _buyerStorageContract.GetElementByPhoneNumber("+8-888-888-88-88"), Is.Null);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var buyer = CreateModel(Guid.NewGuid().ToString());
        _buyerStorageContract.AddElement(buyer);
        AssertElement(GetBuyerFromDatabase(buyer.Id), buyer);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var buyer = CreateModel(Guid.NewGuid().ToString(), "New Fio", "+5-555-555-55-55", 500);
        InsertBuyerToDatabaseAndReturn(buyer.Id);
        Assert.That(() => _buyerStorageContract.AddElement(buyer), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSamePhoneNumber_Test()
    {
        var buyer = CreateModel(Guid.NewGuid().ToString(), "New Fio", "+5-555-555-55-55", 500);
        InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString(), phoneNumber: buyer.PhoneNumber);
        Assert.That(() => _buyerStorageContract.AddElement(buyer), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var buyer = CreateModel(Guid.NewGuid().ToString(), "New Fio", "+5-555-555-55-55", 500);
        InsertBuyerToDatabaseAndReturn(buyer.Id);
        _buyerStorageContract.UpdElement(buyer);
        AssertElement(GetBuyerFromDatabase(buyer.Id), buyer);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _buyerStorageContract.UpdElement(CreateModel(Guid.NewGuid().ToString())), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_UpdElement_WhenHaveRecordWithSamePhoneNumber_Test()
    {
        var buyer = CreateModel(Guid.NewGuid().ToString(), "New Fio", "+5-555-555-55-55", 500);
        InsertBuyerToDatabaseAndReturn(buyer.Id, phoneNumber: "+7-777-777-77-77");
        InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString(), phoneNumber: buyer.PhoneNumber);
        Assert.That(() => _buyerStorageContract.UpdElement(buyer), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var buyer = InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        _buyerStorageContract.DelElement(buyer.Id);
        var element = GetBuyerFromDatabase(buyer.Id);
        Assert.That(element, Is.Null);
    }

    [Test]
    public void Try_DelElement_WhenHaveSalesByThisBuyer_Test()
    {
        var buyer = InsertBuyerToDatabaseAndReturn(Guid.NewGuid().ToString());
        var workerId = Guid.NewGuid().ToString();
        TheBallDbContext.Workers.Add(new Worker() { Id = workerId, FIO = "test", PostId = Guid.NewGuid().ToString() });
        TheBallDbContext.Sales.Add(new Sale() { Id = Guid.NewGuid().ToString(), WorkerId = workerId, BuyerId = buyer.Id, Sum = 10, DiscountType = DiscountType.None, Discount = 0 });
        TheBallDbContext.Sales.Add(new Sale() { Id = Guid.NewGuid().ToString(), WorkerId = workerId, BuyerId = buyer.Id, Sum = 10, DiscountType = DiscountType.None, Discount = 0 });
        TheBallDbContext.SaveChanges();
        var salesBeforeDelete = TheBallDbContext.Sales.Where(x => x.BuyerId == buyer.Id).ToArray();
        _buyerStorageContract.DelElement(buyer.Id);
        var element = GetBuyerFromDatabase(buyer.Id);
        var salesAfterDelete = TheBallDbContext.Sales.Where(x => x.BuyerId == buyer.Id).ToArray();
        Assert.Multiple(() =>
        {
            Assert.That(element, Is.Null);
            Assert.That(salesBeforeDelete, Has.Length.EqualTo(2));
            Assert.That(salesAfterDelete, Is.Empty);
            Assert.That(TheBallDbContext.Sales.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _buyerStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    private Buyer InsertBuyerToDatabaseAndReturn(string id, string fio = "somefio", string phoneNumber = "+7-777-777-77-77", double discountSize = 15)
    {
        var buyer = new Buyer() { Id = id, FIO = fio, PhoneNumber = phoneNumber, DiscountSize = discountSize };
        TheBallDbContext.Buyers.Add(buyer);
        TheBallDbContext.SaveChanges();
        return buyer;
    }

    private static void AssertElement(BuyerDataModel? actual, Buyer expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.FIO, Is.EqualTo(expected.FIO));
            Assert.That(actual.PhoneNumber, Is.EqualTo(expected.PhoneNumber));
            Assert.That(actual.DiscountSize, Is.EqualTo(expected.DiscountSize));
        });
    }

    private static BuyerDataModel CreateModel(string id, string fio = "test", string phoneNumber = "+7-777-777-77-77", double discountSize = 10)
        => new(id, fio, phoneNumber, discountSize);

    private Buyer? GetBuyerFromDatabase(string id) => TheBallDbContext.Buyers.FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Buyer? actual, BuyerDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.FIO, Is.EqualTo(expected.FIO));
            Assert.That(actual.PhoneNumber, Is.EqualTo(expected.PhoneNumber));
            Assert.That(actual.DiscountSize, Is.EqualTo(expected.DiscountSize));
        });
    }
}
