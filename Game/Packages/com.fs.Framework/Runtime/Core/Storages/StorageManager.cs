using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.Storages
{
	using Providers;

	public interface IStorageSerializable<T>
    {
		byte[] Serialize();
		T Deserialize(ref byte[] value);
	}

	public interface IStorageManager<T>
	{
		Task Init(IStorageProvider remove, IStorageProvider local);

		Task<T> LoadData();

		Task SaveData(T obj);
	}

	public partial class StorageManager<T> : IStorageManager<T>
		where T: IStorageSerializable<T>, new()
	{
		private const string FILE_USER_DATA = "data.save";
		private IStorageProvider m_Rempve;
		private IStorageProvider m_Local;
		private string m_Key => "78532DdSYVjdh74lfu‡„V2m6ry_wCIsIIYkøvM1222326zVc";
		private IStorageSerializable<T> m_Serializable;

		Task IStorageManager<T>.Init(IStorageProvider remove, IStorageProvider local)
		{
			m_Rempve = remove;
			m_Local = local;
			m_Serializable = Activator.CreateInstance<T>();
			return Task.Run(
				() => 
				{
				});
		}

		Task<T> IStorageManager<T>.LoadData()
		{
			return Task<object>.Run(
                async () =>
				{
					var data = await m_Local.Load(FILE_USER_DATA);
					return m_Serializable.Deserialize(ref data);
				});
		}

		Task IStorageManager<T>.SaveData(T obj)
        {
			return Task.Run(
                () =>
                {
					byte[] data = obj.Serialize();
                    return m_Local.Save(FILE_USER_DATA, ref data);
                });
		}
	}
}