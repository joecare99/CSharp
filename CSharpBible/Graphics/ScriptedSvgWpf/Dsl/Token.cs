namespace ScriptedSvgWpf.Dsl;

public enum TokenKind
{
    EndOfFile,
    Identifier,
    Integer,
    Float,
    String,
    Plus,
    PlusPlus,
    PlusEqual,
    Minus,
    MinusMinus,
    MinusEqual,
    Star,
    StarEqual,
    Slash,
    SlashEqual,
    Percent,
    Bang,
    Equal,
    EqualEqual,
    BangEqual,
    Less,
    LessEqual,
    Greater,
    GreaterEqual,
    AndAnd,
    OrOr,
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Comma,
    Semicolon,
    Dot
}

public readonly record struct Token(TokenKind Kind, string Lexeme, int Position, int Line, int Column)
{
    public override string ToString() => $"{Kind} '{Lexeme}' ({Line},{Column})";
}
