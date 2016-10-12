using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Runtime.InteropServices;
using System.IO;
using ProtoBuf;

namespace LuaFramework
{
    public class ConstDefine
    {
        public static int NF_PACKET_HEAD_SIZE = 6;
        public static int MAX_PACKET_LEN = 655360;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgHead
    {
        public MsgHead()
        {
            unMsgID = 0;
            unDataLen = 0;
        }
        public UInt16 unMsgID;
        public UInt32 unDataLen;
    };

    public class NetworkManager : Manager
    {
        private SocketClient socket;
        static readonly object m_lockObject = new object();
        static Queue<KeyValuePair<int, ByteBuffer>> mEvents = new Queue<KeyValuePair<int, ByteBuffer>>();

        public NFGUID mMainID = new NFGUID();

        SocketClient SocketClient
        {
            get
            {
                if(socket == null)
                    socket = new SocketClient();

                return socket;
            }
        }

        void Awake()
        {
            Init();
        }

        void Init()
        {
            SocketClient.OnRegister();
        }

        public void OnInit()
        {
            CallMethod("Start");
        }

        public void Unload()
        {
            CallMethod("Unload");
        }

        /// <summary>
        /// 执行Lua方法
        /// </summary>
        public object[] CallMethod(string func, params object[] args)
        {
            return Util.CallMethod("Network", func, args);
        }

        ///------------------------------------------------------------------------------------
        public static void AddEvent(int _event, ByteBuffer data)
        {
            lock(m_lockObject)
            {
                mEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
            }
        }

        /// <summary>
        /// 交给Command，这里不想关心发给谁。
        /// </summary>
        void Update()
        {
            if(mEvents.Count > 0)
            {
                while(mEvents.Count > 0)
                {
                    KeyValuePair<int, ByteBuffer> _event = mEvents.Dequeue();
                    facade.SendMessageCommand(NotiConst.DISPATCH_MESSAGE, _event);
                }
            }
        }

        /// <summary>
        /// 发送链接请求
        /// </summary>
        public void SendConnect()
        {
            SocketClient.SendConnect();
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        public void SendMsg(int uMsgID, ByteBuffer buffer)
        {
            NFMsg.MsgBase xData = new NFMsg.MsgBase();
            xData.player_id = Util.NFToPB(mMainID);
            xData.msg_data = buffer.ToBytes();
            MemoryStream body = new MemoryStream();
            Serializer.Serialize<NFMsg.MsgBase>(body, xData);
            MsgHead head = new MsgHead();
            head.unMsgID = (UInt16)uMsgID;
            head.unDataLen = (UInt32)body.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;
            byte[] bodyByte = body.ToArray();
            byte[] headByte = StructureTransform.StructureToByteArrayEndian(head);
            byte[] sendBytes = new byte[head.unDataLen];
            headByte.CopyTo(sendBytes, 0);
            bodyByte.CopyTo(sendBytes, headByte.Length);
            SocketClient.SendMessage(sendBytes);
            buffer.Close();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy()
        {
            SocketClient.OnRemove();
            Debug.Log("~NetworkManager was destroy");
        }
    }
}