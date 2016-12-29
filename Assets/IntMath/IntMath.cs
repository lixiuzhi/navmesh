using System;
using UnityEngine;

public class IntMath
{
	public static IntFactor atan2(int y, int x)
	{
		int num;
		int num2;
		if (x < 0)
		{
			if (y < 0)
			{
				x = -x;
				y = -y;
				num = 1;
			}
			else
			{
				x = -x;
				num = -1;
			}
			num2 = -31416;
		}
		else
		{
			if (y < 0)
			{
				y = -y;
				num = -1;
			}
			else
			{
				num = 1;
			}
			num2 = 0;
		}
		int dIM = IntAtan2Table.DIM;
		long num3 = (long)(dIM - 1);
		long b = (long)((x >= y) ? x : y);
		int num4 = (int)IntMath.Divide((long)x * num3, b);
		int num5 = (int)IntMath.Divide((long)y * num3, b);
		int num6 = IntAtan2Table.table[num5 * dIM + num4];
		return new IntFactor
		{
			numerator = (long)((num6 + num2) * num),
			denominator = 10000L
		};
	}

	public static IntFactor acos(long nom, long den)
	{
		int num = (int)IntMath.Divide(nom * (long)IntAcosTable.HALF_COUNT, den) + IntAcosTable.HALF_COUNT;
		num = Mathf.Clamp(num, 0, IntAcosTable.COUNT);
		return new IntFactor
		{
			numerator = (long)IntAcosTable.table[num],
			denominator = 10000L
		};
	}
     

