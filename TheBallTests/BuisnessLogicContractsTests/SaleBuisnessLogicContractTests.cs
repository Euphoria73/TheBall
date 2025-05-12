using TheBallBuisnessLogic.Implementations;
using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TheBallTests.BuisnessLogicContractsTests;

[TestFixture]
internal class SaleBuisnessLogicContractTests
{
    private SaleBuisnessLogicContract _saleBusinessLogicContract;
    private Mock<ISaleStorageContract> _saleStorageContract;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _saleStorageContract = new Mock<ISaleStorageContract>();
        _saleBusinessLogicContract = new SaleBuisnessLogicContract(_saleStorageContract.Object, new Mock<ILogger>().Object);
    }

    [SetUp]
    public void SetUp()
    {
        _saleStorageContract.Reset();
    }

    [Test]
    public void GetAllSalesByPeriod_ReturnListOfRecords_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        var listOriginal = new List<SaleDataModel>()
        {
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false,
                [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)]),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
        };
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(listOriginal);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByPeriod(date, date.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _saleStorageContract.Verify(x => x.GetList(date, date.AddDays(1), null, null, null), Times.Once);
    }

    [Test]
    public void GetAllSalesByPeriod_ReturnEmptyList_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns([]);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByPeriod_IncorrectDates_ThrowException_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByPeriod(date, date), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByPeriod(date, date.AddSeconds(-1)), Throws.TypeOf<IncorrectDatesException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByPeriod_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<NullListException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByPeriod_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<StorageException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByWorkerByPeriod_ReturnListOfRecords_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        var workerId = Guid.NewGuid().ToString();
        var listOriginal = new List<SaleDataModel>()
        {
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false,
                [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)]),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
        };
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(listOriginal);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(workerId, date, date.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _saleStorageContract.Verify(x => x.GetList(date, date.AddDays(1), workerId, null, null), Times.Once);
    }

    [Test]
    public void GetAllSalesByWorkerByPeriod_ReturnEmptyList_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns([]);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByWorkerByPeriod_IncorrectDates_ThrowException_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(Guid.NewGuid().ToString(), date, date), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(Guid.NewGuid().ToString(), date, date.AddSeconds(-1)), Throws.TypeOf<IncorrectDatesException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByWorkerByPeriod_WorkerIdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(string.Empty, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ArgumentNullException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByWorkerByPeriod_WorkerIdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod("workerId", DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ValidationException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByWorkerByPeriod_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<NullListException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByWorkerByPeriod_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByWorkerByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<StorageException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByBuyerByPeriod_ReturnListOfRecords_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        var buyerId = Guid.NewGuid().ToString();
        var listOriginal = new List<SaleDataModel>()
        {
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false,
                [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)]),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
        };
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(listOriginal);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(buyerId, date, date.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _saleStorageContract.Verify(x => x.GetList(date, date.AddDays(1), null, buyerId, null), Times.Once);
    }

    [Test]
    public void GetAllSalesByBuyerByPeriod_ReturnEmptyList_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns([]);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByBuyerByPeriod_IncorrectDates_ThrowException_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(Guid.NewGuid().ToString(), date, date), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(Guid.NewGuid().ToString(), date, date.AddSeconds(-1)), Throws.TypeOf<IncorrectDatesException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByBuyerByPeriod_BuyerIdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(string.Empty, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ArgumentNullException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByBuyerByPeriod_BuyerIdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod("buyerId", DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ValidationException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByBuyerByPeriod_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<NullListException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByBuyerByPeriod_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByBuyerByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<StorageException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByProductByPeriod_ReturnListOfRecords_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        var productId = Guid.NewGuid().ToString();
        var listOriginal = new List<SaleDataModel>()
        {
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false,
                [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)]),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false, []),
        };
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(listOriginal);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(productId, date, date.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _saleStorageContract.Verify(x => x.GetList(date, date.AddDays(1), null, null, productId), Times.Once);
    }

    [Test]
    public void GetAllSalesByProductByPeriod_ReturnEmptyList_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns([]);
        //Act
        var list = _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByProductByPeriod_IncorrectDates_ThrowException_Test()
    {
        //Arrange
        var date = DateTime.UtcNow;
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(Guid.NewGuid().ToString(), date, date), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(Guid.NewGuid().ToString(), date, date.AddSeconds(-1)), Throws.TypeOf<IncorrectDatesException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByProductByPeriod_ProductIdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(string.Empty, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ArgumentNullException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByProductByPeriod_ProductIdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByGiftByPeriod("productId", DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<ValidationException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalesByProductByPeriod_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<NullListException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalesByProductByPeriod_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetAllSalesByGiftByPeriod(Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<StorageException>());
        _saleStorageContract.Verify(x => x.GetList(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetSaleByData_GetById_ReturnRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var record = new SaleDataModel(id, Guid.NewGuid().ToString(), null, 10, DiscountType.None, 0, false,
                [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)]);
        _saleStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        //Act
        var element = _saleBusinessLogicContract.GetSaleByData(id);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _saleStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetSaleByData_EmptyData_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetSaleByData(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _saleBusinessLogicContract.GetSaleByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _saleStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetSaleByData_IdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetSaleByData("saleId"), Throws.TypeOf<ValidationException>());
        _saleStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetSaleByData_GetById_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetSaleByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _saleStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetSaleByData_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.GetSaleByData(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _saleStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertSale_CorrectRecord_Test()
    {
        //Arrange
        var flag = false;
        var record = new SaleDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 10, DiscountType.None, 10,
            false, [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)]);
        _saleStorageContract.Setup(x => x.AddElement(It.IsAny<SaleDataModel>()))
            .Callback((SaleDataModel x) =>
            {
                flag = x.Id == record.Id && x.WorkerId == record.WorkerId && x.BuyerId == record.BuyerId &&
                    x.SaleDate == record.SaleDate && x.Sum == record.Sum && x.DiscountType == record.DiscountType &&
                    x.Discount == record.Discount && x.IsCancel == record.IsCancel && x.Gifts.Count == record.Gifts.Count &&
                    x.Gifts.First().GiftId == record.Gifts.First().GiftId &&
                    x.Gifts.First().SaleId == record.Gifts.First().SaleId &&
                    x.Gifts.First().Count == record.Gifts.First().Count;
            });
        //Act
        _saleBusinessLogicContract.InsertSale(record);
        //Assert
        _saleStorageContract.Verify(x => x.AddElement(It.IsAny<SaleDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertSale_RecordWithExistsData_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.AddElement(It.IsAny<SaleDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.InsertSale(new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(), 10, DiscountType.None, 10, false, [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)])), Throws.TypeOf<ElementExistsException>());
        _saleStorageContract.Verify(x => x.AddElement(It.IsAny<SaleDataModel>()), Times.Once);
    }

    [Test]
    public void InsertSale_NullRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.InsertSale(null), Throws.TypeOf<ArgumentNullException>());
        _saleStorageContract.Verify(x => x.AddElement(It.IsAny<SaleDataModel>()), Times.Never);
    }

    [Test]
    public void InsertSale_InvalidRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.InsertSale(new SaleDataModel("id", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 10, DiscountType.None, 10, false, [])), Throws.TypeOf<ValidationException>());
        _saleStorageContract.Verify(x => x.AddElement(It.IsAny<SaleDataModel>()), Times.Never);
    }

    [Test]
    public void InsertSale_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.AddElement(It.IsAny<SaleDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.InsertSale(new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(), 10, DiscountType.None, 10, false, [new SaleGiftDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5)])), Throws.TypeOf<StorageException>());
        _saleStorageContract.Verify(x => x.AddElement(It.IsAny<SaleDataModel>()), Times.Once);
    }

    [Test]
    public void CancelSale_CorrectRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _saleStorageContract.Setup(x => x.DelElement(It.Is((string x) => x == id))).Callback(() => { flag = true; });
        //Act
        _saleBusinessLogicContract.CancelSale(id);
        //Assert
        _saleStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void CancelSale_RecordWithIncorrectId_ThrowException_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(id));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.CancelSale(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _saleStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void CancelSale_IdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.CancelSale(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _saleBusinessLogicContract.CancelSale(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _saleStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CancelSale_IdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.CancelSale("id"), Throws.TypeOf<ValidationException>());
        _saleStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CancelSale_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _saleStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _saleBusinessLogicContract.CancelSale(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _saleStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }
}
