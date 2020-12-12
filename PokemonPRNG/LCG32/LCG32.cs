using System;
using System.Collections.Generic;

namespace PokemonPRNG.LCG32
{
    class LCG32
    {
        private readonly uint MultiplecationConst, AdditionConst, Reverse_MultiplecationConst, Reverse_AdditionConst;
        private readonly uint[] At, Bt, Ct, Dt;
        internal LCG32(LCGType type)
        {
            MultiplecationConst = type.MultiplecationConst;
            AdditionConst = type.AdditionConst;
            Reverse_MultiplecationConst = type.Reverse_MultiplecationConst;
            Reverse_AdditionConst = type.Reverse_AdditionConst;

            At = new uint[32]; Bt = new uint[32]; Ct = new uint[32]; Dt = new uint[32];
            At[0] = MultiplecationConst;
            Bt[0] = AdditionConst;
            Ct[0] = Reverse_MultiplecationConst;
            Dt[0] = Reverse_AdditionConst;
            for (int i = 1; i < 32; i++)
            {
                At[i] = At[i - 1] * At[i - 1];
                Bt[i] = Bt[i - 1] * (1 + At[i - 1]);
                Ct[i] = Ct[i - 1] * Ct[i - 1];
                Dt[i] = Dt[i - 1] * (1 + Ct[i - 1]);
            }
        }

        internal uint NextSeed(uint seed) { return seed * MultiplecationConst + AdditionConst; }
        internal uint NextSeed(uint seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * At[i] + Bt[i];

            return seed;
        }
        internal uint PrevSeed(uint seed) { return seed * Reverse_MultiplecationConst + Reverse_AdditionConst; }
        internal uint PrevSeed(uint seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * Ct[i] + Dt[i];

            return seed;
        }

        internal uint CalcIndex(uint seed)
        {
            return CalcIndex(seed, MultiplecationConst, AdditionConst, 32);
        }
        private uint CalcIndex(uint seed, uint A, uint B, uint order)
        {
            if (order == 0) return 0;
            else if ((seed & 1) == 0) return CalcIndex(seed / 2, A * A, (A + 1) * B / 2, order - 1) * 2;
            else return CalcIndex((A * seed + B) / 2, A * A, (A + 1) * B / 2, order - 1) * 2 - 1;
        }

    }
}