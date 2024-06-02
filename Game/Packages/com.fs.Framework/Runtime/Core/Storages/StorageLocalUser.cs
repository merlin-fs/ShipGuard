using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.Core.Storages.Providers
{
    public class LocalProvider: IStorageProvider
    {
        private readonly string m_RootPath;

        public LocalProvider(string rootPath)
        {
            m_RootPath = rootPath;
        }

        Task<byte[]> IStorageProvider.Load(string url, float? requestTimeout)
        {
            return Task<byte[]>.Run(
                () =>
                {
                    string fileName = GetLocalPath(url);

                    if (!File.Exists(fileName))
                        return new byte[] { };

                    byte[] dataEncrypted;
                    dataEncrypted = File.ReadAllBytes(fileName);
                    return dataEncrypted;

                    /* Decrypted
                    byte[] dataDecrypted = AES.DecryptBinary(dataEncrypted, m_Key);

                    if (dataDecrypted.Length < 16)
                    {
                        throw new SecurityException();
                    }
                    byte[] hash = new byte[16];
                    Array.Copy(dataDecrypted, 0, hash, 0, hash.Length);
                    byte[] data = new byte[dataDecrypted.Length - hash.Length];
                    Array.Copy(dataDecrypted, hash.Length, data, 0, data.Length);

                    MD5 md5 = MD5.Create();
                    byte[] hashCur = md5.ComputeHash(data);

                    if (!hash.SequenceEqual(hashCur))
                    {
                        throw new SecurityException();
                    }
                    */
                });
        }

        Task IStorageProvider.Save(string url, ref byte[] data, float? requestTimeout)
        {
            var dataEncirypted = data;
            return Task.Run(
                () =>
                {
                    /* Encirypted
                    MD5 md5 = MD5.Create();
                    byte[] hash = md5.ComputeHash(data);
                    byte[] dataEncirypted;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(hash, 0, hash.Length);
                        ms.Write(data, 0, data.Length);
                        dataEncirypted = AES.EncryptBinary(ms.ToArray(), m_Key);
                    }
                    */

                    string fileName = GetLocalPath(url);
                    File.WriteAllBytes(fileName, dataEncirypted);
                });
        }

        private string GetLocalPath (string path)
        {
            return Path.Combine(m_RootPath, path);
        }
    }
}