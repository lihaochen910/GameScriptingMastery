using System.Collections.Generic;

/// <summary>
/// 指令
/// </summary>
public class Instr {
	public OpCode opCode;
	public int opCount;
	public List<Op> opList;

	public override string ToString ( ) {
		return $"指令:{opCode} 参数:{opCount}";
	}
}
