using Microsoft.Azure.Devices;

namespace Sefeko.AzureFunctionsTest.Common
{
    public class RegistryManagerFactory : IRegistryManagerFactory
    {
        public IRegistryManager CreateFromConnectionString(string connectionString)
        {
            return new RegistryManagerWrapper(RegistryManager.CreateFromConnectionString(connectionString));
        }

    }
}
