using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR
namespace UnityEditor.Inspector
{
    [ExecuteInEditMode]
    public class PlayFromScene : EditorWindow
    {
        private static string[] m_SceneNames;
        private static EditorBuildSettingsScene[] m_Scenes;

        [SerializeField]
        private string m_WorkScene = "";
        [SerializeField]
        private int m_WorkSceneIndex = 0;
        [SerializeField]
        private string m_LaunchScene = "";
        [SerializeField]
        private int m_LaunchSceneIndex = 0;
        private bool m_WasLaunched = false;
        private bool m_IsLaunchFromThisScript = false;

        [MenuItem("Tools/Play From Scene %F5")]
        public static void Run()
        { 
            GetWindow<PlayFromScene>(); 
        }

        void OnEnable()
        {
            m_Scenes = EditorBuildSettings.scenes;

            List<string> sceneNames = new List<string>();
            foreach (string scene in m_Scenes.Select(x => AsSpacedCamelCase(Path.GetFileNameWithoutExtension(x.path))))
                sceneNames.Add(scene);

            m_SceneNames = sceneNames.ToArray();
        }

        void Update()
        {
            if (EditorApplication.isPlaying)
                m_WasLaunched = true;

            if (!EditorApplication.isPlaying && m_IsLaunchFromThisScript && m_WasLaunched)
            {
                m_IsLaunchFromThisScript = false;
                m_WasLaunched = false;

                var scene = EditorSceneManager.GetActiveScene();

                if (scene.IsValid() && scene.name != m_WorkScene)
                    EditorSceneManager.OpenScene(m_WorkScene);
            }
            Repaint();
        }

        void OnGUI()
        {
            if (EditorApplication.isPlaying)
                return;

            if (m_SceneNames == null)
                return;

            m_WorkSceneIndex = EditorGUILayout.Popup("Work scene", m_WorkSceneIndex, m_SceneNames);
            if (m_WorkSceneIndex >= 0 && m_WorkSceneIndex < m_Scenes.Length)
                m_WorkScene = m_Scenes[m_WorkSceneIndex].path;

            if (GUILayout.Button("Go to scene"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(m_WorkScene);
                }
            }

            GUILayout.Space(20.0f);

            m_LaunchSceneIndex = EditorGUILayout.Popup("Launch scene", m_LaunchSceneIndex, m_SceneNames);
            if (m_LaunchSceneIndex >= 0 && m_LaunchSceneIndex < m_Scenes.Length)
                m_LaunchScene = m_Scenes[m_LaunchSceneIndex].path;

            if (GUILayout.Button("Play"))
            {
                m_IsLaunchFromThisScript = true;
                m_WasLaunched = false;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(m_LaunchScene);
                    EditorApplication.isPlaying = true;
                }
            }
        }

        public static string AsSpacedCamelCase(string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length * 2);

            sb.Append(char.ToUpper(text[0]));

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    sb.Append(' ');

                sb.Append(text[i]);
            }

            return sb.ToString();
        }
    }
}
#endif