/**
The MIT License (MIT)

Copyright(C) 2013 <Hooke HU>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
**/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HMF
{
    public class Hmf
    {
        private List<int> ints = new List<int>();
        private List<double> doubles = new List<double>();
        private List<bool> bools = new List<bool>();
        private List<string> strs = new List<string>();

        private Stream stream = null;

        public Hmf()
        {
        }

        private void Reset()
        {
            ints = new List<int>();
            doubles = new List<double>();
            strs = new List<string>();
            bools = new List<bool>();
            bools.Add(false);
            bools.Add(true);
            stream = null;
        }

        private void SetStream(Stream stream)
        {
            this.stream = stream;
        }

        public void WriteObject(object obj, Stream stream)
        {
            SetStream(stream);
            RealWrite(obj);
            MergeAll();
        }

        private void RealWrite(object obj)
        {
            if(obj.GetType() == typeof(int))
            {
                WriteInt((int)obj);
            }
            else if(obj.GetType() == typeof(List<object>))
            {
                WriteArray((List<object>)obj);
            }
            else if(obj.GetType() == typeof(Dictionary<object, object>))
            {
                WriteDict((Dictionary<object, object>)obj);
            }
            else if(obj.GetType() == typeof(double))
            {
                WriteDouble((double)obj);
            }
            else if(obj.GetType() == typeof(string))
            {
                WriteString((string)obj);
            }
        }

        private void WriteInt(int v)
        {
            Util.WriteVarint((int)Tag.INT_TAG, stream);
            int idx = ints.IndexOf(v);
            if(idx != -1)
            {
                Util.WriteVarint(idx, stream);
                return;
            }
            ints.Add(v);
            idx = ints.Count - 1;
            Util.WriteVarint(idx, stream);
        }

        private void WriteDouble(double v)
        {
            Util.WriteVarint((int)Tag.DOUBLE_TAG, stream);
            int idx = doubles.IndexOf(v);
            if (idx != -1)
            {
                Util.WriteVarint(idx, stream);
                return;
            }
            doubles.Add(v);
            idx = doubles.Count - 1;
            Util.WriteVarint(idx, stream);
        }

        private void WriteString(string v)
        {
            Util.WriteVarint((int)Tag.STRING_TAG, stream);
            int idx = strs.IndexOf(v);
            if (idx != -1)
            {
                Util.WriteVarint(idx, stream);
                return;
            }
            strs.Add(v);
            idx = strs.Count - 1;
            Util.WriteVarint(idx, stream);
        }

        private void WriteArray(List<object> v)
        {
            Util.WriteVarint((int)Tag.ARRAY_TAG, stream);
            int len = v.Count;
            Util.WriteVarint(len, stream);
            for (int i = 0; i < len; i++)
            {
                RealWrite(v[i]);
            }
        }

        private void WriteDict(Dictionary<object, object> v)
        {
            Util.WriteVarint((int)Tag.OBJECT_TAG, stream);
            int len = v.Count;
            Util.WriteVarint(len, stream);
            foreach (var i in v)
            {
                RealWrite(i.Key);
                RealWrite(i.Value);
            }
        }

        private void MergeAll()
        {
            MemoryStream st = new MemoryStream();
            Util.WriteVarint(ints.Count, st);
            for(int i = 0; i < ints.Count; i++)
            {
                Util.WriteVarint(ints[i], st);
            }
            Util.WriteVarint(doubles.Count, st);
            for(int i = 0; i < doubles.Count; i++)
            {
                Util.WriteDobule(doubles[i], st);
            }
            Util.WriteVarint(strs.Count, st);
            for(int i = 0; i < strs.Count; i++)
            {
                Util.WriteVarint(strs[i].Length, st);
                Util.WriteStr(strs[i], st);
            }
            stream.Position = 0;
            st.Position = 0;
            int stLen = (int)st.Length;
            int streamLen = (int)st.Length;
            byte[] stBytes = new byte[stLen];
            byte[] streamBytes = new byte[streamLen];
            stream.Read(streamBytes, 0, streamLen);
            st.Read(stBytes, 0, stLen);
            stream.Position = 0;
            stream.Write(stBytes, 0, stLen);
            stream.Write(streamBytes, 0, streamLen);
        }

        public object ReadObject(Stream stream)
        {
                SetStream(stream);
                InitPool();
#if UNITY_IPHONE
#else
                System.Console.WriteLine("p " + stream.Position);
#endif
                byte tag = (byte)Util.ReadVarint(stream);
                object rst = null;
                if(tag == Tag.ARRAY_TAG)
                {
                    rst = ReadArray();
                    Reset();
                    return rst;
                }
                else if(tag == Tag.OBJECT_TAG)
                {
                    rst = ReadDict();
                    Reset();
                    return rst;
                }
#if UNITY_IPHONE
#else
                System.Console.WriteLine("fuck " + tag);
#endif
                Reset();
                return null;
        }

        private void InitPool()
        {
            int len_ints = Util.ReadVarint(stream);
            for(int i = 0; i < len_ints; i++)
            {
                this.ints.Add(Util.ReadVarint(stream));
            }
            int len_doubles = Util.ReadVarint(stream);
            for (int i = 0; i < len_doubles; i++)
            {
                this.doubles.Add(Util.ReadDouble(stream));
            }
            int len_strs = Util.ReadVarint(stream);
            for(int i = 0; i < len_strs; i++)
            {
                int l = Util.ReadVarint(stream);
                this.strs.Add(Util.ReadStr(l, stream));
            }
        }

        private List<object> ReadArray()
        {
            List<object> rst = new List<object>();
            int len = Util.ReadVarint(stream);
            for(int i = 0; i < len; i++)
            {
                byte tag = (byte)Util.ReadVarint(stream);
                if(tag == Tag.ARRAY_TAG)
                {
                    rst.Add(ReadArray());
                }
                else if(tag == Tag.OBJECT_TAG)
                {
                    rst.Add(ReadDict());
                }
                else if(tag == Tag.INT_TAG)
                {
                    rst.Add(ints[Util.ReadVarint(stream)]);
                }
                else if(tag == Tag.STRING_TAG)
                {
                    rst.Add(strs[Util.ReadVarint(stream)]);
                }
                else if(tag == Tag.DOUBLE_TAG)
                {
                    rst.Add(strs[Util.ReadVarint(stream)]);
                }

            }
            return rst;
        }

        private Dictionary<object, object> ReadDict()
        {
            Dictionary<object, object> rst = new Dictionary<object, object>();
            int len = Util.ReadVarint(stream);
            for (int i = 0; i < len; i++)
            {
                byte tag = (byte)Util.ReadVarint(stream);
                object k = null;
                if(tag == Tag.ARRAY_TAG)
                {
                    k = ReadArray();
                }
                else if(tag == Tag.OBJECT_TAG)
                {
                    k = ReadDict();
                }
                else if(tag == Tag.INT_TAG)
                {
                    k = ints[Util.ReadVarint(stream)];
                }
                else if(tag == Tag.STRING_TAG)
                {
                    k = strs[Util.ReadVarint(stream)];
                }
                else if(tag == Tag.DOUBLE_TAG)
                {
                    k = doubles[Util.ReadVarint(stream)];
                }

                tag = (byte)Util.ReadVarint(stream);
                object v = null;
                if (tag == Tag.ARRAY_TAG)
                {
                    v = ReadArray();
                }
                else if (tag == Tag.OBJECT_TAG)
                {
                    v = ReadDict();
                }
                else if (tag == Tag.INT_TAG)
                {
                    v = ints[Util.ReadVarint(stream)];
                }
                else if (tag == Tag.STRING_TAG)
                {
                    v = strs[Util.ReadVarint(stream)];
                }
                else if (tag == Tag.DOUBLE_TAG)
                {
                    v = doubles[Util.ReadVarint(stream)];
                }
                rst.Add(k, v);

            }
            return rst;
        }
    }
}
