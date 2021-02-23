using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZxDemoCommon.Memory
{
   public class MemoryCacheHelper
    {
        public static  IMemoryCache memoryCache = null;
        //泛型缓存的根本原理就在于类的静态构造函数只执行一次
        //而不同的泛型会被认为不同的类
         static MemoryCacheHelper()
        {
            if (memoryCache==null)
            {
                memoryCache = new MemoryCache(new MemoryCacheOptions());
            }
        }
        public static void SetCache(string key,object value)
        {
            memoryCache.Set(key, value,TimeSpan.FromMinutes(1));
        }

        public static object GetCache(string key)
        {
            return memoryCache.Get(key);
        }
    }
}
