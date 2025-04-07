
using TheBallContracts.DataModels;

namespace TheBallContracts.StoragesContracts;

public interface ISalaryStorageContract
{
    List<SalaryDataModel> GetList(DateTime startDate, DateTime endDate, string? workerId = null);

    void AddElement(SalaryDataModel salaryDataModel);
}
