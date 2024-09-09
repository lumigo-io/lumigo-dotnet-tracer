using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class DynamicIgnoreResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Try to serialize the property; skip if it throws an error
        property.ShouldSerialize = instance =>
        {
            try
            {
                // Try accessing the value of the property to detect if it causes issues
                var value = property.ValueProvider.GetValue(instance);
                return true; // Property is serializable
            }
            catch
            {
                // Ignore property if it can't be serialized
                return false;
            }
        };

        return property;
    }
}
