using System;

namespace Sefeko.AzureFunctionsTest.Common.Attributes
{
    /// <summary>
    /// Uses the property name for serialisation. Use Contract Serialiser <see cref="ShortPropertyContractResolver"/>
    /// </summary>
    public class ShortPropertyAttribute : Attribute
    {
        /// <summary>
        /// The property name to use for serialisation/deserialisation
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public ShortPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
