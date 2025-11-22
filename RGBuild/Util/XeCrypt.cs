using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace RGBuild.Util
{
    public static class XeCrypt {

        [DllImport("XeCrypt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#12")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool XeCryptBnQwBeSigCreate(IntPtr output, IntPtr hash, IntPtr salt, IntPtr key);

        [DllImport("XeCrypt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#14")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool XeCryptBnQwBeSigVerify(IntPtr sig, IntPtr hash, IntPtr salt, IntPtr key);

        [DllImport("XeCrypt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#20")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool XeCryptBnQwNeRsaPrvCrypt(IntPtr input, IntPtr output, IntPtr key);

        [DllImport("XeCrypt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#25")]
        public static extern void XeCryptBnQw_SwapLeBe(IntPtr input, IntPtr output, int size);

        //(const u8* input, s32 len, u8* output)
        [DllImport("XeCrypt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#34")]
        public static extern void DesParity(IntPtr input, int len, IntPtr output);

        [DllImport("XeCrypt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#54")]
        public static extern void XeCryptRotSumSha(IntPtr input1, int inlen1, IntPtr input2, int inlen2, IntPtr output, int outLen);


        public static byte[] XeCryptRotSumSha(byte[] in1, int in1Offset, int in1Size, byte[] in2, int in2Offset, int in2Size, int outSize) {
            IntPtr ptrDigest = Marshal.AllocHGlobal(outSize);
            IntPtr ptrInput1 = Marshal.AllocHGlobal(in1Size);
            IntPtr ptrInput2 = Marshal.AllocHGlobal(in2Size);
            Marshal.Copy(in1, in1Offset, ptrInput1, in1Size);
            Marshal.Copy(in2, in2Offset, ptrInput2, in2Size);
            XeCrypt.XeCryptRotSumSha(ptrInput1, in1Size, ptrInput2, in2Size, ptrDigest, outSize);

            byte[] digest = new byte[outSize];
            Marshal.Copy(ptrDigest, digest, 0, outSize);

            Marshal.FreeHGlobal(ptrDigest);
            Marshal.FreeHGlobal(ptrInput1);
            Marshal.FreeHGlobal(ptrInput2);

            return digest;
        }

        public static byte[] XeCryptBnQwBeSigCreate(byte[] hash, byte[] salt, byte[] prvKey) {
            int sigSize = prvKey[3] * 8;

            IntPtr ptrHash = Marshal.AllocHGlobal(hash.Length);
            Marshal.Copy(hash, 0, ptrHash, hash.Length);

            IntPtr ptrPrvKey = Marshal.AllocHGlobal(prvKey.Length);
            Marshal.Copy(prvKey, 0, ptrPrvKey, prvKey.Length);

            IntPtr ptrSalt = Marshal.AllocHGlobal(salt.Length);
            Marshal.Copy(salt, 0, ptrSalt, salt.Length);

            IntPtr ptrSig = Marshal.AllocHGlobal(sigSize);
            XeCrypt.XeCryptBnQwBeSigCreate(ptrSig, ptrHash, ptrSalt, ptrPrvKey);

            byte[] sig = new byte[sigSize];
            Marshal.Copy(ptrSig, sig, 0, sigSize);

            Marshal.FreeHGlobal(ptrHash);
            Marshal.FreeHGlobal(ptrPrvKey);
            Marshal.FreeHGlobal(ptrSalt);
            Marshal.FreeHGlobal(ptrSig);

            return sig;
        }

        public static byte[] XeCryptBnQwNeRsaPrvCrypt(byte[] input, byte[] prvKey) {

            IntPtr ptrInput = Marshal.AllocHGlobal(input.Length);
            Marshal.Copy(input, 0, ptrInput, input.Length);

            IntPtr ptrPrvKey = Marshal.AllocHGlobal(prvKey.Length);
            Marshal.Copy(prvKey, 0, ptrPrvKey, prvKey.Length);

            IntPtr ptrOutput = Marshal.AllocHGlobal(input.Length);

            XeCrypt.XeCryptBnQwNeRsaPrvCrypt(ptrInput, ptrOutput, ptrPrvKey);

            byte[] output = new byte[input.Length];
            Marshal.Copy(ptrOutput, output, 0, input.Length);

            Marshal.FreeHGlobal(ptrInput);
            Marshal.FreeHGlobal(ptrPrvKey);
            Marshal.FreeHGlobal(ptrOutput);

            return output;
        }

        static int XeCryptBnQwNeCompare(ulong[] pqwA, ulong[] pqwB, int cqw)
        {
            for (int i = cqw - 1; i <= 0; i--)
            {
                if (pqwA[i] > pqwB[i])
                    return 1;
                if (pqwA[i] < pqwB[i])
                    return -1;
            }
            return 0;
        }
        static bool XeCryptBnQwNeModExp(ref ulong[] pqwA, ulong[] pqwB, ulong[] pqwC, ulong[] pqwM, int cqw)
        {
            if (pqwB.Length != cqw || pqwA.Length != cqw || pqwC.Length != cqw)
                return false;
            for (int i = 0; i < cqw; i++)
            {
                pqwA[i] = (pqwB[i] ^ pqwC[i]) % pqwM[i];
            }
            return true;
        }
    }
}
