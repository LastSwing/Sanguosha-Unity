using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace CommonClass
{
    public static class JsonUntity
    {
        /// <summary>
        /// 将字典类型序列化为json字符串
        /// </summary>
        /// <typeparam name="TKey">字典key</typeparam>
        /// <typeparam name="TValue">字典value</typeparam>
        /// <param name="dict">要序列化的字典数据</param>
        /// <returns>json字符串</returns>
        public static string Dictionary2Json<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict.Count == 0)
                return string.Empty;

            string jsonStr = JsonConvert.SerializeObject(dict);
            return jsonStr;
        }

        /// <summary>
        /// 将json字符串反序列化为字典类型
        /// </summary>
        /// <typeparam name="TKey">字典key</typeparam>
        /// <typeparam name="TValue">字典value</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>字典数据</returns>
        public static Dictionary<TKey, TValue> Json2Dictionary<TKey, TValue>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return new Dictionary<TKey, TValue>();

            Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);

            return jsonDict;

        }

        public static string Object2Json<T>(T t)
        {
            string jsonStr = JsonConvert.SerializeObject(t);
            return jsonStr;
        }
        public static T Json2Object<T>(string jsonStr)
        {
            T result = JsonConvert.DeserializeObject<T>(jsonStr);
            return result;
        }

        public static List<T> Json2List<T>(string jsonStr) {
            List<T> result = JsonConvert.DeserializeObject<List<T>>(jsonStr);
            return result;
        }

        public static List<string> IntList2StringList(List<int> list)
        {
            List<string> stringList = new List<string>();
            foreach (int num in list)
                stringList.Add(num.ToString());

            return stringList;
        }

        public static List<int> StringList2IntList(List<string> list)
        {
            List<int> intList = new List<int>();
            foreach (string str in list)
            {
                if (int.TryParse(str, out int num))
                    intList.Add(num);
            }

            return intList;
        }

        public static string DataSet2Json(DataSet dataSet)
        {
            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            return json;
        }
        
    }
}
