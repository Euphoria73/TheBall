

using System.Diagnostics;
using System.Xml.Linq;
using TheBallContracts.Enums;
using TheBallContracts.Exceptions;
using TheBallContracts.Extensions;
using TheBallContracts.Infrastructure;

namespace TheBallContracts.DataModels;

public class SalaryDataModel(string workerId, DateTime salaryDate, double salary) : IValidation
{
    public string WorkerId { get; private set; } = workerId;
    public DateTime SalaryDate { get; private set; } = salaryDate;
    public double Salary { get; private set; } = salary;

    public void Validate()
    {
        if (WorkerId.IsEmpty())
            throw new ValidationException("Field WorkerId is empty");

        if (!WorkerId.IsGuid())
            throw new ValidationException("Field WorkerId is not unique");
                
        if (Salary <= 0)
            throw new ValidationException("Field Salary is not a positive");
    }
}
