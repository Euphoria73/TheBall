
namespace TheBallContracts.Exceptions;

public class StorageException(Exception ex) : Exception($"Error while working in storage: {ex.Message}", ex)
{
}
