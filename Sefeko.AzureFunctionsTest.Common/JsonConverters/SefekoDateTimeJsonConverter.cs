using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sefeko.AzureFunctionsTest.Common.JsonConverters
{
	/// <summary>
	/// 
	/// </summary>
	public class SefekoDateTimeJsonConverter : DateTimeConverterBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		public override void WriteJson(JsonWriter writer, [CanBeNull] object value, JsonSerializer serializer)
		{
			if (value == null)
				return;

			var valueType = value.GetType();
			if (
				valueType == typeof(DateTime) ||
				valueType == typeof(DateTime?))
			{
				if (value is DateTime valueDateTime)
				{
					writer.DateFormatString = serializer.DateFormatString;
					writer.WriteValue(valueDateTime);
				}
			}

			if (
				valueType == typeof(DateTimeOffset) ||
				valueType == typeof(DateTimeOffset?))
			{
				if (value is DateTimeOffset valueDateTime)
				{
					writer.DateFormatString = serializer.DateFormatString;
					writer.WriteValue(valueDateTime);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="objectType"></param>
		/// <param name="existingValue"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		[CanBeNull]
		public override object ReadJson(JsonReader reader, Type objectType, [CanBeNull] object existingValue, JsonSerializer serializer)
		{
			if (reader?.Value == null)
				return null;

			var returnValue = DateTime.Parse(reader.Value.ToString());
			return returnValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectType"></param>
		/// <returns></returns>
		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
				return true;

			if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
				return true;

			return false;
		}
	}
}
