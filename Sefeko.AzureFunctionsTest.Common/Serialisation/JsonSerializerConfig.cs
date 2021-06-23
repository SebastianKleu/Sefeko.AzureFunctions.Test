using System;
using Newtonsoft.Json;

namespace Sefeko.AzureFunctionsTest.Common.Serialisation
{
    /// <summary>
    /// Json Serialisation Configuration
    /// </summary>
    public class JsonSerializerConfig : IDisposable
    {
        private readonly Func<JsonSerializerSettings> _savedFunc;

        /// <summary>
        /// Json Serialisation Configuration
        /// </summary>
        /// <param name="settings"></param>
        public JsonSerializerConfig(JsonSerializerSettings settings)
        {
            _savedFunc = JsonConvert.DefaultSettings;
            JsonConvert.DefaultSettings = () => settings;
            CurrentSettings = settings;
        }

        /// <summary>
        /// Retrieves the current settings
        /// </summary>
        public JsonSerializerSettings CurrentSettings { get; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            JsonConvert.DefaultSettings = _savedFunc;
        }
    }
}
