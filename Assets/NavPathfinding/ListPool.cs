#if !UNITY_EDITOR
// Extra optimizations when not running in the editor, but less error checking
#define ASTAR_OPTIMIZE_POOLING
#endif

using System;
using System.Collections.Generic;

public static class ListPool<T>
{
    /** Internal pool */
    static List<List<T>> pool = new List<List<T>>();
    static HashSet<List<T>> inPool = new HashSet<List<T>>();

    const int MaxCapacitySearchLength = 8;

    public static List<T> Claim()
    {

        {
            if (pool.Count > 0)
            {
                List<T> ls = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
                inPool.Remove(ls);
                return ls;
            } 
            return new List<T>();
        }
    }


    public static List<T> Claim(int capacity)
    {

        {
            List<T> list = null;
            int listIndex = -1;
            for (int i = 0; i < pool.Count && i < MaxCapacitySearchLength; i++)
            {
                // ith last item
                var candidate = pool[pool.Count - 1 - i];

                if (candidate.Capacity >= capacity)
                {
                    pool.RemoveAt(pool.Count - 1 - i);
                    inPool.Remove(candidate);
                    return candidate;
                }
                else if (list == null || candidate.Capacity > list.Capacity)
                {
                    list = candidate;
                    listIndex = pool.Count - 1 - i;
                }
            }

            if (list == null)
            {
                list = new List<T>(capacity);
            }
            else
            {
                list.Capacity = capacity;
                // Swap current item and last item to enable a more efficient removal
                pool[listIndex] = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
                inPool.Remove(list);
            }
            return list;
        }
    }


    public static void Warmup(int count, int size)
    {
        {
            var tmp = new List<T>[count];
            for (int i = 0; i < count; i++) tmp[i] = Claim(size);
            for (int i = 0; i < count; i++) Release(tmp[i]);
        }
    }


    public static void Release(List<T> list)
    {
        list.Clear();
        {
            if (!inPool.Add(list))
            {
                throw new InvalidOperationException("You are trying to pool a list twice. Please make sure that you only pool it once.");
            }
            pool.Add(list);
        }
    }


    public static void Clear()
    {
        {
            inPool.Clear();
            pool.Clear();
        }
    }

    public static int GetSize()
    {
        return pool.Count;
    }
}
