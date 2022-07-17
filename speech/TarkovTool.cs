#region

using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace speech;

public class TarkovTool
{
    private readonly string _query;
    private readonly string[] _details;
    private readonly HttpClient _httpClient;

    public TarkovTool(string query, string[] details)
    {
        _query = query;
        _details = details;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.tarkov.dev/graphql");
    }

    public async Task<string[]?> GetResponse(KeyValuePair<string, string> args)
    {
        var responses = new List<string>();


        var arguments = args.Key[^1] == 's' ? $"({args.Key}: {args.Value})" : $"({args.Key}: \"{args.Value}\")";


        /*if (_args is not string)
        {
            arguments = arguments = $"({_args.Key}: {_args.Value})";
        } else
       {
                
        }
            arguments = $"({_args.Key}: {_args.Value})";*/

        var details = _details.Aggregate<string?, string?>(null, (current, detail) => current + $"{detail} ");

        details = details?.TrimEnd();

        var q = "{" + _query + arguments + "{" + details + "}}";

        /*var request = new GraphQLRequest
        {
            Query = q
        };*/

        //var r = client.SendQueryAsync<>(request);

        //var responses = await client.SendQueryAsync<dynamic>(request);
        var response = await _httpClient.PostAsJsonAsync("", new Dictionary<string, string> { { "query", q } });

        var jsonString = await response.Content.ReadAsStringAsync();

        var json = JObject.Parse(jsonString);
        //var value = json.TryGetValue("item", out var name);
        //json.First.ToString();
        //Console.WriteLine(json.First.First.Type.ToString());


        if (json.First == null) return responses.Count == 0 ? null : responses.ToArray();
        if (json.First.First == null) return responses.Count == 0 ? null : responses.ToArray();
        if (json.First.First.First == null) return responses.Count == 0 ? null : responses.ToArray();
        var result = json.First.First.First.First;
        if (result == null) return null;
        if (result.Type == JTokenType.Array)
        {
            responses.AddRange(from item in result
                from child in item.Children()
                from baby in child.Children()
                select baby.ToString());
        }
        else
        {
            var children = result.Children();
            responses.AddRange(from child in children from baby in child.Children() select baby.ToString());
        }

        return responses.Count == 0 ? null : responses.ToArray();
    }

    public async Task<string[]?> GetResponse()
    {
        var responses = new List<string>();


        //var arguments = args.Key[^1] == 's' ? $"({args.Key}: {args.Value})" : $"({args.Key}: \"{args.Value}\")";


        /*if (_args is not string)
        {
            arguments = arguments = $"({_args.Key}: {_args.Value})";
        } else
       {
                
        }
            arguments = $"({_args.Key}: {_args.Value})";*/

        var details = _details.Aggregate<string?, string?>(null, (current, detail) => current + $"{detail} ");

        details = details?.TrimEnd();

        var q = "{" + _query + "{" + details + "}}";

        /*var request = new GraphQLRequest
        {
            Query = q
        };*/

        //var r = client.SendQueryAsync<>(request);

        //var responses = await client.SendQueryAsync<dynamic>(request);
        var response = await _httpClient.PostAsJsonAsync("", new Dictionary<string, string> { { "query", q } });

        var jsonString = await response.Content.ReadAsStringAsync();

        var json = JObject.Parse(jsonString);
        //var value = json.TryGetValue("item", out var name);
        //json.First.ToString();
        //Console.WriteLine(json.First.First.Type.ToString());


        if (json.First == null) return responses.Count == 0 ? null : responses.ToArray();
        if (json.First.First == null) return responses.Count == 0 ? null : responses.ToArray();
        if (json.First.First.First == null) return responses.Count == 0 ? null : responses.ToArray();
        var result = json.First.First.First.First;
        if (result == null) return null;
        if (result.Type == JTokenType.Array)
        {
            responses.AddRange(from item in result
                from child in item.Children()
                from baby in child.Children()
                select baby.ToString());
        }
        else
        {
            var children = result.Children();
            responses.AddRange(from child in children from baby in child.Children() select baby.ToString());
        }

        return responses.Count == 0 ? null : responses.ToArray();
    }
}