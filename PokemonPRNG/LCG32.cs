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
    namespace StandardLCG
    {
        public static class StandardLCGExtension
        {
            private static readonly LCG32 lcg = new LCG32(LCGType.StandardLCG);
            /// <summary>
            /// 初期seedと消費数を指定してseedを取得します.
            /// </summary>
            /// <param name="InitialSeed">初期seed</param>
            /// <param name="n">消費数</param>
            /// <returns></returns>
            public static uint GetSeed(uint InitialSeed, uint n) { return lcg.NextSeed(InitialSeed, n); }

            /// <summary>
            /// 次のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed) { return lcg.NextSeed(seed); }

            /// <summary>
            /// n先のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed, uint n) { return lcg.NextSeed(seed, n); }

            /// <summary>
            /// 前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed) { return lcg.PrevSeed(seed); }

            /// <summary>
            /// n前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed, uint n) { return lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedを1つ進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed) { return seed = lcg.NextSeed(seed); }

            /// <summary>
            /// seedをn進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed, uint n) { return seed = lcg.NextSeed(seed, n); }

            /// <summary>
            /// seedを1つ戻します. 
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed) { return seed = lcg.PrevSeed(seed); }

            /// <summary>
            /// seedをn戻します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed, uint n) { return seed = lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedから16bitの乱数値を取得します. 渡したseedは更新されます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>16bit乱数値</returns>
            public static uint GetRand(ref this uint seed) { return seed.Advance() >> 16; }

            /// <summary>
            /// seedから0 ~ (m-1)の乱数を取得します.
            /// 16bit乱数値から0 ~ (m-1)の値を取得する処理はSetGetRandTypeで変更できます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="modulo"></param>
            /// <returns>o~(module-1)の値</returns>
            public static uint GetRand(ref this uint seed, uint m) { return (seed.Advance() >> 16) % m; }

            /// <summary>
            /// 指定したseedの0x0からの消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed) { return lcg.CalcIndex(seed); }

            /// <summary>
            /// 指定したseedの指定した初期seedから消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="InitialSeed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed, uint InitialSeed) { return lcg.CalcIndex(seed) - lcg.CalcIndex(InitialSeed); }
        }
    }
    namespace ReverseStdLCG
    {
        public static class ReverseStdLCGExtension
        {
            private static readonly LCG32 lcg = new LCG32(LCGType.ReverseStdLCG);
            /// <summary>
            /// 初期seedと消費数を指定してseedを取得します.
            /// </summary>
            /// <param name="InitialSeed">初期seed</param>
            /// <param name="n">消費数</param>
            /// <returns></returns>
            public static uint GetSeed(uint InitialSeed, uint n) { return lcg.NextSeed(InitialSeed, n); }

            /// <summary>
            /// 次のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed) { return lcg.NextSeed(seed); }

            /// <summary>
            /// n先のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed, uint n) { return lcg.NextSeed(seed, n); }

            /// <summary>
            /// 前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed) { return lcg.PrevSeed(seed); }

            /// <summary>
            /// n前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed, uint n) { return lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedを1つ進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed) { return seed = lcg.NextSeed(seed); }

            /// <summary>
            /// seedをn進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed, uint n) { return seed = lcg.NextSeed(seed, n); }

            /// <summary>
            /// seedを1つ戻します. 
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed) { return seed = lcg.PrevSeed(seed); }

            /// <summary>
            /// seedをn戻します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed, uint n) { return seed = lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedから16bitの乱数値を取得します. 渡したseedは更新されます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>16bit乱数値</returns>
            public static uint GetRand(ref this uint seed) { return seed.Advance() >> 16; }

            /// <summary>
            /// seedから0 ~ (m-1)の乱数を取得します.
            /// 16bit乱数値から0 ~ (m-1)の値を取得する処理はSetGetRandTypeで変更できます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="modulo"></param>
            /// <returns>o~(module-1)の値</returns>
            public static uint GetRand(ref this uint seed, uint m) { return (seed.Advance() >> 16) % m; }

            /// <summary>
            /// 指定したseedの0x0からの消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed) { return lcg.CalcIndex(seed); }

            /// <summary>
            /// 指定したseedの指定した初期seedから消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="InitialSeed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed, uint InitialSeed) { return lcg.CalcIndex(seed) - lcg.CalcIndex(InitialSeed); }
        }
    }
    namespace GCLCG
    {
        public static class GCLCGExtension
        {
            private static readonly LCG32 lcg = new LCG32(LCGType.GCLCG);
            /// <summary>
            /// 次のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed) { return lcg.NextSeed(seed); }

            /// <summary>
            /// n先のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed, uint n) { return lcg.NextSeed(seed, n); }

            /// <summary>
            /// 前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed) { return lcg.PrevSeed(seed); }

            /// <summary>
            /// n前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed, uint n) { return lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedを1つ進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed) { return seed = lcg.NextSeed(seed); }

            /// <summary>
            /// seedをn進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed, uint n) { return seed = lcg.NextSeed(seed, n); }

            /// <summary>
            /// seedを1つ戻します. 
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed) { return seed = lcg.PrevSeed(seed); }

            /// <summary>
            /// seedをn戻します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed, uint n) { return seed = lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedから16bitの乱数値を取得します. 渡したseedは更新されます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>16bit乱数値</returns>
            public static uint GetRand(ref this uint seed) { return seed.Advance() >> 16; }

            public static float GetRand_f(ref this uint seed) { return (seed.Advance() >> 16) / 65536.0f; }

            /// <summary>
            /// seedから0 ~ (m-1)の乱数を取得します.
            /// 16bit乱数値から0 ~ (m-1)の値を取得する処理はSetGetRandTypeで変更できます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="modulo"></param>
            /// <returns>o~(module-1)の値</returns>
            public static uint GetRand(ref this uint seed, uint m) { return (seed.Advance() >> 16) % m; }

            /// <summary>
            /// 指定したseedの0x0からの消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed) { return lcg.CalcIndex(seed); }

            /// <summary>
            /// 指定したseedの指定した初期seedから消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="InitialSeed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed, uint InitialSeed) { return lcg.CalcIndex(seed) - lcg.CalcIndex(InitialSeed); }
        }
    }
    namespace StaticLCG
    {
        public static class StaticLCGExtension
        {
            private static readonly LCG32 lcg = new LCG32(LCGType.StaticLCG);
            /// <summary>
            /// 次のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed) { return lcg.NextSeed(seed); }

            /// <summary>
            /// n先のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint NextSeed(this uint seed, uint n) { return lcg.NextSeed(seed, n); }

            /// <summary>
            /// 前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed) { return lcg.PrevSeed(seed); }

            /// <summary>
            /// n前のseedを取得します. 渡したseedは変更されません.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static uint PrevSeed(this uint seed, uint n) { return lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedを1つ進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed) { return seed = lcg.NextSeed(seed); }

            /// <summary>
            /// seedをn進めます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Advance(ref this uint seed, uint n) { return seed = lcg.NextSeed(seed, n); }

            /// <summary>
            /// seedを1つ戻します. 
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed) { return seed = lcg.PrevSeed(seed); }

            /// <summary>
            /// seedをn戻します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="n"></param>
            /// <returns>更新後のseed</returns>
            public static uint Back(ref this uint seed, uint n) { return seed = lcg.PrevSeed(seed, n); }

            /// <summary>
            /// seedから16bitの乱数値を取得します. 渡したseedは更新されます.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns>16bit乱数値</returns>
            public static uint GetRand(ref this uint seed) { return seed.Advance() >> 16; }

            /// <summary>
            /// seedから0 ~ (m-1)の乱数を取得します.
            /// 16bit乱数値から0 ~ (m-1)の値を取得する処理はSetGetRandTypeで変更できます.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="modulo"></param>
            /// <returns>o~(module-1)の値</returns>
            public static uint GetRand(ref this uint seed, uint m) { return (seed.Advance() >> 16) % m; }

            /// <summary>
            /// 指定したseedの0x0からの消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed) { return lcg.CalcIndex(seed); }

            /// <summary>
            /// 指定したseedの指定した初期seedから消費数を取得します.
            /// </summary>
            /// <param name="seed"></param>
            /// <param name="InitialSeed"></param>
            /// <returns></returns>
            public static uint GetIndex(this uint seed, uint InitialSeed) { return lcg.CalcIndex(seed) - lcg.CalcIndex(InitialSeed); }
        }
    }

    namespace Util
    {
        public static class LCG32Utils
        {
            public static IEnumerable<uint> GenerateSeedCollections(this uint seed, int count, Func<uint, uint> successor)
            {
                if (count <= 0) yield break;
                yield return seed;
                for (int i = 0; i < count - 1; i++)
                    yield return seed = successor(seed);
            }
            public static IEnumerable<(uint seed, T byProduct)> GenerateSeedCollection<T>(this uint seed, int count, Func<uint, (uint, T)> successor)
            {
                for (int i = 0; i < count; i++)
                {
                    (var next, var bp) = successor(seed);
                    yield return (seed, bp);
                    seed = next;
                }
            }
        }
    }
}