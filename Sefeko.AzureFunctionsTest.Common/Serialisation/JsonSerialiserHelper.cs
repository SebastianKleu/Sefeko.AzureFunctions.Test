using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sefeko.AzureFunctionsTest.Common.JsonContractResolvers;
using Sefeko.AzureFunctionsTest.Common.JsonConverters;

namespace Sefeko.AzureFunctionsTest.Common.Serialisation
{
	/// <summary>
	/// Helper methods for the serialization of the payloads
	/// </summary>
	public static class JsonSerialiserHelper
	{
		/// <summary>
		/// Use these settings to serialize payloads. This ensure compatibility with the
		/// C++ JSON serialization.
		/// </summary>
		public static  JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
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


		/// <summary>
		/// The same as <see cref="DefaultSettings"/>, but the JSON is always nicely formatted for display purposes
		/// </summary>
		public static JsonSerializerSettings FormattedSettings = new JsonSerializerSettings
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

		/// <summary>
		/// Optimised the json for smallest possible size
		/// </summary>
		public static JsonSerializerSettings SizeOptimisedSettings = new JsonSerializerSettings
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

		/// <summary>
		/// Default settings with Unix Time Converter added
		/// </summary>
		public static JsonSerializerSettings DefaultWithUnixTime
		{
			get
			{
				var defaultSettings = DefaultSettings;
				var sefekoDateTimeConverter = defaultSettings.Converters.FirstOrDefault(a => a.GetType() == typeof(SefekoDateTimeJsonConverter));
				if (sefekoDateTimeConverter != null)
					defaultSettings.Converters.Remove(sefekoDateTimeConverter);
				defaultSettings.Converters.Add(new UnixDateTimeConverter());
				return defaultSettings;
			}
		}

		/// <summary>
		/// The same as <see cref="DefaultSettings"/>, but the string converter is removed to show enums as integers
		/// </summary>
		public static JsonSerializerSettings IntegerEnums
		{
			get
			{
				var returnValue = DefaultSettings;
				var stringEnumConverter =
					returnValue.Converters?.FirstOrDefault(a => a.GetType() == typeof(StringEnumConverter));
				if (stringEnumConverter != null)
					returnValue.Converters.Remove(stringEnumConverter);
				return returnValue;
			}
		}
	}
}
