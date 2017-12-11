using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

public class IOSUtils 
{
	public static byte[] CreateMD5(byte[] data)
	{
		using(MD5 md5=MD5.Create())
		{
			return md5.ComputeHash(data);
		}
	}

	public static string FormatMD5(byte [] data)
	{
		return System.BitConverter.ToString(data).Replace("-","").ToLower();
	}

	[DllImport("__Internal")]
	public static extern int InitSSJJEngine();

	[DllImport("__Internal")]
	public static extern int LoginSSJJEngine();

	[DllImport("__Internal")]
	public static extern int OpenLogBeforeLogin();

	[DllImport("__Internal")]
	public static extern int OpenSSJJEngineAppLog();

	[DllImport("__Internal")]
	public static extern int UpdateSSJJEngine();

	[DllImport("__Internal")]
	public static extern int ClearLocalAcount();

	[DllImport("__Internal")]
	public static extern int RoleLevelLog(char [] RoleName,char [] ServerID);

	[DllImport("__Internal")]
	public static extern int CreateRoleLog(char [] RoleName,char [] ServerID);

	[DllImport("__Internal")]
	public static extern int ChooseServerLog(char [] ServerID);

	[DllImport("__Internal")]
	private static extern void InstallIpa(byte [] loaderPath);

	public static void UpdateLoader(string strLoaderPath)
	{
		InstallIpa(Encoding.Default.GetBytes(strLoaderPath));
	}
}


public static partial class IOSCSharpExtension
{
	//cannot be generic extension like Dictionary<int,T> SortByKey<T>();
	//because this would cause a JIT runtime exception
	public static Dictionary<int,string> SortByKey(this Dictionary<int,string> dic)
	{
		Dictionary<int,string> 	dicRet=new Dictionary<int, string>();
		List<int> 				order=new List<int>();
		foreach(var v in dic)
		{
			order.Add(v.Key);
		}
		order.Sort();
		foreach(var v in order)
		{
			dicRet.Add(v,dic[v]);
		}
		return dicRet;
	}
	public static Dictionary<int,int> SortByKey(this Dictionary<int,int> dic)
	{
		Dictionary<int,int> 	dicRet=new Dictionary<int, int>();
		List<int> 				order=new List<int>();
		foreach(var v in dic)
		{
			order.Add(v.Key);
		}
		order.Sort();
		foreach(var v in order)
		{
			dicRet.Add(v,dic[v]);
		}
		return dicRet;
	}
}
	