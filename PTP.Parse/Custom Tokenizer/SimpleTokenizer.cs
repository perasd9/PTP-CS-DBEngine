namespace PTP.Parse.Custom_Tokenizer
{
    public enum TokenType
    {
        Word,
        Number,
        Symbol,
        Whitespace,
        EndOfFile
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
    }

    public class SimpleTokenizer
    {
        private TextReader _reader;
        private int _currentChar;
        public Token CurrentToken { get; set; }

        public SimpleTokenizer(TextReader reader)
        {
            _reader = reader;
            _currentChar = _reader.Read();
        }

        public Token NextToken()
        {
            while(char.IsWhiteSpace((char)_currentChar))
            {
                _currentChar = _reader.Read();
            }

            if (_currentChar == -1)
            {
                CurrentToken = new Token() { Type = TokenType.EndOfFile, Value = null };
                return CurrentToken;
            }

            char ch = (char)_currentChar;

            if(char.IsDigit(ch))
            {
                string number = "";

                while(char.IsDigit((char) _currentChar))
                {
                    number += _currentChar;

                    _currentChar = _reader.Read();
                }
                CurrentToken = new Token() { Type = TokenType.EndOfFile, Value = number };

                return CurrentToken;
            }

            if(char.IsLetter(ch))
            {
                string word = "";

                while (char.IsLetter((char)_currentChar))
                {
                    word += _currentChar;

                    _currentChar = _reader.Read();
                }
                CurrentToken = new Token() { Type = TokenType.EndOfFile, Value = word };

                return CurrentToken;
            }

            _currentChar = _reader.Read();

            CurrentToken = new Token() { Type = TokenType.EndOfFile, Value = ch.ToString() };

            return CurrentToken;
        }
    }
}
