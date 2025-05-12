
using Microsoft.EntityFrameworkCore;
using TheBallContracts.DataModels;
using TheBallDatabase.Implementations;
using TheBallDatabase.Models;

namespace TheBallTests.StoragesContracts;

[TestFixture]
internal class SalaryStorageContractTests : BaseStorageContractTest
{
    private SalaryStorageContract _salaryStorageContract;
    private Worker _worker;

    [SetUp]
    public void SetUp()
    {
        _salaryStorageContract = new SalaryStorageContract(TheBallDbContext);
        _worker = InsertWorkerToDatabaseAndReturn();
    }

    [TearDown]
    public void TearDown()
    {
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Salaries\" CASCADE;");
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Workers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var salary = InsertSalaryToDatabaseAndReturn(_worker.Id, workerSalary: 100);
        InsertSalaryToDatabaseAndReturn(_worker.Id);
        InsertSalaryToDatabaseAndReturn(_worker.Id);
        var list = _salaryStorageContract.GetList(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(), salary);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _salaryStorageContract.GetList(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetList_OnlyInDatePeriod_Test()
    {
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(-2));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(-1).AddMinutes(-5));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(-1).AddMinutes(5));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(1).AddMinutes(-5));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(1).AddMinutes(5));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(-2));
        var list = _salaryStorageContract.GetList(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void Try_GetList_ByWorker_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn("name 2");
        InsertSalaryToDatabaseAndReturn(_worker.Id);
        InsertSalaryToDatabaseAndReturn(_worker.Id);
        InsertSalaryToDatabaseAndReturn(worker.Id);
        var list = _salaryStorageContract.GetList(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), _worker.Id);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list.All(x => x.WorkerId == _worker.Id));
        });
    }

    [Test]
    public void Try_GetList_ByWorkerOnlyInDatePeriod_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn("name 2");
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(-2));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(-1).AddMinutes(5));
        InsertSalaryToDatabaseAndReturn(worker.Id, salaryDate: DateTime.UtcNow.AddDays(-1).AddMinutes(5));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(1).AddMinutes(-5));
        InsertSalaryToDatabaseAndReturn(worker.Id, salaryDate: DateTime.UtcNow.AddDays(1).AddMinutes(-5));
        InsertSalaryToDatabaseAndReturn(_worker.Id, salaryDate: DateTime.UtcNow.AddDays(-2));
        var list = _salaryStorageContract.GetList(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), _worker.Id);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list.All(x => x.WorkerId == _worker.Id));
        });
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var salary = CreateModel(_worker.Id);
        _salaryStorageContract.AddElement(salary);
        AssertElement(GetSalaryFromDatabaseByWorkerId(_worker.Id), salary);
    }

    private Worker InsertWorkerToDatabaseAndReturn(string workerFIO = "fio")
    {
        var worker = new Worker() { Id = Guid.NewGuid().ToString(), PostId = Guid.NewGuid().ToString(), FIO = workerFIO, IsDeleted = false };
        TheBallDbContext.Workers.Add(worker);
        TheBallDbContext.SaveChanges();
        return worker;
    }

    private Salary InsertSalaryToDatabaseAndReturn(string workerId, double workerSalary = 1, DateTime? salaryDate = null)
    {
        var salary = new Salary() { WorkerId = workerId, WorkerSalary = workerSalary, SalaryDate = salaryDate ?? DateTime.UtcNow };
        TheBallDbContext.Salaries.Add(salary);
        TheBallDbContext.SaveChanges();
        return salary;
    }

    private static void AssertElement(SalaryDataModel? actual, Salary expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.WorkerId, Is.EqualTo(expected.WorkerId));
            Assert.That(actual.Salary, Is.EqualTo(expected.WorkerSalary));
        });
    }

    private static SalaryDataModel CreateModel(string workerId, double workerSalary = 1, DateTime? salaryDate = null)
        => new(workerId, salaryDate ?? DateTime.UtcNow, workerSalary);

    private Salary? GetSalaryFromDatabaseByWorkerId(string id) => TheBallDbContext.Salaries.FirstOrDefault(x => x.WorkerId == id);

    private static void AssertElement(Salary? actual, SalaryDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.WorkerId, Is.EqualTo(expected.WorkerId));
            Assert.That(actual.WorkerSalary, Is.EqualTo(expected.Salary));
        });
    }
}
