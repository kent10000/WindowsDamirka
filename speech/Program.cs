#region

using System.Globalization;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Humanizer;
using speech;

#endregion

#pragma warning disable CA1416
//Only works on Windows

var itemList = new TarkovTool("items", new[] { "shortName" });
var priceGetter = new TarkovTool("items", new[] { "avg24hPrice" });
var list = await itemList.GetResponse();
if (list == null) throw new HttpRequestException();
list = list.Select(x => x.Replace("\"", "")).ToArray();

var prefixes = new[] { "whats the price of", "how much is" };

var grammar = SpeechRec.MakeGrammar(list, prefixes, true);

var engine = new SpeechRecognitionEngine(new CultureInfo("en-US"));
engine.SetInputToDefaultAudioDevice();
engine.LoadGrammar(grammar);
engine.RecognizeAsync(RecognizeMode.Multiple);

var speech = new SpeechSynthesizer();
const string voice = "Microsoft Irina Desktop";
try
{
    speech.SelectVoice(voice);
}
catch (Exception)
{
    Console.WriteLine(
        "Warn: Unable to find voice, \"{0}\".\nIf you would like Damirka to speak with a russian accent,\nplease install the Russian voice pack in (Settings > Speech)",
        voice);
    Console.WriteLine("Damirka will now speak with the default voice instead.\n");
}

/*var voices = speech.GetInstalledVoices();

//list each voice in the list
foreach (var voice in voices)
{
    Console.WriteLine(voice.VoiceInfo.Name);
}*/

//Todo: add more commands from api
engine.SpeechRecognized += async (_, eventArgs) =>
{
    var text = eventArgs.Result.Text;
    if (text.Contains("Damirka")) text = text.Replace("Damirka", "").Trim();
    var item = (from phrase in prefixes where text.ToLower().Contains(phrase) select text.Replace(phrase, "").Trim())
        .FirstOrDefault();
    if (item == null) return;
    //Console.WriteLine(t);
    var itemPrice = await priceGetter.GetResponse(new KeyValuePair<string, dynamic>("name", item));
    //var itemPrice = await priceGetter.GetResponse(new[] {new KeyValuePair<string, dynamic>("names", new[] {"Toolset", "Awl"}), /*new KeyValuePair<string, dynamic>("type", ItemType.barter)*/});
    if (itemPrice == null || itemPrice.Length == 0)
    {
        speech.SpeakAsync("Item not found");
        return;
    }

    if (itemPrice[0] == "0")
    {
        speech.SpeakAsync(item + " is not for sale on the flea market");
        return;
    }

    var price = int.Parse(itemPrice[0]);
    speech.SpeakAsync("The price of " + item + " is " + price.ToWords() + " roubles");
};
engine.RecognizeAsyncStop();
engine.EmulateRecognizeAsync("Damirka how much is toolset"); 

Console.WriteLine("Damirka is listening...\nYou are free to minimize the window.\nPress any key to exit...");
Console.ReadKey(true);