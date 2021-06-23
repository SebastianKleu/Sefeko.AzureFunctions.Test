using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sefeko.AzureFunctionsTest.Common.JsonContractResolvers;
using Sefeko.AzureFunctionsTest.Common.JsonConverters;

namespace Sefeko.AzureFunctionsTest.Common.Serialisation
{
	/// <summary>
	/// 
	/// </summary>
	public class SefekoJsonSerialiseSettings : ISefekoJsonSerialiseSettings
	{
		/// <summary>
		/// Use these settings to serialize payloads. This ensure compatibility with the
		/// C++ JSON serialization.
		/// </summary>
		public JsonSerializerSettings DefaultSettings()
		{
			return new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.None,
#if DEBUG
				Formatting = Formatting.Indented,
#else
            Formatting = Formatting.None,
#endif

				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				DefaultValueHandling = DefaultValueHandling.Populate,
				Converters =
			{
				new StringEnumConverter(),
				new SefekoDateTimeJsonConverter()
			},
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}


		/// <summary>
		/// The same as <see cref="DefaultSettings"/>, but the JSON is always nicely formatted for display purposes
		/// </summary>
		public JsonSerializerSettings FormattedSettings()
		{
			return new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.None,
				Formatting = Formatting.Indented,
				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				DefaultValueHandling = DefaultValueHandling.Populate,
				Converters =
				{
					new StringEnumConverter(),
					new SefekoDateTimeJsonConverter()
				},
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}

		/// <summary>
		/// Optimised the json for smallest possible size
		/// </summary>
		public JsonSerializerSettings SizeOptimisedSettings()
		{
			return new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.None,
				Formatting = Formatting.None,
				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				DefaultValueHandling = DefaultValueHandling.Populate,
				Converters =
				{
					new StringEnumConverter(),
					new UnixDateTimeConverter()
				},
				ContractResolver = new ShortPropertyContractResolver(new CamelCaseNamingStrategy())
			};
		}

		/// <summary>
		/// Default settings with Unix Time Converter added
		/// </summary>
		public JsonSerializerSettings DefaultWithUnixTime()
		{
			var defaultSettings = DefaultSettings();
			var sefekoDateTimeConverter = defaultSettings.Converters.FirstOrDefault(a => a.GetType() == typeof(SefekoDateTimeJsonConverter));
			if (sefekoDateTimeConverter != null)
				defaultSettings.Converters.Remove(sefekoDateTimeConverter);
			defaultSettings.Converters.Add(new UnixDateTimeConverter());
			return defaultSettings;
		}

		/// <summary>
		/// The same as <see cref="DefaultSettings"/>, but the string converter is removed to show enums as integers
		/// </summary>
		public JsonSerializerSettings IntegerEnums()
		{
			var returnValue = DefaultSettings();
			var stringEnumConverter =
					returnValue.Converters?.FirstOrDefault(a => a.GetType() == typeof(StringEnumConverter));
			if (stringEnumConverter != null)
				returnValue.Converters.Remove(stringEnumConverter);
			return returnValue;
		}
	}
}
