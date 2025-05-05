

using Microsoft.EntityFrameworkCore;
using TheBallContracts.DataModels;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallDatabase;
using TheBallDatabase.Implementations;
using TheBallDatabase.Models;

namespace TheBallTests.StoragesContracts;

[TestFixture]
internal class PostStorageContractTests : BaseStorageContractTest
{
    private PostStorageContract _postStorageContract;

    [SetUp]
    public void SetUp()
    {
        _postStorageContract = new PostStorageContract(TheBallDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        TheBallDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Posts\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1");
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 2");
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 3");
        var list = _postStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == post.PostId), post);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _postStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetList_OnlyActual_Test()
    {
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1", isActual: true);
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 2", isActual: true);
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 3", isActual: false);
        var list = _postStorageContract.GetList(onlyActual: true);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(!list.Any(x => !x.IsActual));
        });
    }

    [Test]
    public void Try_GetList_IncludeNoActual_Test()
    {
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1", isActual: true);
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 2", isActual: true);
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 3", isActual: false);
        var list = _postStorageContract.GetList(onlyActual: false);
        Assert.That(list, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(3));
            Assert.That(list.Count(x => x.IsActual), Is.EqualTo(2));
            Assert.That(list.Count(x => !x.IsActual), Is.EqualTo(1));
        });
    }

    [Test]
    public void Try_GetPostWithHistory_WhenHaveRecords_Test()
    {
        var postId = Guid.NewGuid().ToString();
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1", isActual: true);
        InsertPostToDatabaseAndReturn(postId, "name 2", isActual: true);
        InsertPostToDatabaseAndReturn(postId, "name 2", isActual: false);
        var list = _postStorageContract.GetPostWithHistory(postId);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
    }

    [Test]
    public void Try_GetPostWithHistory_WhenNoRecords_Test()
    {
        var postId = Guid.NewGuid().ToString();
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1", isActual: true);
        InsertPostToDatabaseAndReturn(postId, "name 2", isActual: true);
        InsertPostToDatabaseAndReturn(postId, "name 2", isActual: false);
        var list = _postStorageContract.GetPostWithHistory(Guid.NewGuid().ToString());
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_postStorageContract.GetElementById(post.PostId), post);
    }

    [Test]
    public void Try_GetElementById_WhenNoRecord_Test()
    {
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _postStorageContract.GetElementById(Guid.NewGuid().ToString()), Is.Null);
    }

    [Test]
    public void Try_GetElementById_WhenRecordHasDeleted_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), isActual: false);
        Assert.That(() => _postStorageContract.GetElementById(post.PostId), Is.Null);
    }

    [Test]
    public void Try_GetElementById_WhenTrySearchById_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _postStorageContract.GetElementById(post.Id), Is.Null);
    }

    [Test]
    public void Try_GetElementByName_WhenHaveRecord_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_postStorageContract.GetElementByName(post.PostName), post);
    }

    [Test]
    public void Try_GetElementByName_WhenNoRecord_Test()
    {
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString());
        Assert.That(() => _postStorageContract.GetElementByName("name"), Is.Null);
    }

    [Test]
    public void Try_GetElementByName_WhenRecordHasDeleted_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), isActual: false);
        Assert.That(() => _postStorageContract.GetElementById(post.PostName), Is.Null);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString(), isActual: true);
        _postStorageContract.AddElement(post);
        AssertElement(GetPostFromDatabaseByPostId(post.Id), post);
    }

    [Test]
    public void Try_AddElement_WhenActualIsFalse_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString(), isActual: false);
        Assert.That(() => _postStorageContract.AddElement(post), Throws.Nothing);
        AssertElement(GetPostFromDatabaseByPostId(post.Id), CreateModel(post.Id, isActual: true));
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameName_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString(), "name unique", isActual: true);
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), postName: post.PostName, isActual: true);
        Assert.That(() => _postStorageContract.AddElement(post), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSamePostIdAndActualIsTrue_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString(), isActual: true);
        InsertPostToDatabaseAndReturn(post.Id, isActual: true);
        Assert.That(() => _postStorageContract.AddElement(post), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString());
        InsertPostToDatabaseAndReturn(post.Id, isActual: true);
        _postStorageContract.UpdElement(post);
        var posts = TheBallDbContext.Posts.Where(x => x.PostId == post.Id).OrderByDescending(x => x.ChangeDate);
        Assert.That(posts.Count(), Is.EqualTo(2));
        AssertElement(posts.First(), CreateModel(post.Id, isActual: true));
        AssertElement(posts.Last(), CreateModel(post.Id, isActual: false));
    }

    [Test]
    public void Try_UpdElement_WhenActualIsFalse_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString(), isActual: false);
        InsertPostToDatabaseAndReturn(post.Id, isActual: true);
        _postStorageContract.UpdElement(post);
        AssertElement(GetPostFromDatabaseByPostId(post.Id), CreateModel(post.Id, isActual: true));
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _postStorageContract.UpdElement(CreateModel(Guid.NewGuid().ToString())), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_UpdElement_WhenHaveRecordWithSameName_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString(), "New Name");
        InsertPostToDatabaseAndReturn(post.Id, postName: "name");
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), postName: post.PostName);
        Assert.That(() => _postStorageContract.UpdElement(post), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_UpdElement_WhenRecordWasDeleted_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString());
        InsertPostToDatabaseAndReturn(post.Id, isActual: false);
        Assert.That(() => _postStorageContract.UpdElement(post), Throws.TypeOf<ElementDeletedException>());
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), isActual: true);
        _postStorageContract.DelElement(post.PostId);
        var element = GetPostFromDatabaseByPostId(post.PostId);
        Assert.Multiple(() =>
        {
            Assert.That(element, Is.Not.Null);
            Assert.That(!element!.IsActual);
        });
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _postStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_DelElement_WhenRecordWasDeleted_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), isActual: false);
        Assert.That(() => _postStorageContract.DelElement(post.PostId), Throws.TypeOf<ElementDeletedException>());
    }

    [Test]
    public void Try_ResElement_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), isActual: false);
        _postStorageContract.ResElement(post.PostId);
        var element = GetPostFromDatabaseByPostId(post.PostId);
        Assert.Multiple(() =>
        {
            Assert.That(element, Is.Not.Null);
            Assert.That(element!.IsActual);
        });
    }

    [Test]
    public void Try_ResElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _postStorageContract.ResElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    [Test]
    public void Try_ResElement_WhenRecordNotWasDeleted_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), isActual: true);
        Assert.That(() => _postStorageContract.ResElement(post.PostId), Throws.Nothing);
    }

    private Post InsertPostToDatabaseAndReturn(string id, string postName = "test", PostType postType = PostType.Supervisor, double salary = 10, bool isActual = true, DateTime? changeDate = null)
    {
        var post = new Post() { Id = Guid.NewGuid().ToString(), PostId = id, PostName = postName, PostType = postType, Salary = salary, IsActual = isActual, ChangeDate = changeDate ?? DateTime.UtcNow };
        TheBallDbContext.Posts.Add(post);
        TheBallDbContext.SaveChanges();
        return post;
    }

    private static void AssertElement(PostDataModel? actual, Post expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.PostId));
            Assert.That(actual.PostName, Is.EqualTo(expected.PostName));
            Assert.That(actual.PostType, Is.EqualTo(expected.PostType));
            Assert.That(actual.Salary, Is.EqualTo(expected.Salary));
            Assert.That(actual.IsActual, Is.EqualTo(expected.IsActual));
        });
    }

    private static PostDataModel CreateModel(string postId, string postName = "test", PostType postType = PostType.Supervisor, double salary = 10, bool isActual = false, DateTime? changeDate = null)
        => new(postId, postName, postType, salary, isActual, changeDate ?? DateTime.UtcNow);

    private Post? GetPostFromDatabaseByPostId(string id) => TheBallDbContext.Posts.Where(x => x.PostId == id).OrderByDescending(x => x.ChangeDate).FirstOrDefault();

    private static void AssertElement(Post? actual, PostDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.PostId, Is.EqualTo(expected.Id));
            Assert.That(actual.PostName, Is.EqualTo(expected.PostName));
            Assert.That(actual.PostType, Is.EqualTo(expected.PostType));
            Assert.That(actual.Salary, Is.EqualTo(expected.Salary));
            Assert.That(actual.IsActual, Is.EqualTo(expected.IsActual));
        });
    }
}
