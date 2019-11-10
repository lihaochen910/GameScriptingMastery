using System.Collections.Generic;

/// <summary>
/// 标签表
/// </summary>
public class LabelTable {
	
	public FastList < LabelNode > table;

	public LabelTable ( ) {
		table = new FastList < LabelNode >();
	}

	public int AddLabel ( string ident, int targetIndex, int funcIndex ) {
		
		if ( GetLabelByIdent ( ident, funcIndex ) != null ) {
			return -1;
		}

		var newLabel = new LabelNode { ident = ident, targetIndex = targetIndex, funcIndex = funcIndex, index = table.Length };

		table.Push ( newLabel );

		return newLabel.index;
	}

	public LabelNode GetLabelByIdent ( string ident, int funcIndex ) {
		
		foreach ( LabelNode label in table ) {
			if ( label.ident == ident && label.funcIndex == funcIndex ) {
				return label;
			}
		}

		return null;
	}
	
}

/// <summary>
/// 函数表节点
/// </summary>
public class LabelNode {
	public int    index;       // 索引
	public string ident;       // 标识符
	public int    targetIndex; // 目标指令的索引
	public int    funcIndex;   // 标签所属的函数
}
