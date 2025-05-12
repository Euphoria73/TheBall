﻿using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class ManufacturerDataModel (string id, string manufacturerName, string? prevManufacturerName, string? prevPrevManufacturerName) : IValidation
{
    public string Id { get; private set; } = id;
    public string ManufacturerName { get; private set; } = manufacturerName;
    public string? PrevManufacturerName { get; private set; } = prevManufacturerName;
    public string? PrevPrevManufacturerName { get; private set; } = prevPrevManufacturerName;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");

        if (!Id.IsGuid())
            throw new ValidationException("Field Id is not unique");

        if (ManufacturerName.IsEmpty())
            throw new ValidationException("Field Name is empty");
    }
}
