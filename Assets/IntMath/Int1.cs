using System;

[Serializable]
public struct Int1
{
	public int i;

	public float scalar
	{
		get
		{
			return (float)this.i * 0.001f;
		}
	}

	public Int1(int i)
	{
		this.i = i;
	}

	public Int1(float f)
	{
		this.i = (int)Math.Round((double)(f * 1000f));
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		Int1 vInt = (Int1)o;
		return this.i == vInt.i;
	}

	public override int GetHashCode()
	{
		return this.i.GetHashCode();
	}

	public static Int1 Min(Int1 a, Int1 b)
	{
		return new Int1(Math.Min(a.i, b.i));
	}

	public static Int1 Max(Int1 a, Int1 b)
	{
		return new Int1(Math.Max(a.i, b.i));
	}

	public override string ToString()
	{
		return this.scalar.ToString();
	}

	public static explicit operator Int1(float f)
	{
		return new Int1((int)Math.Round((double)(f * 1000f)));
	}

	public static implicit operator Int1(int i)
	{
		return new Int1(i);
	}

	public static explicit operator float(Int1 ob)
	{
		return (float)ob.i * 0.001f;
	}

	public static explicit operator long(Int1 ob)
	{
		return (long)ob.i;
	}

	public static Int1 operator +(Int1 a, Int1 b)
	{
		return new Int1(a.i + b.i);
	}

	public static Int1 operator -(Int1 a, Int1 b)
	{
		return new Int1(a.i - b.i);
	}

	public static bool operator ==(Int1 a, Int1 b)
	{
		return a.i == b.i;
	}

	public static bool operator !=(Int1 a, Int1 b)
	{
		return a.i != b.i;
	}
}
