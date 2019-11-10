using System;

/// <summary>
/// 运行时堆栈
/// </summary>
public class RuntimeStack {
	
	public FastList< Op > elements;
	public int            size;
	public int            topIndex;
	public int            frameIndex; 	// 当前堆栈框架顶部的索引
	public bool           dynamicStack; // 动态增长的堆栈大小?

	public void SetStackSize ( int size = 1024 ) {
		elements = new FastList < Op >( size );
//		for ( var i = 0; i < size; ++i ) {
//			elements [ i ] = new Op { type = OpType.Null };
//		}
	}
	
	public Op GetStackValue ( int stackIdx ) {
		int realIdx = ResolveStackIndex ( stackIdx );
		if ( realIdx < 0 || realIdx >= elements.Capacity ) {
			throw new IndexOutOfRangeException ( string.Format ( Messages.ERROR_MSG_RUNTIME_STACK_INDEX_OUT_OF_BOUNDS, realIdx, elements.Capacity ) );
		}

		if ( elements [ realIdx ] == null ) {
			elements [ realIdx ] = new Op { type = OpType.Null };
		}
		return elements [ realIdx ];
	}

	public void SetStackValue ( int stackIdx, Op val ) {
		int realIdx = ResolveStackIndex ( stackIdx );
		if ( realIdx < 0 || realIdx >= elements.Capacity ) {
			throw new IndexOutOfRangeException ( string.Format ( Messages.ERROR_MSG_RUNTIME_STACK_INDEX_OUT_OF_BOUNDS, realIdx, elements.Capacity ) );
		}
		
//		Console.WriteLine ( $"SetStackValue() 在堆栈索引 [{realIdx}] 处写入值: {val}" );
		elements [ realIdx ] = val;
	}
	
	public void Push ( Op val ) {
		
		if ( topIndex < 0 || topIndex >= elements.Capacity ) {
			throw new IndexOutOfRangeException ( string.Format ( Messages.ERROR_MSG_RUNTIME_STACK_INDEX_OUT_OF_BOUNDS, topIndex, elements.Capacity ) );
		}
		
		elements [ topIndex ] = val;
		++topIndex;

		if ( topIndex >= elements.Capacity ) {
			throw new StackOverflowException ( Messages.ERROR_MSG_STACK_OVERFLOW_WHEN_PUSH );
		}
		
//		Console.WriteLine ( $"Push() 将值: {val} 压入堆栈索引 [{topIndex - 1}] 处" );
	}

	public Op Peek () {
		if ( topIndex - 1 < 0 ) {
			return elements [ 0 ];
		}
		return elements [ topIndex ];
	}
	
	public Op Pop () {
		
		--topIndex;
		
		if ( topIndex < 0 || topIndex >= elements.Capacity ) {
			throw new IndexOutOfRangeException ( string.Format ( Messages.ERROR_MSG_RUNTIME_STACK_INDEX_OUT_OF_BOUNDS, topIndex, elements.Capacity ) );
		}
		
//		Console.WriteLine ( $"Pop() 将值: {elements [ topIndex ]} 弹出堆栈, 栈顶索引 [{topIndex}]" );
		return elements [ topIndex ];
	}
	
	public void PushFrame ( int size ) {
		topIndex   += size;
		frameIndex = topIndex;
		
		if ( topIndex >= elements.Capacity ) {
			throw new StackOverflowException ( Messages.ERROR_MSG_STACK_OVERFLOW_WHEN_PUSH_FRAME );
		}

		for ( var i = topIndex - size; i < topIndex; ++i ) {
			if ( elements [ i ] == null ) {
				elements [ i ] = new Op { type = OpType.Null };
			}
		}
		
//		Console.WriteLine ( $"PushFrame() 栈顶索引增加 [{topIndex - size}] => [{topIndex}]" );
	}
	
	public void PopFrame ( int size ) {
		topIndex -= size;
//		Console.WriteLine ( $"PopFrame() 栈顶索引减少 [{topIndex + size}] => [{topIndex}]" );
	}

	#region Index
	private int GetRealIdx ( int absStackIdx ) {

		return topIndex + absStackIdx;
		
		if ( absStackIdx >= 0 ) {
			return absStackIdx;
		}

		return elements.Length + absStackIdx;
	}
	
	public int ResolveStackIndex ( int stackIdx ) {
		return stackIdx > 0 ? stackIdx : stackIdx + frameIndex;
	}
	#endregion
	
	public override string ToString () {
		return $"Runtime堆栈信息: 栈顶索引:{topIndex} 栈顶元素:{(topIndex != 0 ? elements[topIndex - 1] : elements[topIndex])} 堆栈帧:{frameIndex}";
	}
}
