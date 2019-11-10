using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 符号表
/// </summary>
public class SymbolTable {
	
	public FastList< SymbolNode > table;

	public SymbolTable () {
		table = new FastList < SymbolNode >();
	}

	public int AddSymbol ( string ident, int size, int stackIndex, int funcIndex ) {
		
		if ( GetSymbolByIdent ( ident, funcIndex ) != null ) {
			return -1;
		}
		
		var newSym = new SymbolNode { ident = ident, stackIndex = stackIndex, funcIndex = funcIndex, index = table.Length };

		table.Push ( newSym );

		return newSym.index;
	}

	public SymbolNode GetSymbolByIdent ( string ident, int funcIndex ) {
		
		foreach ( SymbolNode sym in table ) {
			if ( sym.ident == ident ) {
				if ( sym.funcIndex == funcIndex || sym.stackIndex >= 0 ) {
					return sym;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// 获取符号堆栈索引
	/// </summary>
	/// <param name="ident"></param>
	/// <param name="funcIndex"></param>
	/// <returns></returns>
	public int GetStackIndexByIdent ( string ident, int funcIndex ) {
		var sym = GetSymbolByIdent ( ident, funcIndex );
		return sym != null ? sym.stackIndex : -1;
	}
	
	/// <summary>
	/// 获取符号大小
	/// </summary>
	/// <param name="ident"></param>
	/// <param name="funcIndex"></param>
	/// <returns></returns>
	public int GetSizeByIdent ( string ident, int funcIndex ) {
		var sym = GetSymbolByIdent ( ident, funcIndex );
		return sym != null ? sym.size : -1;
	}
	
}

/// <summary>
/// 符号表节点
/// </summary>
public class SymbolNode {
	public int    index;         // 索引
	public string ident;          // 标识符
	public int    size;    		// 大小
	public int    stackIndex;    // 符号指向的堆栈索引
	public int    funcIndex; // 符号所在函数
}
