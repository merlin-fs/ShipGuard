using System;
using System.Text;
using System.Threading;

namespace Common.Core
{

    /// <summary>
    /// Inspired by <see href="https://github.com/aspnet/KestrelHttpServer/blob/6fde01a825cffc09998d3f8a49464f7fbe40f9c4/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>,
    /// this class generates an efficient 20-bytes ID which is the concatenation of a <c>base36</c> encoded
    /// machine name and <c>base32</c> encoded <see cref="long"/> using the alphabet <c>0-9</c> and <c>A-V</c>.
    /// </summary>
    public sealed class Generator16ID
    {
        private const string ENCODE_32_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
        private readonly sbyte[] m_Prefix = new sbyte[6];
        private long m_LastId;
        private int m_PrefixLen;

        private readonly ThreadLocal<sbyte[]> m_CharBufferThreadLocal;

        public Generator16ID(string prefix, long? lastId)
        {
            m_PrefixLen = PopulatePrefix(prefix);
            m_LastId = lastId ?? DateTime.UtcNow.Ticks;
            m_CharBufferThreadLocal = new ThreadLocal<sbyte[]>(() =>
            {
                var buffer = new sbyte[16];
                for (int i = 0; i < m_PrefixLen - 1; i++)
                {
                    buffer[i] = m_Prefix[(m_PrefixLen - i) - 1];
                }

                buffer[m_PrefixLen - 1] = (sbyte)'-';
                return buffer;
            });
        }

        /// <summary>
        /// Returns an ID. e.g: <c>XOGLN1-0HLHI1F5INOFA</c>
        /// </summary>
        public string NextStr => GenerateImplString(Interlocked.Increment(ref m_LastId));
        public sbyte[] Next  => GenerateImpl(Interlocked.Increment(ref m_LastId));

        private sbyte[] GenerateImpl(long id)
        {
            var buffer = m_CharBufferThreadLocal.Value;
            for (var i = m_PrefixLen; i < 16; i++)
            {
                var shr = (i - m_PrefixLen) * 5;
                buffer[i] = (sbyte)ENCODE_32_CHARS[(int)(id >> shr) & 31];
            }

            return buffer;
        }
    
        private unsafe string GenerateImplString(long id)
        {
            var buffer = GenerateImpl(id);
            fixed (sbyte* ptr = &buffer[0])
                return new string(ptr, 0, buffer.Length);
        }

        private int PopulatePrefix(string prefix)
        {
            var i = m_Prefix.Length - 1;
            var j = 0;
            while (i >= 0)
            {
                if (j < prefix.Length)
                {
                    m_Prefix[i] = (sbyte)prefix[j];
                    j++;
                }
                else
                {
                    m_Prefix[i] = (sbyte)'0';
                }
                i--;
            }

            return Math.Min(m_Prefix.Length, 6);
        }
    }
}