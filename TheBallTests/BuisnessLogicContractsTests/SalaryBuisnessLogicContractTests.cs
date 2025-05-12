using TheBallBuisnessLogic.Implementations;
using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TheBallTests.BuisnessLogicContractsTests;

[TestFixture]
internal class SalaryBuisnessLogicContractTests
{
    private SalaryBuisnessLogicContract _salaryBusinessLogicContract;
    private Mock<ISalaryStorageContract> _salaryStorageContract;
    private Mock<ISaleStorageContract> _saleStorageContract;
    private Mock<IPostStorageContract> _postStorageContract;
    private Mock<IWorkerStorageContract> _workerStorageContract;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _salaryStorageContract = new Mock<ISalaryStorageContract>();
        _saleStorageContract = new Mock<ISaleStorageContract>();
        _postStorageContract = new Mock<IPostStorageContract>();
        _workerStorageContract = new Mock<IWorkerStorageContract>();
        _salaryBusinessLogicContract = new SalaryBuisnessLogicContract(_salaryStorageContract.Object,
            _saleStorageContract.Object, _postStorageContract.Object, _workerStorageContract.Object, new Mock<ILogger>().Object);
    }

    [SetUp]
    public void SetUp()
    {
        _salaryStorageContract.Reset();
        _saleStorageContract.Reset();
        _postStorageContract.Reset();
        _workerStorageContract.Reset();
    }

    [Test]
    public void GetAllSalaries_ReturnListOfRecords_Test()
    {
        //Arrange
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(1);
        var listOriginal = new List<SalaryDataModel>()
        {
            new(Guid.NewGuid().ToString(), DateTime.UtcNow, 10),
            new(Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(1), 14),
            new(Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(-1), 30),
        };
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(listOriginal);
        //Act
        var list = _salaryBusinessLogicContract.GetAllSalariesByPeriod(startDate, endDate);
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _salaryStorageContract.Verify(x => x.GetList(startDate, endDate, null), Times.Once);
    }

    [Test]
    public void GetAllSalaries_ReturnEmptyList_Test()
    {
        //Arrange
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns([]);
        //Act
        var list = _salaryBusinessLogicContract.GetAllSalariesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalaries_IncorrectDates_ThrowException_Test()
    {
        //Arrange
        var dateTime = DateTime.UtcNow;
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(dateTime, dateTime), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(dateTime, dateTime.AddSeconds(-1)), Throws.TypeOf<IncorrectDatesException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalaries_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<NullListException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalaries_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<StorageException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalariesByWorker_ReturnListOfRecords_Test()
    {
        //Arrange
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(1);
        var workerId = Guid.NewGuid().ToString();
        var listOriginal = new List<SalaryDataModel>()
        {
            new(Guid.NewGuid().ToString(), DateTime.UtcNow, 10),
            new(Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(1), 14),
            new(Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(-1), 30),
        };
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(listOriginal);
        //Act
        var list = _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(startDate, endDate, workerId);
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _salaryStorageContract.Verify(x => x.GetList(startDate, endDate, workerId), Times.Once);
    }

    [Test]
    public void GetAllSalariesByWorker_ReturnEmptyList_Test()
    {
        //Arrange
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns([]);
        //Act
        var list = _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid().ToString());
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalariesByWorker_IncorrectDates_ThrowException_Test()
    {
        //Arrange
        var dateTime = DateTime.UtcNow;
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(dateTime, dateTime, Guid.NewGuid().ToString()), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(dateTime, dateTime.AddSeconds(-1), Guid.NewGuid().ToString()), Throws.TypeOf<IncorrectDatesException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalariesByWorker_WorkerIdIsNUllOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), string.Empty), Throws.TypeOf<ArgumentNullException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalariesByWorker_WorkerIdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), "workerId"), Throws.TypeOf<ValidationException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalariesByWorker_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid().ToString()), Throws.TypeOf<NullListException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalariesByWorker_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void CalculateSalaryByMounth_CalculateSalary_Test()
    {
        //Arrange
        var workerId = Guid.NewGuid().ToString();
        var saleSum = 200.0;
        var postSalary = 2000.0;
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, saleSum, DiscountType.None, 0, false, [])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, postSalary, true, DateTime.UtcNow));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false)]);
        var sum = 0.0;
        var expectedSum = postSalary + saleSum * 0.1;
        _salaryStorageContract.Setup(x => x.AddElement(It.IsAny<SalaryDataModel>()))
            .Callback((SalaryDataModel x) =>
            {
                sum = x.Salary;
            });
        //Act
        _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow);
        //Assert
        Assert.That(sum, Is.EqualTo(expectedSum));
    }

    [Test]
    public void CalculateSalaryByMounth_WithSeveralWorkers_Test()
    {
        //Arrange
        var worker1Id = Guid.NewGuid().ToString();
        var worker2Id = Guid.NewGuid().ToString();
        var worker3Id = Guid.NewGuid().ToString();
        var list = new List<WorkerDataModel>() {
            new(worker1Id, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false),
            new(worker2Id, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false),
            new(worker3Id, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false)
        };
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), worker1Id, null, 1, DiscountType.None, 0, false, []),
            new SaleDataModel(Guid.NewGuid().ToString(), worker1Id, null, 1, DiscountType.None, 0, false, []),
            new SaleDataModel(Guid.NewGuid().ToString(), worker2Id, null, 1, DiscountType.None, 0, false, []),
            new SaleDataModel(Guid.NewGuid().ToString(), worker3Id, null, 1, DiscountType.None, 0, false, []),
            new SaleDataModel(Guid.NewGuid().ToString(), worker3Id, null, 1, DiscountType.None, 0, false, [])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000, true, DateTime.UtcNow));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns(list);
        //Act
        _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow);
        //Assert
        _salaryStorageContract.Verify(x => x.AddElement(It.IsAny<SalaryDataModel>()), Times.Exactly(list.Count));
    }

    [Test]
    public void CalculateSalaryByMounth_WithoitSalesByWorker_Test()
    {
        //Arrange
        var postSalary = 2000.0;
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, postSalary, true, DateTime.UtcNow));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false)]);
        var sum = 0.0;
        var expectedSum = postSalary;
        _salaryStorageContract.Setup(x => x.AddElement(It.IsAny<SalaryDataModel>()))
            .Callback((SalaryDataModel x) =>
            {
                sum = x.Salary;
            });
        //Act
        _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow);
        //Assert
        Assert.That(sum, Is.EqualTo(expectedSum));
    }

    [Test]
    public void CalculateSalaryByMounth_SaleStorageReturnNull_ThrowException_Test()
    {
        //Arrange
        var workerId = Guid.NewGuid().ToString();
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000, true, DateTime.UtcNow));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false)]);
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<NullListException>());
    }

    [Test]
    public void CalculateSalaryByMounth_PostStorageReturnNull_ThrowException_Test()
    {
        //Arrange
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, 200, DiscountType.None, 0, false, [])]);
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false)]);
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<NullListException>());
    }

    [Test]
    public void CalculateSalaryByMounth_WorkerStorageReturnNull_ThrowException_Test()
    {
        //Arrange
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, 200, DiscountType.None, 0, false, [])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000, true, DateTime.UtcNow));
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<NullListException>());
    }

    [Test]
    public void CalculateSalaryByMounth_SaleStorageThrowException_ThrowException_Test()
    {
        //Arrange
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new StorageException(new InvalidOperationException()));
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000, true, DateTime.UtcNow));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false)]);
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<StorageException>());
    }

    [Test]
    public void CalculateSalaryByMounth_PostStorageThrowException_ThrowException_Test()
    {
        //Arrange
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, 200, DiscountType.None, 0, false, [])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Throws(new StorageException(new InvalidOperationException()));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow, false)]);
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<StorageException>());
    }

    [Test]
    public void CalculateSalaryByMounth_WorkerStorageThrowException_ThrowException_Test()
    {
        //Arrange
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, 200, DiscountType.None, 0, false, [])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>()))
            .Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000, true, DateTime.UtcNow));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<StorageException>());
    }
}
