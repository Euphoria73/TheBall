
using Microsoft.EntityFrameworkCore;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallDatabase.Implementations;
using TheBallDatabase.Models;

namespace TheBallTests.StoragesContracts;

[TestFixture]
internal class WorkerStorageContractTests : BaseStorageContractTest
{
    private WorkerStorageContract _workerStorageContract;

    [SetUp]
    public void SetUp()
    {
        _workerStorageContract = new WorkerStorageContract(TheBallDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Workers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 1");
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 2");
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 3");
        var list = _workerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(), worker);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _workerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetList_ByPostId_Test()
    {
        var postId = Guid.NewGuid().ToString();
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 1", postId);
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 2", postId);
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 3");
        var list = _workerStorageContract.GetList(postId: postId);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(list.All(x => x.PostId == postId));
    }

    [Test]
    public void Try_GetList_ByBirthDate_Test()
    {
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 1", birthDate: DateTime.UtcNow.AddYears(-25));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 2", birthDate: DateTime.UtcNow.AddYears(-21));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 3", birthDate: DateTime.UtcNow.AddYears(-20));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 4", birthDate: DateTime.UtcNow.AddYears(-19));
        var list = _workerStorageContract.GetList(fromBirthDate: DateTime.UtcNow.AddYears(-21).AddMinutes(-1), toBirthDate: DateTime.UtcNow.AddYears(-20).AddMinutes(1));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
    }

    [Test]
    public void Try_GetList_ByEmploymentDate_Test()
    {
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 1", employmentDate: DateTime.UtcNow.AddDays(-2));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 2", employmentDate: DateTime.UtcNow.AddDays(-1));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 3", employmentDate: DateTime.UtcNow.AddDays(1));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 4", employmentDate: DateTime.UtcNow.AddDays(2));
        var list = _workerStorageContract.GetList(fromEmploymentDate: DateTime.UtcNow.AddDays(-1).AddMinutes(-1), toEmploymentDate: DateTime.UtcNow.AddDays(1).AddMinutes(1));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
    }

    [Test]
    public void Try_GetList_ByAllParameters_Test()
    {
        var postId = Guid.NewGuid().ToString();
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 1", postId, birthDate: DateTime.UtcNow.AddYears(-25), employmentDate: DateTime.UtcNow.AddDays(-2));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 2", postId, birthDate: DateTime.UtcNow.AddYears(-22), employmentDate: DateTime.UtcNow.AddDays(-1));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 3", postId, birthDate: DateTime.UtcNow.AddYears(-21), employmentDate: DateTime.UtcNow.AddDays(-1));
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 4", birthDate: DateTime.UtcNow.AddYears(-20), employmentDate: DateTime.UtcNow.AddDays(1));
        var list = _workerStorageContract.GetList(postId: postId, fromBirthDate: DateTime.UtcNow.AddYears(-21).AddMinutes(-1), toBirthDate: DateTime.UtcNow.AddYears(-20).AddMinutes(1), fromEmploymentDate: DateTime.UtcNow.AddDays(-1).AddMinutes(-1), toEmploymentDate: DateTime.UtcNow.AddDays(1).AddMinutes(1));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(1));
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_workerStorageContract.GetElementById(worker.Id), worker);
    }

    [Test]
    public void Try_GetElementById_WhenNoRecord_Test()
    {
        Assert.That(() => _workerStorageContract.GetElementById(Guid.NewGuid().ToString()), Is.Null);
    }

    [Test]
    public void Try_GetElementByFIO_WhenHaveRecord_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_workerStorageContract.GetElementByFIO(worker.FIO), worker);
    }

    [Test]
    public void Try_GetElementByFIO_WhenNoRecord_Test()
    {
        Assert.That(() => _workerStorageContract.GetElementByFIO("New Fio"), Is.Null);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var worker = CreateModel(Guid.NewGuid().ToString());
        _workerStorageContract.AddElement(worker);
        AssertElement(GetWorkerFromDatabase(worker.Id), worker);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var worker = CreateModel(Guid.NewGuid().ToString());
        InsertWorkerToDatabaseAndReturn(worker.Id);
        Assert.That(() => _workerStorageContract.AddElement(worker), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var worker = CreateModel(Guid.NewGuid().ToString(), "New Fio");
        InsertWorkerToDatabaseAndReturn(worker.Id);
        _workerStorageContract.UpdElement(worker);
        AssertElement(GetWorkerFromDatabase(worker.Id), worker);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _workerStorageContract.UpdElement(CreateModel(Guid.NewGuid().ToString())), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWasDeleted_Test()
    {
        var worker = CreateModel(Guid.NewGuid().ToString());
        InsertWorkerToDatabaseAndReturn(worker.Id, isDeleted: true);
        Assert.That(() => _workerStorageContract.UpdElement(worker), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString());
        _workerStorageContract.DelElement(worker.Id);
        var element = GetWorkerFromDatabase(worker.Id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.IsDeleted);
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _workerStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWasDeleted_Test()
    {
        var worker = CreateModel(Guid.NewGuid().ToString());
        InsertWorkerToDatabaseAndReturn(worker.Id, isDeleted: true);
        Assert.That(() => _workerStorageContract.DelElement(worker.Id), Throws.TypeOf<ElementNotFoundException>());
    }

    private Worker InsertWorkerToDatabaseAndReturn(string id, string fio = "test", string? postId = null, DateTime? birthDate = null, DateTime? employmentDate = null, bool isDeleted = false)
    {
        var worker = new Worker() { Id = id, FIO = fio, PostId = postId ?? Guid.NewGuid().ToString(), BirthDate = birthDate ?? DateTime.UtcNow.AddYears(-20), EmploymentDate = employmentDate ?? DateTime.UtcNow, IsDeleted = isDeleted };
        TheBallDbContext.Workers.Add(worker);
        TheBallDbContext.SaveChanges();
        return worker;
    }

    private static void AssertElement(WorkerDataModel? actual, Worker expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.PostId, Is.EqualTo(expected.PostId));
            Assert.That(actual.FIO, Is.EqualTo(expected.FIO));
            Assert.That(actual.BirthDate, Is.EqualTo(expected.BirthDate));
            Assert.That(actual.EmploymentDate, Is.EqualTo(expected.EmploymentDate));
            Assert.That(actual.IsDeleted, Is.EqualTo(expected.IsDeleted));
        });
    }

    private static WorkerDataModel CreateModel(string id, string fio = "fio", string? postId = null, DateTime? birthDate = null, DateTime? employmentDate = null, bool isDeleted = false) =>
        new(id, fio, postId ?? Guid.NewGuid().ToString(), birthDate ?? DateTime.UtcNow.AddYears(-20), employmentDate ?? DateTime.UtcNow, isDeleted);

    private Worker? GetWorkerFromDatabase(string id) => TheBallDbContext.Workers.FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Worker? actual, WorkerDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.PostId, Is.EqualTo(expected.PostId));
            Assert.That(actual.FIO, Is.EqualTo(expected.FIO));
            Assert.That(actual.BirthDate, Is.EqualTo(expected.BirthDate));
            Assert.That(actual.EmploymentDate, Is.EqualTo(expected.EmploymentDate));
            Assert.That(actual.IsDeleted, Is.EqualTo(expected.IsDeleted));
        });
    }
}
