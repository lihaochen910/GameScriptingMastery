using System.Collections.Generic;

/// <summary>
/// 函数表
/// </summary>
public class FuncTable {

	/// <summary>
	/// 添加函数时，错误代码
	/// </summary>
	public const int ERROR_IDX = -1;
	
	public FastList< FuncNode > table;

	public FuncTable () {
		table = new FastList < FuncNode >();
	}

	public int AddFunc ( string name, int entryPoint ) {
		
		if ( GetFuncByName ( name ) != null ) {
			return ERROR_IDX;
		}
		
		var newFunc = new FuncNode { name = name, entryPoint = entryPoint, index = table.Length };

		table.Push ( newFunc );

		return newFunc.index;
	}

	public FuncNode GetFuncByName ( string name ) {
		
		foreach ( FuncNode func in table ) {
			if ( func.name == name ) {
				return func;
			}
		}

		return null;
	}
	

	public void SetFuncInfo ( string name, int paramCount, int localDataSize ) {
		
		var func = GetFuncByName ( name );

		if ( func == null ) {
			return;
		}
		
		func.paramCount = paramCount;
		func.localDataSize = localDataSize;
	}
}

/// <summary>
/// 函数表节点
/// </summary>
public class FuncNode {
	public int index;			// 索引
	public string name;			// 名称
	public int entryPoint;		// 入口点
	public int paramCount;		// 参数个数
	public int localDataSize;	// 局部数据大小
	public int stackFrameSize;	// 局部数据大小
}
