
using TheBallContracts.DataModels;

namespace TheBallContracts.BuisnessLogicContracts;

public interface IBuyerBuisnessLogicContract
{
    List<BuyerDataModel> GetAllBuyers();

    BuyerDataModel GetBuyerByData(string data);

    void InsertBuyer(BuyerDataModel buyerDataModel);

    void UpdateBuyer(BuyerDataModel buyerDataModel);

    void DeleteBuyer(string id);
}
