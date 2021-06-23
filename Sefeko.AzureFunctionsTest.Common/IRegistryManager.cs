using System.Threading;
using System.Threading.Tasks;

namespace Sefeko.AzureFunctionsTest.Common
{
    public interface IRegistryManager
    {
        /// <summary>Retrieves the specified Device object.</summary>
        /// <param name="deviceId">The id of the device to be retrieved.</param>
        /// <returns>The Device object.</returns>
        Task<Microsoft.Azure.Devices.Device> GetDeviceAsync(string deviceId);

        /// <summary>Retrieves the specified Device object.</summary>
        /// <param name="deviceId">The id of the device to be retrieved.</param>
        /// <param name="cancellationToken">
        /// The token which allows the the operation to be cancelled.
        /// </param>
        /// <returns>The Device object.</returns>
        Task<Microsoft.Azure.Devices.Device> GetDeviceAsync(
            string deviceId,
            CancellationToken cancellationToken);

    }
}
