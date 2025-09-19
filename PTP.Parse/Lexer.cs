using PTP.Parse.Custom_Tokenizer;
using PTP.Parse.Interfaces;

namespace PTP.Parse
{
    public class Lexer : ILexer
    {
        public ICollection<string> Keywords { get; set; }

        public SimpleTokenizer Tokenizer { get; set; }

        public Lexer(string text)
        {
            Keywords = this.InitKeywords();
            Tokenizer = new SimpleTokenizer(new StringReader(text));
            this.NextToken();
        }

        private void NextToken()
        {
            Tokenizer.NextToken();
        }

        private ICollection<string> InitKeywords()
        {
            return new List<string> { "select", "from", "where", "and", "insert","into", "values", "delete", 
                "update", "set", "create", "table", "varchar", "int", "view", "as", "index", "on" };
        }

        public void EatDelim(char delimiter)
        {
            if (!this.MatchDelim(delimiter))
                throw new ArgumentException($"Expected delimiter {delimiter} but found {Tokenizer.CurrentToken.Value}");
            this.NextToken();
        }

        public string EatId()
        {
            if (!this.MatchId())
                throw new ArgumentException($"Expected identifier but found {Tokenizer.CurrentToken.Value}");
            string id = Tokenizer.CurrentToken.Value;
            this.NextToken();
            return id;
        }

        public int EatIntConstant()
        {
            if (!this.MatchIntConstant())
                throw new ArgumentException($"Expected integer constant but found {Tokenizer.CurrentToken.Value}");
            int val = int.Parse(Tokenizer.CurrentToken.Value);
            this.NextToken();
            return val;
        }

        public void EatKeyword(string word)
        {
            if (!this.MatchKeyword(word))
                throw new ArgumentException($"Expected keyword {word} but found {Tokenizer.CurrentToken.Value}");
            this.NextToken();
        }

        public string EatStringConstant()
        {
            if (!this.MatchStringConstant())
                throw new ArgumentException($"Expected string constant but found {Tokenizer.CurrentToken.Value}");
            string val = Tokenizer.CurrentToken.Value;
            this.NextToken();
            return val;
        }

        public bool MatchDelim(char delimiter)
        {
            return delimiter == Tokenizer.CurrentToken.Value?[0];
        }

        public bool MatchId()
        {
            return Tokenizer.CurrentToken.Type == TokenType.Word && !Keywords.Contains(Tokenizer.CurrentToken.Value, StringComparer.OrdinalIgnoreCase);
        }

        public bool MatchIntConstant()
        {
            return Tokenizer.CurrentToken.Type == TokenType.Number;
        }

        public bool MatchKeyword(string word)
        {
            return Tokenizer.CurrentToken.Type == TokenType.Word && Tokenizer.CurrentToken.Value.Equals(word, StringComparison.OrdinalIgnoreCase);
        }

        public bool MatchStringConstant()
        {
            return Tokenizer.CurrentToken.Value?[0] == '\'';
        }
    }
}
