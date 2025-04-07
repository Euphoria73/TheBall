

using TheBallContracts.DataModels;

namespace TheBallContracts.BuisnessLogicContracts;

public interface ISalaryBuisnessLogicContract
{
    List<SalaryDataModel> GetAllSalariesByPeriod(DateTime fromDate, DateTime toDate);

    List<SalaryDataModel> GetAllSalariesByPeriodByWorker(DateTime fromDate, DateTime toDate, string workerId);

    void CalculateSalaryByMounth(DateTime date);
}
