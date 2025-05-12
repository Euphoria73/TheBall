using TheBallBuisnessLogic.Implementations;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TheBallTests.BuisnessLogicContractsTests;

internal class BuyerBuisnessLogicContractTests
{
    private BuyerBuisnessLogicContract _buyerBusinessLogicContract;
    private Mock<IBuyerStorageContract> _buyerStorageContract;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _buyerStorageContract = new Mock<IBuyerStorageContract>();
        _buyerBusinessLogicContract = new BuyerBuisnessLogicContract(_buyerStorageContract.Object, new Mock<ILogger>().Object);
    }

    [SetUp]
    public void SetUp()
    {
        _buyerStorageContract.Reset();
    }

    [Test]
    public void GetAllBuyers_ReturnListOfRecords_Test()
    {
        //Arrange
        var listOriginal = new List<BuyerDataModel>()
        {
            new(Guid.NewGuid().ToString(), "fio1", "+7-777-777-77-77", 0),
            new(Guid.NewGuid().ToString(), "fio2", "+7-234-123-45-67", 10),
            new(Guid.NewGuid().ToString(), "fio3", "+7-777-777-7777", 0),
        };
        _buyerStorageContract.Setup(x => x.GetList()).Returns(listOriginal);
        //Act
        var list = _buyerBusinessLogicContract.GetAllBuyers();
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
    }

    [Test]
    public void GetAllBuyers_ReturnEmptyList_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.GetList()).Returns([]);
        //Act
        var list = _buyerBusinessLogicContract.GetAllBuyers();
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _buyerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllBuyers_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.GetAllBuyers(), Throws.TypeOf<NullListException>());
        _buyerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllBuyers_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.GetList()).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.GetAllBuyers(), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetBuyerByData_GetById_ReturnRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var record = new BuyerDataModel(id, "fio", "+7-111-111-11-11", 0);
        _buyerStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        //Act
        var element = _buyerBusinessLogicContract.GetBuyerByData(id);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_GetByFio_ReturnRecord_Test()
    {
        //Arrange
        var fio = "fio";
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), fio, "+7-111-111-11-11", 0);
        _buyerStorageContract.Setup(x => x.GetElementByFIO(fio)).Returns(record);
        //Act
        var element = _buyerBusinessLogicContract.GetBuyerByData(fio);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.FIO, Is.EqualTo(fio));
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_GetByPhoneNumber_ReturnRecord_Test()
    {
        //Arrange
        var phoneNumber = "+7-111-111-11-11";
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), "fio", phoneNumber, 0);
        _buyerStorageContract.Setup(x => x.GetElementByPhoneNumber(phoneNumber)).Returns(record);
        //Act
        var element = _buyerBusinessLogicContract.GetBuyerByData(phoneNumber);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.PhoneNumber, Is.EqualTo(phoneNumber));
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_EmptyData_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetBuyerByData_GetById_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetBuyerByData_GetByFio_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("fio"), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetBuyerByData_GetByPhoneNumber_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("+7-111-111-11-12"), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _buyerStorageContract.Setup(x => x.GetElementByFIO(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _buyerStorageContract.Setup(x => x.GetElementByPhoneNumber(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("fio"), Throws.TypeOf<StorageException>());
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("+7-111-111-11-12"), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertBuyer_CorrectRecord_Test()
    {
        //Arrange
        var flag = false;
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 10);
        _buyerStorageContract.Setup(x => x.AddElement(It.IsAny<BuyerDataModel>()))
            .Callback((BuyerDataModel x) =>
            {
                flag = x.Id == record.Id && x.FIO == record.FIO &&
                    x.PhoneNumber == record.PhoneNumber && x.DiscountSize == record.DiscountSize;
            });
        //Act
        _buyerBusinessLogicContract.InsertBuyer(record);
        //Assert
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertBuyer_RecordWithExistsData_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.AddElement(It.IsAny<BuyerDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(new(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<ElementExistsException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void InsertBuyer_NullRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(null), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void InsertBuyer_InvalidRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(new BuyerDataModel("id", "fio", "+7-111-111-11-11", 10)), Throws.TypeOf<ValidationException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void InsertBuyer_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.AddElement(It.IsAny<BuyerDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(new(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateBuyer_CorrectRecord_Test()
    {
        //Arrange
        var flag = false;
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0);
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>()))
            .Callback((BuyerDataModel x) =>
            {
                flag = x.Id == record.Id && x.FIO == record.FIO &&
                    x.PhoneNumber == record.PhoneNumber && x.DiscountSize == record.DiscountSize;
            });
        //Act
        _buyerBusinessLogicContract.UpdateBuyer(record);
        //Assert
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void UpdateBuyer_RecordWithIncorrectData_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>())).Throws(new ElementNotFoundException(""));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateBuyer_RecordWithExistsData_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<ElementExistsException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateBuyer_NullRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(null), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateBuyer_InvalidRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new BuyerDataModel("id", "fio", "+7-111-111-11-11", 10)), Throws.TypeOf<ValidationException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateBuyer_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void DeleteBuyer_CorrectRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _buyerStorageContract.Setup(x => x.DelElement(It.Is((string x) => x == id))).Callback(() => { flag = true; });
        //Act
        _buyerBusinessLogicContract.DeleteBuyer(id);
        //Assert
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void DeleteBuyer_RecordWithIncorrectId_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(""));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void DeleteBuyer_IdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteBuyer_IdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer("id"), Throws.TypeOf<ValidationException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteBuyer_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _buyerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }
}
