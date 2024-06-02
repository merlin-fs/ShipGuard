using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public interface IConfigManager
    {
        Task<bool> TryLoadConfigs();
        Task<bool> TryUpdateConfigs(IConfigFilesLoader configFilesLoader);
    }


    public interface IConfigFilesLoader
    {
        IEnumerable<IConfigFile> ConfigFiles { get; }

        void AddConfigFile(string id, IConfigFile configFile);

        IConfigFile GetConfigFile(string id);

        IEnumerable<Task> LoadConfig();

        void Clear();
    }

    public interface IConfigFileLoader
    {
        string PathRoot { get; }

        Task<string> LoadFile(string file);

        void RemoveFile(string fileName);
    }

    public interface IConfigFile
    {
        string OverrideParam { get; }

        string OriginFileName { get; }

        string OverrideFileName { get; set; }

        string CurrentFileName { get; }

        public IConfigFileLoader CurLoader { get; set; }

        Task LoadConfig();

        T[] GetParsedData<T>();
    }
}