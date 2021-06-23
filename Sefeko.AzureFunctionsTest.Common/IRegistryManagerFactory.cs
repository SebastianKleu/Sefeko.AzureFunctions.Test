namespace Sefeko.AzureFunctionsTest.Common
{
    public interface IRegistryManagerFactory
    {
        IRegistryManager CreateFromConnectionString(string connectionString);
    }
}
