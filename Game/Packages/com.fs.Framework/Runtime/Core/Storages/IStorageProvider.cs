using System;
using System.Threading.Tasks;

namespace Common.Core.Storages.Providers
{
    public interface IStorageProvider
    {
        Task<byte[]> Load(string url, float? requestTimeout = null);
        Task Save(string url, ref byte[] data, float? requestTimeout = null);
    }
}