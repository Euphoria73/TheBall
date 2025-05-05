using TheBallContracts.Exceptions;
using TheBallDatabase.Models;
using Microsoft.EntityFrameworkCore;
using TheBallDatabase.Implementations;
using TheBallContracts.DataModels;
using TheBallContracts.Enums;

namespace TheBallTests.StoragesContracts;

[TestFixture]
internal class GiftStorageContractTests : BaseStorageContractTest
{

    private GiftStorageContract _giftStorageContract;
    private Manufacturer _manufacturer;

    [SetUp]
    public void SetUp()
    {
        _giftStorageContract = new GiftStorageContract(TheBallDbContext);
        _manufacturer = InsertManufacturerToDatabaseAndReturn();
    }

    [TearDown]
    public void TearDown()
    {
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Gifts\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Manufacturers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1");
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 2");
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 3");
        var list = _giftStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == gift.Id), gift);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _giftStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetList_OnlyActual_Test()
    {
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1", isDeleted: true);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 2", isDeleted: false);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 3", isDeleted: false);
        var list = _giftStorageContract.GetList(onlyActive: true);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(!list.Any(x => x.IsDeleted));
        });
    }

    [Test]
    public void Try_GetList_IncludeNoActual_Test()
    {
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1", isDeleted: true);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 2", isDeleted: true);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 3", isDeleted: false);
        var list = _giftStorageContract.GetList(onlyActive: false);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(3));
            Assert.That(list.Count(x => x.IsDeleted), Is.EqualTo(2));
            Assert.That(list.Count(x => !x.IsDeleted), Is.EqualTo(1));
        });
    }

    [Test]
    public void Try_GetList_ByManufacturer_Test()
    {
        var manufacruer = InsertManufacturerToDatabaseAndReturn("name 2");
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1", isDeleted: true);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 2", isDeleted: false);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), manufacruer.Id, "name 3", isDeleted: false);
        var list = _giftStorageContract.GetList(manufacturerId: _manufacturer.Id, onlyActive: false);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list.All(x => x.ManufacturerId == _manufacturer.Id));
        });
    }

    [Test]
    public void Try_GetList_ByManufacturerOnlyActual_Test()
    {
        var manufacruer = InsertManufacturerToDatabaseAndReturn("name 2");
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1", isDeleted: true);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 2", isDeleted: false);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), manufacruer.Id, "name 3", isDeleted: false);
        var list = _giftStorageContract.GetList(manufacturerId: _manufacturer.Id, onlyActive: true);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list.All(x => x.ManufacturerId == _manufacturer.Id && !x.IsDeleted));
        });
    }

    [Test]
    public void Try_GetHistoryByGiftId_WhenHaveRecords_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1");
        InsertGiftHistoryToDatabaseAndReturn(gift.Id, 20, DateTime.UtcNow.AddDays(-1));
        InsertGiftHistoryToDatabaseAndReturn(gift.Id, 30, DateTime.UtcNow.AddMinutes(-10));
        InsertGiftHistoryToDatabaseAndReturn(gift.Id, 40, DateTime.UtcNow.AddDays(1));
        var list = _giftStorageContract.GetHistoryByGiftId(gift.Id);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
    }

    [Test]
    public void Try_GetHistoryByGiftId_WhenNoRecords_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1");
        InsertGiftHistoryToDatabaseAndReturn(gift.Id, 20, DateTime.UtcNow.AddDays(-1));
        InsertGiftHistoryToDatabaseAndReturn(gift.Id, 30, DateTime.UtcNow.AddMinutes(-10));
        InsertGiftHistoryToDatabaseAndReturn(gift.Id, 40, DateTime.UtcNow.AddDays(1));
        var list = _giftStorageContract.GetHistoryByGiftId(Guid.NewGuid().ToString());
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id);
        AssertElement(_giftStorageContract.GetElementById(gift.Id), gift);
    }

    [Test]
    public void Try_GetElementById_WhenNoRecord_Test()
    {
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id);
        Assert.That(() => _giftStorageContract.GetElementById(Guid.NewGuid().ToString()), Is.Null);
    }

    [Test]
    public void Try_GetElementById_WhenRecordHasDeleted_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: true);
        Assert.That(() => _giftStorageContract.GetElementById(gift.Id), Is.Null);
    }

    [Test]
    public void Try_GetElementByName_WhenHaveRecord_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id);
        AssertElement(_giftStorageContract.GetElementByName(gift.Name), gift);
    }

    [Test]
    public void Try_GetElementByName_WhenNoRecord_Test()
    {
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id);
        Assert.That(() => _giftStorageContract.GetElementByName("name"), Is.Null);
    }

    [Test]
    public void Try_GetElementByName_WhenRecordHasDeleted_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: true);
        Assert.That(() => _giftStorageContract.GetElementById(gift.Name), Is.Null);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: false);
        _giftStorageContract.AddElement(gift);
        AssertElement(GetGiftFromDatabaseById(gift.Id), gift);
    }

    [Test]
    public void Try_AddElement_WhenIsDeletedIsTrue_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: true);
        Assert.That(() => _giftStorageContract.AddElement(gift), Throws.Nothing);
        AssertElement(GetGiftFromDatabaseById(gift.Id), CreateModel(gift.Id, _manufacturer.Id, isDeleted: false));
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id);
        InsertGiftToDatabaseAndReturn(gift.Id, _manufacturer.Id, giftName: "name unique");
        Assert.That(() => _giftStorageContract.AddElement(gift), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameName_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, "name unique", isDeleted: false);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, giftName: gift.Name, isDeleted: false);
        Assert.That(() => _giftStorageContract.AddElement(gift), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameNameButOneWasDeleted_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, "name unique", isDeleted: false);
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, giftName: gift.Name, isDeleted: true);
        Assert.That(() => _giftStorageContract.AddElement(gift), Throws.Nothing);
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: false);
        InsertGiftToDatabaseAndReturn(gift.Id, _manufacturer.Id, isDeleted: false);
        _giftStorageContract.UpdElement(gift);
        AssertElement(GetGiftFromDatabaseById(gift.Id), gift);
    }

    [Test]
    public void Try_UpdElement_WhenIsDeletedIsTrue_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: true);
        InsertGiftToDatabaseAndReturn(gift.Id, _manufacturer.Id, isDeleted: false);
        _giftStorageContract.UpdElement(gift);
        AssertElement(GetGiftFromDatabaseById(gift.Id), CreateModel(gift.Id, _manufacturer.Id, isDeleted: false));
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _giftStorageContract.UpdElement(CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id)), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_UpdElement_WhenHaveRecordWithSameName_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, "name unique", isDeleted: false);
        InsertGiftToDatabaseAndReturn(gift.Id, _manufacturer.Id, giftName: "name");
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, giftName: gift.Name);
        Assert.That(() => _giftStorageContract.UpdElement(gift), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_UpdElement_WhenHaveRecordWithSameNameButOneWasDeleted_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, "name unique", isDeleted: false);
        InsertGiftToDatabaseAndReturn(gift.Id, _manufacturer.Id, giftName: "name");
        InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, giftName: gift.Name, isDeleted: true);
        Assert.That(() => _giftStorageContract.UpdElement(gift), Throws.Nothing);
    }

    [Test]
    public void Try_UpdElement_WhenRecordWasDeleted_Test()
    {
        var gift = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id);
        InsertGiftToDatabaseAndReturn(gift.Id, _manufacturer.Id, isDeleted: true);
        Assert.That(() => _giftStorageContract.UpdElement(gift), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: false);
        _giftStorageContract.DelElement(gift.Id);
        var element = GetGiftFromDatabaseById(gift.Id);
        Assert.Multiple(() =>
        {
            Assert.That(element, Is.Not.Null);
            Assert.That(element!.IsDeleted);
        });
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _giftStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_DelElement_WhenRecordWasDeleted_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: true);
        Assert.That(() => _giftStorageContract.DelElement(gift.Id), Throws.TypeOf<ElementNotFoundException>());
    }

    private Manufacturer InsertManufacturerToDatabaseAndReturn(string manufacturerName = "name")
    {
        var manufacrurer = new Manufacturer() { Id = Guid.NewGuid().ToString(), ManufacturerName = manufacturerName };
        TheBallDbContext.Manufacturers.Add(manufacrurer);
        TheBallDbContext.SaveChanges();
        return manufacrurer;
    }

    private Gift InsertGiftToDatabaseAndReturn(string id, string manufacturerId, string giftName = "test", GiftType giftType = GiftType.Toys, double price = 1, bool isDeleted = false)
    {
        var gift = new Gift() { Id = id, ManufacturerId = manufacturerId, Name = giftName, GiftType = giftType, Price = price, IsDeleted = isDeleted };
        TheBallDbContext.Gifts.Add(gift);
        TheBallDbContext.SaveChanges();
        return gift;
    }

    private GiftHistory InsertGiftHistoryToDatabaseAndReturn(string giftId, double price, DateTime changeDate)
    {
        var giftHistory = new GiftHistory() { Id = Guid.NewGuid().ToString(), GiftId = giftId, OldPrice = price, ChangeDate = changeDate };
        TheBallDbContext.GiftHistories.Add(giftHistory);
        TheBallDbContext.SaveChanges();
        return giftHistory;
    }

    private static void AssertElement(GiftDataModel? actual, Gift expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.ManufacturerId, Is.EqualTo(expected.ManufacturerId));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.GiftType, Is.EqualTo(expected.GiftType));
            Assert.That(actual.Price, Is.EqualTo(expected.Price));
            Assert.That(actual.IsDeleted, Is.EqualTo(expected.IsDeleted));
        });
    }

    private static GiftDataModel CreateModel(string id, string manufacturerId, string giftName = "test", GiftType giftType = GiftType.Chocolate, double price = 1, bool isDeleted = false)
        => new(id, giftName, giftType, manufacturerId, price, isDeleted);

    private Gift? GetGiftFromDatabaseById(string id) => TheBallDbContext.Gifts.FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Gift? actual, GiftDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.ManufacturerId, Is.EqualTo(expected.ManufacturerId));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.GiftType, Is.EqualTo(expected.GiftType));
            Assert.That(actual.Price, Is.EqualTo(expected.Price));
            Assert.That(actual.IsDeleted, Is.EqualTo(expected.IsDeleted));
        });
    }
}
