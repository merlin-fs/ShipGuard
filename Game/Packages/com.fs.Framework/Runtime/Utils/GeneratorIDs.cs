using System;
using System.Buffers;
using System.Threading;

namespace Common.Core
{
    public static class GeneratorIDs
    {
        private const string ENCODE_32_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
        private static long m_LastId = DateTime.UtcNow.Ticks;

        public static string GetNextID() => GenerateSpan(Interlocked.Increment(ref m_LastId));

        private static string GenerateSpan(long id) => string.Create(13, id, m_WriteToStringMemory);

        // ReSharper disable once ConvertClosureToMethodGroup
        private static readonly SpanAction<char, long> m_WriteToStringMemory = (span, id) => WriteToStringMemory(span, id);

        private static void WriteToStringMemory(Span<char> span, long id)
        {
            span[12] = ENCODE_32_CHARS[(int)id & 31];
            span[11] = ENCODE_32_CHARS[(int)(id >> 5) & 31];
            span[10] = ENCODE_32_CHARS[(int)(id >> 10) & 31];
            span[9] = ENCODE_32_CHARS[(int)(id >> 15) & 31];
            span[8] = ENCODE_32_CHARS[(int)(id >> 20) & 31];
            span[7] = ENCODE_32_CHARS[(int)(id >> 25) & 31];
            span[6] = ENCODE_32_CHARS[(int)(id >> 30) & 31];
            span[5] = ENCODE_32_CHARS[(int)(id >> 35) & 31];
            span[4] = ENCODE_32_CHARS[(int)(id >> 40) & 31];
            span[3] = ENCODE_32_CHARS[(int)(id >> 45) & 31];
            span[2] = ENCODE_32_CHARS[(int)(id >> 50) & 31];
            span[1] = ENCODE_32_CHARS[(int)(id >> 55) & 31];
            span[0] = ENCODE_32_CHARS[(int)(id >> 60) & 31];
        }
    }
}