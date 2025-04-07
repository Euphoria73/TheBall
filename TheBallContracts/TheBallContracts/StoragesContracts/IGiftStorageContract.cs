
using TheBallContracts.DataModels;

namespace TheBallContracts.StoragesContracts;

public interface IGiftStorageContract
{
    List<GiftDataModel> GetList(bool onlyActive = true, string? manufacturerId = null);

    List<GiftHistoryDataModel> GetHistoryByGiftId(string giftId);

    GiftDataModel? GetElementById(string id);

    GiftDataModel? GetElementByName(string name);

    void AddElement(GiftDataModel giftDataModel);

    void UpdElement(GiftDataModel giftDataModel);

    void DelElement(string id);
}
