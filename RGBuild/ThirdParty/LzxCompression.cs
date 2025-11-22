using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RGBuild.Util {

    public unsafe class LzxCompression {

        #region DLLIMPORTS

        [DllImport("LZX.dll", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern uint LCICreateCompression(out uint pcbDataBlockMax, LZX_CONFIGURATION* pvConfiguration, out uint pcbDestBufferMin, out IntPtr pmchHandle, LCICreateCompression_Callback pfnCallback, IntPtr pfciData);

        [DllImport("LZX.dll", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern uint LCICompress(IntPtr pmchHandle, byte[] pbSrc, uint cbSrc, byte[] pbDst, uint cbDst, out uint pcbResult);

        [DllImport("LZX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LCIFlushCompressorOutput(IntPtr pmchHandle);

        [DllImport("LZX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LCIDestroyCompression(IntPtr pmchHandle);

        [DllImport("LZX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LCIResetCompression(IntPtr pmchHandle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate uint LCICreateCompression_Callback(IntPtr fci_data, byte* pbCompressed, uint cbCompressed, uint cbDecompressed);

        #endregion

        public struct LZXBlock {
            public uint cbCompressed;
            public uint cbUncompressed;
            public byte[] pbCompressed;
        }

        public struct LZX_CONFIGURATION {
            public uint CompressionWindowSize;
            public uint SecondPartitionSize;
        }

        private IntPtr pHandle = IntPtr.Zero;
        List<LZXBlock> blocks = new List<LZXBlock>();
        LCICreateCompression_Callback callback;
        private int streamCount = 0;

        public unsafe LzxCompression(uint CompressionWindowSize, uint DecompressedBlockSize) {

            uint dataBlockMax = 0x8000;

            LZX_CONFIGURATION config = new LZX_CONFIGURATION();
            config.CompressionWindowSize = CompressionWindowSize;
            config.SecondPartitionSize = DecompressedBlockSize;

            uint dstBufferMin = 0;

            callback = new LCICreateCompression_Callback(compression_callback);

            uint result = LCICreateCompression(out dataBlockMax, &config, out dstBufferMin, out pHandle, callback, IntPtr.Zero);

            if(result != 0) {
                throw new Exception($"LCICreateCompression failed with 0x{result:x}");
            }

        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public unsafe List<LZXBlock> Compress(byte[] contents) {
            uint dstResultLength = 0;
            for (int i = 0; i * 0x8000 < contents.Length; i++) {

                uint result;
                for (int t = 0; t < 3; t++) {
                    try {
                        byte[] block = new byte[0x8000];
                        int blockSize = Math.Min(contents.Length - (i * 0x8000), 0x8000);
                        Buffer.BlockCopy(contents, i * 0x8000, block, 0, blockSize);
                        result = LCICompress(pHandle, block, (uint)blockSize, null, 0x8000 + 0x1800, out dstResultLength);
                        if (result != 0) {
                            throw new Exception($"LCICompress failed - 0x{result:x}");
                        }
                        if (contents.Length > 0x1E84800) {
                            // this is a hack for files bigger than 32MB
                            LCIFlushCompressorOutput(pHandle);
                        }
                        break;
                    } catch (Exception e) {
                        Console.WriteLine("LCICompress exception thrown: " + e.ToString());
                        LCIResetCompression(pHandle);
                    }
                }
            }
            LCIFlushCompressorOutput(pHandle);

            return blocks;
        }

        public unsafe uint compression_callback(IntPtr fci_data, byte* pbCompressed, uint cbCompressed, uint cbUncompressed) {
            LZXBlock block = new LZXBlock();

            block.pbCompressed = new byte[cbCompressed];
            block.cbCompressed = cbCompressed;
            block.cbUncompressed = cbUncompressed;

            for (var i = 0; i < cbCompressed; i++) {
                block.pbCompressed[i] = *pbCompressed;
                pbCompressed++;
            }

            blocks.Add(block);

            return 0;
        }

        public void Reset() {
            LCIResetCompression(pHandle);
            blocks.Clear();
        }
    }
}
