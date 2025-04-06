using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class PostDataModel (string id, string postId, string name, PostType postType, double salary, bool isActual, DateTime changeDate) : IValidation
{
    public string Id { get; private set; } = id;
    public string PostId {  get; private set; } = postId;
    public string Name { get; private set; } = name;
    public PostType PostType { get; private set; } = postType;
    public double Salary { get; private set; } = salary;
    public bool IsActual { get; private set; } = isActual;
    public DateTime ChangeDate { get; private set; } = changeDate;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");

        if (!Id.IsGuid())
            throw new ValidationException("Field Id is not unique");

        if (PostId.IsEmpty())
            throw new ValidationException("Field PostId is empty");

        if (!PostId.IsGuid())
            throw new ValidationException("Field PostId is not unique");

        if (Name.IsEmpty())
            throw new ValidationException("Field Name is empty");

        if (PostType == PostType.None)
            throw new ValidationException("Field PostType is empty");

        if (Salary <= 0)
            throw new ValidationException("Field Salary is not positive");
    }
}
