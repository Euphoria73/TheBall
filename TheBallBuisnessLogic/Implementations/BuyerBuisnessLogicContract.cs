using TheBallContracts.BuisnessLogicContracts;
using TheBallContracts.DataModels;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.StoragesContracts;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TheBallBuisnessLogic.Implementations;

internal class BuyerBuisnessLogicContract(IBuyerStorageContract buyerStorageContract, ILogger logger) : IBuyerBuisnessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IBuyerStorageContract _buyerStorageContract = buyerStorageContract;
    public List<BuyerDataModel> GetAllBuyers()
    {
        _logger.LogInformation("GetAllBuyers");
        return _buyerStorageContract.GetList() ?? throw new NullListException();
    }

    public BuyerDataModel GetBuyerByData(string data)
    {
        _logger.LogInformation("Get element by data: {data}", data);
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _buyerStorageContract.GetElementById(data) ?? throw new ElementNotFoundException(data);
        }
        if (Regex.IsMatch(data, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
        {
            return _buyerStorageContract.GetElementByPhoneNumber(data) ?? throw new ElementNotFoundException(data);
        }
        return _buyerStorageContract.GetElementByFIO(data) ?? throw new ElementNotFoundException(data);
    }

    public void InsertBuyer(BuyerDataModel buyerDataModel)
    {
        _logger.LogInformation("New data: {json}", JsonSerializer.Serialize(buyerDataModel));
        ArgumentNullException.ThrowIfNull(buyerDataModel);
        buyerDataModel.Validate();
        _buyerStorageContract.AddElement(buyerDataModel);
    }

    public void UpdateBuyer(BuyerDataModel buyerDataModel)
    {
        _logger.LogInformation("Update data: {json}", JsonSerializer.Serialize(buyerDataModel));
        ArgumentNullException.ThrowIfNull(buyerDataModel);
        buyerDataModel.Validate();
        _buyerStorageContract.UpdElement(buyerDataModel);
    }

    public void DeleteBuyer(string id)
    {
        _logger.LogInformation("Delete by id: {id}", id);
        if (id.IsEmpty())
        {
            throw new ArgumentNullException(nameof(id));
        }
        if (!id.IsGuid())
        {
            throw new ValidationException("Id is not a unique identifier");
        }
        _buyerStorageContract.DelElement(id);
    }
}
