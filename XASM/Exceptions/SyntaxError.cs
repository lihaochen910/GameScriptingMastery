
using System;

public class SyntaxError : Exception
{
	public SyntaxError(string message)
		: base($"{message}\n)") {}

	public SyntaxError(string message, int line, int column)
		: base($"{message}\n({line},{column})") {}
}
