using System;
using UnityEngine;

[Serializable]
public struct Int3
{
	public const int Precision = 1000;

	public const float FloatPrecision = 1000f;

	public const float PrecisionFactor = 0.001f;

	public int x;

	public int y;

	public int z;

	public static readonly Int3 zero = new Int3(0, 0, 0);  

    public static readonly Int3 one = new Int3(1000, 1000, 1000);

    public static readonly Int3 min = new Int3(int.MinValue, int.MinValue, int.MinValue);

    public static readonly Int3 half = new Int3(500, 500, 500);

	public static readonly Int3 forward = new Int3(0, 0, 1000);

	public static readonly Int3 up = new Int3(0, 1000, 0);

	public static readonly Int3 right = new Int3(1000, 0, 0);

	public int this[int i]
	{
		get
		{
			return (i != 0) ? ((i != 1) ? this.z : this.y) : this.x;
		}
		set
		{
			if (i == 0)
			{
				this.x = value;
			}
			else if (i == 1)
			{
				this.y = value;
			}
			else
			{
				this.z = value;
			}
		}
	}

	public Vector3 vec3
	{
		get
		{
			return new Vector3((float)this.x * 0.001f, (float)this.y * 0.001f, (float)this.z * 0.001f);
		}
	}

	public Int2 xz
	{
		get
		{
			return new Int2(this.x, this.z);
		}
	}

	public int magnitude
	{
		get
		{
			long num = (long)this.x;
			long num2 = (long)this.y;
			long num3 = (long)this.z;
			return IntMath.Sqrt(num * num + num2 * num2 + num3 * num3);
		}
	}

	public int magnitude2D
	{
		get
		{
			long num = (long)this.x;
			long num2 = (long)this.z;
			return IntMath.Sqrt(num * num + num2 * num2);
		}
	}

	public int costMagnitude
	{
		get
		{
			return this.magnitude;
		}
	}

