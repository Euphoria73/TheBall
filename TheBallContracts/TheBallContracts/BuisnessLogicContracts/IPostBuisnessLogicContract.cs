
using TheBallContracts.DataModels;

namespace TheBallContracts.BuisnessLogicContracts;

public interface IPostBuisnessLogicContract
{
    List<PostDataModel> GetAllPosts(bool onlyActive);

    List<PostDataModel> GetAllDataOfPost(string postId);

    PostDataModel GetPostByData(string data);

    void InsertPost(PostDataModel postDataModel);

    void UpdatePost(PostDataModel postDataModel);

    void DeletePost(string id);

    void RestorePost(string id);
}
