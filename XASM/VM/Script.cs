
public class Script {
	
	public int globalDataSize;
	
	public bool isMainFuncPresent;
	public int  mainFuncIndex;
	public bool isPaused;
	public int  pauseEndTime;

	public Op _ret_val;		// 寄存器

	public InstrStream      instrStream;
	public RuntimeStack    stack;
	public FuncTable        funcTable;
	public HostAPICallTable hostApiCallTable;
	
	public SymbolTable      symbolTable;
	public LabelTable       labelTable;
	public StringTable      stringTable;

	public Script () {
		
		// 寄存器初始化
		_ret_val = new Op { type = OpType.Null };
		
		funcTable   = new FuncTable ();
		symbolTable = new SymbolTable ();
		labelTable  = new LabelTable ();
		stringTable = new StringTable ();
		instrStream = new InstrStream ();
		stack       = new RuntimeStack ();
		
		// 初始化运行时堆栈大小
		stack.SetStackSize ( 1024 );
	}
	
}
