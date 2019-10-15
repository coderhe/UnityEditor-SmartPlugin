using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class CleanMissingScript : EditorWindow
{
    [MenuItem("Tools/CleanMissingScript")]
    public static void Clean()
    {
        var obj = Selection.activeGameObject;
        if (obj == null)
        {
            Debug.LogError("没有选中父组件！！！");
            return;
        }
        
        int sum = 0;
        if(obj.transform.childCount <= 0)
        {
            CleanObject(obj.transform, ref sum);
            EditorUtility.DisplayDialog("提示", "清除完成,清理个数：" + sum, "确定");
            return;
        }

        for (int i = 0; i < obj.transform.childCount; ++i)
        {
            Transform transChild = obj.transform.GetChild(i);
            CleanObject(transChild, ref sum);
        }
        EditorUtility.DisplayDialog("提示", "清除完成,清理个数：" + sum, "确定");
    }

    public static void CleanObject(Transform child, ref int sum)
    {
        //判断是否存在于Hierarchy面板上
        if (child.gameObject.hideFlags == HideFlags.None)
        {
            var components = child.gameObject.GetComponents<Component>();
            SerializedObject so = new SerializedObject(child.gameObject);
            var soProperties = so.FindProperty("m_Component");
            int r = 0;
            for (int j = 0; j < components.Length; j++)
            {
                if (components[j] == null)
                {
                    soProperties.DeleteArrayElementAtIndex(j - r);
                    Debug.LogError("清除了物体：" + child.gameObject.name + " 的一个missing脚本");
                    r++;
                }
            }
            if (r > 0)
            {
                so.ApplyModifiedProperties();
                AssetDatabase.Refresh();
            }
            sum += r;
        }
    }
}
