using TheBallContracts.BuisnessLogicContracts;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TheBallContracts.StoragesContracts;

namespace TheBallBuisnessLogic.Implementations;

internal class WorkerBuisnessLogicContract(IWorkerStorageContract workerStorageContract, ILogger logger) : IWorkerBuisnessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IWorkerStorageContract _workerStorageContract = workerStorageContract;

    public List<WorkerDataModel> GetAllWorkers(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {onlyActive}", onlyActive);
        return _workerStorageContract.GetList(onlyActive) ?? throw new NullListException();
    }

    public List<WorkerDataModel> GetAllWorkersByPost(string postId, bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {postId}, {onlyActive},", postId, onlyActive);
        if (postId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(postId));
        }
        if (!postId.IsGuid())
        {
            throw new ValidationException("The value in the field postId is not a unique.");
        }
        return _workerStorageContract.GetList(onlyActive, postId) ?? throw new NullListException();
    }

    public List<WorkerDataModel> GetAllWorkersByBirthDate(DateTime fromDate, DateTime toDate, bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {onlyActive}, {fromDate}, {toDate}", onlyActive, fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
        {
            throw new IncorrectDatesException(fromDate, toDate);
        }
        return _workerStorageContract.GetList(onlyActive, fromBirthDate: fromDate, toBirthDate: toDate) ?? throw new NullListException();
    }

    public List<WorkerDataModel> GetAllWorkersByEmploymentDate(DateTime fromDate, DateTime toDate, bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {onlyActive}, {fromDate}, {toDate}", onlyActive, fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
        {
            throw new IncorrectDatesException(fromDate, toDate);
        }
        return _workerStorageContract.GetList(onlyActive, fromEmploymentDate: fromDate, toEmploymentDate: toDate) ?? throw new NullListException();
    }

    public WorkerDataModel GetWorkerByData(string data)
    {
        _logger.LogInformation("Get element by data: {data}", data);
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _workerStorageContract.GetElementById(data) ?? throw new ElementNotFoundException(data);
        }
        return _workerStorageContract.GetElementByFIO(data) ?? throw new ElementNotFoundException(data);
    }

    public void InsertWorker(WorkerDataModel workerDataModel)
    {
        _logger.LogInformation("New data: {json}", JsonSerializer.Serialize(workerDataModel));
        ArgumentNullException.ThrowIfNull(workerDataModel);
        workerDataModel.Validate();
        _workerStorageContract.AddElement(workerDataModel);
    }

    public void UpdateWorker(WorkerDataModel workerDataModel)
    {
        _logger.LogInformation("Update data: {json}", JsonSerializer.Serialize(workerDataModel));
        ArgumentNullException.ThrowIfNull(workerDataModel);
        workerDataModel.Validate();
        _workerStorageContract.UpdElement(workerDataModel);
    }

    public void DeleteWorker(string id)
    {
        _logger.LogInformation("Delete by id: {id}", id);
        if (id.IsEmpty())
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Id is not a unique");
        }
        _workerStorageContract.DelElement(id);
    }
}
