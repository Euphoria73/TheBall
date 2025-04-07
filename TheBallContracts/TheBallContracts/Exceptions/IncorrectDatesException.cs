
namespace TheBallContracts.Exceptions;

public class IncorrectDatesException(DateTime start, DateTime end) : Exception($"The end date must be later than the start date.. StartDate: {start:dd.MM.YYYY}. EndDate: {end:dd.MM.YYYY}")
{
}
