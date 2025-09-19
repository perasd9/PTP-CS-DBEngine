using PTP.Parse.Custom_Tokenizer;

namespace PTP.Parse.Interfaces
{
    public interface ILexer
    {
        public ICollection<string> Keywords { get; }
        public SimpleTokenizer Tokenizer { get; }
        public bool MatchDelim(char delimiter);
        public bool MatchIntConstant();
        public bool MatchStringConstant();
        public bool MatchKeyword(string word);
        public bool MatchId();
        public void EatDelim(char delimiter);
        public int EatIntConstant();
        public string EatStringConstant();
        public void EatKeyword(string word);
        public string EatId();
    }
}
