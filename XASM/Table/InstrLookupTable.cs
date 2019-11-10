using System.Collections.Generic;

/// <summary>
/// 指令查找表
/// </summary>
public class InstrLookupTable {
	
	/// <summary>
	/// 查找表将保存的指令的最大个数
	/// </summary>
	public const int MAX_INSTR_LOOKUP_COUNT = 256;

	/// <summary>
	/// 指令助记符字符串的最大长度
	/// </summary>
	public const int MAX_INSTR_MNEMONIC_SIZE = 16;

	private static InstrLookup[]         instrTable;
	private static int                   instrIndex; // 使用一个静态变量来保存表中下一条指令的索引
	
	public static void InitInstrTable () {

        instrIndex = 0;
		
		instrTable = new InstrLookup[ MAX_INSTR_LOOKUP_COUNT ];
		
		for ( int i = 0; i < MAX_INSTR_LOOKUP_COUNT; ++i ) {
			instrTable [ i ] = new InstrLookup ();
		}

        int localInstrIdx = instrIndex;

        /*
		 * Mov	Destination, Source
		 */
        localInstrIdx = AddInstrLookup( OpCode.Mov.ToString (), OpCode.Mov, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );

		
		// ---- Arithmetic
		/*
		 * Add	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Add.ToString (), OpCode.Add, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Sub	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Sub.ToString (), OpCode.Sub, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Mul	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Mul.ToString (), OpCode.Mul, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Div	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Div.ToString (), OpCode.Div, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Mod	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Mod.ToString (), OpCode.Mod, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Exp	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Exp.ToString (), OpCode.Exp, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Neg	Destination
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Neg.ToString (), OpCode.Neg, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Inc	Destination
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Inc.ToString (), OpCode.Inc, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Dec	Destination
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Dec.ToString (), OpCode.Dec, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		// ---- Bitwise
		/*
		 * And	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.And.ToString (), OpCode.And, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Or	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Or.ToString (), OpCode.Or, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * XOr	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.XOr.ToString (), OpCode.XOr, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Not	Destination
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Not.ToString (), OpCode.Not, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * ShL	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.ShL.ToString (), OpCode.ShL, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * ShR	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.ShR.ToString (), OpCode.ShR, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		// ---- String
		/*
		 * Concat	Destination, Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Concat.ToString (), OpCode.Concat, 2 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * GetChar	Destination, Source, Index
		 */
		localInstrIdx = AddInstrLookup ( OpCode.GetChar.ToString (), OpCode.GetChar, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.INT |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * SetChar	Destination, Source, Index
		 */
		localInstrIdx = AddInstrLookup ( OpCode.SetChar.ToString (), OpCode.SetChar, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		// ---- Conditional Branching
		/*
		 * Jmp	Label
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Jmp.ToString (), OpCode.Jmp, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.LINE_LABEL );
		
		/*
		 * JE	Op0, Op1, Label
		 */
		localInstrIdx = AddInstrLookup ( OpCode.JE.ToString (), OpCode.JE, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.LINE_LABEL );
		
		/*
		 * JNE	Op0, Op1, Label
		 */
		localInstrIdx = AddInstrLookup ( OpCode.JNE.ToString (), OpCode.JNE, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.LINE_LABEL );
		
		/*
		 * JG	Op0, Op1, Label
		 */
		localInstrIdx = AddInstrLookup ( OpCode.JG.ToString (), OpCode.JG, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.LINE_LABEL );
		
		/*
		 * JL	Op0, Op1, Label
		 */
		localInstrIdx = AddInstrLookup ( OpCode.JL.ToString (), OpCode.JL, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.LINE_LABEL );
		
		/*
		 * JGE	Op0, Op1, Label
		 */
		localInstrIdx = AddInstrLookup ( OpCode.JGE.ToString (), OpCode.JGE, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.LINE_LABEL );
		
		/*
		 * JLE	Op0, Op1, Label
		 */
		localInstrIdx = AddInstrLookup ( OpCode.JLE.ToString (), OpCode.JLE, 3 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 1, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		SetOpType ( localInstrIdx, 2, OpFlagType.LINE_LABEL );
		
		// ---- The Stack Interface
		/*
		 * Push	Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Push.ToString (), OpCode.Push, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Pop	Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Pop.ToString (), OpCode.Pop, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		// ---- The Function Interface
		/*
		 * Call	Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Call.ToString (), OpCode.Call, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.FUNC_NAME );
		
		/*
		 * Ret	Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Ret.ToString (), OpCode.Ret, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.FUNC_NAME );
		
		/*
		 * CallHost	Source
		 */
		localInstrIdx = AddInstrLookup ( OpCode.CallHost.ToString (), OpCode.CallHost, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.HOST_API_CALL );
		
		// ---- Miscellaneous
		/*
		 * Pause	Duration
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Pause.ToString (), OpCode.Pause, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
		
		/*
		 * Exit	Code
		 */
		localInstrIdx = AddInstrLookup ( OpCode.Exit.ToString (), OpCode.Exit, 1 );
		SetOpType ( localInstrIdx, 0, OpFlagType.INT |
		                           OpFlagType.FLOAT |
		                           OpFlagType.STRING |
		                           OpFlagType.MEM_REF |
		                           OpFlagType.REG );
	}

	public static int AddInstrLookup ( string mnemonic, OpCode opCode, int opCount ) {
		if ( instrIndex >= MAX_INSTR_LOOKUP_COUNT ) {
			return -1;
		}

		var instr = instrTable [ instrIndex ];
		if ( instr == null ) {
			instr                     = new InstrLookup ();
			instrTable [ instrIndex ] = instr;
		}

		instr.mnemonic = mnemonic;
		instr.opCode   = opCode;
		instr.opCount  = opCount;
		instr.opList   = new OpFlagType[ opCount ];

		return instrIndex++;
	}

	public static void SetOpType ( int instrIndex, int opIndex, OpFlagType opFlagType ) {
		instrTable [ instrIndex ].opList [ opIndex ] = opFlagType;
	}

	public bool GetInstrByMnemonic ( string mnemonic, out InstrLookup instrLookup ) {
		
		for ( int i = 0; i < MAX_INSTR_LOOKUP_COUNT; ++i ) {
			if ( instrTable [ i ].mnemonic == mnemonic ) {
				instrLookup = instrTable [ i ];
				return true;
			}
		}

		instrLookup = null;
		return false;
	}
	
}

/// <summary>
/// 查找指令
/// </summary>
public class InstrLookup {
	public string   mnemonic; // 助记字符串
	public OpCode   opCode;   // 操作码
	public int      opCount;  // 操作数个数
	public OpFlagType[] opList;   // 操作数列表指针
}
