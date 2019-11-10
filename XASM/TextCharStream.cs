using System;
using System.Runtime.CompilerServices;

public class TextCharStream {
	
	private string text;
	private int    position;

	public int Position {
		get => position;
	}

	public string Data {
		get => text;
	}

	public char Current {
		get => text [ position ];
	}

	public TextCharStream ( string input ) {
		text     = input;
		position = 0;
	}

	public bool SetPosition ( int pos ) {
		if ( pos >= 0 && pos <= text.Length ) {
			position = pos;
			return true;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TheEnd () {
		return position >= text.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public char NextChar () {
		return text [ position++ ];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void BackChar () {
		if ( position > 0 && position <= text.Length ) {
			position--;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public char Peek ( int number ) {
		
		int idx = position + ( number - 1 );
		
		if ( idx < 0 || idx > text.Length ) {
			return char.MinValue;
		}
		
		return text [ idx ];
	}

	public bool Peek ( int number, out char c ) {
		
		int idx = position + ( number - 1 );
		
		if ( idx > text.Length || idx < 0 ) {
			c = default;
			return false;
		}

		c = text [ idx ];
		return true;
	}
}
