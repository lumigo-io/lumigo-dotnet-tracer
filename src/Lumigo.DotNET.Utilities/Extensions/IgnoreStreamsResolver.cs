using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lumigo.DotNET.Utilities.Extensions
{
    public class IgnoreStreamsResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (typeof(Stream).IsAssignableFrom(property.PropertyType))
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}
