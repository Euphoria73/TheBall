using TheBallContracts.Infrastructure;

namespace TheBallTests.Infrastructure;

internal class ConfigurationDatabaseTest : IConfigurationDatabase
{
    public string ConnectionString =>
        "Host=localhost;Port=5432;Database=rpp;Username=postgres;Password=postgres;";
}
