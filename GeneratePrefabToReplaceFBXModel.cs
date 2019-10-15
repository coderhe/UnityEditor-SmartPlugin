using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GeneratePrefabToReplaceFBXModel
{
    private static string prefabDirectory = "/Resources/Scenes";
    private static string prefabExtension = ".prefab";

    [MenuItem("Tools/模型文件生成并替换为prefab")]
    public static void Generate()
    {
        var obj = Selection.activeGameObject;
        if (obj == null)
        {
            EditorUtility.DisplayDialog("WARNING", "没有选择任何东西", "OK");
            return;
        }

        string modelAssetPath = string.Concat(Application.dataPath, prefabDirectory);
        string modelFullPath = string.Concat("Assets", prefabDirectory);
        if (!Directory.Exists(modelFullPath + "/prefab"))
        {
            AssetDatabase.CreateFolder(modelFullPath, "prefab");
        }

        string genPrefabFullName = string.Empty;
        GameObject _assetGameObject = null;
        GameObject _gameObject = null;
        GameObject cloneObj = null;
        Transform transChildsChild;
        for (int i = 0; i < obj.transform.childCount;)
        {
            Transform transChild = obj.transform.GetChild(i);
            if (PrefabUtility.GetPrefabAssetType(transChild.gameObject) == PrefabAssetType.Model)
            {
                _assetGameObject = transChild.gameObject;
                cloneObj = GameObject.Instantiate<GameObject>(_assetGameObject);
                genPrefabFullName = string.Concat(modelAssetPath, "/prefab/", _assetGameObject.name, prefabExtension);
                Object prefabObj = PrefabUtility.CreateEmptyPrefab(genPrefabFullName);
                _gameObject = PrefabUtility.ReplacePrefab(cloneObj, prefabObj);
                GameObject obj1 = GameObject.Instantiate<GameObject>(_gameObject);
                obj1.transform.parent = _assetGameObject.transform.parent;
                obj1.name = _assetGameObject.name;
                GameObject.DestroyImmediate(_assetGameObject);
            }            
            else
                ++i;
        }

        EditorUtility.DisplayDialog("WARNING", "替换完成", "OK");
        AssetDatabase.Refresh();
    }
}
