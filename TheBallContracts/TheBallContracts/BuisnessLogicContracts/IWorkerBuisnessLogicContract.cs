
using TheBallContracts.DataModels;

namespace TheBallContracts.BuisnessLogicContracts;

public interface IWorkerBuisnessLogicContract
{
    List<WorkerDataModel> GetAllWorkers(bool onlyActive = true);

    List<WorkerDataModel> GetAllWorkersByPost(string postId, bool onlyActive = true);

    List<WorkerDataModel> GetAllWorkersByBirthDate(DateTime fromDate, DateTime toDate, bool onlyActive = true);

    List<WorkerDataModel> GetAllWorkersByEmploymentDate(DateTime fromDate, DateTime toDate, bool onlyActive = true);

    WorkerDataModel GetWorkerByData(string data);

    void InsertWorker(WorkerDataModel workerDataModel);

    void UpdateWorker(WorkerDataModel workerDataModel);

    void DeleteWorker(string id);
}
