
using System.Text.RegularExpressions;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class GiftDataModel (string id, string name, GiftType giftType ,string manufacturerId, double price, bool isDeleted) : IValidation
{
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public GiftType GiftType { get; private set; } = giftType;
    public string ManufacturerId { get; private set; } = manufacturerId;
    public double Price { get; private set; } = price;
    public bool IsDeleted { get; private set; } = isDeleted;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");

        if (!Id.IsGuid())
            throw new ValidationException("Field Id is not unique");

        if (Name.IsEmpty())
            throw new ValidationException("Field Name is empty");

        if (GiftType == GiftType.None)
            throw new ValidationException("Field GiftType is empty");


        if (ManufacturerId.IsEmpty())
            throw new ValidationException("Field ManufacturerId is empty");

        if (!ManufacturerId.IsGuid())
            throw new ValidationException("Field ManufacturerId is not unique");

        if (Price <= 0)
            throw new ValidationException("Field Price is not a positive");
    }
}
