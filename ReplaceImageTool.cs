public class ReplaceImageTool : EditorWindow
{
    [MenuItem("Tools/替换预制体中的Image为RawImage[可扩展为任意组件替换]")]
    public static void OnShow()
    {
        EditorWindow.CreateInstance<ReplaceImageTool>().Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("替换选中的GUI预制体"))
        {
            var obj = Selection.activeGameObject;
            if (obj == null)
            {
                EditorUtility.DisplayDialog("WARNING", "没有选择任何东西", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(obj);
            if (path.EndsWith(".prefab"))
            {
                Replace(obj as GameObject);
                EditorUtility.DisplayDialog("TIP", "替换完成", "OK");
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("替换所有GUI预制体"))
        {
            string[] sDirs = { "Assets/Resources/UIPrefabs" };
            string[] sAssets = AssetDatabase.FindAssets("t:Prefab", sDirs);
            for (int i = 0; i < sAssets.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(sAssets[i]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Replace(prefab);
            }
            EditorUtility.DisplayDialog("TIP", "替换完成", "OK");
        }
    }

    private void Replace(GameObject prefab)
    {
        var images = prefab.GetComponentsInChildren<Image>(true);
        foreach (var item in images)
        {
            if (item.GetComponent<Mask>() != null)
                continue;
                
            Color col = item.color;
            GameObject obj = item.gameObject;
            GameObject.DestroyImmediate(item, true);
            RawImage rawImg = obj.AddComponent<RawImage>();
            if (img != null)
            {
                if (sp != null)
                    rawImg.texture = null;
                if (col != null)
                    rawImg.color = col;
            }
        }
        
        PrefabUtility.SavePrefabAsset(prefab);
    }
