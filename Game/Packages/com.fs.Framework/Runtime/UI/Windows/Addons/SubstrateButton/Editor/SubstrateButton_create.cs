using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace Common.UI.Windows
{
	public class SubstrateButton_create
	{
		[MenuItem("GameObject/UI/SubstrateButton")]
		public static void CreateButton()
		{
			GameObject obj = new GameObject("SubstrateButton");
			Text t = obj.AddComponent<Text>();
			t.font = null;
			t.text = string.Empty;

			obj.AddComponent<SubstrateButton>();

			RectTransform rectTransform = obj.GetComponent<RectTransform>();
			rectTransform.sizeDelta = new Vector2(5000.0f, 5000.0f);

			GameObject[] objects = Selection.gameObjects;
			if (objects.Length == 1)
			{
				obj.transform.SetParent(objects[0].transform);
				obj.layer = objects[0].layer;
			}
		}
	}
}
#endif
