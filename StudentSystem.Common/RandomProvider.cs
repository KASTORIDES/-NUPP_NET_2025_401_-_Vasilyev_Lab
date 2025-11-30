using System;
using System.Threading;

namespace StudentSystem.Common
{
    // Безпечний генератор випадкових чисел для багатопотоковості
    public static class RandomProvider
    {
        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> _threadRandom = new ThreadLocal<Random>(() =>
            new Random(Interlocked.Increment(ref _seed))
        );

        public static Random GetThreadRandom() => _threadRandom.Value;
    }
}
