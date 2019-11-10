using System;

public class Token {
	
	public string     value;
	public TokenType  type;
	public SourceSpan span;

	/*
	public Token(TokenType type, string value)
	{
		this.type  = type;
		this.value = value;
	}
	*/
        
	public Token ( TokenType type, string value, SourceSpan span ) {
		this.type = type;
		this.value = value;
		this.span = span;
	}

	public override string ToString () {
		return $"Token < {type} >: {(value == "\n" ? " " : value)}   ({span.Start.Line},{span.Start.Column})";
	}
}

public enum TokenType : byte {
	
	/// <summary>
	/// 引号
	/// </summary>
	QUOTE,	// ""
	
	/// <summary>
	/// 逗号
	/// </summary>
	COMMA,			// ,
	
	/// <summary>
	/// 冒号
	/// </summary>
	COLON,			// :
	
	/// <summary>
	/// 左中括号
	/// </summary>
	OPEN_BRACKET,	// [
	
	/// <summary>
	/// 右中括号
	/// </summary>
	CLOSE_BRACKET,	// ]
	
	/// <summary>
	/// 换行
	/// </summary>
	NEWLINE,		// \n
	
	/// <summary>
	/// 左大括号
	/// </summary>
	OPEN_BRACE,		// {
	
	/// <summary>
	/// 右大括号
	/// </summary>
	CLOSE_BRACE,	// }
	
	/// <summary>
	/// 整型字面量
	/// </summary>
	INT,
	
	/// <summary>
	/// 浮点型字面量
	/// </summary>
	FLOAT,
	
	/// <summary>
	/// 字符串字面量
	/// </summary>
	STRING,
	
	/// <summary>
	/// 标识符
	/// </summary>
	IDENT,
	
	/// <summary>
	/// 指令
	/// </summary>
	INSTR,
	
	/// <summary>
	/// SetStackSize关键字
	/// </summary>
	SET_STACK_SIZE,
	
	/// <summary>
	/// var关键字
	/// </summary>
	VAR,
	
	/// <summary>
	/// func关键字
	/// </summary>
	FUNC,
	
	/// <summary>
	/// param关键字
	/// </summary>
	PARAM,
	
	/// <summary>
	/// _RetVal寄存器
	/// </summary>
	REG_RETVAL,
	
	/// <summary>
	/// 错误编码
	/// </summary>
	INVALID,
	
	/// <summary>
	/// 文件结尾
	/// </summary>
	EOF
}
