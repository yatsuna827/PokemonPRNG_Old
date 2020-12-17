﻿using System;
using System.Collections.Generic;

namespace PokemonPRNG.LCG32
{
    // アルゴリズム参考
    // https://oupo.hatenadiary.com/entry/20100112/1263306151
    // https://oupo.hatenadiary.com/entry/20150128/1422413218
    class LCG32
    {
        private readonly uint mulConst, addConst, mulInvConst, addInvConst;
        private readonly uint[,] mulConstCache, addConstCache, mulInvConstCache, addInvConstCache;
        internal LCG32(LCGType type)
        {
            mulConst = type.MultiplecationConst;
            addConst = type.AdditionConst;
            mulInvConst = type.Reverse_MultiplecationConst;
            addInvConst = type.Reverse_AdditionConst;

            var At = new uint[32]; 
            var Bt = new uint[32]; 
            var Ct = new uint[32]; 
            var Dt = new uint[32];
            At[0] = mulConst;
            Bt[0] = addConst;
            Ct[0] = mulInvConst;
            Dt[0] = addInvConst;
            for (int i = 1; i < 32; i++)
            {
                At[i] = At[i - 1] * At[i - 1];
                Bt[i] = Bt[i - 1] * (1 + At[i - 1]);
                Ct[i] = Ct[i - 1] * Ct[i - 1];
                Dt[i] = Dt[i - 1] * (1 + Ct[i - 1]);
            }

            mulConstCache = new uint[4, 256]; 
            addConstCache = new uint[4, 256]; 
            mulInvConstCache = new uint[4, 256]; 
            addInvConstCache = new uint[4, 256];
            for (int i = 0; i < 4; i++)
            {
                var t = i << 3;
                mulConstCache[i, 0] = 1;
                addConstCache[i, 0] = 0;
                mulInvConstCache[i, 0] = 1;
                addInvConstCache[i, 0] = 0;

                mulConstCache[i, 1] = At[t];
                addConstCache[i, 1] = Bt[t];
                mulInvConstCache[i, 1] = Ct[t];
                addInvConstCache[i, 1] = Dt[t];
                for (int k = 2; k < 256; k++)
                {
                    mulConstCache[i, k] = mulConstCache[i, k - 1] * At[t];
                    addConstCache[i, k] = addConstCache[i, k - 1] * At[t] + Bt[t];
                    mulInvConstCache[i, k] = mulInvConstCache[i, k - 1] * Ct[t];
                    addInvConstCache[i, k] = addInvConstCache[i, k - 1] * Ct[t] + Dt[t];
                }
            }
        }

        internal uint NextSeed(uint seed) => seed * mulConst + addConst;
        internal uint NextSeed(uint seed, uint n)
        {
            for (int i = 0; i < 4; i++) seed = seed * mulConstCache[i, (n >> (i * 8)) & 0xff] + addConstCache[i, (n >> (i * 8)) & 0xff];
            return seed;
        }
        internal uint PrevSeed(uint seed) => seed * mulInvConst + addInvConst;
        internal uint PrevSeed(uint seed, uint n)
        {
            for (int i = 0; i < 4; i++) seed = seed * mulInvConstCache[i, (n >> (i * 8)) & 0xff] + addInvConstCache[i, (n >> (i * 8)) & 0xff];
            return seed;
        }

        internal uint CalcIndex(uint seed)
        {
            return CalcIndex(seed, mulConst, addConst, 32);
        }
        private uint CalcIndex(uint seed, uint A, uint B, uint order)
        {
            if (order == 0) return 0;
            else if ((seed & 1) == 0) return CalcIndex(seed / 2, A * A, (A + 1) * B / 2, order - 1) * 2;
            else return CalcIndex((A * seed + B) / 2, A * A, (A + 1) * B / 2, order - 1) * 2 - 1;
        }

    }
}