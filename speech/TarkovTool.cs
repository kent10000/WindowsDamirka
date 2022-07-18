#region

using System.Collections;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace speech;

public class TarkovTool
{
    private readonly string _query;
    private readonly string[] _details;
    private readonly HttpClient _httpClient;
    private LanguageCode _languageCode;

    public TarkovTool(string query, string[] details, LanguageCode languageCode = LanguageCode.en)
    {
        _query = query;
        _details = details;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.tarkov.dev/graphql");
        _languageCode = languageCode;
    }
    
    public void SetLanguageCode(LanguageCode languageCode)
    {
        _languageCode = languageCode;
    }

    public async Task<string[]?> GetResponse(IEnumerable<GraphQlRequest> args)
    {
        var responses = new List<string>();


        //var arguments = args.Aggregate("(", (current, arg) => current + (arg.ArgumentName[^1] == 's' ? $"{arg.ArgumentName}: {arg.ArgumentValue}" : $"{arg.ArgumentName}: \"{arg.ArgumentValue}\"" + ", "));

        var arguments = "(";
        
        if (_languageCode != LanguageCode.en)
        {
            arguments += $"lang: {_languageCode}, ";
        }
        
        
        foreach (var arg in args)
        {
            if (arg.ArgumentValue is string)
            {
                arguments += $"{arg.ArgumentName}: \"{arg.ArgumentValue}\", ";
            } else if (IsArray(arg.ArgumentValue))
            {
                if (arg.ArgumentValue.GetType() == typeof(string[]))
                {
                    arguments += $"{arg.ArgumentName}: [";
                    foreach (var item in arg.ArgumentValue)
                    {
                        arguments += $"\"{item}\", ";
                    }
                    arguments = arguments.Remove(arguments.Length - 2);
                    arguments += "], ";
                }
                else
                {
                    arguments += $"{arg.ArgumentName}: [";
                    foreach (var item in arg.ArgumentValue)
                    {
                        arguments += $"{item}, ";
                    }
                    arguments = arguments.Remove(arguments.Length - 2);
                    arguments += "], ";
                }
            }
            else
            {
                arguments += $"{arg.ArgumentName}: {arg.ArgumentValue}, ";
            }
        }
        arguments = arguments.Remove(arguments.Length - 2);
        arguments += ")";
        
        
        


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

        string q;
        if (_languageCode == LanguageCode.en) //saves time
        {
            q = "{" + _query + "{" + details + "}}";
        }
        else
        {
            q = "{" + _query + "(" + "lang: " + _languageCode + ")" + "{" + details + "}}";
        }
        

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

    public async Task<string[]?> GetResponse(GraphQlRequest args)
    {
        return await GetResponse(new[] { args });
    }
    
    private static bool IsArray(dynamic value)
    {
        try
        {
            var dummy = value[0];
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    
}

public struct GraphQlRequest 
{
    public string ArgumentName { get; }

    public dynamic ArgumentValue { get; }


    public GraphQlRequest(string argumentName, ItemType argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, ItemCategoryName argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, TraderName argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, ItemSourceName argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, RequirementType argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, StatusCode argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, ItemType[] argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, ItemCategoryName[] argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, TraderName[] argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, ItemSourceName[] argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, RequirementType[] argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, StatusCode[] argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, string argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    public GraphQlRequest(string argumentName, IEnumerable argumentValue)
    {
        ArgumentName = argumentName;
        ArgumentValue = argumentValue;
    }
    
}

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
public enum LanguageCode {
    cz,
    de,
    en,
    es,
    fr,
    hu,
    ru,
    tr,
    zh
}

public enum ItemCategoryName {
    Ammo,
    AmmoContainer,
    ArmBand,
    Armor,
    ArmoredEquipment,
    AssaultCarbine,
    AssaultRifle,
    AssaultScope,
    AuxiliaryMod,
    Backpack,
    Barrel,
    BarterItem,
    Battery,
    Bipod,
    BuildingMaterial,
    ChargingHandle,
    ChestRig,
    CombMuzzleDevice,
    CombTactDevice,
    CommonContainer,
    CompactReflexSight,
    Compass,
    CylinderMagazine,
    Drink,
    Drug,
    Electronics,
    Equipment,
    EssentialMod,
    FaceCover,
    Flashhider,
    Flashlight,
    Food,
    FoodAndDrink,
    Foregrip,
    Fuel,
    FunctionalMod,
    GasBlock,
    GearMod,
    GrenadeLauncher,
    Handguard,
    Handgun,
    Headphones,
    Headwear,
    HouseholdGoods,
    Info,
    Ironsight,
    Jewelry,
    Key,
    KeyMechanical,
    Keycard,
    Knife,
    LockingContainer,
    Lubricant,
    Machinegun,
    Magazine,
    Map,
    MarksmanRifle,
    MedicalItem,
    MedicalSupplies,
    Medikit,
    Meds,
    Money,
    Mount,
    MuzzleDevice,
    NightVision,
    Other,
    PistolGrip,
    PortContainer,
    PortableRangeFinder,
    Receiver,
    ReflexSight,
    RepairKits,
    Revolver,
    SMG,
    Scope,
    Shotgun,
    Sights,
    Silencer,
    SniperRifle,
    SpecialItem,
    SpecialScope,
    SpringDrivenCylinder,
    Stimulant,
    Stock,
    ThermalVision,
    ThrowableWeapon,
    Tool,
    VisObservDevice,
    Weapon,
    WeaponMod
}

public enum ItemType {
    ammo,
    ammoBox,
    any,
    armor,
    backpack,
    barter,
    container,
    glasses,
    grenade,
    gun,
    headphones,
    helmet,
    injectors,
    keys,
    markedOnly,
    meds,
    mods,
    noFlea,
    pistolGrip,
    preset,
    provisions,
    rig,
    suppressor,
    wearable
}

public enum TraderName {
    prapor, 
    therapist,
    fence,
    skier,
    peacekeeper,
    mechanic,
    ragman,
    jaeger
}

public enum ItemSourceName {
    prapor, 
    therapist,
    fence,
    skier,
    peacekeeper,
    mechanic,
    ragman,
    jaeger,
    fleaMarket
}

public enum RequirementType {
    playerLevel,
    loyaltyLevel,
    questCompleted,
    stationLevel
}

public enum StatusCode {
    OK,
    Updating,
    Unstable,
    Down
}