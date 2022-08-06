using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class GeneralUITextDataMan : EditorWindow
{
    [MenuItem("Tools/通用UI文本数据管理")]
    public static void OnShow()
    {
        EditorWindow.CreateInstance<GeneralUITextDataMan>().Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("保存选中的UI预制体的Label"))
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
                ApplyText(obj as GameObject);
                EditorUtility.DisplayDialog("TIP", "替换完成", "OK");
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("保存所有UI预制体的Label"))
        {
            string[] sDirs = { "Assets/Resources/" };
            string[] sAssets = AssetDatabase.FindAssets("t:Prefab", sDirs);
            for (int i = 0; i < sAssets.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(sAssets[i]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                ApplyAllTexts(prefab);
            }
            EditorUtility.DisplayDialog("TIP", "替换完成", "OK");
        }
    }

    private void ApplyText(GameObject prefab)
    {
        TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Config/static_labels.txt");
        if (asset != null && asset.text != string.Empty)
        {
            Dictionary<string, object> jsonObject = (Dictionary<string, object>)MiniJSON.Json.Deserialize(asset.text);
            var labels = prefab.GetComponentsInChildren<UILabel>(true);
            foreach (var item in labels)
            {
                object content = null;
                string path = GenLabelPath(prefab.name, item.transform);
                if (jsonObject.TryGetValue(path, out content))
                    jsonObject[path] = item.text;
                else
                    jsonObject.Add(path, item.text);
            }

            string jsonStr = UnicodeToString(MiniJSON.Json.Serialize(jsonObject));
            File.WriteAllText(Application.dataPath + "/Resources/Config/static_labels.txt", jsonStr);
        }
    }

    private string EncodingToUTF8(string originString)
    {
        var utf8 = Encoding.UTF8;
        byte[] utfBytes = utf8.GetBytes(originString);
        return utf8.GetString(utfBytes, 0, utfBytes.Length);
    }

#region  unicode 编码/解码
    //字符串unicode 编码
    private string ChineseToUnicode(string originString)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(originString);
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < bytes.Length; i += 2)
        {
            sb.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x2").PadLeft(2, '0'), bytes[i].ToString("x2").PadLeft(2, '0'));
        }

        return sb.ToString();
    }
    
    //字符串unicode 解码
    private string UnicodeToChinese(string originString)
    {
        Regex regex = new Regex(@"\\u([0-9A-F]){4}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        StringBuilder sb = new StringBuilder();

        var results = regex.Matches(originString);
        sb.Remove(0, sb.Length);
        foreach (Match item in results)
        {
            string s = item.Value.Replace("\\u", "");
            sb.Append(Convert.ToChar(Convert.ToInt32(s, 16)).ToString());
        }

        return sb.ToString();
    }
    
    //字符串中unicode字符转为中文
    private string UnicodeToString(string originString)
    {
        return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                originString, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
    }
#endregion

    private void ApplyAllTexts(GameObject prefab)
    {
        Dictionary<string, string> jsonObject = new Dictionary<string, string>();
        var labels = prefab.GetComponentsInChildren<UILabel>(true);
        foreach (var item in labels)
        {
            string path = GenLabelPath(prefab.name, item.transform);
            jsonObject.Add(path, item.text);
        }

        string jsonStr = MiniJSON.Json.Serialize(jsonObject);
        File.WriteAllText(Application.dataPath + "/Resources/Config/static_labels.txt", jsonStr);
    }

    private string GenLabelPath(string targetName, Transform baseObj)
    {
        string TempName = baseObj.name;
        while (true)
        {
            if (targetName == baseObj.name)
            {
                break;
            }

            baseObj = baseObj.parent;
            TempName = baseObj.name + "/" + TempName;
        }

        return TempName;
    }
}
