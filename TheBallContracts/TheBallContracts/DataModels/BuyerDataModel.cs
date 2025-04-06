﻿using System.Text.RegularExpressions;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public partial class BuyerDataModel(string id, string fio, string phoneNuber, double discountSize) : IValidation
{
    public string Id { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public string PhoneNumber { get; private set; } = phoneNuber;
    public double DiscountSize { get; private set; } = discountSize;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");

        if (!Id.IsGuid())
            throw new ValidationException("Field Id is not unique");

        if (FIO.IsEmpty())
            throw new ValidationException("Field FIO is empty");

        if (PhoneNumber.IsEmpty())
            throw new ValidationException("Field PhoneNumber is empty");

            if (MyRegex().IsMatch(PhoneNumber))
            throw new ValidationException("Field PhoneNumber is not a PhoneNumber");

        if (DiscountSize < 0)
            throw new ValidationException("Field DiscountSize is not a positive");
    }

    [GeneratedRegex(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$")]
    private static partial Regex MyRegex();
}
