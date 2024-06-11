using System;
using System.Threading.Tasks;

namespace Common.Storage
{
    public interface IReader
    {
        Task<byte[]> Load(IStorageContext context);
    }
}