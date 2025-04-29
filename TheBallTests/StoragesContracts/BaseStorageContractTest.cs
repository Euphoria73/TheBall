using TheBallDatabase;
using TheBallTests.Infrastructure;

namespace TheBallTests.StoragesContracts;

internal abstract class BaseStorageContractTest
{
    protected TheBallDbContext TheBallDbContext { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TheBallDbContext = new TheBallDbContext(new ConfigurationDatabaseTest());

        TheBallDbContext.Database.EnsureDeleted();
        TheBallDbContext.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TheBallDbContext.Database.EnsureDeleted();
        TheBallDbContext.Dispose();
    }
}
