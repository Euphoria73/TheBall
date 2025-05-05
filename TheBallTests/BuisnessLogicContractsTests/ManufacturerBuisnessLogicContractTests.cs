using TheBallBuisnessLogic.Implementations;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace CatHasPawsTests.BusinessLogicsContractsTests;

[TestFixture]
internal class ManufacturerBuisnessLogicContractTests
{
    private ManufacturerBuisnessLogicContract _manufacturerBusinessLogicContract;
    private Mock<IManufacturerStorageContract> _manufacturerStorageContract;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _manufacturerStorageContract = new Mock<IManufacturerStorageContract>();
        _manufacturerBusinessLogicContract = new ManufacturerBuisnessLogicContract(_manufacturerStorageContract.Object, new Mock<ILogger>().Object);
    }

    [SetUp]
    public void SetUp()
    {
        _manufacturerStorageContract.Reset();
    }

    [Test]
    public void GetAllManufacturers_ReturnListOfRecords_Test()
    {
        //Arrange
        var listOriginal = new List<ManufacturerDataModel>()
        {
            new(Guid.NewGuid().ToString(), "name 1", null, null),
            new(Guid.NewGuid().ToString(), "name 2", null, null),
            new(Guid.NewGuid().ToString(), "name 3", null, null),
        };
        _manufacturerStorageContract.Setup(x => x.GetList()).Returns(listOriginal);
        //Act
        var list = _manufacturerBusinessLogicContract.GetAllManufacturers();
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
    }

    [Test]
    public void GetAllManufacturers_ReturnEmptyList_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.GetList()).Returns([]);
        //Act
        var list = _manufacturerBusinessLogicContract.GetAllManufacturers();
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _manufacturerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllManufacturers_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.GetAllManufacturers(), Throws.TypeOf<NullListException>());
        _manufacturerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllManufacturers_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.GetList()).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.GetAllManufacturers(), Throws.TypeOf<StorageException>());
        _manufacturerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetManufacturerByData_GetById_ReturnRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var record = new ManufacturerDataModel(id, "name", null, null);
        _manufacturerStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        //Act
        var element = _manufacturerBusinessLogicContract.GetManufacturerByData(id);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _manufacturerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetManufacturerByData_GetByName_ReturnRecord_Test()
    {
        //Arrange
        var manufacturerName = "name";
        var record = new ManufacturerDataModel(Guid.NewGuid().ToString(), manufacturerName, null, null);
        _manufacturerStorageContract.Setup(x => x.GetElementByName(manufacturerName)).Returns(record);
        //Act
        var element = _manufacturerBusinessLogicContract.GetManufacturerByData(manufacturerName);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.ManufacturerName, Is.EqualTo(manufacturerName));
        _manufacturerStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetManufacturerByData_GetByOldName_ReturnRecord_Test()
    {
        //Arrange
        var manufacturerOldName = "name before";
        var record = new ManufacturerDataModel(Guid.NewGuid().ToString(), "name", manufacturerOldName, null);
        _manufacturerStorageContract.Setup(x => x.GetElementByOldName(manufacturerOldName)).Returns(record);
        //Act
        var element = _manufacturerBusinessLogicContract.GetManufacturerByData(manufacturerOldName);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.PrevManufacturerName, Is.EqualTo(manufacturerOldName));
        _manufacturerStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
        _manufacturerStorageContract.Verify(x => x.GetElementByOldName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetManufacturerByData_EmptyData_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.GetManufacturerByData(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _manufacturerBusinessLogicContract.GetManufacturerByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _manufacturerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _manufacturerStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Never);
        _manufacturerStorageContract.Verify(x => x.GetElementByOldName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetManufacturerByData__GetById_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.GetManufacturerByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _manufacturerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _manufacturerStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Never);
        _manufacturerStorageContract.Verify(x => x.GetElementByOldName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetManufacturerByData_GetByNameOrOldName_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.GetManufacturerByData("name"), Throws.TypeOf<ElementNotFoundException>());
        _manufacturerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _manufacturerStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
        _manufacturerStorageContract.Verify(x => x.GetElementByOldName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetManufacturerByData_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _manufacturerStorageContract.Setup(x => x.GetElementByName(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.GetManufacturerByData(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        Assert.That(() => _manufacturerBusinessLogicContract.GetManufacturerByData("name"), Throws.TypeOf<StorageException>());
        _manufacturerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _manufacturerStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
        _manufacturerStorageContract.Verify(x => x.GetElementByOldName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetManufacturerByData_GetByOldName_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.GetElementByOldName(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.GetManufacturerByData("name"), Throws.TypeOf<StorageException>());
        _manufacturerStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
        _manufacturerStorageContract.Verify(x => x.GetElementByOldName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertManufacturer_CorrectRecord_Test()
    {
        //Arrange
        var flag = false;
        var record = new ManufacturerDataModel(Guid.NewGuid().ToString(), "name", null, null);
        _manufacturerStorageContract.Setup(x => x.AddElement(It.IsAny<ManufacturerDataModel>()))
            .Callback((ManufacturerDataModel x) =>
            {
                flag = x.Id == record.Id && x.ManufacturerName == record.ManufacturerName;
            });
        //Act
        _manufacturerBusinessLogicContract.InsertManufacturer(record);
        //Assert
        _manufacturerStorageContract.Verify(x => x.AddElement(It.IsAny<ManufacturerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertManufacturer_RecordWithExistsData_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.AddElement(It.IsAny<ManufacturerDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.InsertManufacturer(new(Guid.NewGuid().ToString(), "name", null, null)), Throws.TypeOf<ElementExistsException>());
        _manufacturerStorageContract.Verify(x => x.AddElement(It.IsAny<ManufacturerDataModel>()), Times.Once);
    }

    [Test]
    public void InsertManufacturer_NullRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.InsertManufacturer(null), Throws.TypeOf<ArgumentNullException>());
        _manufacturerStorageContract.Verify(x => x.AddElement(It.IsAny<ManufacturerDataModel>()), Times.Never);
    }

    [Test]
    public void InsertManufacturer_InvalidRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.InsertManufacturer(new ManufacturerDataModel("id", "name", null, null)), Throws.TypeOf<ValidationException>());
        _manufacturerStorageContract.Verify(x => x.AddElement(It.IsAny<ManufacturerDataModel>()), Times.Never);
    }

    [Test]
    public void InsertManufacturer_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.AddElement(It.IsAny<ManufacturerDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.InsertManufacturer(new(Guid.NewGuid().ToString(), "name", null, null)), Throws.TypeOf<StorageException>());
        _manufacturerStorageContract.Verify(x => x.AddElement(It.IsAny<ManufacturerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateManufacturer_CorrectRecord_Test()
    {
        //Arrange
        var flag = false;
        var record = new ManufacturerDataModel(Guid.NewGuid().ToString(), "name", null, null);
        _manufacturerStorageContract.Setup(x => x.UpdElement(It.IsAny<ManufacturerDataModel>()))
            .Callback((ManufacturerDataModel x) =>
            {
                flag = x.Id == record.Id && x.ManufacturerName == record.ManufacturerName;
            });
        //Act
        _manufacturerBusinessLogicContract.UpdateManufacturer(record);
        //Assert
        _manufacturerStorageContract.Verify(x => x.UpdElement(It.IsAny<ManufacturerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void UpdateManufacturer_RecordWithIncorrectData_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.UpdElement(It.IsAny<ManufacturerDataModel>())).Throws(new ElementNotFoundException(""));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.UpdateManufacturer(new(Guid.NewGuid().ToString(), "name", null, null)), Throws.TypeOf<ElementNotFoundException>());
        _manufacturerStorageContract.Verify(x => x.UpdElement(It.IsAny<ManufacturerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateManufacturer_RecordWithExistsData_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.UpdElement(It.IsAny<ManufacturerDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.UpdateManufacturer(new(Guid.NewGuid().ToString(), "name", null, null)), Throws.TypeOf<ElementExistsException>());
        _manufacturerStorageContract.Verify(x => x.UpdElement(It.IsAny<ManufacturerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateManufacturer_NullRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.UpdateManufacturer(null), Throws.TypeOf<ArgumentNullException>());
        _manufacturerStorageContract.Verify(x => x.UpdElement(It.IsAny<ManufacturerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateManufacturer_InvalidRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.UpdateManufacturer(new ManufacturerDataModel("id", "name", null, null)), Throws.TypeOf<ValidationException>());
        _manufacturerStorageContract.Verify(x => x.UpdElement(It.IsAny<ManufacturerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateManufacturer_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.UpdElement(It.IsAny<ManufacturerDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.UpdateManufacturer(new(Guid.NewGuid().ToString(), "name", null, null)), Throws.TypeOf<StorageException>());
        _manufacturerStorageContract.Verify(x => x.UpdElement(It.IsAny<ManufacturerDataModel>()), Times.Once);
    }

    [Test]
    public void DeleteManufacturer_CorrectRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _manufacturerStorageContract.Setup(x => x.DelElement(It.Is((string x) => x == id))).Callback(() => { flag = true; });
        //Act
        _manufacturerBusinessLogicContract.DeleteManufacturer(id);
        //Assert
        _manufacturerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void DeleteManufacturer_RecordWithIncorrectId_ThrowException_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        _manufacturerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(id));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.DeleteManufacturer(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _manufacturerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void DeleteManufacturer_IdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.DeleteManufacturer(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _manufacturerBusinessLogicContract.DeleteManufacturer(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _manufacturerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteManufacturer_IdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.DeleteManufacturer("id"), Throws.TypeOf<ValidationException>());
        _manufacturerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteManufacturer_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _manufacturerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _manufacturerBusinessLogicContract.DeleteManufacturer(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _manufacturerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }
}