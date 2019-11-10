using System;
using System.Runtime.InteropServices;

/// <summary>
/// 操作数
/// </summary>
[StructLayout ( LayoutKind.Explicit, Size = 8 )]
public class Op {
	
	[FieldOffset ( 0 )] public OpType type;

	[FieldOffset ( 2 )] public int   intLiteral;       // 整型字面量
	[FieldOffset ( 2 )] public float floatLiteral;     // 浮点型字面量
	[FieldOffset ( 2 )] public int   stringTableIndex; // 字符串表索引
	[FieldOffset ( 2 )] public int   stackIndex;       // 堆栈索引
	[FieldOffset ( 2 )] public int   instrIndex;       // 指令索引
	[FieldOffset ( 2 )] public int   funcIndex;        // 函数索引
	[FieldOffset ( 2 )] public int   hostAPICallIndex; // 主应用程序API调用索引
	[FieldOffset ( 2 )] public int   reg;              // 寄存器码

	[FieldOffset ( 6 )] public int offsetIndex; // 偏移量的索引

	public static implicit operator int ( Op op ) {
		return op.intLiteral;
	}
	
	public static implicit operator float ( Op op ) {
		return op.floatLiteral;
	}

	public void CopyTo ( Op dest ) {
		dest.type = type;
		dest.intLiteral = intLiteral;
		dest.floatLiteral = floatLiteral;
		dest.stringTableIndex = stringTableIndex;
		dest.stackIndex = stackIndex;
		dest.instrIndex = instrIndex;
		dest.funcIndex = funcIndex;
		dest.funcIndex = funcIndex;
		dest.hostAPICallIndex = hostAPICallIndex;
		dest.reg = reg;
		dest.offsetIndex = offsetIndex;
	}

	public override string ToString () {
		string msg = string.Empty;
		switch ( type ) {
			case OpType.INT: msg += $"int: {intLiteral}"; break;
			case OpType.FLOAT: msg += $"float: {floatLiteral}"; break;
			case OpType.STRING: msg += $"string: {stringTableIndex}(idx)"; break;
			case OpType.ABS_STACK_INDEX: msg += $"abs_stack_idx: {stackIndex}"; break;
			case OpType.REL_STACK_INDEX: msg += $"rel_stack_idx: {stackIndex}"; break;
			case OpType.INSTR_INDEX: msg += $"instr_idx: {instrIndex}"; break;
			case OpType.FUNC_INDEX: msg += $"func_table_idx: {funcIndex}"; break;
			case OpType.HOST_API_CALL_INDEX: msg += hostAPICallIndex; break;
			case OpType.REG: msg += $"reg: {reg}"; break;
			case OpType.Null: msg += $"op null"; break;
		}
		
		return msg;
	}
}

/// <summary>
/// 操作数类型
/// </summary>
public enum OpType : Int16 {
	
	/// <summary>
	/// 整型
	/// </summary>
	INT = 1,
	
	/// <summary>
	/// 浮点型
	/// </summary>
	FLOAT = 2,
	
	/// <summary>
	/// 字符串型
	/// </summary>
	STRING = 4,
	
	/// <summary>
	/// 绝对堆栈索引(变量或者使用整型字面量索引的数组)
	/// </summary>
	ABS_STACK_INDEX = 8,
	
	/// <summary>
	/// 相对堆栈索引(使用变量索引的数组)
	/// </summary>
	REL_STACK_INDEX = 16,
	
	/// <summary>
	/// 指令索引(用于跳转目标)
	/// </summary>
	INSTR_INDEX = 32,
	
	/// <summary>
	/// 函数索引(用于Call指令中)
	/// </summary>
	FUNC_INDEX = 64,
	
	/// <summary>
	/// 主应用程序API调用索引(用于CallHost指令中)
	/// </summary>
	HOST_API_CALL_INDEX = 128,
	
	/// <summary>
	/// 寄存器
	/// </summary>
	REG = 256,
	
	/// <summary>
	/// 空值
	/// </summary>
	Null = 512
}

public enum OpFlagType : Int16 {
	INT           = 1,
	FLOAT         = 2,
	STRING        = 4,
	MEM_REF       = 8,
	LINE_LABEL    = 16,
	FUNC_NAME     = 32,
	HOST_API_CALL = 64,
	REG           = 128
}

public enum OpCode {
	Mov,
	
	Add,
	Sub,
	Mul,
	Div,
	Mod,
	Exp,
	Neg,
	Inc,
	Dec,
	
	And,
	Or,
	XOr,
	Not,
	ShL,
	ShR,
	
	Concat,
	GetChar,
	SetChar,
	
	Jmp,
	JE,
	JNE,
	JG,
	JL,
	JGE,
	JLE,
	
	Push,
	Pop,
	
	Call,
	Ret,
	CallHost,
    	
	Pause,
	Exit
}
