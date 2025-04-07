
namespace TheBallContracts.Exceptions;

public class ElementExistsException(string paramName, string paramValue) : Exception($"There is already an element with value{paramValue} of parameter {paramName}")
{
    public string ParamName { get; private set; } = paramName;

    public string ParamValue { get; private set; } = paramValue;
}
