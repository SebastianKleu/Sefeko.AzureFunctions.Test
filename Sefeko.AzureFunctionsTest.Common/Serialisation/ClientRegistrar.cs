using Microsoft.Extensions.DependencyInjection;

namespace Sefeko.AzureFunctionsTest.Common.Serialisation
{
	/// <summary>
	/// 
	/// </summary>
	public static class ClientRegistrar
	{
		/// <summary>
		/// Registers the serialisation clients
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection RegisterSerialisationClients(this IServiceCollection services) =>
			services.AddTransient<ISefekoJsonSerialiseSettings, SefekoJsonSerialiseSettings>();
	}
}
