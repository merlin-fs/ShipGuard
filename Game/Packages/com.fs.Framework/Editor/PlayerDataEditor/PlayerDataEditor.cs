using System;
using UnityEditor;
using UnityEngine;

namespace Common.Core.Profiles
{
    using Storages;

    public class PlayerDataEditor : EditorWindow
    {
        private SerializedProperty _serializedProperty;
        private SerializedObject _serializedObject;
        private PlayerDataContainer _dataContainer;

        [MenuItem("Tools/Game/PlayerProfile")]
        private static void ShowWindow()
        {
            var window = GetWindow<PlayerDataEditor>();
            window.titleContent = new GUIContent("PlayerProfile");
            window.Show();
        }
        
        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying || PlayerProfile.Instance == null)
            {
                _serializedProperty = null;
                _serializedObject = null;
                return;
            }

            if (GUILayout.Button("Save to local"))
            {
                SaveToLocal();
            }

            // var processes = DragAndDropProcess.DragAndDropBuffer.DropProcesses;
            //
            // foreach (var process in processes)
            // {
            //     EditorGUILayout.LabelField($"process {process}");
            // }
            //
            // return;

            if (_dataContainer == null)
            {
                if (PlayerProfile.Instance != null)
                {
                    UpdatePlayerProfile();
                    //return;
                }
            }
            else
            {
                if (_dataContainer.PlayerProfile != PlayerProfile.Instance)
                {
                    UpdatePlayerProfile();
                    //return;
                }
            }
            
            if (_serializedObject == null) return;

            if (!_serializedObject.targetObject)
            {
                _serializedProperty = null;
                _serializedObject = null;
                return;
            }
            
            _serializedObject.Update();

            EditorGUILayout.PropertyField(_serializedProperty);
            
            _serializedObject.ApplyModifiedProperties();
        }

        void UpdatePlayerProfile()
        {
            _dataContainer = CreateInstance<PlayerDataContainer>();
            _dataContainer.PlayerProfile = PlayerProfile.Instance;
            _serializedObject = new SerializedObject(_dataContainer);
            _serializedProperty = _serializedObject.FindProperty("PlayerProfile");
        }

        void SaveToLocal()
        {
            /*!!!
            var storageObj = DIContext.Root.Get<object>("PlayerProfile");
            var storage = storageObj as IStorageManager<PlayerProfile>;
            storage.SaveData(_dataContainer.PlayerProfile);
            */
        }
    }
}