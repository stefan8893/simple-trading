using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleTrading.TestInfrastructure.Authentication;

public static class HttpContentExtensions
{
    public static async Task<T> Deserialize<T>(this HttpContent content)
    {
        var result = await content.ReadFromJsonAsync<T>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = {new JsonStringEnumConverter()}
        });


        return result ?? throw new Exception($"Error while deserializing {typeof(T).Name}");
    }
}