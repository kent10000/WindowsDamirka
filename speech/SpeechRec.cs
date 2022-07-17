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
    
    public static Grammar MakeGrammar(string[] words, string[] prefixes)
    {
        
        var choices = new Choices(words);
        var gb = new GrammarBuilder(choices);
        var gbp = new GrammarBuilder();
        foreach (var prefix in prefixes)
        {
            var gbt = new GrammarBuilder(prefix);
            gbt.Append(gb);
            gbp = GrammarBuilder.Add(gbp, gbt);
        }
        
        var g = new Grammar(gbp);
        return g;
    }
}