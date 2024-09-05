using Newtonsoft.Json;

namespace System
{
    static class JsonExtension
    {
        public static string ToJson(this object obj, bool ignoreNull = false, bool indented = true)
        {
            if (obj == null) return null;

            var formatting = indented ? Formatting.Indented : Formatting.None;
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include,
            };
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }
    }
}
