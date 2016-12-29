using System;
using UnityEngine;

[Serializable]
public struct Int2
{
	public int x;

	public int y;

	public static Int2 zero = default(Int2);

	private static readonly int[] Rotations = new int[]
	{
		1,
		0,
		0,
		1,
		0,
		1,
		-1,
		0,
		-1,
		0,
		0,
		-1,
		0,
		-1,
		1,
		0
	};

	public int sqrMagnitude
	{
		get
		{
			return this.x * this.x + this.y * this.y;
		}
	}

	public long sqrMagnitudeLong
	{
		get
		{
			long num = (long)this.x;
			long num2 = (long)this.y;
			return num * num + num2 * num2;
		}
	}

	public int magnitude
	{
		get
		{
			long num = (long)this.x;
			long num2 = (long)this.y;
			return IntMath.Sqrt(num * num + num2 * num2);
		}
	}

	public Int2 normalized
	{
		get
		{
			Int2 result = new Int2(this.x, this.y);
			result.Normalize();
			return result;
		}
	}

	public Int2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static int Dot(Int2 a, Int2 b)
	{
		return a.x * b.x + a.y * b.y;
	}

	public static long DotLong(ref Int2 a, ref Int2 b)
	{
		return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
	}

	public static long DotLong(Int2 a, Int2 b)
	{
		return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
	}

	public static long DetLong(ref Int2 a, ref Int2 b)
	{
		return (long)a.x * (long)b.y - (long)a.y * (long)b.x;
	}

	public static long DetLong(Int2 a, Int2 b)
	{
		return (long)a.x * (long)b.y - (long)a.y * (long)b.x;
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		Int2 vInt = (Int2)o;
		return this.x == vInt.x && this.y == vInt.y;
	}

	public override int GetHashCode()
	{
		return this.x * 49157 + this.y * 98317;
	}

	public static Int2 Rotate(Int2 v, int r)
	{
		r %= 4;
		return new Int2(v.x * Int2.Rotations[r * 4] + v.y * Int2.Rotations[r * 4 + 1], v.x * Int2.Rotations[r * 4 + 2] + v.y * Int2.Rotations[r * 4 + 3]);
	}

	public static Int2 Min(Int2 a, Int2 b)
	{
		return new Int2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
	}

	public static Int2 Max(Int2 a, Int2 b)
	{
		return new Int2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
	}

	public static Int2 FromInt3XZ(Int3 o)
	{
		return new Int2(o.x, o.z);
	}

	public static Int3 ToInt3XZ(Int2 o)
	{
		return new Int3(o.x, 0, o.y);
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"(",
			this.x,
			", ",
			this.y,
			")"
		});
	}

	public void Min(ref Int2 r)
	{
		this.x = Mathf.Min(this.x, r.x);
		this.y = Mathf.Min(this.y, r.y);
	}

	public void Max(ref Int2 r)
	{
		this.x = Mathf.Max(this.x, r.x);
		this.y = Mathf.Max(this.y, r.y);
	}

	public void Normalize()
	{
		long num = (long)(this.x * 100);
		long num2 = (long)(this.y * 100);
		long num3 = num * num + num2 * num2;
		if (num3 == 0L)
		{
			return;
		}
		long b = (long)IntMath.Sqrt(num3);
		this.x = (int)IntMath.Divide(num * 1000L, b);
		this.y = (int)IntMath.Divide(num2 * 1000L, b);
	}

	public static Int2 ClampMagnitude(Int2 v, int maxLength)
	{
		long sqrMagnitudeLong = v.sqrMagnitudeLong;
		long num = (long)maxLength;
		if (sqrMagnitudeLong > num * num)
		{
			long b = (long)IntMath.Sqrt(sqrMagnitudeLong);
			int num2 = (int)IntMath.Divide((long)(v.x * maxLength), b);
			int num3 = (int)IntMath.Divide((long)(v.x * maxLength), b);
			return new Int2(num2, num3);
		}
		return v;
	}

	public static explicit operator Vector2(Int2 ob)
	{
		return new Vector2((float)ob.x * 0.001f, (float)ob.y * 0.001f);
	}

	public static explicit operator Int2(Vector2 ob)
	{
		return new Int2((int)Math.Round((double)(ob.x * 1000f)), (int)Math.Round((double)(ob.y * 1000f)));
	}

	public static Int2 operator +(Int2 a, Int2 b)
	{
		return new Int2(a.x + b.x, a.y + b.y);
	}

	public static Int2 operator -(Int2 a, Int2 b)
	{
		return new Int2(a.x - b.x, a.y - b.y);
	}

	public static bool operator ==(Int2 a, Int2 b)
	{
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(Int2 a, Int2 b)
	{
		return a.x != b.x || a.y != b.y;
	}

	public static Int2 operator -(Int2 lhs)
	{
		lhs.x = -lhs.x;
		lhs.y = -lhs.y;
		return lhs;
	}

	public static Int2 operator *(Int2 lhs, int rhs)
	{
		lhs.x *= rhs;
		lhs.y *= rhs;
		return lhs;
	}
}
