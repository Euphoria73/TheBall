using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class ManufacturerDataModel (string id, string name, string? prevName, string? prevPrevName) : IValidation
{
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string? PrevName { get; private set; } = prevName;
    public string? PrevPrevName { get; private set; } = prevPrevName;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");

        if (!Id.IsGuid())
            throw new ValidationException("Field Id is not unique");

        if (Name.IsEmpty())
            throw new ValidationException("Field Name is empty");
    }
}
