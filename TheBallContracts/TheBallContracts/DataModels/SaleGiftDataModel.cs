
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class SaleGiftDataModel(string saleId, string giftId, int count) : IValidation
{
    public string SaleId { get; private set; } = saleId;
    public string GiftId { get; private set; } = giftId;
    public int Count { get; private set; } = count;

    public void Validate()
    {
        if (SaleId.IsEmpty())
            throw new ValidationException("Field SaleId is empty");

        if (!SaleId.IsGuid())
            throw new ValidationException("Field SaleId is not unique");
                
        if (GiftId.IsEmpty())
            throw new ValidationException("Field GiftId is empty");

        if (!GiftId.IsGuid())
            throw new ValidationException("Field GiftId is not unique");

        if (Count <= 0)
            throw new ValidationException("Field Count is not a positive");
    }
}
