public static class Messages {
	
	public const string UNEXPECTED_TOKEN         = "Unexpected token {0}";
	public const string UNEXPECTED_TOKEN_ILLEGAL = "Unexpected token ILLEGAL";
	
	public const string ERROR_MSG_LOCAL_SETSTACKSIZE = "set_stack_size 只能在全局范围内调用";
	
	public const string ERROR_MSG_FUNC_NAME_ERROR = "func 后面应该是函数名";
	public const string ERROR_MSG_FUNC_NOT_FOUND = "函数定义未找到:{0}";
	public const string ERROR_MSG_FUNC_DUPLICATE_DEFINITION = "函数名重复定义:{0}";
	public const string ERROR_MSG_FUNC_PARAM_DUPLICATE_DEFINITION = "函数参数重复定义:{0}";
	public const string ERROR_MSG_FUNC_VAR_DUPLICATE_DEFINITION = "函数内变量重复定义:{0}";
	public const string ERROR_MSG_FUNC_START_MISMATCH = "函数体定义应该以 { 开始";
	public const string ERROR_MSG_FUNC_END_MISMATCH = "} 只能出现在函数体结尾";
	public const string ERROR_MSG_FUNC_END_NOT_FOUND = "函数体定义没有正常结束";
	public const string ERROR_MSG_FUNC_PARAM_WRONG_LOCATION = "param 只能出现在函数体中";
	public const string ERROR_MSG_FUNC_PARAM_WRONG_FORMAT = "param 格式错误";
	
	public const string ERROR_MSG_VAR_NAME_ERROR = "var 后面应该是变量名";
	public const string ERROR_MSG_VAR_ARRAY_START_ERROR = "var数组应该以 [ 开始";
	public const string ERROR_MSG_VAR_ARRAY_SIZE_FORMAT_ERROR = "var数组长度类型应该是Int";
	public const string ERROR_MSG_VAR_ARRAY_END_ERROR = "var数组应该以 ] 结束";
	public const string ERROR_MSG_VAR_NOT_FOUND = "变量未找到:{0}";
	public const string ERROR_MSG_VAR_TYPE_IS_NOT_A_ARRAY = "该变量类型不是数组:{0}";
	public const string ERROR_MSG_VAR_ARRAY_INDEX_TYPE_NOT_SUPPORT = "数组索引类型不支持:{0}";
	public const string ERROR_MSG_GLOBAL_VAR_DUPLICATE_DEFINITION = "全局变量重复定义:{0}";
	
	public const string ERROR_MSG_LABEL_WRONG_FORMAT = "跳转标签后面应该是 :(冒号) ";
	public const string ERROR_MSG_LABEL_WRONG_LOCATION = "跳转标签 只能出现在函数体中";
	public const string ERROR_MSG_LABEL_DUPLICATE_DEFINITION = "跳转标签名重复定义:{0}";
	public const string ERROR_MSG_LABEL_NOT_FOUND = "跳转标签定义未找到:{0}";
	
	public const string ERROR_MSG_INVALID_TOKEN = "错误的输入???";
	public const string ERROR_MSG_INVALID_PARAM_TYPE = "无效的参数类型:{0}";

	public const string ERROR_MSG_INSTR_WRONG_LOCATION = "指令 只能出现在函数体中";
	public const string ERROR_MSG_INSTR_WRONG_PARAM = "指令:{0} 参数({1})类型错误: {2} ";
	public const string ERROR_MSG_INSTR_NOT_FOUND = "未找到指令定义:{0}";
	public const string ERROR_MSG_INSTR_PARAM_MISS_COMMA = "指令参数缺少逗号";
	public const string ERROR_MSG_INSTR_END_MISMATCH = "指令应该以换行符结束";
	
	public const string ERROR_MSG_STRING_END_NOT_FOUND = "String类型值定义缺少: \"";
	
	public const string ERROR_MSG_INDEX_OUT_OF_BOUNDS = "数组索引越界:{0}";
	
	public const string ERROR_MSG_RUNTIME_STACK_INDEX_OUT_OF_BOUNDS = "访问运行时堆栈值时索引越界:{0} 堆栈大小:{1}";
	
	public const string ERROR_MSG_STACK_OVERFLOW_WHEN_PUSH = "向堆栈中Push值时发生堆栈溢出";
	public const string ERROR_MSG_STACK_OVERFLOW_WHEN_PUSH_FRAME = "向堆栈中PushFrame值时发生堆栈溢出";
 }
