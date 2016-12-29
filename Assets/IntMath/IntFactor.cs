using System;

[Serializable]
public struct IntFactor
{
	public long numerator; //·Ö×Ó

	public long denominator; //·ÖÄ¸

	[NonSerialized]
	public static IntFactor zero = new IntFactor(0L, 1L);

	[NonSerialized]
	public static IntFactor one = new IntFactor(1L, 1L);

	[NonSerialized]
	public static IntFactor pi = new IntFactor(31416L, 10000L);

	[NonSerialized]
	public static IntFactor twoPi = new IntFactor(62832L, 10000L);

	private static long mask_ = 9223372036854775807L;  //long max

	private static long upper_ = 16777215L;

	public int roundInt
	{
		get
		{
			return (int)IntMath.Divide(this.numerator, this.denominator);
		}
	}

	public int integer
	{
		get
		{
			return (int)(this.numerator / this.denominator);
		}
	}

	public float single
	{
		get
		{
			double num = (double)this.numerator / (double)this.denominator;
			return (float)num;
		}
	}

	public bool IsPositive
	{
		get
		{
            if(this.denominator == 0L)
			UnityEngine.Debug.LogError( "IntFactor: denominator is zero !");
			if (this.numerator == 0L)
			{
				return false;
			}
			bool flag = this.numerator > 0L;
			bool flag2 = this.denominator > 0L;
			return !(flag ^ flag2);
		}
	}

	public bool IsNegative
	{
		get
        {
            if (this.denominator == 0L)
                UnityEngine.Debug.LogError("IntFactor: denominator is zero !"); 
			if (this.numerator == 0L)
			{
				return false;
			}
			bool flag = this.numerator > 0L;
			bool flag2 = this.denominator > 0L;
			return flag ^ flag2;
		}
	}

	public bool IsZero
	{
		get
		{
			return this.numerator == 0L;
		}
	}

	public IntFactor Inverse
	{
		get
		{
			return new IntFactor(this.denominator, this.numerator);
		}
	}

	public IntFactor(long n, long d)
	{
		this.numerator = n;
		this.denominator = d;
	}

	public override bool Equals(object obj)
	{
		return obj != null && base.GetType() == obj.GetType() && this == (IntFactor)obj;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return this.single.ToString();
	}

	public void strip()
	{
		while ((this.numerator & IntFactor.mask_) > IntFactor.upper_ && (this.denominator & IntFactor.mask_) > IntFactor.upper_)
		{
			this.numerator >>= 1;
			this.denominator >>= 1;
		}
	}

	public static bool operator <(IntFactor a, IntFactor b)
	{
		long num = a.numerator * b.denominator;
		long num2 = b.numerator * a.denominator;
		bool flag = b.denominator > 0L ^ a.denominator > 0L;
		return (!flag) ? (num < num2) : (num > num2);
	}

	public static bool operator >(IntFactor a, IntFactor b)
	{
		long num = a.numerator * b.denominator;
		long num2 = b.numerator * a.denominator;
		bool flag = b.denominator > 0L ^ a.denominator > 0L;
		return (!flag) ? (num > num2) : (num < num2);
	}

	public static bool operator <=(IntFactor a, IntFactor b)
	{
		long num = a.numerator * b.denominator;
		long num2 = b.numerator * a.denominator;
		bool flag = b.denominator > 0L ^ a.denominator > 0L;
		return (!flag) ? (num <= num2) : (num >= num2);
	}

	public static bool operator >=(IntFactor a, IntFactor b)
	{
		long num = a.numerator * b.denominator;
		long num2 = b.numerator * a.denominator;
		bool flag = b.denominator > 0L ^ a.denominator > 0L;
		return (!flag) ? (num >= num2) : (num <= num2);
	}

	public static bool operator ==(IntFactor a, IntFactor b)
	{
		return a.numerator * b.denominator == b.numerator * a.denominator;
	}

	public static bool operator !=(IntFactor a, IntFactor b)
	{
		return a.numerator * b.denominator != b.numerator * a.denominator;
	}

	public static bool operator <(IntFactor a, long b)
	{
		long num = a.numerator;
		long num2 = b * a.denominator;
		return (a.denominator <= 0L) ? (num > num2) : (num < num2);
	}

	public static bool operator >(IntFactor a, long b)
	{
		long num = a.numerator;
		long num2 = b * a.denominator;
		return (a.denominator <= 0L) ? (num < num2) : (num > num2);
	}

	public static bool operator <=(IntFactor a, long b)
	{
		long num = a.numerator;
		long num2 = b * a.denominator;
		return (a.denominator <= 0L) ? (num >= num2) : (num <= num2);
	}

	public static bool operator >=(IntFactor a, long b)
	{
		long num = a.numerator;
		long num2 = b * a.denominator;
		return (a.denominator <= 0L) ? (num <= num2) : (num >= num2);
	}

	public static bool operator ==(IntFactor a, long b)
	{
		return a.numerator == b * a.denominator;
	}

	public static bool operator !=(IntFactor a, long b)
	{
		return a.numerator != b * a.denominator;
	}

	public static IntFactor operator +(IntFactor a, IntFactor b)
	{
		return new IntFactor
		{
			numerator = a.numerator * b.denominator + b.numerator * a.denominator,
			denominator = a.denominator * b.denominator
		};
	}

	public static IntFactor operator +(IntFactor a, long b)
	{
		a.numerator += b * a.denominator;
		return a;
	}

	public static IntFactor operator -(IntFactor a, IntFactor b)
	{
		return new IntFactor
		{
			numerator = a.numerator * b.denominator - b.numerator * a.denominator,
			denominator = a.denominator * b.denominator
		};
	}

	public static IntFactor operator -(IntFactor a, long b)
	{
		a.numerator -= b * a.denominator;
		return a;
	}

	public static IntFactor operator *(IntFactor a, long b)
	{
		a.numerator *= b;
		return a;
	}

	public static IntFactor operator /(IntFactor a, long b)
	{
		a.denominator *= b;
		return a;
	}

	public static Int3 operator *(Int3 v, IntFactor f)
	{
		return IntMath.Divide(v, f.numerator, f.denominator);
	}

	public static Int2 operator *(Int2 v, IntFactor f)
	{
		return IntMath.Divide(v, f.numerator, f.denominator);
	}

	public static Int3 operator /(Int3 v, IntFactor f)
	{
		return IntMath.Divide(v, f.denominator, f.numerator);
	}

	public static Int2 operator /(Int2 v, IntFactor f)
	{
		return IntMath.Divide(v, f.denominator, f.numerator);
	}

	public static int operator *(int i, IntFactor f)
	{
		return (int)IntMath.Divide((long)i * f.numerator, f.denominator);
	}

	public static IntFactor operator -(IntFactor a)
	{
		a.numerator = -a.numerator;
		return a;
	}
}
