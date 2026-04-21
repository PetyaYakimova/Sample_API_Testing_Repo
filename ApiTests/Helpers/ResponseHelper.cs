using Newtonsoft.Json;

public static class ResponseHelper
{
    public static T Deserialize<T>(string content)
    {
        return JsonConvert.DeserializeObject<T>(content);
    }
}