	public float worldMagnitude
	{
		get
		{
			double num = (double)this.x;
			double num2 = (double)this.y;
			double num3 = (double)this.z;
			return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3) * 0.001f;
		}
	}

	public double sqrMagnitude
	{
		get
		{
			double num = (double)this.x;
			double num2 = (double)this.y;
			double num3 = (double)this.z;
			return num * num + num2 * num2 + num3 * num3;
		}
	}

	public long sqrMagnitudeLong
	{
		get
		{
			long num = (long)this.x;
			long num2 = (long)this.y;
			long num3 = (long)this.z;
			return num * num + num2 * num2 + num3 * num3;
		}
	}

	public long sqrMagnitudeLong2D
	{
		get
		{
			long num = (long)this.x;
			long num2 = (long)this.z;
			return num * num + num2 * num2;
		}
	}

	public int unsafeSqrMagnitude
	{
		get
		{
			return this.x * this.x + this.y * this.y + this.z * this.z;
		}
	}

	public Int3 abs
	{
		get
		{
			return new Int3(Math.Abs(this.x), Math.Abs(this.y), Math.Abs(this.z));
		}
	}

	[Obsolete("Same implementation as .magnitude")]
	public float safeMagnitude
	{
		get
		{
			double num = (double)this.x;
			double num2 = (double)this.y;
			double num3 = (double)this.z;
			return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
		}
	}

	[Obsolete(".sqrMagnitude is now per default safe (.unsafeSqrMagnitude can be used for unsafe operations)")]
	public float safeSqrMagnitude
	{
		get
		{
			float num = (float)this.x * 0.001f;
			float num2 = (float)this.y * 0.001f;
			float num3 = (float)this.z * 0.001f;
			return num * num + num2 * num2 + num3 * num3;
		}
	}

	public Int3(Vector3 position)
	{
		this.x = (int)Math.Round((double)(position.x * 1000f));
		this.y = (int)Math.Round((double)(position.y * 1000f));
		this.z = (int)Math.Round((double)(position.z * 1000f));
	}

	public Int3(int _x, int _y, int _z)
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
	}


	public Int3 DivBy2()
	{
		this.x >>= 1;
		this.y >>= 1;
		this.z >>= 1;
		return this;
	}

    /// new  /////////////////////////////////////////////////////
    public static Int3 NormalXZ(Int3 start, Int3 end)
    {
        long x = end.x - start.x;
        long z = end.z - start.z;
        long len =  IntMath.Sqrt(x * x + z * z);  

        Int3 nor = new Int3();  

        nor.x = (int)IntMath.Divide(x*1000,len);
        nor.y = 0;
        nor.z = (int)IntMath.Divide(z * 1000,len); 
        return nor;
    } 
    public static Int3 Border(Int3 start, Int3 end, float dist)
    {
        var n = Int3.NormalXZ(start, end);
        Int3 v = new Int3();
        v.x = start.x + (int)(n.x * dist);
        v.y = start.y;// + (int)(n.y * dist);
        v.z = start.z + (int)(n.z * dist);
        return v;
    }

    public static long DistXZ(Int3 start, Int3 end)
    {
        long x = end.x - start.x;
        long z = end.z - start.z;
        long len = IntMath.Sqrt(x * x + z * z);
        return len;
    }

    public static long Dist(Int3 start, Int3 end)
    {
        long x = end.x - start.x;
        long y = end.y - start.y;
        long z = end.z - start.z;
        long len = IntMath.Sqrt(x * x + y * y + z * z);
        return len;
    }

    /// /////////////////////////////////////////////////////  

    public static float AngleXZ(Int3 lhs, Int3 rhs)
    {
        double num = (double)Int3.Dot(lhs, rhs) / ((double)lhs.magnitude * (double)rhs.magnitude);
        num = ((num >= -1.0) ? ((num <= 1.0) ? num : 1.0) : -1.0);
        var f=  (float)Math.Acos(num); 
        var cross = Vector3.Cross(lhs.vec3, rhs.vec3);
        if (cross.y < 0)
            f = -f;
        return f;
    }

 /*   public static IntFactor AngleXZ(Int3 lhs, Int3 rhs)
    {
        lhs.y = 0;
        rhs.y = 0;
        var r = AngleInt(lhs, rhs);
       // float angle = r.numerator * 1f / r.denominator;
        var cross = Int3.Cross(lhs, rhs);
        if (cross.y < 0)
            r.numerator = -r.numerator;
        return r;
    }
    */

    public static IntFactor AngleInt(Int3 lhs, Int3 rhs)
	{
		long den = (long)lhs.magnitude * (long)rhs.magnitude;
		return IntMath.acos((long)Int3.Dot(ref lhs, ref rhs), den);
	}

	public static int Dot(ref Int3 lhs, ref Int3 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
	}

	public static int Dot(Int3 lhs, Int3 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
	}

	public static long DotLong(Int3 lhs, Int3 rhs)
	{
		return (long)lhs.x * (long)rhs.x + (long)lhs.y * (long)rhs.y + (long)lhs.z * (long)rhs.z;
	}

	public static long DotLong(ref Int3 lhs, ref Int3 rhs)
	{
		return (long)lhs.x * (long)rhs.x + (long)lhs.y * (long)rhs.y + (long)lhs.z * (long)rhs.z;
	}

	public static long DotXZLong(ref Int3 lhs, ref Int3 rhs)
	{
		return (long)lhs.x * (long)rhs.x + (long)lhs.z * (long)rhs.z;
	}

	public static long DotXZLong(Int3 lhs, Int3 rhs)
	{
		return (long)lhs.x * (long)rhs.x + (long)lhs.z * (long)rhs.z;
	}

	public static Int3 Cross(ref Int3 lhs, ref Int3 rhs)
	{
		return new Int3(IntMath.Divide(lhs.y * rhs.z - lhs.z * rhs.y, 1000), IntMath.Divide(lhs.z * rhs.x - lhs.x * rhs.z, 1000), IntMath.Divide(lhs.x * rhs.y - lhs.y * rhs.x, 1000));
	}

	public static Int3 Cross(Int3 lhs, Int3 rhs)
	{
		return new Int3(IntMath.Divide(lhs.y * rhs.z - lhs.z * rhs.y, 1000), IntMath.Divide(lhs.z * rhs.x - lhs.x * rhs.z, 1000), IntMath.Divide(lhs.x * rhs.y - lhs.y * rhs.x, 1000));
	}

	public static Int3 MoveTowards(Int3 from, Int3 to, int dt)
	{
		if ((to - from).sqrMagnitudeLong <= (long)(dt * dt))
		{
			return to;
		}
		return from + (to - from).NormalizeTo(dt);
	}

	public Int3 Normal2D()
	{
		return new Int3(this.z, this.y, -this.x);
	}

	public Int3 NormalizeTo(int newMagn)
	{
		long num = (long)(this.x * 100);
		long num2 = (long)(this.y * 100);
		long num3 = (long)(this.z * 100);
		long num4 = num * num + num2 * num2 + num3 * num3;
		if (num4 == 0L)
		{
			return this;
		}
		long b = (long)IntMath.Sqrt(num4);
		long num5 = (long)newMagn;
		this.x = (int)IntMath.Divide(num * num5, b);
		this.y = (int)IntMath.Divide(num2 * num5, b);
		this.z = (int)IntMath.Divide(num3 * num5, b);
		return this;
	}

	public long Normalize()
	{
		long num = (long)((long)this.x << 7);
		long num2 = (long)((long)this.y << 7);
		long num3 = (long)((long)this.z << 7);
		long num4 = num * num + num2 * num2 + num3 * num3;
		if (num4 == 0L)
		{
			return 0L;
		}
		long num5 = (long)IntMath.Sqrt(num4);
		long num6 = 1000L;
		this.x = (int)IntMath.Divide(num * num6, num5);
		this.y = (int)IntMath.Divide(num2 * num6, num5);
		this.z = (int)IntMath.Divide(num3 * num6, num5);
		return num5 >> 7;
	}

	public Int3 RotateY(ref IntFactor radians)
	{
		IntFactor IntVectorFactor;
		IntFactor IntVectorFactor2;
		IntMath.sincos(out IntVectorFactor, out IntVectorFactor2, radians.numerator, radians.denominator);
		long num = IntVectorFactor2.numerator * IntVectorFactor.denominator;
		long num2 = IntVectorFactor2.denominator * IntVectorFactor.numerator;
		long b = IntVectorFactor2.denominator * IntVectorFactor.denominator;
		Int3 vInt;
		vInt.x = (int)IntMath.Divide((long)this.x * num + (long)this.z * num2, b);
		vInt.z = (int)IntMath.Divide((long)(-(long)this.x) * num2 + (long)this.z * num, b);
		vInt.y = 0;
		return vInt.NormalizeTo(1000);
	}

	public Int3 RotateY(int degree)
	{
		IntFactor IntVectorFactor;
		IntFactor IntVectorFactor2;
		IntMath.sincos(out IntVectorFactor, out IntVectorFactor2, (long)(31416 * degree), 1800000L);
		long num = IntVectorFactor2.numerator * IntVectorFactor.denominator;
		long num2 = IntVectorFactor2.denominator * IntVectorFactor.numerator;
		long b = IntVectorFactor2.denominator * IntVectorFactor.denominator;
		Int3 vInt;
		vInt.x = (int)IntMath.Divide((long)this.x * num + (long)this.z * num2, b);
		vInt.z = (int)IntMath.Divide((long)(-(long)this.x) * num2 + (long)this.z * num, b);
		vInt.y = 0;
		return vInt.NormalizeTo(1000);
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"( ",
			this.x,
			", ",
			this.y,
			", ",
			this.z,
			")"
		});
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		Int3 vInt = (Int3)o;
		return this.x == vInt.x && this.y == vInt.y && this.z == vInt.z;
	}

	public override int GetHashCode()
	{
		return this.x * 73856093 ^ this.y * 19349663 ^ this.z * 83492791;
	}

	public static Int3 Lerp(Int3 a, Int3 b, float f)
	{
		return new Int3(Mathf.RoundToInt((float)a.x * (1f - f)) + Mathf.RoundToInt((float)b.x * f), Mathf.RoundToInt((float)a.y * (1f - f)) + Mathf.RoundToInt((float)b.y * f), Mathf.RoundToInt((float)a.z * (1f - f)) + Mathf.RoundToInt((float)b.z * f));
	}

	public static Int3 Lerp(Int3 a, Int3 b, IntFactor f)
	{
		return new Int3((int)IntMath.Divide((long)(b.x - a.x) * f.numerator, f.denominator) + a.x, (int)IntMath.Divide((long)(b.y - a.y) * f.numerator, f.denominator) + a.y, (int)IntMath.Divide((long)(b.z - a.z) * f.numerator, f.denominator) + a.z);
	}

	public static Int3 Lerp(Int3 a, Int3 b, int factorNom, int factorDen)
	{
		return new Int3(IntMath.Divide((b.x - a.x) * factorNom, factorDen) + a.x, IntMath.Divide((b.y - a.y) * factorNom, factorDen) + a.y, IntMath.Divide((b.z - a.z) * factorNom, factorDen) + a.z);
	}

	public long XZSqrMagnitude(Int3 rhs)
	{
		long num = (long)(this.x - rhs.x);
		long num2 = (long)(this.z - rhs.z);
		return num * num + num2 * num2;
	}

	public long XZSqrMagnitude(ref Int3 rhs)
	{
		long num = (long)(this.x - rhs.x);
		long num2 = (long)(this.z - rhs.z);
		return num * num + num2 * num2;
	}

	public bool IsEqualXZ(Int3 rhs)
	{
		return this.x == rhs.x && this.z == rhs.z;
	}

	public bool IsEqualXZ(ref Int3 rhs)
	{
		return this.x == rhs.x && this.z == rhs.z;
	}

	public static bool operator ==(Int3 lhs, Int3 rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
	}

	public static bool operator !=(Int3 lhs, Int3 rhs)
	{
		return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
	}

	public static explicit operator Int3(Vector3 ob)
	{
		return new Int3((int)Math.Round((double)(ob.x * 1000f)), (int)Math.Round((double)(ob.y * 1000f)), (int)Math.Round((double)(ob.z * 1000f)));
	}

	public static explicit operator Vector3(Int3 ob)
	{
		return new Vector3((float)ob.x * 0.001f, (float)ob.y * 0.001f, (float)ob.z * 0.001f);
	}

	public static Int3 operator -(Int3 lhs, Int3 rhs)
	{
		lhs.x -= rhs.x;
		lhs.y -= rhs.y;
		lhs.z -= rhs.z;
		return lhs;
	}

	public static Int3 operator -(Int3 lhs)
	{
		lhs.x = -lhs.x;
		lhs.y = -lhs.y;
		lhs.z = -lhs.z;
		return lhs;
	}

	public static Int3 operator +(Int3 lhs, Int3 rhs)
	{
		lhs.x += rhs.x;
		lhs.y += rhs.y;
		lhs.z += rhs.z;
		return lhs;
	}

	public static Int3 operator *(Int3 lhs, int rhs)
	{
		lhs.x *= rhs;
		lhs.y *= rhs;
		lhs.z *= rhs;
		return lhs;
	}

	public static Int3 operator *(Int3 lhs, float rhs)
	{
		lhs.x = (int)Math.Round((double)((float)lhs.x * rhs));
		lhs.y = (int)Math.Round((double)((float)lhs.y * rhs));
		lhs.z = (int)Math.Round((double)((float)lhs.z * rhs));
		return lhs;
	}

	public static Int3 operator *(Int3 lhs, double rhs)
	{
		lhs.x = (int)Math.Round((double)lhs.x * rhs);
		lhs.y = (int)Math.Round((double)lhs.y * rhs);
		lhs.z = (int)Math.Round((double)lhs.z * rhs);
		return lhs;
	}

	public static Int3 operator *(Int3 lhs, Vector3 rhs)
	{
		lhs.x = (int)Math.Round((double)((float)lhs.x * rhs.x));
		lhs.y = (int)Math.Round((double)((float)lhs.y * rhs.y));
		lhs.z = (int)Math.Round((double)((float)lhs.z * rhs.z));
		return lhs;
	}

	public static Int3 operator *(Int3 lhs, Int3 rhs)
	{
		lhs.x *= rhs.x;
		lhs.y *= rhs.y;
		lhs.z *= rhs.z;
		return lhs;
	}

	public static Int3 operator /(Int3 lhs, float rhs)
	{
		lhs.x = (int)Math.Round((double)((float)lhs.x / rhs));
		lhs.y = (int)Math.Round((double)((float)lhs.y / rhs));
		lhs.z = (int)Math.Round((double)((float)lhs.z / rhs));
		return lhs;
	}

	public static implicit operator string(Int3 ob)
	{
		return ob.ToString();
	}
}
