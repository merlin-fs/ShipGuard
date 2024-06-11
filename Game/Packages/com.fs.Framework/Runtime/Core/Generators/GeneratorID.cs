using System;
using System.Threading;

namespace Common.Core
{

    /// <summary>
    /// Inspired by <see href="https://github.com/aspnet/KestrelHttpServer/blob/6fde01a825cffc09998d3f8a49464f7fbe40f9c4/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>,
    /// this class generates an efficient 20-bytes ID which is the concatenation of a <c>base36</c> encoded
    /// machine name and <c>base32</c> encoded <see cref="long"/> using the alphabet <c>0-9</c> and <c>A-V</c>.
    /// </summary>
    public sealed class GeneratorID
    {
        private const string ENCODE_32_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
        private static readonly char[] m_Prefix = new char[6];
        private static long m_LastId;

        private static readonly ThreadLocal<char[]> m_CharBufferThreadLocal =
            new ThreadLocal<char[]>(() =>
            {
                var buffer = new char[20];
                buffer[0] = m_Prefix[0];
                buffer[1] = m_Prefix[1];
                buffer[2] = m_Prefix[2];
                buffer[3] = m_Prefix[3];
                buffer[4] = m_Prefix[4];
                buffer[5] = m_Prefix[5];
                buffer[6] = '-';
                return buffer;
            });

        public GeneratorID(string prefix, long? lastId)
        {
            PopulatePrefix(prefix);
            m_LastId = lastId ?? DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Returns an ID. e.g: <c>XOGLN1-0HLHI1F5INOFA</c>
        /// </summary>
        public string Next => GenerateImpl(Interlocked.Increment(ref m_LastId));

        private static string GenerateImpl(long id)
        {
            var buffer = m_CharBufferThreadLocal.Value;

            buffer[7]  = ENCODE_32_CHARS[(int)(id >> 60) & 31];
            buffer[8]  = ENCODE_32_CHARS[(int)(id >> 55) & 31];
            buffer[9]  = ENCODE_32_CHARS[(int)(id >> 50) & 31];
            buffer[10] = ENCODE_32_CHARS[(int)(id >> 45) & 31];
            buffer[11] = ENCODE_32_CHARS[(int)(id >> 40) & 31];
            buffer[12] = ENCODE_32_CHARS[(int)(id >> 35) & 31];
            buffer[13] = ENCODE_32_CHARS[(int)(id >> 30) & 31];
            buffer[14] = ENCODE_32_CHARS[(int)(id >> 25) & 31];
            buffer[15] = ENCODE_32_CHARS[(int)(id >> 20) & 31];
            buffer[16] = ENCODE_32_CHARS[(int)(id >> 15) & 31];
            buffer[17] = ENCODE_32_CHARS[(int)(id >> 10) & 31];
            buffer[18] = ENCODE_32_CHARS[(int)(id >> 5) & 31];
            buffer[19] = ENCODE_32_CHARS[(int)id & 31];

            return new string(buffer, 0, buffer.Length);
        }

        private static void PopulatePrefix(string prefix)
        {
            var i = m_Prefix.Length - 1;
            var j = 0;
            while (i >= 0)
            {
                if (j < prefix.Length)
                {
                    m_Prefix[i] = prefix[j];
                    j++;
                }
                else
                {
                    m_Prefix[i] = '0';
                }
                i--;
            }
        }
    }
}