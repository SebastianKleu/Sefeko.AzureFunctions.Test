using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sefeko.AzureFunctionsTest.Common.Attributes;

namespace Sefeko.AzureFunctionsTest.Common.JsonContractResolvers
{
    /// <summary>
    /// Resolves properties with <see cref="ShortPropertyAttribute"/> attribute
    /// </summary>
    public class ShortPropertyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 
        /// </summary>
        public ShortPropertyContractResolver(NamingStrategy namingStrategy = null)
        {
            if (namingStrategy != null)
                NamingStrategy = namingStrategy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            // Let the base class create all the JsonProperties 
            // using the short names
            var list = base.CreateProperties(type, memberSerialization);

            // Now inspect each property and replace the 
            // short name with the real property name
            foreach (var prop in list)
            {
                var shortPropertyAttribute = prop.AttributeProvider?.GetAttributes(typeof(ShortPropertyAttribute), true)?.FirstOrDefault();
                if (shortPropertyAttribute is ShortPropertyAttribute shortProperty)
                    prop.PropertyName = NamingStrategy?.GetPropertyName(shortProperty.PropertyName, false) ?? shortProperty.PropertyName;
                else
                    prop.PropertyName = NamingStrategy?.GetPropertyName(prop.UnderlyingName ?? string.Empty, false) ?? prop.UnderlyingName;
            }

            return list;
        }
    }
}
