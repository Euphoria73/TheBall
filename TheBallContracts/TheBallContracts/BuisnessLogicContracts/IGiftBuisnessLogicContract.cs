

using TheBallContracts.DataModels;

namespace TheBallContracts.BuisnessLogicContracts;

public interface IGiftBuisnessLogicContract
{
    List<GiftDataModel> GetAllGifts(bool onlyActive = true);

    List<GiftDataModel> GetAllGiftsByManufacturer(string manufacturerId, bool onlyActive = true);

    List<GiftHistoryDataModel> GetGiftHistoryByProduct(string productId);

    GiftDataModel GetGiftByData(string data);

    void InsertGift(GiftDataModel giftDataModel);

    void UpdateGift(GiftDataModel giftDataModel);

    void DeleteGift(string id);
}
