using TheBallContracts.DataModels;

namespace TheBallContracts.StoragesContracts;

public interface IPostStorageContract
{
    List<PostDataModel> GetList(bool onlyActual = true);
    List<PostDataModel> GetPostWithHistory(string postId);
    PostDataModel? GetElementById(string id);
    PostDataModel? GetElementByName(string name);

    void AddElement(PostDataModel postDataModel);

    void UpdElement(PostDataModel postDataModel);

    void DelElement(string id);

    void ResElement(string id);
}
