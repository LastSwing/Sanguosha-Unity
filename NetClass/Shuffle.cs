using System;
using System.Collections.Generic;

namespace CommonClass
{
    public class Shuffle
    {
        public static void shuffle<T>(ref List<T> list)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            List<T> newList = new List<T>();//儲存結果的集合
            foreach (T item in list)
            {
                newList.Insert(rand.Next(0, newList.Count), item);
            }
            newList.Remove(list[0]);//移除list[0]的值
            newList.Insert(rand.Next(0, newList.Count), list[0]);//再重新隨機插入第一筆

            list = newList;
        }
    }
}
