using System;
using System.Collections.Generic;

/// <summary>
/// 固定键值对为 string 与 object 的 Lua table 字典
/// </summary>
public class LuaTable : IEnumerable<KeyValuePair<string, object>>
{
    private Dictionary<string, object> m_dic;
    private Dictionary<string, bool> m_keyIsStringDic;

    public int Count { get { return m_dic.Count; } }

    public Dictionary<string, object>.KeyCollection Keys { get { return m_dic.Keys; } }
    public Dictionary<string, object>.ValueCollection Values { get { return m_dic.Values; } }

    public object this[string key]
    {
        get
        {
            return m_dic[key];
        }
        set
        {
            m_dic[key] = value;
        }
    }
 
    public LuaTable()
    {
        m_dic = new Dictionary<string, object>();
        m_keyIsStringDic = new Dictionary<string, bool>();
    }

    public void Add(int key, object value)
    {
        Add(key.ToString(), false, value);
    }

    public void Add(float key, object value)
    {

    	Add(Convert.ToInt32(key).ToString(), false, value);
    }

    public void Add(string key, object value)
    {
        Add(key, true, value);
    }

    public void Add(string key, bool isString, object value)
    {
        m_dic.Add(key, value);
        m_keyIsStringDic.Add(key, isString);
    }

    public bool Remove(string key)
    {
        return m_dic.Remove(key);
    }

    public void Clear()
    {
        m_dic.Clear();
    }

    public bool IsKeyString(string key)
    {
#if UNITY_IPHONE
		bool bRet=true;
		if(m_keyIsStringDic.ContainsKey(key))
		{
			bRet=m_keyIsStringDic[key];
		}
		return bRet;
#else
        return m_keyIsStringDic.GetValueOrDefault(key, true);
#endif
    }

    public bool IsLuaTable(string key)
    {
        if (ContainsKey(key))
        {
            var obj = m_dic[key];
            if (obj.GetType() == typeof(LuaTable))
            {
                return true;
            }
        }
        return false;
    }

    public LuaTable GetLuaTable(string key)
    {
        if (IsLuaTable(key))
            return m_dic[key] as LuaTable;
        else
            return null;
    }

    public bool TryGetLuaTable(string key, out LuaTable value)
    {
        if (IsLuaTable(key))
        {
            value = m_dic[key] as LuaTable;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    public bool TryGetValue(string key, out object value)
    {
        return m_dic.TryGetValue(key, out value);
    }

    public bool ContainsKey(string key)
    {
        return m_dic.ContainsKey(key);
    }

    public bool ContainsValue(object value)
    {
        return m_dic.ContainsValue(value);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return m_dic.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return m_dic.GetEnumerator();
    }

    public override string ToString()
    {
        //return base.ToString();
        var luatable = Mogo.RPC.Utils.PackLuaTable(this);
        return luatable;
    }
}

public static class DictionaryExtend
{
    public static T Get<T>(this T[] array, int index)
    {
        if (array == null)
        {
            Mogo.Util.LoggerHelper.Critical("Array is null.");
            return default(T) == null ? GetDefaultValue<T>() : default(T);
        }
        else if (array.Length <= index)
        {
            Mogo.Util.LoggerHelper.Critical(String.Format("Index '{0}' is out of range.", index));
            return default(T) == null ? GetDefaultValue<T>() : default(T);
        }
        else
        {
            return array[index];
        }
    }

    public static T Get<T>(this List<T> list, int index)
    {
        if (list == null)
        {
            Mogo.Util.LoggerHelper.Critical("List is null.");
            return default(T) == null ? GetDefaultValue<T>() : default(T);
        }
        else if (list.Count <= index)
        {
            Mogo.Util.LoggerHelper.Critical(String.Format("Index '{0}' is out of range.", index));
            return default(T) == null ? GetDefaultValue<T>() : default(T);
        }
        else
        {
            return list[index];
        }
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary == null)
        {
            Mogo.Util.LoggerHelper.Critical("Dictionary is null.");
            return default(TValue) == null ? GetDefaultValue<TValue>() : default(TValue);
        }
        else if (!dictionary.ContainsKey(key))
        {
            Mogo.Util.LoggerHelper.Critical(String.Format("Key '{0}' is not exist.", key));
            return default(TValue) == null ? GetDefaultValue<TValue>() : default(TValue);
        }
        else
        {
            return dictionary[key];
        }
    }

    public static T GetDefaultValue<T>()
    {
        var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
        if (constructor == null)
            return default(T);
        else
            return (T)constructor.Invoke(null);
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    public static TValue GetValueOrDefaultValueProvider<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
    {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValueProvider();
    }
}
