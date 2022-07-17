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
    
    public static Grammar MakeGrammar(string[] words, string prefix)
    {
        
        var choices = new Choices(words);
        var gb = new GrammarBuilder(choices);
        var gbp = new GrammarBuilder(prefix);
        gbp.Append(gb);

        var g = new Grammar(gbp);
        return g;
    }
}