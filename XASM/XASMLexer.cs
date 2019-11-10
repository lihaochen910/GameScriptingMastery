using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/// <summary>
/// 词法分析器，用于解析源文件中每一个有效的单词
/// </summary>
public class XASMLexer {
	
	public const char Quote         = '\'';
	public const char DoubleQuote   = '"';
	public const char Colon         = ':';
	public const char StartComment  = '#';
	public const char EndOfLine     = '\n';
	public const char Comma         = ',';
	public const char Open_Bracket  = '[';
	public const char Close_Bracket = ']';
	public const char Open_Brace    = '{';
	public const char Close_Brace   = '}';

	private const string Separators = ";()[],.|{}";
	private const string Keyword_Var = "var";
	private const string Keyword_Func = "func";
	private const string Keyword_Param = "param";
	private const string Keyword_RetVal = "_ret_val";
	private const string Keyword_SetStackSize = "set_stack_size";
	public const string Keyworkd_Main = "main";

	private static string[] operators = new string[]
		{ "+", "-", "*", "/", "=", "<", ">", "!", "==", "<=", ">=", "!=", "=>", ".." };

	private TextCharStream  stream;
	private Stack< Token >  tokens = new Stack < Token >();
	private SourcePosition  tokenStartPosition;

	public XASMLexer ( string text ) {
		stream = new TextCharStream ( text );
	}
	
	public SourcePosition LexerPosition {
		get {
			int nLine         = 1;
			int nLastLineChar = 0;
			for ( int i = 0; i < stream.Position; ++i ) {
				if ( stream.Data [ i ].Equals ( EndOfLine ) ) {
					nLine++;
					nLastLineChar = i;
				}
			}

			int nCol = stream.Position - nLastLineChar;
			return new SourcePosition ( nLine, nCol, stream.Position );
		}
	}

