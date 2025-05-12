using TheBallContracts.DataModels;

namespace TheBallContracts.StoragesContracts;

public interface ISaleStorageContract
{
    List<SaleDataModel> GetList(DateTime? startDate = null, DateTime? endDate = null, string? workerId = null, string? buyerId = null, string? giftId = null);

    SaleDataModel? GetElementById(string id);

    void AddElement(SaleDataModel saleDataModel);

    void DelElement(string id);
}
