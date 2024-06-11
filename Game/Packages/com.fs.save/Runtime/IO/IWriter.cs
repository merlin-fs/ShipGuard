using System;
using System.Threading.Tasks;

namespace Common.Storage
{
    public interface IWriter
    {
        Task Save(IStorageContext context, ref byte[] data);
    }
}