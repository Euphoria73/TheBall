using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallDatabase.Implementations;
using TheBallDatabase.Models;
using TheBallDatabase;
using Microsoft.EntityFrameworkCore;

namespace TheBallTests.StoragesContracts;

[TestFixture]
internal class SaleStorageContractTests : BaseStorageContractTest
{
    private SaleStorageContract _saletStorageContract;
    private Buyer _buyer;
    private Worker _worker;
    private Gift _gift;
    private Manufacturer _manufacturer;

    [SetUp]
    public void SetUp()
    {
        _saletStorageContract = new SaleStorageContract(TheBallDbContext);
        _manufacturer = InsertManufacturerToDatabaseAndReturn();
        _buyer = InsertBuyerToDatabaseAndReturn();
        _worker = InsertWorkerToDatabaseAndReturn();
        _gift = InsertGiftToDatabaseAndReturn();
    }

    [TearDown]
    public void TearDown()
    {
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Sales\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Buyers\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Workers\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Gifts\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Manufacturers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 5)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, null, gifts: [(_gift.Id, 10)]);
        var list = _saletStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == sale.Id), sale);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _saletStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetList_ByPeriod_Test()
    {
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, saleDate: DateTime.UtcNow.AddDays(-1).AddMinutes(-3), gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, saleDate: DateTime.UtcNow.AddDays(-1).AddMinutes(3), gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, null, saleDate: DateTime.UtcNow.AddDays(1).AddMinutes(-3), gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, null, saleDate: DateTime.UtcNow.AddDays(1).AddMinutes(3), gifts: [(_gift.Id, 1)]);
        var list = _saletStorageContract.GetList(startDate: DateTime.UtcNow.AddDays(-1), endDate: DateTime.UtcNow.AddDays(1));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
    }

    [Test]
    public void Try_GetList_ByWorkerId_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn("Other worker");
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(worker.Id, null, gifts: [(_gift.Id, 1)]);
        var list = _saletStorageContract.GetList(workerId: _worker.Id);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(list.All(x => x.WorkerId == _worker.Id));
    }

    [Test]
    public void Try_GetList_ByBuyerId_Test()
    {
        var buyer = InsertBuyerToDatabaseAndReturn("Other fio", "+8-888-888-88-88");
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, buyer.Id, gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, null, gifts: [(_gift.Id, 1)]);
        var list = _saletStorageContract.GetList(buyerId: _buyer.Id);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(list.All(x => x.BuyerId == _buyer.Id));
    }

    [Test]
    public void Try_GetList_ByProductId_Test()
    {
        var gift = InsertGiftToDatabaseAndReturn("Other name");
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 5)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1), (gift.Id, 4)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, null, gifts: [(gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, null, gifts: [(gift.Id, 1), (_gift.Id, 1)]);
        var list = _saletStorageContract.GetList(giftId: _gift.Id);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        Assert.That(list.All(x => x.Gifts.Any(y => y.GiftId == _gift.Id)));
    }

    [Test]
    public void Try_GetList_ByAllParameters_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn("Other worker");
        var buyer = InsertBuyerToDatabaseAndReturn("Other fio", "+8-888-888-88-88");
        var gift = InsertGiftToDatabaseAndReturn("Other name");
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, saleDate: DateTime.UtcNow.AddDays(-1).AddMinutes(-3), gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(worker.Id, null, saleDate: DateTime.UtcNow.AddDays(-1).AddMinutes(3), gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(worker.Id, _buyer.Id, saleDate: DateTime.UtcNow.AddDays(-1).AddMinutes(3), gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(worker.Id, _buyer.Id, saleDate: DateTime.UtcNow.AddDays(-1).AddMinutes(3), gifts: [(gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, buyer.Id, saleDate: DateTime.UtcNow.AddDays(1).AddMinutes(-3), gifts: [(_gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, saleDate: DateTime.UtcNow.AddDays(1).AddMinutes(-3), gifts: [(gift.Id, 1)]);
        InsertSaleToDatabaseAndReturn(worker.Id, null, saleDate: DateTime.UtcNow.AddDays(1).AddMinutes(-3), gifts: [(_gift.Id, 1)]);
        var list = _saletStorageContract.GetList(startDate: DateTime.UtcNow.AddDays(-1), endDate: DateTime.UtcNow.AddDays(1), workerId: _worker.Id, buyerId: _buyer.Id, giftId: gift.Id);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(1));
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)]);
        AssertElement(_saletStorageContract.GetElementById(sale.Id), sale);
    }

    [Test]
    public void Try_GetElementById_WhenNoRecord_Test()
    {
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)]);
        Assert.That(() => _saletStorageContract.GetElementById(Guid.NewGuid().ToString()), Is.Null);
    }

    [Test]
    public void Try_GetElementById_WhenRecordHasCanceled_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)], isCancel: true);
        AssertElement(_saletStorageContract.GetElementById(sale.Id), sale);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var sale = CreateModel(Guid.NewGuid().ToString(), _worker.Id, _buyer.Id, 1, DiscountType.RegularCustomer, 1, false, [_gift.Id]);
        _saletStorageContract.AddElement(sale);
        AssertElement(GetSaleFromDatabaseById(sale.Id), sale);
    }

    [Test]
    public void Try_AddElement_WhenIsDeletedIsTrue_Test()
    {
        var sale = CreateModel(Guid.NewGuid().ToString(), _worker.Id, _buyer.Id, 1, DiscountType.RegularCustomer, 1, true, [_gift.Id]);
        Assert.That(() => _saletStorageContract.AddElement(sale), Throws.Nothing);
        AssertElement(GetSaleFromDatabaseById(sale.Id), CreateModel(sale.Id, _worker.Id, _buyer.Id, 1, DiscountType.RegularCustomer, 1, false, [_gift.Id]));
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)], isCancel: false);
        _saletStorageContract.DelElement(sale.Id);
        var element = GetSaleFromDatabaseById(sale.Id);
        Assert.Multiple(() =>
        {
            Assert.That(element, Is.Not.Null);
            Assert.That(element!.IsCancel);
        });
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _saletStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_DelElement_WhenRecordWasCanceled_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, gifts: [(_gift.Id, 1)], isCancel: true);
        Assert.That(() => _saletStorageContract.DelElement(sale.Id), Throws.TypeOf<ElementDeletedException>());
    }

    private Buyer InsertBuyerToDatabaseAndReturn(string fio = "test", string phoneNumber = "+7-777-777-77-77")
    {
        var buyer = new Buyer() { Id = Guid.NewGuid().ToString(), FIO = fio, PhoneNumber = phoneNumber, DiscountSize = 10 };
        TheBallDbContext.Buyers.Add(buyer);
        TheBallDbContext.SaveChanges();
        return buyer;
    }

    private Worker InsertWorkerToDatabaseAndReturn(string fio = "test")
    {
        var worker = new Worker() { Id = Guid.NewGuid().ToString(), FIO = fio, PostId = Guid.NewGuid().ToString() };
        TheBallDbContext.Workers.Add(worker);
        TheBallDbContext.SaveChanges();
        return worker;
    }

    private Manufacturer InsertManufacturerToDatabaseAndReturn()
    {
        var manufacrurer = new Manufacturer() { Id = Guid.NewGuid().ToString(), ManufacturerName = "name" };
        TheBallDbContext.Manufacturers.Add(manufacrurer);
        TheBallDbContext.SaveChanges();
        return manufacrurer;
    }

    private Gift InsertGiftToDatabaseAndReturn(string giftName = "test", GiftType giftType = GiftType.Chocolate, double price = 1, bool isDeleted = false)
    {
        var gift = new Gift() { Id = Guid.NewGuid().ToString(), ManufacturerId = _manufacturer.Id, Name = giftName, GiftType = giftType, Price = price, IsDeleted = isDeleted };
        TheBallDbContext.Gifts.Add(gift);
        TheBallDbContext.SaveChanges();
        return gift;
    }

    private Sale InsertSaleToDatabaseAndReturn(string workerId, string? buyerId, DateTime? saleDate = null, double sum = 1, DiscountType discountType = DiscountType.OnSale, double discount = 0, bool isCancel = false, List<(string, int)>? gifts = null)
    {
        var sale = new Sale() { WorkerId = workerId, BuyerId = buyerId, SaleDate = saleDate ?? DateTime.UtcNow, Sum = sum, DiscountType = discountType, Discount = discount, IsCancel = isCancel, SaleGifts = [] };
        if (gifts is not null)
        {
            foreach (var elem in gifts)
            {
                sale.SaleGifts.Add(new SaleGift { GiftId = elem.Item1, SaleId = sale.Id, Count = elem.Item2 });
            }
        }
        TheBallDbContext.Sales.Add(sale);
        TheBallDbContext.SaveChanges();
        return sale;
    }

    private static void AssertElement(SaleDataModel? actual, Sale expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.WorkerId, Is.EqualTo(expected.WorkerId));
            Assert.That(actual.BuyerId, Is.EqualTo(expected.BuyerId));
            Assert.That(actual.DiscountType, Is.EqualTo(expected.DiscountType));
            Assert.That(actual.Discount, Is.EqualTo(expected.Discount));
            Assert.That(actual.IsCancel, Is.EqualTo(expected.IsCancel));
        });

        if (expected.SaleGifts is not null)
        {
            Assert.That(actual.Gifts, Is.Not.Null);
            Assert.That(actual.Gifts, Has.Count.EqualTo(expected.SaleGifts.Count));
            for (int i = 0; i < actual.Gifts.Count; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actual.Gifts[i].GiftId, Is.EqualTo(expected.SaleGifts[i].GiftId));
                    Assert.That(actual.Gifts[i].Count, Is.EqualTo(expected.SaleGifts[i].Count));
                });
            }
        }
        else
        {
            Assert.That(actual.Gifts, Is.Null);
        }
    }

    private static SaleDataModel CreateModel(string id, string workerId, string? buyerId, double sum, DiscountType discountType, double discount, bool isCancel, List<string> productIds)
    {
        var products = productIds.Select(x => new SaleGiftDataModel(id, x, 1)).ToList();
        return new(id, workerId, buyerId, sum, discountType, discount, isCancel, products);
    }

    private Sale? GetSaleFromDatabaseById(string id) => TheBallDbContext.Sales.Include(x => x.SaleGifts).FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Sale? actual, SaleDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.WorkerId, Is.EqualTo(expected.WorkerId));
            Assert.That(actual.BuyerId, Is.EqualTo(expected.BuyerId));
            Assert.That(actual.DiscountType, Is.EqualTo(expected.DiscountType));
            Assert.That(actual.Discount, Is.EqualTo(expected.Discount));
            Assert.That(actual.IsCancel, Is.EqualTo(expected.IsCancel));
        });

        if (expected.Gifts is not null)
        {
            Assert.That(actual.SaleGifts, Is.Not.Null);
            Assert.That(actual.SaleGifts, Has.Count.EqualTo(expected.Gifts.Count));
            for (int i = 0; i < actual.SaleGifts.Count; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actual.SaleGifts[i].GiftId, Is.EqualTo(expected.Gifts[i].GiftId));
                    Assert.That(actual.SaleGifts[i].Count, Is.EqualTo(expected.Gifts[i].Count));
                });
            }
        }
        else
        {
            Assert.That(actual.SaleGifts, Is.Null);
        }
    }
}
