
using TheBallContracts.DataModels;

namespace TheBallContracts.BuisnessLogicContracts;

public interface ISaleBuisnessLogicContract
{
    List<SaleDataModel> GetAllSalesByPeriod(DateTime fromDate, DateTime toDate);

    List<SaleDataModel> GetAllSalesByWorkerByPeriod(string workerId, DateTime fromDate, DateTime toDate);

    List<SaleDataModel> GetAllSalesByBuyerByPeriod(string buyerId, DateTime fromDate, DateTime toDate);

    List<SaleDataModel> GetAllSalesByGiftByPeriod(string giftId, DateTime fromDate, DateTime toDate);

    SaleDataModel GetSaleByData(string data);

    void InsertSale(SaleDataModel saleDataModel);

    void CancelSale(string id);
}
