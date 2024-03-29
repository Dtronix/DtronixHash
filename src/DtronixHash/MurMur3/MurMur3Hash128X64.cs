﻿// Copyright 2012 Darren Kopp
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace DtronixHash.MurMur3
{
    public class MurMur3Hash128X64 : MurMur3Hash128
    {
        const ulong C1 = 0x87c37b91114253d5UL;
        const ulong C2 = 0x4cf5ad432745937fUL;

        private ulong H1 { get; set; }
        private ulong H2 { get; set; }



        public MurMur3Hash128X64(uint seed = 0)
            : base(seed)
        {
            Reset();
        }

        private void Reset()
        {
            // initialize hash values to seed values
            H1 = H2 = Seed;
        }

        public override void Initialize()
        {
            Reset();
            base.Initialize();
        }

        public override unsafe void TransformBlock(ReadOnlyMemory<byte> memory)
        {
            var length = memory.Length;
            if (length == 0)
                return;

            base.TransformBlock(memory);

            int remainder = length & 15;
            int blocks = length / 16;

            using var hMemory = memory.Pin();
            byte* d = (byte*) hMemory.Pointer;
            ulong* current = (ulong*) d;

            while (blocks-- > 0)
            {
                // a variant of original algorithm optimized for processor instruction pipelining
                H1 ^= (*current++ * C1).RotateLeft(31) * C2;
                H1 = (H1.RotateLeft(27) + H2) * 5 + 0x52dce729;

                H2 ^= (*current++ * C2).RotateLeft(33) * C1;
                H2 = (H2.RotateLeft(31) + H1) * 5 + 0x38495ab5;
            }

            if (remainder > 0)
            {
                byte* tail = d + (length - remainder);

                // create our keys and initialize to 0
                ulong k1 = 0, k2 = 0;

                // determine how many bytes we have left to work with based on length
                switch (remainder)
                {
                    case 15:
                        k2 ^= (ulong) tail[14] << 48;
                        goto case 14;
                    case 14:
                        k2 ^= (ulong) tail[13] << 40;
                        goto case 13;
                    case 13:
                        k2 ^= (ulong) tail[12] << 32;
                        goto case 12;
                    case 12:
                        k2 ^= (ulong) tail[11] << 24;
                        goto case 11;
                    case 11:
                        k2 ^= (ulong) tail[10] << 16;
                        goto case 10;
                    case 10:
                        k2 ^= (ulong) tail[9] << 8;
                        goto case 9;
                    case 9:
                        k2 ^= (ulong) tail[8] << 0;
                        goto case 8;
                    case 8:
                        k1 ^= (ulong) tail[7] << 56;
                        goto case 7;
                    case 7:
                        k1 ^= (ulong) tail[6] << 48;
                        goto case 6;
                    case 6:
                        k1 ^= (ulong) tail[5] << 40;
                        goto case 5;
                    case 5:
                        k1 ^= (ulong) tail[4] << 32;
                        goto case 4;
                    case 4:
                        k1 ^= (ulong) tail[3] << 24;
                        goto case 3;
                    case 3:
                        k1 ^= (ulong) tail[2] << 16;
                        goto case 2;
                    case 2:
                        k1 ^= (ulong) tail[1] << 8;
                        goto case 1;
                    case 1:
                        k1 ^= (ulong) tail[0] << 0;
                        break;
                }

                H2 ^= (k2 * C2).RotateLeft(33) * C1;
                H1 ^= (k1 * C1).RotateLeft(31) * C2;
            }
        }

        public override unsafe Memory<byte> FinalizeHash()
        {
            ulong len = (ulong) Length;
            H1 ^= len;
            H2 ^= len;

            H1 += H2;
            H2 += H1;

            H1 = H1.FMix();
            H2 = H2.FMix();

            H1 += H2;
            H2 += H1;

            var result = new Memory<byte>(new byte[16]);

            using var hMemory = result.Pin();
            var r = (ulong*) hMemory.Pointer;

            r[0] = H1;
            r[1] = H2;
            
            return result;
        }
    }
}
