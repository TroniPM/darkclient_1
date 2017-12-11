// 模块名   :  TCPClientWorker
// 创建者   :  Steven Yang
// 创建日期 :  2012-12-8
// 描    述 :  客户端网络接收类

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Mogo.RPC;
using System.Collections.Generic;
using System.Threading;

namespace Mogo.Util
{
    /// <summary>
    ///TCP Client 类，用于异步收发包
    ///做了并包处理
    ///使用 .net TcpClient 实现
    /// </summary>
    public class TCPClientWorker
    {
        private Queue<byte[]> m_sendQueue = new Queue<byte[]>();
        private Queue<byte[]> m_recvQueue = new Queue<byte[]>();
        private const int MAX_BUFFER_SIZE = 65535;
        /// <summary>
        /// 预留字节长度
        /// </summary>
        private const int RESERVE_SIZE = 2;
        private byte[] m_sendBuffer;
        private byte[] m_recvBuffer;
        private int m_nRecvBufferSize = 0;
        private Socket m_socket = null;
        /// <summary>
        /// 消息包长度类型转换器
        /// </summary>
        private VObject m_headLengthConverter;
        /// <summary>
        /// 异步读取数据线程。
        /// </summary>
        private Thread m_receiveThread;
        /// <summary>
        /// 异步发送数据线程。
        /// </summary>
        private Thread m_sendThread;
        /// <summary>
        /// 接收数据队列同步锁。
        /// </summary>
        private readonly object m_recvQueueLocker = new object();
        /// <summary>
        /// 发送数据队列同步锁。
        /// </summary>
        private readonly object m_sendQueueLocker = new object();
        /// <summary>
        /// 网络通信同步锁。
        /// </summary>
        private readonly object m_tcpClientLocker = new object();
        /// <summary>
        /// 读取流数据量标记
        /// </summary>
        private Int32 bytesRead;

        private uint m_ConnectChecker = 0;
        private readonly object m_connectCheckerLocker = new object();
        private bool m_asynSendSwitch = true;

        public Action OnNetworkDisconnected;

        public TCPClientWorker()
        {
            m_sendBuffer = new byte[MAX_BUFFER_SIZE];
            m_recvBuffer = new byte[MAX_BUFFER_SIZE];
            m_headLengthConverter = VUInt32.Instance;

            if (m_sendThread == null)
            {
                LoggerHelper.Debug("init AsynSend");
                m_sendThread = new Thread(new ThreadStart(AsynSend));
                m_sendThread.IsBackground = true;
            }

            if (!m_sendThread.IsAlive)
            {
                LoggerHelper.Debug("Start AsynSend: " + m_asynSendSwitch);
                m_sendThread.Start();
            }
        }

        /// <summary>
        /// 帧驱动函数
        /// </summary>
        public void Process()
        {
            //DoSend();
        }

        /// <summary>
        /// 提供给上层的发送函数。 仅将数据放入发送队列
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            lock (m_sendQueueLocker)
                m_sendQueue.Enqueue(bytes);
        }

