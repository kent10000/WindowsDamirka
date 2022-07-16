using System.Speech.Recognition;
using System.Speech.Synthesis;
using speech;

#pragma warning disable CA1416
//Only works on Windows

var itemList = new TarkovTool("items", new[] { "shortName" });

var list = await itemList.GetResponse();

if (list == null)
{
    throw new HttpRequestException();
}

//remove every \" from the list
list = list.Select(x => x.Replace("\"", "")).ToArray();



var c = new Choices(list);



var items = new GrammarBuilder(c);

var price = new GrammarBuilder("price");
price.Append(items);



var grammar = new Grammar(price);

var engine = new SpeechRecognizer();
engine.LoadGrammar(grammar);

var speech = new SpeechSynthesizer();

var priceGetter = new TarkovTool("items", new []{"avg24hPrice"});

engine.SpeechRecognized += (async (_, eventArgs) =>
{
    var t = eventArgs.Result.Words[1].Text;
    //Console.WriteLine(t);
    var itemPrice = await priceGetter.GetResponse(new KeyValuePair<string, string>("name", t));
    if (itemPrice == null || itemPrice.Length == 0)
    {
        speech.SpeakAsync("Item not found");
        return;
    } 
    if (itemPrice[0] == "0")
    {
        speech.SpeakAsync(t + " is not for sale on the flea market");
        return;
    }
    speech.SpeakAsync("The price of " + t + " is " + itemPrice[0] + " roubles");
    
    
});
//for testing api
/*
var levels = new TarkovTool("playerLevels",new[]{"level", "exp"});

var l = await levels.GetResponse();

foreach (var i in l)
{
    Console.WriteLine(i);
}*/



Console.WriteLine("Press any key to exit...");
Console.ReadKey();