    public static IntFactor sin(long nom, long den)
	{
		int index = IntSinCosTable.getIndex(nom, den);
		return new IntFactor((long)IntSinCosTable.sin_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static IntFactor cos(long nom, long den)
	{
		int index = IntSinCosTable.getIndex(nom, den);
		return new IntFactor((long)IntSinCosTable.cos_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static void sincos(out IntFactor s, out IntFactor c, long nom, long den)
	{
		int index = IntSinCosTable.getIndex(nom, den);
		s = new IntFactor((long)IntSinCosTable.sin_table[index], (long)IntSinCosTable.FACTOR);
		c = new IntFactor((long)IntSinCosTable.cos_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static void sincos(out IntFactor s, out IntFactor c, IntFactor angle)
	{
		int index = IntSinCosTable.getIndex(angle.numerator, angle.denominator);
		s = new IntFactor((long)IntSinCosTable.sin_table[index], (long)IntSinCosTable.FACTOR);
		c = new IntFactor((long)IntSinCosTable.cos_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static long Divide(long a, long b)
	{
		long num = (long)((ulong)((a ^ b) & -9223372036854775808L) >> 63);
		long num2 = num * -2L + 1L;
		return (a + b / 2L * num2) / b;
	}

	public static int Divide(int a, int b)
	{
        //µÃµ½·ûºÅ
		int num = (int)((uint)((a ^ b) & -2147483648) >> 31);
		int num2 = num * -2 + 1;
		return (a + b / 2 * num2) / b;
	}

	public static Int3 Divide(Int3 a, long m, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x * m, b);
		a.y = (int)IntMath.Divide((long)a.y * m, b);
		a.z = (int)IntMath.Divide((long)a.z * m, b);
		return a;
	}

	public static Int2 Divide(Int2 a, long m, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x * m, b);
		a.y = (int)IntMath.Divide((long)a.y * m, b);
		return a;
	}

	public static Int3 Divide(Int3 a, int b)
	{
		a.x = IntMath.Divide(a.x, b);
		a.y = IntMath.Divide(a.y, b);
		a.z = IntMath.Divide(a.z, b);
		return a;
	}

	public static Int3 Divide(Int3 a, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x, b);
		a.y = (int)IntMath.Divide((long)a.y, b);
		a.z = (int)IntMath.Divide((long)a.z, b);
		return a;
	}

	public static Int2 Divide(Int2 a, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x, b);
		a.y = (int)IntMath.Divide((long)a.y, b);
		return a;
	}

	public static uint Sqrt32(uint a)
	{
		uint num = 0u;
		uint num2 = 0u;
		for (int i = 0; i < 16; i++)
		{
			num2 <<= 1;
			num <<= 2;
			num += a >> 30;
			a <<= 2;
			if (num2 < num)
			{
				num2 += 1u;
				num -= num2;
				num2 += 1u;
			}
		}
		return num2 >> 1 & 65535u;
	}

	public static ulong Sqrt64(ulong a)
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		for (int i = 0; i < 32; i++)
		{
			num2 <<= 1;
			num <<= 2;
			num += a >> 62;
			a <<= 2;
			if (num2 < num)
			{
				num2 += 1uL;
				num -= num2;
				num2 += 1uL;
			}
		}
		return num2 >> 1 & unchecked((ulong)-1);
	}

	public static long SqrtLong(long a)
	{
		if (a <= 0L)
		{
			return 0L;
		}
		if (a <= unchecked((long)(unchecked((ulong)-1))))
		{
			return (long)((ulong)IntMath.Sqrt32((uint)a));
		}
		return (long)IntMath.Sqrt64((ulong)a);
	}

	public static int Sqrt(long a)
	{
		if (a <= 0L)
		{
			return 0;
		}
		if (a <= unchecked((long)(unchecked((ulong)-1))))
		{
			return (int)IntMath.Sqrt32((uint)a);
		}
		return (int)IntMath.Sqrt64((ulong)a);
	}

	public static long Clamp(long a, long min, long max)
	{
		if (a < min)
		{
			return min;
		}
		if (a > max)
		{
			return max;
		}
		return a;
	}

	public static long Max(long a, long b)
	{
		return (a <= b) ? b : a;
	}

	public static Int3 Transform(ref Int3 point, ref Int3 axis_x, ref Int3 axis_y, ref Int3 axis_z, ref Int3 trans)
	{
		return new Int3(IntMath.Divide(axis_x.x * point.x + axis_y.x * point.y + axis_z.x * point.z, 1000) + trans.x, IntMath.Divide(axis_x.y * point.x + axis_y.y * point.y + axis_z.y * point.z, 1000) + trans.y, IntMath.Divide(axis_x.z * point.x + axis_y.z * point.y + axis_z.z * point.z, 1000) + trans.z);
	}

	public static Int3 Transform(Int3 point, ref Int3 axis_x, ref Int3 axis_y, ref Int3 axis_z, ref Int3 trans)
	{
		return new Int3(IntMath.Divide(axis_x.x * point.x + axis_y.x * point.y + axis_z.x * point.z, 1000) + trans.x, IntMath.Divide(axis_x.y * point.x + axis_y.y * point.y + axis_z.y * point.z, 1000) + trans.y, IntMath.Divide(axis_x.z * point.x + axis_y.z * point.y + axis_z.z * point.z, 1000) + trans.z);
	}

	public static Int3 Transform(ref Int3 point, ref Int3 axis_x, ref Int3 axis_y, ref Int3 axis_z, ref Int3 trans, ref Int3 scale)
	{
		long num = (long)point.x * (long)scale.x;
		long num2 = (long)point.y * (long)scale.x;
		long num3 = (long)point.z * (long)scale.x;
		return new Int3((int)IntMath.Divide((long)axis_x.x * num + (long)axis_y.x * num2 + (long)axis_z.x * num3, 1000000L) + trans.x, (int)IntMath.Divide((long)axis_x.y * num + (long)axis_y.y * num2 + (long)axis_z.y * num3, 1000000L) + trans.y, (int)IntMath.Divide((long)axis_x.z * num + (long)axis_y.z * num2 + (long)axis_z.z * num3, 1000000L) + trans.z);
	}

	public static Int3 Transform(ref Int3 point, ref Int3 forward, ref Int3 trans)
	{
		Int3 up = Int3.up;
		Int3 vInt = Int3.Cross(Int3.up, forward);
		return IntMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
	}

	public static Int3 Transform(Int3 point, Int3 forward, Int3 trans)
	{
		Int3 up = Int3.up;
		Int3 vInt = Int3.Cross(Int3.up, forward);
		return IntMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
	}

	public static Int3 Transform(Int3 point, Int3 forward, Int3 trans, Int3 scale)
	{
		Int3 up = Int3.up;
		Int3 vInt = Int3.Cross(Int3.up, forward);
		return IntMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans, ref scale);
	}

	public static int Lerp(int src, int dest, int nom, int den)
	{
		return IntMath.Divide(src * den + (dest - src) * nom, den);
	}

	public static long Lerp(long src, long dest, long nom, long den)
	{
		return IntMath.Divide(src * den + (dest - src) * nom, den);
	}

	public static bool IsPowerOfTwo(int x)
	{
		return (x & x - 1) == 0;
	}

	public static int CeilPowerOfTwo(int x)
	{
		x--;
		x |= x >> 1;
		x |= x >> 2;
		x |= x >> 4;
		x |= x >> 8;
		x |= x >> 16;
		x++;
		return x;
	}

	public static void SegvecToLinegen(ref Int2 segSrc, ref Int2 segVec, out long a, out long b, out long c)
	{
		a = (long)segVec.y;
		b = (long)(-(long)segVec.x);
		c = (long)segVec.x * (long)segSrc.y - (long)segSrc.x * (long)segVec.y;
	}

	private static bool IsPointOnSegment(ref Int2 segSrc, ref Int2 segVec, long x, long y)
	{
		long num = x - (long)segSrc.x;
		long num2 = y - (long)segSrc.y;
		return (long)segVec.x * num + (long)segVec.y * num2 >= 0L && num * num + num2 * num2 <= segVec.sqrMagnitudeLong;
	}

	public static bool IntersectSegment(ref Int2 seg1Src, ref Int2 seg1Vec, ref Int2 seg2Src, ref Int2 seg2Vec, out Int2 interPoint)
	{
		long num;
		long num2;
		long num3;
		IntMath.SegvecToLinegen(ref seg1Src, ref seg1Vec, out num, out num2, out num3);
		long num4;
		long num5;
		long num6;
		IntMath.SegvecToLinegen(ref seg2Src, ref seg2Vec, out num4, out num5, out num6);
		long num7 = num * num5 - num4 * num2;
		if (num7 != 0L)
		{
			long num8 = IntMath.Divide(num2 * num6 - num5 * num3, num7);
			long num9 = IntMath.Divide(num4 * num3 - num * num6, num7);
			bool result = IntMath.IsPointOnSegment(ref seg1Src, ref seg1Vec, num8, num9) && IntMath.IsPointOnSegment(ref seg2Src, ref seg2Vec, num8, num9);
			interPoint.x = (int)num8;
			interPoint.y = (int)num9;
			return result;
		}
		interPoint = Int2.zero;
		return false;
	}

	public static bool PointInPolygon(ref Int2 pnt, Int2[] plg)
	{
		if (plg == null || plg.Length < 3)
		{
			return false;
		}
		bool flag = false;
		int i = 0;
		int num = plg.Length - 1;
		while (i < plg.Length)
		{
			Int2 vInt = plg[i];
			Int2 vInt2 = plg[num];
			if ((vInt.y <= pnt.y && pnt.y < vInt2.y) || (vInt2.y <= pnt.y && pnt.y < vInt.y))
			{
				int num2 = vInt2.y - vInt.y;
				long num3 = (long)(pnt.y - vInt.y) * (long)(vInt2.x - vInt.x) - (long)(pnt.x - vInt.x) * (long)num2;
				if (num2 > 0)
				{
					if (num3 > 0L)
					{
						flag = !flag;
					}
				}
				else if (num3 < 0L)
				{
					flag = !flag;
				}
			}
			num = i++;
		}
		return flag;
	}

	public static bool SegIntersectPlg(ref Int2 segSrc, ref Int2 segVec, Int2[] plg, out Int2 nearPoint, out Int2 projectVec)
	{
		nearPoint = Int2.zero;
		projectVec = Int2.zero;
		if (plg == null || plg.Length < 2)
		{
			return false;
		}
		bool result = false;
		long num = -1L;
		int num2 = -1;
		for (int i = 0; i < plg.Length; i++)
		{
			Int2 vInt = plg[(i + 1) % plg.Length] - plg[i];
			Int2 vInt2;
			if (IntMath.IntersectSegment(ref segSrc, ref segVec, ref plg[i], ref vInt, out vInt2))
			{
				long sqrMagnitudeLong = (vInt2 - segSrc).sqrMagnitudeLong;
				if (num < 0L || sqrMagnitudeLong < num)
				{
					nearPoint = vInt2;
					num = sqrMagnitudeLong;
					num2 = i;
					result = true;
				}
			}
		}
		if (num2 >= 0)
		{
			Int2 lhs = plg[(num2 + 1) % plg.Length] - plg[num2];
			Int2 vInt3 = segSrc + segVec - nearPoint;
			long num3 = (long)vInt3.x * (long)lhs.x + (long)vInt3.y * (long)lhs.y;
			if (num3 < 0L)
			{
				num3 = -num3;
				lhs = -lhs;
			}
			long sqrMagnitudeLong2 = lhs.sqrMagnitudeLong;
			projectVec.x = (int)IntMath.Divide((long)lhs.x * num3, sqrMagnitudeLong2);
			projectVec.y = (int)IntMath.Divide((long)lhs.y * num3, sqrMagnitudeLong2);
		}
		return result;
	}
}
