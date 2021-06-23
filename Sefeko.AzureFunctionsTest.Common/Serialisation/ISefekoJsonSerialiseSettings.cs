using Newtonsoft.Json;

namespace Sefeko.AzureFunctionsTest.Common.Serialisation
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISefekoJsonSerialiseSettings
	{
		/// <summary>
		/// Use these settings to serialize payloads. This ensure compatibility with the
		/// C++ JSON serialization.
		/// </summary>
		JsonSerializerSettings DefaultSettings();

		/// <summary>
		/// The same as <see cref="SefekoJsonSerialiseSettings.DefaultSettings"/>, but the JSON is always nicely formatted for display purposes
		/// </summary>
		JsonSerializerSettings FormattedSettings();

		/// <summary>
		/// Optimised the json for smallest possible size
		/// </summary>
		JsonSerializerSettings SizeOptimisedSettings();

		/// <summary>
		/// Default settings with Unix Time Converter added
		/// </summary>
		JsonSerializerSettings DefaultWithUnixTime();

		/// <summary>
		/// The same as <see cref="SefekoJsonSerialiseSettings.DefaultSettings"/>, but the string converter is removed to show enums as integers
		/// </summary>
		JsonSerializerSettings IntegerEnums();
	}
}
