using System;

/// <summary>
/// Represents a source position as line number and column offset, where
/// the first line is 1 and first column is 0.
/// </summary>
/// <remarks>
/// A position where <see cref="Line"/> and <see cref="Column"/> are zero
/// is an allowed (and the default) value but considered an invalid
/// position.
/// </remarks>
public readonly struct SourcePosition : IEquatable<SourcePosition>
{
	public int Line   { get; }
	public int Column { get; }
	public int Index { get; }

	public SourcePosition(int line, int column, int index)
	{
		Line = line >= 0 ? line
			: throw new ArgumentOutOfRangeException(nameof(line), line, new ArgumentOutOfRangeException().Message);

		Column = column;

//            Column = line > 0 && column >= 0
//                     || line == 0 && column == 0 // if line is 0 then column MUST BE 0!
//                ? column
//                : throw new ArgumentOutOfRangeException(nameof(column), column, new ArgumentOutOfRangeException().Message);

		Index = index;
	}

	public override bool Equals(object obj) =>
		obj is SourcePosition other && Equals(other);

	public bool Equals(SourcePosition other) =>
		Line == other.Line && Column == other.Column;

	public override int GetHashCode() =>
		unchecked((Line * 397) ^ Column);

	public override string ToString()
		=> Line.ToString(System.Globalization.CultureInfo.InvariantCulture)
		   + ","
		   + Column.ToString(System.Globalization.CultureInfo.InvariantCulture);

	public static bool operator ==(SourcePosition left, SourcePosition right) => left.Equals(right);
	public static bool operator !=(SourcePosition left, SourcePosition right) => !left.Equals(right);

	public void Deconstruct(out int line, out int column)
	{
		line   = Line;
		column = Column;
	}
}

public readonly struct SourceSpan : IEquatable<SourceSpan>
{
	private readonly SourcePosition _end;
	private readonly SourcePosition _start;

	public SourcePosition End => _end;
	public int Length => _end.Index - _start.Index;
	public SourcePosition Start => _start;

	public SourceSpan (SourcePosition start, SourcePosition end) {
		_start = start;
		_end   = end;
	}

	public static bool operator != (SourceSpan left, SourceSpan right) {
		return !left.Equals ( right );
	}

	public static bool operator == (SourceSpan left, SourceSpan right) {
		return left.Equals ( right );
	}

	public override bool Equals (object obj) {
		if ( obj is SourceSpan ) {
			return Equals ( (SourceSpan)obj );
		}
		return base.Equals ( obj );
	}

	public bool Equals (SourceSpan other) {
		return other.Start == Start && other.End == End;
	}

	public override int GetHashCode () {
		return 0x509CE ^ Start.GetHashCode () ^ End.GetHashCode ();
	}

	public override string ToString () {
		return $"{_start.Line} {_start.Column}";
	}
}
