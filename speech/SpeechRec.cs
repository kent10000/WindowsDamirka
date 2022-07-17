using System.Speech.Recognition;

namespace speech;
#pragma warning disable CA1416
public static class SpeechRec
{
    public static Grammar MakeGrammar(string[] words)
    {
        
        var choices = new Choices(words);
        GrammarBuilder gb = new GrammarBuilder(choices);
        Grammar g = new Grammar(gb);
        return g;
    }
    
    public static Grammar MakeGrammar(string[] words, string[] prefixes, bool prefixDamirka = false)
    {
        var gbList = new List<GrammarBuilder>(); 
        var choices = new Choices(words);
        var gb = new GrammarBuilder(choices);
        foreach (var prefix in prefixes)
        {
            var gbt = new GrammarBuilder(prefix);
            gbt.Append(gb);
            gbList.Add(gbt);
        }
        
        var gbChoices = new Choices(gbList.ToArray());
        
        var grammarBuilderChoices = new GrammarBuilder(gbChoices);
        var gbd = new GrammarBuilder("Damirka");
        gbd.Append(grammarBuilderChoices);
        var g = new Grammar(gbd);
        return g;
    }
}