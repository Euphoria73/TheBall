using TheBallContracts.BuisnessLogicContracts;
using TheBallContracts.DataModels;
using TheBallContracts.StoragesContracts;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TheBallBuisnessLogic.Implementations;

internal class GiftBuisnessLogicContract(IGiftStorageContract giftStorageContract, ILogger logger) : IGiftBuisnessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IGiftStorageContract _giftStorageContract = giftStorageContract;
    public List<GiftDataModel> GetAllGifts(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllGifts params: {onlyActive}", onlyActive);
        return _giftStorageContract.GetList(onlyActive) ?? throw new NullListException();
    }

    public List<GiftDataModel> GetAllGiftsByManufacturer(string manufacturerId, bool onlyActive = true)
    {
        if (manufacturerId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(manufacturerId));
        }
        if (!manufacturerId.IsGuid())
        {
            throw new ValidationException("The value in the field manufacturerId is not a unique.");
        }
        _logger.LogInformation("GetAllGifts params: {manufacturerId}, {onlyActive}", manufacturerId, onlyActive);
        return _giftStorageContract.GetList(onlyActive, manufacturerId) ?? throw new NullListException();
    }

    public GiftDataModel GetGiftByData(string data)
    {
        _logger.LogInformation("Get element by data: {data}", data);
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _giftStorageContract.GetElementById(data) ?? throw new ElementNotFoundException(data);
        }
        return _giftStorageContract.GetElementByName(data) ?? throw new ElementNotFoundException(data);
    }

    public List<GiftHistoryDataModel> GetGiftHistoryByGift(string giftId)
    {
        _logger.LogInformation("GetProductHistoryByProduct for {productId}", giftId);
        if (giftId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(giftId));
        }
        if (!giftId.IsGuid())
        {
            throw new ValidationException("The value in the field productId is not a unique identifier.");
        }
        return _giftStorageContract.GetHistoryByGiftId(giftId) ?? throw new NullListException();
    }

    public void InsertGift(GiftDataModel giftDataModel)
    {
        _logger.LogInformation("New data: {json}", JsonSerializer.Serialize(giftDataModel));
        ArgumentNullException.ThrowIfNull(giftDataModel);
        giftDataModel.Validate();
        _giftStorageContract.AddElement(giftDataModel);
    }

    public void UpdateGift(GiftDataModel giftDataModel)
    {
        _logger.LogInformation("Update data: {json}", JsonSerializer.Serialize(giftDataModel));
        ArgumentNullException.ThrowIfNull(giftDataModel);
        giftDataModel.Validate();
        _giftStorageContract.UpdElement(giftDataModel);
    }
    public void DeleteGift(string id)
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
        _giftStorageContract.DelElement(id);
    }
}
