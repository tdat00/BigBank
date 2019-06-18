using Newtonsoft.Json;

namespace LeeVox.Demo.BigBank.Core
{
    public static class ObjectExtension
    {
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}
