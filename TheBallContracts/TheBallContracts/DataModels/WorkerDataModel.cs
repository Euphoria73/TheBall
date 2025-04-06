
using System.Diagnostics;
using System.Xml.Linq;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class WorkerDataModel(string id, string fio, string postId, DateTime birthDate, DateTime employmentDate, bool isDeleted) : IValidation
{
    public string Id { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public string PostId { get; private set; } = postId;
    public DateTime BirthDate { get; private set; } = birthDate;
    public DateTime EmploymentDate { get; private set; } = employmentDate;
    public bool IsDeleted { get; private set; } = isDeleted;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");

        if (!Id.IsGuid())
            throw new ValidationException("Field Id is not unique");

        if (FIO.IsEmpty())
            throw new ValidationException("Field FIO is empty");

        if (PostId.IsEmpty())
            throw new ValidationException("Field PostId is empty");

        if (!PostId.IsGuid())
            throw new ValidationException("Field PostId is not unique");

        if (BirthDate.Date > DateTime.Now.AddYears(-14).Date)
            throw new ValidationException($"Minors cannot be hired (BirthDate = {BirthDate.ToShortDateString()})");

        if (EmploymentDate.Date < BirthDate.Date)
            throw new ValidationException("The date of employment cannot be less than date of birthday");

        if ((EmploymentDate - BirthDate).TotalDays / 365 < 14)
            throw new ValidationException($"Minors cannot be hired (EmploymentDate = {EmploymentDate.ToShortDateString()}, BirthDate = {BirthDate.ToShortDateString()})");
    }
}
