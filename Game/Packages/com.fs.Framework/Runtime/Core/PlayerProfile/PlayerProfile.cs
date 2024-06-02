using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Core.Profiles
{
    using Storages;

    public interface IPlayerModel : IDisposable
    {
        void Init(bool isNewProfile);
    }
    
    [Serializable]
    public partial class PlayerProfile: IStorageSerializable<PlayerProfile>
    {
        #region internal
        [SerializeField]
        int m_SaveVersion;

        private List<IPlayerModel> m_Models = new List<IPlayerModel>();

        public static PlayerProfile Instance { get; private set; }

        public int SaveVersion { get => m_SaveVersion; set => m_SaveVersion = value; }

        public bool IsConfigured { get; private set; }
        #endregion


        private void StartConfigure()
        {
            IsConfigured = false;

        }

        partial void Configure();

        private void FinishConfigure()
        {
            IsConfigured = true;
        }

        #region internal
        private void ReleaseInstance()
        {
            foreach (var playerModel in m_Models)
            {
                playerModel.Dispose();
            }
        }

        private void InitModel<T>(ref T model) 
            where T : IPlayerModel, new()
        {
            var isNewProfile = model == null;
            if (isNewProfile)
            {
                model = new T();
            }
            model.Init(isNewProfile);
            m_Models.Add(model);
        }

        public static void Initialize(PlayerProfile savedProfile)
        {
            if (Instance != null)
            {
                Instance.ReleaseInstance();
            }

            Instance = savedProfile ?? new PlayerProfile();

            Instance.StartConfigure();
            try
            {
                Instance.Configure();
            }
            finally
            {
                Instance.FinishConfigure();
            }
        }
        #endregion
        #region IStorageSerializable
        byte[] IStorageSerializable<PlayerProfile>.Serialize()
        {
            var str = JsonUtility.ToJson(this);
            var data = Encoding.UTF8.GetBytes(str);
            return data;
        }

        PlayerProfile IStorageSerializable<PlayerProfile>.Deserialize(ref byte[] value)
        {
            var str = Encoding.UTF8.GetString(value);
            return JsonUtility.FromJson<PlayerProfile>(str);
        }
        #endregion
    }
}