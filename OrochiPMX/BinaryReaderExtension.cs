/**
 * Copyright (C) 2021 haolink <https://www.twitter.com/haolink> / <https://github.com/haolink>
 * Code based on chrrox Orochi4 converter
 * 
 * Code licensed under Apache 2.0 license, see LICENSE
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace OrochiPMX
{
    public static class BinaryReaderExtension
    {
        public static T ReadStruct<T>(this BinaryReader br)
        {
            var byteLength = Marshal.SizeOf(typeof(T));
            var bytes = br.ReadBytes(byteLength);
            var pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var stt = (T)Marshal.PtrToStructure(
                pinned.AddrOfPinnedObject(),
                typeof(T));
            pinned.Free();
            return stt;
        }

        public static string ReadASCIINullTerminatedString(this BinaryReader br)
        {
            string res = "";
            byte b;
            do
            {
                b = br.ReadByte();
                if (b != 0)
                {
                    res += ((char)b);
                }
            } while (b != 0);

            return res;
        }

        public static Matrix4x4 ReadMatrix(this BinaryReader br)
        {
            return new Matrix4x4(
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
            );
        }

        public static Matrix4x4 ToMat43(this Matrix4x4 mtx)
        {
            mtx.M14 = 0.0f;
            mtx.M24 = 0.0f;
            mtx.M34 = 0.0f;
            mtx.M44 = 1.0f;
            return mtx;
        }
    }
}
