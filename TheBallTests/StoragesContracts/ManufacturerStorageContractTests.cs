

using Microsoft.EntityFrameworkCore;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallDatabase.Implementations;
using TheBallDatabase.Models;

namespace TheBallTests.StoragesContracts;

[TestFixture]
internal class ManufacturerStorageContractTests : BaseStorageContractTest
{
    private ManufacturerStorageContract _manufacturerStorageContract;

    [SetUp]
    public void SetUp()
    {
        _manufacturerStorageContract = new ManufacturerStorageContract(TheBallDbContext);
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
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1");
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 2");
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 3");
        var list = _manufacturerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _manufacturerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_manufacturerStorageContract.GetElementById(manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_GetElementById_WhenNoRecord_Test()
    {
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _manufacturerStorageContract.GetElementById(Guid.NewGuid().ToString()), Is.Null);
    }

    [Test]
    public void Try_GetElementByName_WhenHaveRecord_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_manufacturerStorageContract.GetElementByName(manufacturer.Name), manufacturer);
    }

    [Test]
    public void Try_GetElementByName_WhenNoRecord_Test()
    {
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _manufacturerStorageContract.GetElementByName("name"), Is.Null);
    }

    [Test]
    public void Try_GetElementByOldName_WhenHaveRecord_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_manufacturerStorageContract.GetElementByOldName(manufacturer.PrevName!), manufacturer);
        AssertElement(_manufacturerStorageContract.GetElementByOldName(manufacturer.PrevPrevName!), manufacturer);
    }

    [Test]
    public void Try_GetElementByOldName_WhenNoRecord_Test()
    {
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _manufacturerStorageContract.GetElementByOldName("name"), Is.Null);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString());
        _manufacturerStorageContract.AddElement(manufacturer);
        AssertElement(GetManufacturerFromDatabase(manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString(), "name unique");
        InsertManufacturerToDatabaseAndReturn(manufacturer.Id);
        Assert.That(() => _manufacturerStorageContract.AddElement(manufacturer), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameManufacturerName_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString(), "name unique");
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), manufacturerName: manufacturer.Name);
        Assert.That(() => _manufacturerStorageContract.AddElement(manufacturer), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString(), "name new", "test", "prev");
        InsertManufacturerToDatabaseAndReturn(manufacturer.Id, manufacturerName: manufacturer.PrevName!, prevManufacturerName: manufacturer.PrevPrevName!);
        _manufacturerStorageContract.UpdElement(CreateModel(manufacturer.Id, "name new", "some name", "some name"));
        AssertElement(GetManufacturerFromDatabase(manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_UpdElement_WhenNoChangeManufacturerName_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString(), "name new", "test", "prev");
        InsertManufacturerToDatabaseAndReturn(manufacturer.Id, manufacturerName: manufacturer.Name!, prevManufacturerName: manufacturer.PrevName!, prevPrevManufacturerName: manufacturer.PrevPrevName!);
        _manufacturerStorageContract.UpdElement(manufacturer);
        AssertElement(GetManufacturerFromDatabase(manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _manufacturerStorageContract.UpdElement(CreateModel(Guid.NewGuid().ToString())), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_UpdElement_WhenHaveRecordWithSameManufacturerName_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString(), "name unique");
        InsertManufacturerToDatabaseAndReturn(manufacturer.Id, manufacturerName: "some name");
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), manufacturerName: manufacturer.Name);
        Assert.That(() => _manufacturerStorageContract.UpdElement(manufacturer), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_DelElement_WhenNoProducts_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        _manufacturerStorageContract.DelElement(manufacturer.Id);
        var element = GetManufacturerFromDatabase(manufacturer.Id);
        Assert.That(element, Is.Null);
    }

    [Test]
    public void Try_DelElement_WhenHaveProducts_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        TheBallDbContext.Gifts.Add(new Gift() { Id = Guid.NewGuid().ToString(), Name = "name", ManufacturerId = manufacturer.Id, Price = 10, IsDeleted = false });
        TheBallDbContext.SaveChanges();
        Assert.That(() => _manufacturerStorageContract.DelElement(manufacturer.Id), Throws.TypeOf<StorageException>());
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _manufacturerStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    private Manufacturer InsertManufacturerToDatabaseAndReturn(string id, string manufacturerName = "test", string prevManufacturerName = "prev", string prevPrevManufacturerName = "prevPrev")
    {
        var manufacturer = new Manufacturer() { Id = id, Name = manufacturerName, PrevName = prevManufacturerName, PrevPrevName = prevPrevManufacturerName };
        TheBallDbContext.Manufacturers.Add(manufacturer);
        TheBallDbContext.SaveChanges();
        return manufacturer;
    }

    private static void AssertElement(ManufacturerDataModel? actual, Manufacturer expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.PrevName, Is.EqualTo(expected.PrevName));
            Assert.That(actual.PrevPrevName, Is.EqualTo(expected.PrevPrevName));
        });
    }

    private static ManufacturerDataModel CreateModel(string id, string manufacturerName = "test", string prevManufacturerName = "prev", string prevPrevManufacturerName = "prevPrev")
        => new(id, manufacturerName, prevManufacturerName, prevPrevManufacturerName);

    private Manufacturer? GetManufacturerFromDatabase(string id) => TheBallDbContext.Manufacturers.FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Manufacturer? actual, ManufacturerDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.PrevName, Is.EqualTo(expected.PrevName));
            Assert.That(actual.PrevPrevName, Is.EqualTo(expected.PrevPrevName));
        });
    }
}
