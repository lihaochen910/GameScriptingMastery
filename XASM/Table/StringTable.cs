using System.Collections.Generic;

/// <summary>
/// 字符串表
/// </summary>
public class StringTable {
	
	public FastList< string > table;

	public StringTable () {
		table = new FastList < string >();
	}

	public int AddString ( string str ) {
		
		// 在链表中查找索引，如果没找到则返回链表的Count
		int idx = table.IndexOf ( str );
		
		if ( idx != -1 ) {
			return idx;
		}

		table.Push ( str );

		return ++idx;
	}
}
