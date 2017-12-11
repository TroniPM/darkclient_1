using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using Mogo.Util;

public class Resource
{
    internal AssetBundleCreateRequest createRequest;
    internal WWW www;
    internal List<Resource> dependencies;
    internal Byte[] fileData;
    public int referenceCount;
    internal UnityEngine.Object m_object;
    internal bool m_isDone;
    //{
    //    get
    //    {
    //        if (SystemSwitch.UseFileSystem)
    //        {
    //            if (createRequest == null)
    //                return false;
    //            return createRequest.isDone;
    //        }
    //        else
    //        {
    //            if (www == null)
    //                return false;
    //            return www.isDone;
    //        }
    //    }
    //}
    internal float m_progress
    {
        get
        {
            //if (SystemSwitch.UseFileSystem)
            //    return createRequest.progress;
            //else
            if (www != null)
                return www.progress;
            else
                return 0;
        }
    }
    public string RelativePath { get; internal set; }

    public bool IsLoading { get; internal set; }

    public bool IsDone
    {
        get
        {
            if (dependencies == null)
            {
                return m_isDone;
            }
            else
            {
                if (!dependencies.All(x => x.IsDone))
                    return false;
                return m_isDone;
            }
        }
        internal set
        {
            m_isDone = value;
        }
    }

    public float Progress
    {
        get
        {
            float total = 1.0f;
            float current = m_progress;
            if (dependencies != null)
            {
                total += dependencies.Count;
                current += dependencies.Aggregate(0.0f, (sum, res) => sum + res.Progress);
            }
            return current / total;
        }
    }

    //public AssetBundle MainAsset
    //{
    //    get
    //    {
    //        if (SystemSwitch.UseFileSystem)
    //        {
    //            if (createRequest == null)
    //                return null;
    //            else
    //                return createRequest.assetBundle;
    //        }
    //        else
    //        {
    //            if (www == null)
    //                return null;
    //            else
    //                return www.assetBundle;
    //        }
    //    }
    //}

    public UnityEngine.Object Object
    {
        get
        {
            return m_object;
            //if (MainAsset == null)
            //    return null;
            //else
            //    return MainAsset.mainAsset;
        }
    }

    //public IEnumerator Wait()
    //{
    //    var enumerator = ResourceManager.LoadAsync(this);
    //    Stack<IEnumerator> stack = new Stack<IEnumerator>();
    //    stack.Push(enumerator);

    //    while (stack.Count > 0)
    //    {
    //        var current = stack.Peek();
    //        if (current.MoveNext())
    //        {
    //            while (current.Current is IEnumerator)
    //            {
    //                stack.Push(current.Current as IEnumerator);
    //                current = stack.Peek();
    //            }
    //            yield return current.Current;
    //        }
    //        else
    //        {
    //            stack.Pop();
    //        }
    //    }
    //}

    public void UnloadAsset()
    {
        if (m_object)
        {
            Resources.UnloadAsset(m_object);
            m_object = null;
            LoggerHelper.Warning("set m_object null: " + RelativePath);
        }
    }

    public void Release(bool unloadAllLoadedObjects)//, bool forceUnloadAsset = false
    {
        if (unloadAllLoadedObjects && m_object)//&& (forceUnloadAsset || (!m_neverUnloadAsset && !forceUnloadAsset))
        {
            //UnloadAsset();
            if (!RelativePath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) && !RelativePath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                Resources.UnloadAsset(m_object);
            m_object = null;
            LoggerHelper.Warning("set m_object null: " + RelativePath);//Debug.LogWarning
        }
        //if (SystemSwitch.UseFileSystem)
        //{
        //    if (this.createRequest != null)
        //    {
        //        if (this.createRequest.assetBundle)
        //            this.createRequest.assetBundle.Unload(unloadAllLoadedObjects);
        //        this.createRequest = null;
        //    }
        //}
        //else
        //{
        if (this.www != null)
        {
            if (this.www.assetBundle)
                this.www.assetBundle.Unload(unloadAllLoadedObjects);
            this.www = null;
        }
        //}
    }

    public override string ToString()
    {
        return String.Format("{0}: rc- {1}, isDone- {2}, go- {3}", RelativePath, referenceCount, IsDone, m_object);
    }
}