	public Token NextToken () {

		if ( tokens.Count > 0 ) {
			return tokens.Pop ();
		}
		
		tokenStartPosition = LexerPosition;

		int ich = NextFirstChar ();

		// 到达字符流结尾
		if ( ich == -1 ) {
			return new Token ( TokenType.EOF, string.Empty, new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		char ch = ( char )ich;

		if ( ch == EndOfLine ) {
			return new Token ( TokenType.NEWLINE, EndOfLine.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}

		if ( ch == Comma ) {
			return new Token ( TokenType.COMMA, Comma.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( ch == Colon ) {
			return new Token ( TokenType.COLON, Colon.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
//		if ( ch == Quote ) {
//			return new Token ( TokenType.QUOTE, Quote.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
//		}
//		
//		if ( ch == DoubleQuote ) {
//			return new Token ( TokenType.QUOTE, DoubleQuote.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
//		}
		
		if ( ch == Open_Bracket ) {
			return new Token ( TokenType.OPEN_BRACKET, Open_Bracket.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( ch == Close_Bracket ) {
			return new Token ( TokenType.CLOSE_BRACKET, Close_Bracket.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( ch == Open_Brace ) {
			return new Token ( TokenType.OPEN_BRACE, Open_Brace.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( ch == Close_Brace ) {
			return new Token ( TokenType.CLOSE_BRACE, Close_Brace.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}

		if ( ch == Quote ) {
			return NextString ( Quote );
		}

		if ( ch == DoubleQuote ) {
			return NextString ( DoubleQuote );
		}

        /*
		if (operators.Contains(ch.ToString()))
		{
		    string value = ch.ToString();
		    ich = NextChar();

		    if (ich >= 0)
		    {
		        value += (char)ich;
		        if ( operators.Contains ( value ) ) {
		            tokenEndPosition = LexerPosition;
		            return new Token(TokenType.Operator, value, new SourceSpan ( tokenStartPosition, tokenEndPosition ));
		        }

		        BackChar();
		    }
		    
		    tokenEndPosition = LexerPosition;
		    return new Token(TokenType.Operator, ch.ToString(), new SourceSpan ( tokenStartPosition, tokenEndPosition ));
		}
		else if (operators.Any(op => op.StartsWith(ch.ToString())))
		{
		    string value = ch.ToString();
		    ich = NextChar();

		    if (ich >= 0)
		    {
		        value += (char)ich;
		        if ( operators.Contains ( value ) ) {
		            tokenEndPosition = LexerPosition;
		            return new Token(TokenType.Operator, value, new SourceSpan ( tokenStartPosition, tokenEndPosition ));
		        }

		        BackChar();
		    }
		}

		if ( Separators.Contains ( ch ) ) {
		    tokenEndPosition = LexerPosition;
		    return new Token(TokenType.Separator, ch.ToString(), new SourceSpan ( tokenStartPosition, tokenEndPosition ));
		}
		*/

        if ( Character.IsDecimalDigit ( ch ) ) {
	        return NextNumericLiteral ( ch );
        }

        if ( Character.IsIdentifierStart ( ch ) ) {
	        return NextIdentifier ( ch );
        }

		throw new SyntaxError ( string.Format ( "unexpected '{0}'", ch ), tokenStartPosition.Index, stream.Position );
	}

	/// <summary>
	/// 移动解析器位置到给定Token的开头
	/// </summary>
	public void RollingBackToTheTokenStart ( Token token ) {
		
		if ( token == null ) {
			return;
		}
		
		stream.SetPosition ( token.span.Start.Index );
	}
	
	/// <summary>
	/// 移动解析器位置到给定Token的结尾
	/// </summary>
	public void RollingBackToTheTokenEnd ( Token token ) {
		
		if ( token == null ) {
			return;
		}
		
		stream.SetPosition ( token.span.End.Index );
	}

	public void Reset () {
		stream.SetPosition ( 0 );
		tokens.Clear ();
	}

	public void PushToken ( Token token ) {
		tokens.Push ( token );
	}

	public char PeekChar ( int num ) {
		return stream.Peek ( num );
	}
	
	public void ThrowUnexpectedToken ( string message = Messages.UNEXPECTED_TOKEN_ILLEGAL ) {
		throw new SyntaxError ( message, LexerPosition.Line, LexerPosition.Column );
	}
	
	private Token NextIdentifier ( char ch ) {
		string value = ch.ToString ();
		int    ich;

		for ( ich = NextChar (); ich >= 0 && ( ( char ) ich == '_' || char.IsLetterOrDigit ( ( char ) ich ) ); ich = NextChar () ) {
			value += ( char )ich;
		}
		
		if ( ich >= 0 )
			BackChar ();

		if ( value == Keyword_Var ) {
			return new Token ( TokenType.VAR, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( value == Keyword_Func ) {
			return new Token ( TokenType.FUNC, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( value == Keyword_Param ) {
			return new Token ( TokenType.PARAM, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( value == Keyword_RetVal ) {
			return new Token ( TokenType.REG_RETVAL, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}
		
		if ( value == Keyword_SetStackSize ) {
			return new Token ( TokenType.SET_STACK_SIZE, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}

		foreach ( OpCode opCode in Enum.GetValues ( typeof ( OpCode ) ) ) {
			if ( value == opCode.ToString () ) {
				return new Token ( TokenType.INSTR, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
			}
		}
		
//		Console.WriteLine ( $"NextIdentifier() return TokenType.IDENT = {value} {LexerPosition}" );
		return new Token ( TokenType.IDENT, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
	}

	private Token NextString ( char init ) {
		string value = string.Empty;
		int    ich;

		for ( ich = NextChar (); ich >= 0 && ( char ) ich != init; ich = NextChar () ) {
			char ch = ( char ) ich;

			// 跳过转义字符\n\t\r
			if ( ch == '\\' ) {
				int ich2 = NextChar ();

				if ( ich2 > 0 ) {
					char ch2 = ( char ) ich2;

					if ( ch2 == 't' ) {
						value += '\t';
						continue;
					}

					if ( ch2 == 'r' ) {
						value += '\r';
						continue;
					}

					if ( ch2 == 'n' ) {
						value += '\n';
						continue;
					}

					value += ch2;
					continue;
				}
			}

			value += ( char ) ich;
		}

		if ( ich < 0 ) {
			throw new SyntaxError ( "unclosed string" );
		}

		return new Token ( TokenType.STRING, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
	}

	private Token NextNumericLiteral ( char ch ) {
		StringBuilder sb = new StringBuilder ();
		bool          isFloat = false;
		
		if ( ch != '.' ) {
			
            var first = ch;
            sb.Append ( first );
            ch = PeekChar ( 1 );

            // Hex number starts with '0x'.
            // Octal number starts with '0'.
            // Octal number in ES6 starts with '0o'.
            // Binary number in ES6 starts with '0b'.
            if ( first == '0' ) {
	            
                if ( ch == 'x' || ch == 'X' ) {
	                NextChar ();
                    return ScanHexLiteral ();
                }

                if ( ch == 'b' || ch == 'B' )  {
	                NextChar ();
                    return ScanBinaryLiteral ();
                }
            }

            ch = ( char )NextChar ();
            while ( Character.IsDecimalDigit ( ch ) ) {
	            sb.Append ( ch );
                ch = ( char )NextChar ();
            }
            
            BackChar ();
		}

		ch = PeekChar ( 1 );
        if ( ch == '.' ) {
	        NextChar ();
            sb.Append ( ch );
            
            while ( Character.IsDecimalDigit ( PeekChar ( 1 ) ) ) {
	            ch = ( char )NextChar ();
	            sb.Append ( ch );
            }
            
            isFloat = true;
        }

//        ch = PeekChar ( 1 );
//        if ( ch == 'e' || ch == 'E' ) {
//	        NextChar ();
//	        sb.Append ( ch );
//
//	        ch = PeekChar ( 1 );
//            if ( ch == '+' || ch == '-' ) {
//	            ch = ( char )NextChar ();
//	            sb.Append ( ch );
//            }
//
//            if ( Character.IsDecimalDigit ( PeekChar ( 1 ) ) ) {
//                while ( Character.IsDecimalDigit ( PeekChar ( 1 ) ) ) {
//	                ch = ( char )NextChar ();
//	                sb.Append ( ch );
//                }
//            }
//            else {
//                ThrowUnexpectedToken();
//            }
//        }
        
        var token = new Token ( isFloat ? TokenType.FLOAT : TokenType.INT, string.Empty, new SourceSpan ( tokenStartPosition, LexerPosition ) );
        var number = sb.ToString ();

        if (long.TryParse(
            number,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out var l)) {
	        token.value = l.ToString ();
        }
        else if (double.TryParse(
            number, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out var d))
        {
	        token.value = d.ToString ();
        }
        else
        {
            d = number.TrimStart().StartsWith("-")
                ? double.NegativeInfinity
                : double.PositiveInfinity;

            token.value = d.ToString ();
        }

        return token;
	}

	private Token NextFloat ( string ivalue ) {
		string value = ivalue + ".";
		int    ich;

		for ( ich = NextChar (); ich >= 0 && char.IsDigit ( ( char ) ich ); ich = NextChar () )
			value += ( char ) ich;

		if ( ich >= 0 )
			BackChar ();

		if ( value.EndsWith ( "." ) ) {
			BackChar ();
			return new Token ( TokenType.INT, ivalue, new SourceSpan ( tokenStartPosition, LexerPosition ) );
		}

		return new Token ( TokenType.FLOAT, value, new SourceSpan ( tokenStartPosition, LexerPosition ) );
	}
	
	// https://tc39.github.io/ecma262/#sec-literals-numeric-literals

	private Token ScanHexLiteral () {

		string number = string.Empty;

		while ( !Eof () ) {
			
			if ( !Character.IsHexDigit ( PeekChar ( 1 ) ) ) {
				break;
			}

			number += NextChar ();
		}
		
		if ( number.Length == 0 ) {
			ThrowUnexpectedToken ();
		}
		
		double value = 0;

		if (number.Length < 16)
		{
			value = Convert.ToInt64(number, 16);
		}
		else if (number.Length > 255)
		{
			value = double.PositiveInfinity;
		}
		else
		{
			double modulo  = 1;
			var    literal = number.ToLowerInvariant();
			var    length  = literal.Length - 1;
			for (var i = length; i >= 0; i--)
			{
				var c = literal[i];

				if (c <= '9')
				{
					value += modulo * (c - '0');
				}
				else
				{
					value += modulo * (c - 'a' + 10);
				}

				modulo *= 16;
			}
		}

		return new Token ( TokenType.INT, value.ToString (), new SourceSpan ( tokenStartPosition, LexerPosition ) );
	}
	
	public Token ScanBinaryLiteral () {
		string number = string.Empty;
		char ch = char.MinValue;
		
		while ( !Eof () ) {
			
			ch = PeekChar ( 1 );
			if ( ch != '0' && ch != '1' ) {
				break;
			}

			number += NextChar ();
		}
		
		if ( number.Length == 0 ) {
			// only 0b or 0B
			ThrowUnexpectedToken ();
		}

		if ( !Eof () ) {
			
			ch = PeekChar ( 1 );
			
			/* istanbul ignore else */
			if ( Character.IsIdentifierStart ( ch ) || Character.IsDecimalDigit ( ch ) ) {
				ThrowUnexpectedToken ();
			}
		}

		return new Token ( TokenType.INT, number, new SourceSpan ( tokenStartPosition, LexerPosition ) );
	}

	/// <summary>
	/// 移动指针跳过后面空白符和换行符，以及注释
	/// </summary>
	/// <returns></returns>
	private int NextFirstChar () {
		int ich = NextChar ();

		while ( true ) {

			while ( ich > 0 && ( char )ich != '\n' && Character.IsWhiteSpace ( ( char )ich ) ) {
//				if ( char.IsWhiteSpace ( ( char )ich ) ) {
//					Console.WriteLine ( $"char.IsWhiteSpace: {ich} {( char )ich} ({LexerPosition.Line},{LexerPosition.Column})" );
//				}
				ich = NextChar ();
            }

//			Console.WriteLine ( $"空白符跳过处理结束，当前位置: {ich} {( char )ich} ({LexerPosition.Line},{LexerPosition.Column})" );

			if ( ich > 0 && ( char )ich == StartComment ) {
				for ( ich = stream.NextChar (); ich >= 0 && ( char )ich != '\n'; )
					ich = stream.NextChar ();

				if ( ich < 0 )
					return -1;

				continue;
			}

			break;
		}

//		Console.WriteLine ( $"NextFirstChar()返回值: {ich} {( char )ich} ({LexerPosition.Line},{LexerPosition.Column})" );

		return ich;
	}

	private int NextChar () {
		if ( stream.TheEnd () ) {
			return -1;
		}
		return stream.NextChar ();
	}

	private void BackChar () {
		stream.BackChar ();
	}

	private bool Eof () {
		return stream.TheEnd ();
	}

	private bool IsCharNumeric ( char c ) {
		return c >= '0' && c <= '9';
	}
	
	private bool IsCharWhitespace ( char c ) {
		return c == ' ' || c == '\t';
	}

	private bool IsCharIdent ( char c ) {
		return ( c >= '0' && c <= '9' ) ||
		       ( c >= 'a' && c <= 'z' ) ||
		       ( c >= 'A' && c <= 'Z' ) ||
		       c == '_';
	}

	private bool IsCharDelimiter ( char c ) {
		return c == ':' || c == ',' || c == ':' ||
		       c == '[' || c == ']' ||
		       c == '{' || c == '}' ||
		       IsCharWhitespace ( c );
	}
}
