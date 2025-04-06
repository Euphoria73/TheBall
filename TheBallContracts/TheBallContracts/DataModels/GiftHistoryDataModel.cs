using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class GiftHistoryDataModel (string giftId, double oldPrice) : IValidation
{
    public string GiftId { get; private set; } = giftId;
    public double OldPrice { get; private set;} = oldPrice;
    public DateTime ChangeDate { get; private set; } = DateTime.UtcNow;

    public void Validate()
    {
        if (GiftId.IsEmpty())
            throw new ValidationException("Field Id is empty");

        if (!GiftId.IsGuid())
            throw new ValidationException("Field Id is not unique");

        if (OldPrice <= 0)
            throw new ValidationException("Field OldPrice is not a positive");
    }
}