        /// <summary>
        ///提供给上层的接受函数， 仅从接受队列获取一个数据包
        ///空队列返回 null
        /// </summary>
        /// <returns></returns>
        public byte[] Recv()
        {
            if (m_recvQueue.Count > 0)
            {
                byte[] res;
                lock (m_recvQueueLocker)
                    res = m_recvQueue.Dequeue();
                return res;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 链接服务器。 失败会抛出字符串异常。
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        public void Connect(string IP, int Port)
        {
            lock (m_tcpClientLocker)
            {
                if ((m_socket != null) && (m_socket.Connected == true))
                {
                    throw new Exception("Exception. the tcpClient has Connectted.");
                }

                try
                {
                    LoggerHelper.Debug("Connect.m_ConnectChecker: " + m_ConnectChecker);
                    m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    m_socket.ReceiveBufferSize = MAX_BUFFER_SIZE;
                    m_socket.NoDelay = true;
                    m_socket.Connect(IP, Port);
                    if (m_socket != null)
                    {
                        if (m_receiveThread == null)
                        {
                            m_receiveThread = new Thread(new ThreadStart(DoReceive));
                            m_receiveThread.IsBackground = true;
                        }

                        if (!m_receiveThread.IsAlive)
                            m_receiveThread.Start();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Exception. the tcpClient do connecting error." + e);
                }
            }
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        public void Close()
        {
            lock (m_connectCheckerLocker)
                m_ConnectChecker++;
            if (m_socket != null)
                m_socket.Close();
            lock (m_tcpClientLocker)
            {
                if ((m_socket != null) && (m_socket.Connected == true))
                    m_socket.Close();
                m_socket = null;
            }
            m_receiveThread = null;//线程不用主动中断，只要保证内部没有执行，清除引用后会自动回收
            GC.Collect();
        }

        /// <summary>
        /// 强迫中断线程
        /// </summary>
        public void Release()
        {
            m_asynSendSwitch = false;//关闭发送死循环
        }

        /// <summary>
        /// 判断是否处于连接状态
        /// </summary>
        /// <returns></returns>
        public bool Connected()
        {
            if ((m_socket != null) && (m_socket.Connected == true))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 启动线程发送数据。
        /// </summary>
        private void AsynSend()
        {
            while (m_asynSendSwitch)
            {
                DoSend();
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 每帧调用， 发送数据。
        /// 并包处理
        /// </summary>
        private void DoSend()
        {
            lock (m_tcpClientLocker)
            {
                if ((m_socket == null) || (m_socket.Connected == false))
                {
                    return;
                }
            }
            int nTotalLength = 0;

            // 并包
            lock (m_sendQueueLocker)
            {
                if (m_sendQueue.Count == 0)
                {
                    return;
                }
                while ((nTotalLength < MAX_BUFFER_SIZE) && m_sendQueue.Count > 0)
                {
                    byte[] packet = m_sendQueue.Peek();
                    if (nTotalLength + RESERVE_SIZE + packet.Length < MAX_BUFFER_SIZE)
                    {
                        byte[] length = m_headLengthConverter.Encode((uint)(packet.Length + RESERVE_SIZE + m_headLengthConverter.VTypeLength));
                        length.CopyTo(m_sendBuffer, nTotalLength);
                        nTotalLength += length.Length;
                        nTotalLength += RESERVE_SIZE;
                        packet.CopyTo(m_sendBuffer, nTotalLength);
                        nTotalLength += packet.Length;
                        m_sendQueue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            // 真正发包到服务器
            try
            {
                m_socket.Send(m_sendBuffer, 0, nTotalLength, SocketFlags.None);
                // 清空发送缓存
                System.Array.Clear(m_sendBuffer, 0, MAX_BUFFER_SIZE);
            }
            catch (Exception e)
            {
                LoggerHelper.Debug("stream write error : " + e.ToString());
            }
        }

        private void DoReceive()
        {
            //读取流
            do
            {
                bytesRead = 0;
                try
                {
                    int size = MAX_BUFFER_SIZE - m_nRecvBufferSize;
                    if (size > 0)
                    {
                        bytesRead = m_socket.Receive(m_recvBuffer, m_nRecvBufferSize, size, SocketFlags.None);
                        m_nRecvBufferSize += bytesRead;
                        if (bytesRead == 0)
                        {//读的长度为0
                            bytesRead = 1;
                        }
                    }
                    else
                    {
                        bytesRead = 1;//缓存不够时继续循环，后面会对缓存数据进行处理
                        LoggerHelper.Warning("buffer not enough");
                    }
                }
                catch (ObjectDisposedException)
                {
                    // 网络流已被关闭，结束接收数据
                    LoggerHelper.Error("tcp close");
                }
                catch (IOException ioex)
                {
                    //捕获WSACancelBlockingCall()导致的异常。
                    //原因：强迫终止一个在进行的阻塞调用。
                    //可直接捕获忽略，应该不会有不良影响。
                    //LoggerHelper.Error("WSACancelBlockCall");
                    LoggerHelper.Except(ioex);
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
                //LoggerHelper.Debug("DataReceive: " + bytesRead);
                SplitPackets();
            } while (bytesRead > 0);

            lock (m_connectCheckerLocker)
                if (m_ConnectChecker > 0)
                {
                    m_ConnectChecker--;
                    LoggerHelper.Debug("Disconnected.m_ConnectChecker: " + m_ConnectChecker);
                }
                else
                {
                    LoggerHelper.Error("receive bytes " + bytesRead);
                    TimerHeap.AddTimer(1000, 0, OnNetworkDisconnected);
                }

            LoggerHelper.Debug("DataReceiveComplete");
        }

        /// <summary>
        /// 从RecvBuffer 中切分出多个Packets, 不足一个 Packet 的部分， 存留在 Buffer 中留待下次Split
        /// </summary>
        private void SplitPackets()
        {
            try
            {
                int offset = 0;
                while (m_nRecvBufferSize > m_headLengthConverter.VTypeLength)
                {
                    try
                    {
                        int nLength = (int)(uint)m_headLengthConverter.Decode(m_recvBuffer, ref offset);
                        if (m_nRecvBufferSize >= nLength)
                        {
                            int realLength = nLength - m_headLengthConverter.VTypeLength - RESERVE_SIZE;
                            offset += RESERVE_SIZE;
                            byte[] packet = new byte[realLength];
                            Buffer.BlockCopy(m_recvBuffer, offset, packet, 0, realLength);
                            lock (m_recvQueueLocker)//此处理为独立线程处理，需加锁，否则会出现丢包
                            {
                                m_recvQueue.Enqueue(packet);
                            }
                            m_nRecvBufferSize -= nLength;
                            offset += realLength;
                        }
                        else
                        {
                            offset -= m_headLengthConverter.VTypeLength;//m_headLengthConverter.Decode()自带偏移，需要调整偏移
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Except(ex);
                        break;
                    }
                }
                // 整理 RecvBuffer， 将buffer 内容前移
                Buffer.BlockCopy(m_recvBuffer, offset, m_recvBuffer, 0, m_nRecvBufferSize);
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                LoggerHelper.Critical("SplitPackets error.");
                Close();
            }
        }
    }
}