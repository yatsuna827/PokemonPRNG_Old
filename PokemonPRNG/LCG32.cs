namespace PokemonPRNG.LCG32
{
    public enum LCGType
    {
        /// <summary>
        /// S[n+1] = 0x41C64E6D*S[n] + 0x6073;
        /// </summary>
        StandardLCG,
        /// <summary>
        /// S[n+1] = 0x343FD*S[n] + 0x269EC3;
        /// </summary>
        GCLCG,
        /// <summary>
        /// S[n+1] = 0x41C64E6D*S[n] + 0x3039;
        /// </summary>
        StaticLCG
    };

    /// <summary>
    /// ポケモンの乱数処理を記述するときに使いそうな処理を集めました.
    /// メソッドの第1引数は基本的にthis修飾子が指定してあるので, seed.Advance()のように, メソッド呼出の感覚で使うことができます.
    /// </summary>
    public static class LCG32
    {
        private static uint MultiplecationConst, AdditionConst, Reverse_MultiplecationConst, Reverse_AdditionConst;
        private static uint[] At, Bt, Ct, Dt;

        static LCG32()
        {
            SetLCGType(LCGType.StandardLCG);
            SetArray();
        }

        /// <summary>
        /// LCGの種類を変更します. 
        /// </summary>
        /// <param name="type"></param>
        public static void SetLCGType(LCGType type)
        {
            (MultiplecationConst, AdditionConst) = type.GetConst();
            (Reverse_MultiplecationConst, Reverse_AdditionConst) = type.GetReverseConst();
        }
        private static void SetArray()
        {
            At = new uint[32]; Bt = new uint[32]; Ct = new uint[32]; Dt = new uint[32];
            At[0] = MultiplecationConst; Bt[0] = AdditionConst; Ct[0] = Reverse_MultiplecationConst; Dt[0] = Reverse_AdditionConst;
            for (int i = 1; i < 32; i++)
            {
                At[i] = At[i - 1] * At[i - 1];
                Bt[i] = Bt[i - 1] * (1 + At[i - 1]);
                Ct[i] = Ct[i - 1] * Ct[i - 1];
                Dt[i] = Dt[i - 1] * (1 + Ct[i - 1]);
            }
        }

        /// <summary>
        /// 初期seedと消費数を指定してseedを取得します.
        /// </summary>
        /// <param name="InitialSeed">初期seed</param>
        /// <param name="n">消費数</param>
        /// <returns></returns>
        public static uint GetLCGSeed(uint InitialSeed, uint n) { return InitialSeed.Advance(n); }

        /// <summary>
        /// 次のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static uint NextSeed(this uint seed) { return seed.Advance(); }

        /// <summary>
        /// n先のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static uint NextSeed(this uint seed, uint n) { return seed.Advance(n); }

        /// <summary>
        /// 前のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static uint PrevSeed(this uint seed) { return seed.Back(); }

        /// <summary>
        /// n前のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static uint PrevSeed(this uint seed, uint n) { return seed.Back(n); }

        /// <summary>
        /// seedを1つ進めます.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>更新後のseed</returns>
        public static uint Advance(ref this uint seed) { return seed = seed * 0x41C64E6D + 0x6073; }

        /// <summary>
        /// seedをn進めます.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns>更新後のseed</returns>
        public static uint Advance(ref this uint seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * At[i] + Bt[i];

            return seed;
        }

        /// <summary>
        /// seedを1つ戻します. 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>更新後のseed</returns>
        public static uint Back(ref this uint seed) { return seed = seed * 0xEEB9EB65 + 0xA3561A1; }

        /// <summary>
        /// seedをn戻します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns>更新後のseed</returns>
        public static uint Back(ref this uint seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * Ct[i] + Dt[i];

            return seed;
        }

        /// <summary>
        /// seedから16bitの乱数値を取得します. 渡したseedは更新されます.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>16bit乱数値</returns>
        public static uint GetRand(ref this uint seed) { return seed.Advance() >> 16; }

        /// <summary>
        /// seedから得られる16bitの乱数値をmoduloで割った余りを取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="modulo"></param>
        /// <returns>o~(module-1)の値</returns>
        public static uint GetRand(ref this uint seed, uint modulo) { return (seed.Advance() >> 16) % modulo; }

        /// <summary>
        /// 指定したseedの0x0からの消費数を取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static uint GetIndex(this uint seed) { return CalcIndex(seed, MultiplecationConst, AdditionConst, 32); }

        /// <summary>
        /// 指定したseedの指定した初期seedから消費数を取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="InitialSeed"></param>
        /// <returns></returns>
        public static uint GetIndex(this uint seed, uint InitialSeed) { return GetIndex(seed) - GetIndex(InitialSeed); }
        private static uint CalcIndex(uint seed, uint A, uint B, uint order)
        {
            if (order == 0) return 0;
            else if ((seed & 1) == 0) return CalcIndex(seed / 2, A * A, (A + 1) * B / 2, order - 1) * 2;
            else return CalcIndex((A * seed + B) / 2, A * A, (A + 1) * B / 2, order - 1) * 2 - 1;
        }

    }

    internal static class LCGParameter
    {
        private static readonly (uint, uint)[] Const = { (0x41C64E6D, 0x6073),(0x343FD, 0x269EC3), (0x41C64E6D, 0x3039) };
        private static readonly (uint, uint)[] ReverseConst = { (0xEEB9EB65, 0xA3561A1), (0xB9B33155, 0xA170F641), (0xEEB9EB65, 0xFC77A683) };
        internal static (uint, uint) GetConst(this LCGType type) { return Const[(int)type]; }
        internal static (uint, uint) GetReverseConst(this LCGType type) { return ReverseConst[(int)type]; }
    }
}