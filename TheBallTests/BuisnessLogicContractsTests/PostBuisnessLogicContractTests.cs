using TheBallBuisnessLogic.Implementations;
using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace CatHasPawsTests.BusinessLogicsContractsTests;

[TestFixture]
internal class PostBusinessLogicContractTests
{
    private PostBuisnessLogicContract _postBusinessLogicContract;
    private Mock<IPostStorageContract> _postStorageContract;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _postStorageContract = new Mock<IPostStorageContract>();
        _postBusinessLogicContract = new PostBuisnessLogicContract(_postStorageContract.Object, new Mock<ILogger>().Object);
    }

    [SetUp]
    public void SetUp()
    {
        _postStorageContract.Reset();
    }

    [Test]
    public void GetAllPosts_ReturnListOfRecords_Test()
    {
        //Arrange
        var listOriginal = new List<PostDataModel>()
        {
            new(Guid.NewGuid().ToString(),"name 1", PostType.Assistant, 10, true, DateTime.UtcNow),
            new(Guid.NewGuid().ToString(), "name 2", PostType.Assistant, 10, false, DateTime.UtcNow),
            new(Guid.NewGuid().ToString(), "name 3", PostType.Assistant, 10, true, DateTime.UtcNow),
        };
        _postStorageContract.Setup(x => x.GetList(It.IsAny<bool>())).Returns(listOriginal);
        //Act
        var listOnlyActive = _postBusinessLogicContract.GetAllPosts(true);
        var listAll = _postBusinessLogicContract.GetAllPosts(false);
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(listAll, Is.Not.Null);
            Assert.That(listOnlyActive, Is.EquivalentTo(listOriginal));
            Assert.That(listAll, Is.EquivalentTo(listOriginal));
        });
        _postStorageContract.Verify(x => x.GetList(true), Times.Once);
        _postStorageContract.Verify(x => x.GetList(false), Times.Once);
    }

    [Test]
    public void GetAllPosts_ReturnEmptyList_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.GetList(It.IsAny<bool>())).Returns([]);
        //Act
        var listOnlyActive = _postBusinessLogicContract.GetAllPosts(true);
        var listAll = _postBusinessLogicContract.GetAllPosts(false);
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(listAll, Is.Not.Null);
            Assert.That(listOnlyActive, Has.Count.EqualTo(0));
            Assert.That(listAll, Has.Count.EqualTo(0));
        });
        _postStorageContract.Verify(x => x.GetList(It.IsAny<bool>()), Times.Exactly(2));
    }

    [Test]
    public void GetAllPosts_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetAllPosts(It.IsAny<bool>()), Throws.TypeOf<NullListException>());
        _postStorageContract.Verify(x => x.GetList(It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public void GetAllPosts_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.GetList(It.IsAny<bool>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetAllPosts(It.IsAny<bool>()), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.GetList(It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_ReturnListOfRecords_Test()
    {
        //Arrange
        var postId = Guid.NewGuid().ToString();
        var listOriginal = new List<PostDataModel>()
        {
            new(postId, "name 1", PostType.Assistant, 10, true, DateTime.UtcNow),
            new(postId, "name 2", PostType.Assistant, 10, false, DateTime.UtcNow)
        };
        _postStorageContract.Setup(x => x.GetPostWithHistory(It.IsAny<string>())).Returns(listOriginal);
        //Act
        var list = _postBusinessLogicContract.GetAllDataOfPost(postId);
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
        _postStorageContract.Verify(x => x.GetPostWithHistory(postId), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_ReturnEmptyList_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.GetPostWithHistory(It.IsAny<string>())).Returns([]);
        //Act
        var list = _postBusinessLogicContract.GetAllDataOfPost(Guid.NewGuid().ToString());
        //Assert
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_PostIdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllDataOfPost_PostIdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost("id"), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllDataOfPost_ReturnNull_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(Guid.NewGuid().ToString()), Throws.TypeOf<NullListException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.GetPostWithHistory(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_GetById_ReturnRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var record = new PostDataModel(id, "name", PostType.Assistant, 10, true, DateTime.UtcNow);
        _postStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        //Act
        var element = _postBusinessLogicContract.GetPostByData(id);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_GetByName_ReturnRecord_Test()
    {
        //Arrange
        var postName = "name";
        var record = new PostDataModel(Guid.NewGuid().ToString(), postName, PostType.Assistant, 10, true, DateTime.UtcNow);
        _postStorageContract.Setup(x => x.GetElementByName(postName)).Returns(record);
        //Act
        var element = _postBusinessLogicContract.GetPostByData(postName);
        //Assert
        Assert.That(element, Is.Not.Null);
        Assert.That(element.PostName, Is.EqualTo(postName));
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_EmptyData_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetPostByData(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.GetPostByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetPostByData_GetById_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetPostByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetPostByData_GetByName_NotFoundRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetPostByData("name"), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _postStorageContract.Setup(x => x.GetElementByName(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.GetPostByData(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        Assert.That(() => _postBusinessLogicContract.GetPostByData("name"), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertPost_CorrectRecord_Test()
    {
        //Arrange
        var flag = false;
        var record = new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10, true, DateTime.UtcNow.AddDays(-1));
        _postStorageContract.Setup(x => x.AddElement(It.IsAny<PostDataModel>()))
            .Callback((PostDataModel x) =>
            {
                flag = x.Id == record.Id && x.PostName == record.PostName && x.PostType == record.PostType && x.Salary == record.Salary &&
                    x.ChangeDate == record.ChangeDate;
            });
        //Act
        _postBusinessLogicContract.InsertPost(record);
        //Assert
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertPost_RecordWithExistsData_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.AddElement(It.IsAny<PostDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.InsertPost(new(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10, true, DateTime.UtcNow)), Throws.TypeOf<ElementExistsException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void InsertPost_NullRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.InsertPost(null), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void InsertPost_InvalidRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.InsertPost(new PostDataModel("id", "name", PostType.Supervisor, 10, true, DateTime.UtcNow)), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void InsertPost_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.AddElement(It.IsAny<PostDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.InsertPost(new(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10, true, DateTime.UtcNow)), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void UpdatePost_CorrectRecord_Test()
    {
        //Arrange
        var flag = false;
        var record = new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10, true, DateTime.UtcNow.AddDays(-1));
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>()))
            .Callback((PostDataModel x) =>
            {
                flag = x.Id == record.Id && x.PostName == record.PostName && x.PostType == record.PostType && x.Salary == record.Salary &&
                    x.ChangeDate == record.ChangeDate;
            });
        //Act
        _postBusinessLogicContract.UpdatePost(record);
        //Assert
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void UpdatePost_RecordWithIncorrectData_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>())).Throws(new ElementNotFoundException(""));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10, true, DateTime.UtcNow)), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void UpdatePost_RecordWithExistsData_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new(Guid.NewGuid().ToString(), "anme", PostType.Supervisor, 10, true, DateTime.UtcNow)), Throws.TypeOf<ElementExistsException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void UpdatePost_NullRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.UpdatePost(null), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void UpdatePost_InvalidRecord_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new PostDataModel("id", "name", PostType.Supervisor, 10, true, DateTime.UtcNow)), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void UpdatePost_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10, true, DateTime.UtcNow)), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void DeletePost_CorrectRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _postStorageContract.Setup(x => x.DelElement(It.Is((string x) => x == id))).Callback(() => { flag = true; });
        //Act
        _postBusinessLogicContract.DeletePost(id);
        //Assert
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void DeletePost_RecordWithIncorrectId_ThrowException_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        _postStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(id));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.DeletePost(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void DeletePost_IdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.DeletePost(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.DeletePost(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeletePost_IdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.DeletePost("id"), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeletePost_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.DeletePost(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void RestorePost_CorrectRecord_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _postStorageContract.Setup(x => x.ResElement(It.Is((string x) => x == id))).Callback(() => { flag = true; });
        //Act
        _postBusinessLogicContract.RestorePost(id);
        //Assert
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void RestorePost_RecordWithIncorrectId_ThrowException_Test()
    {
        //Arrange
        var id = Guid.NewGuid().ToString();
        _postStorageContract.Setup(x => x.ResElement(It.IsAny<string>())).Throws(new ElementNotFoundException(id));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.RestorePost(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void RestorePost_IdIsNullOrEmpty_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.RestorePost(null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.RestorePost(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void RestorePost_IdIsNotGuid_ThrowException_Test()
    {
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.RestorePost("id"), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void RestorePost_StorageThrowError_ThrowException_Test()
    {
        //Arrange
        _postStorageContract.Setup(x => x.ResElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        //Act&Assert
        Assert.That(() => _postBusinessLogicContract.RestorePost(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Once);
    }
}