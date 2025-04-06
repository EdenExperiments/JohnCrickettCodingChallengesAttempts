namespace ccJsonParser.Token
{
    public enum JsonTokenType
    {
        LeftBrace,      // {
        RightBrace,     // }
        LeftBracket,    // [
        RightBracket,   // ]
        Colon,          // :
        Comma,          // ,
        String,
        Number,
        True,
        False,
        Null,
        EndOfFile
    }
} 