using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ProtoBuf;
using UnityEngine;

namespace LuaFramework
{
    class StructureTransform
    {
        static bool bBig = false;//defalut little
        public static void Reverse(byte[] msg)
        {
            if(!bBig)
            {
                Array.Reverse(msg);
            }
        }

        public static void Reverse(byte[] msg, int nOffest, int nSize)
        {
            if(!bBig)
            {
                Array.Reverse(msg, nOffest, nSize);
            }
        }


        public static bool SetEndian(bool bIsBig)
        {
            bBig = bIsBig;
            return bBig;
        }

        public static void ByteArrayToStructureEndian(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            byte[] temparray = (byte[])bytearray.Clone();
            // ŽÓœá¹¹ÌåÖžÕë¹¹Ôìœá¹¹Ìå
            obj = Marshal.PtrToStructure(i, obj.GetType());
            // ×öŽó¶Ë×ª»»
            object thisBoxed = obj;
            Type test = thisBoxed.GetType();
            int reversestartoffset = startoffset;

            // ÁÐŸÙœá¹¹ÌåµÄÃ¿žö³ÉÔ±£¬²¢Reverse
            foreach(var field in test.GetFields())
            {
                object fieldValue = field.GetValue(thisBoxed); // Get value
                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());  //Get Type

                if(typeCode != TypeCode.Object)    //Èç¹ûÎªÖµÀàÐÍ
                {
                    Reverse(temparray, reversestartoffset, Marshal.SizeOf(fieldValue));
                    reversestartoffset += Marshal.SizeOf(fieldValue);
                }
                else
                {
                    reversestartoffset += ((byte[])fieldValue).Length;
                }
            }

            try
            {
                Marshal.Copy(temparray, startoffset, i, len);
            }
            catch(Exception ex)
            {
                Console.WriteLine("ByteArrayToStructure FAIL: error " + ex.ToString());
            }

            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);
        }

        public static byte[] StructureToByteArrayEndian(object obj)
        {
            object thisBoxed = obj;
            Type test = thisBoxed.GetType();
            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(thisBoxed)];
            object fieldValue;
            TypeCode typeCode;
            byte[] temp;

            foreach(var field in test.GetFields())
            {
                fieldValue = field.GetValue(thisBoxed); // Get value
                typeCode = Type.GetTypeCode(fieldValue.GetType());  // get type

                switch(typeCode)
                {
                    case TypeCode.Single: // float
                        {
                            temp = BitConverter.GetBytes((Single)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Single));
                            break;
                        }

                    case TypeCode.Int32:
                        {
                            temp = BitConverter.GetBytes((Int32)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int32));
                            break;
                        }

                    case TypeCode.UInt32:
                        {
                            temp = BitConverter.GetBytes((UInt32)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt32));
                            break;
                        }

                    case TypeCode.Int16:
                        {
                            temp = BitConverter.GetBytes((Int16)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int16));
                            break;
                        }

                    case TypeCode.UInt16:
                        {
                            temp = BitConverter.GetBytes((UInt16)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt16));
                            break;
                        }

                    case TypeCode.Int64:
                        {
                            temp = BitConverter.GetBytes((Int64)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int64));
                            break;
                        }

                    case TypeCode.UInt64:
                        {
                            temp = BitConverter.GetBytes((UInt64)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt64));
                            break;
                        }

                    case TypeCode.Double:
                        {
                            temp = BitConverter.GetBytes((Double)fieldValue);
                            StructureTransform.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Double));
                            break;
                        }

                    case TypeCode.Byte:
                        {
                            data[offset] = (Byte)fieldValue;
                            break;
                        }

                    default:
                        {
                            //System.Diagnostics.Debug.Fail("No conversion provided for this type : " + typeCode.ToString());
                            break;
                        }
                }; // switch

                if(typeCode == TypeCode.Object)
                {
                    int length = ((byte[])fieldValue).Length;
                    Array.Copy(((byte[])fieldValue), 0, data, offset, length);
                    offset += length;
                }
                else
                {
                    offset += Marshal.SizeOf(fieldValue);
                }
            } // foreach

            return data;
        } // Swap
    };
}