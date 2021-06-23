using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Sefeko.AzureFunctionsTest.Common
{
    public class RegistryManagerWrapper : IRegistryManager
    {
        private readonly RegistryManager _registryManager;

        public RegistryManagerWrapper(RegistryManager registryManager)
        {
            _registryManager = registryManager;
        }
        public async Task<Microsoft.Azure.Devices.Device> GetDeviceAsync(string deviceId)
        {
            return await _registryManager.GetDeviceAsync(deviceId);
        }

        public async Task<Microsoft.Azure.Devices.Device> GetDeviceAsync(string deviceId, CancellationToken cancellationToken)
        {
            return await _registryManager.GetDeviceAsync(deviceId, cancellationToken);
        }
    }
}
