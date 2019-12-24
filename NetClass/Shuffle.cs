using System;
using System.Collections.Generic;

namespace CommonClass
{
    public class Shuffle
    {
        public static void shuffle<T>(ref List<T> list)
        {
            if (list.Count <= 1) return;
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            List<T> newList = new List<T>();//儲存結果的集合
            foreach (T item in list)
            {
                newList.Insert(rand.Next(0, newList.Count + 1), item);
            }
            newList.Remove(list[0]);//移除list[0]的值
            newList.Insert(rand.Next(0, newList.Count + 1), list[0]);//再重新隨機插入第一筆

            list = newList;
        }

        public static bool random(int ratenumerator, int denominator)
        {
            if (ratenumerator == 1 && denominator == 2)
            {
                ratenumerator = 2;
                denominator = 4;
            }
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int result = rand.Next(denominator);
            return ratenumerator > result ? true : false;
        }
    }
}
