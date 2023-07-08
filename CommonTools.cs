using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class CommonTools : MonoBehaviour
{
    [MenuItem("CommonTools/生成资源全路径到剪切板(&&)", false, 1)]
    public static void GenerateAssetPathToClipboard()
    {
        if (Selection.objects.Length != 1)
        {
            EditorUtility.DisplayDialog("提示", "请选择一个资源", "好的");
            return;
        }

        UnityEngine.Object curObj = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(curObj);
        path = path.Replace('/', '&');
        NGUITools.clipboard = path;
        EditorUtility.DisplayDialog("提示", "已经生成了资源全路径到剪贴板，路径是: " + path, "好的");
    }

    [MenuItem("CommonTools/文本转UTF-8编码", false, 2)]
    public static void ChangeToUTF8()
    {
        TextAsset[] allAssets = Selection.GetFiltered<TextAsset>(SelectionMode.DeepAssets);
        if (allAssets.Length <= 0)
        {
            EditorUtility.DisplayDialog("提示", "选中文件夹没有内容", "好的");
            return;
        }
        
        int index = 0;
        for (int i = 0; i < allAssets.Length; i++)
        {
            TextAsset textAsset = allAssets[i];
            string path = AssetDatabase.GetAssetPath(textAsset);
            string filePath = Application.dataPath + "/" + path.Substring("Assets/".Length);

            Encoding code = GetTextFileEncodingType(filePath);
            if (code.Equals(Encoding.UTF8))
            {
                Debug.LogFormat("{0} 已经是UTF8编码，不转换", textAsset.name);
                continue;
            }

            index++;
            string content = File.ReadAllText(filePath, code);
            File.WriteAllText(filePath, content, Encoding.UTF8);
            Debug.LogFormat("{0} 转换完毕", textAsset.name);
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("提示", string.Format("你已经处理了{0}个文件了", index), "好的");
    }

    /// <summary>
    /// 获取文本文件的字符编码类型
    /// </summary>
    private static Encoding GetTextFileEncodingType(string fileName)
    {
        Encoding encoding = Encoding.Default;
        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream, encoding);
        byte[] buffer = binaryReader.ReadBytes((int)fileStream.Length);
        binaryReader.Close();
        fileStream.Close();
        if (buffer.Length >= 3 && buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
        {
            encoding = Encoding.UTF8;
        }
        else if (buffer.Length >= 3 && buffer[0] == 254 && buffer[1] == 255 && buffer[2] == 0)
        {
            encoding = Encoding.BigEndianUnicode;
        }
        else if (buffer.Length >= 3 && buffer[0] == 255 && buffer[1] == 254 && buffer[2] == 65)
        {
            encoding = Encoding.Unicode;
        }
        
        return encoding;
    }

    [MenuItem("CommonTools/删除游戏更新目录资源", false, 3)]
    public static void CleanGameUpdateFiles()
    {
        System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(Application.persistentDataPath + "/Update");
        if (info.Exists)
        {
            info.Attributes = System.IO.FileAttributes.Normal;
            System.IO.FileInfo[] files = info.GetFiles();
            int index = 0;
            foreach (System.IO.FileInfo f in files)
            {
                f.Attributes = System.IO.FileAttributes.Normal;
                f.Delete();
                UEShowProgress.UpdateProgress("clear", index);
                index++;
            }
            
            EditorUtility.DisplayDialog("删除成功", "删除更新目录成功", "ok");
        }
        else
        {
            EditorUtility.DisplayDialog("删除失败", "删除更新目录失败，没有这个目录" + info.Name, "ok");
        }
    }

    [MenuItem("CommonTools/清除本地数据存档", false, 4)]
    public static void RemovePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        EditorUtility.DisplayDialog("清除成功", "已经清除本地数据存档", "ok");
    }
    
    [MenuItem("CommonTools/去掉svn只读属性", false, 6)]
    public static void KillSvnNeedLock()
    {
        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        Microsoft.Win32.RegistryKey miscellanyKey = key.CreateSubKey("Software\\Tigris.org\\Subversion\\Config\\miscellany");
        Microsoft.Win32.RegistryKey autopropKey = key.CreateSubKey("Software\\Tigris.org\\Subversion\\Config\\auto-props");
        if (miscellanyKey != null)
        {
            if (miscellanyKey.GetValue("*.*") != null)
                miscellanyKey.DeleteValue("*.*");
                
            if (autopropKey.GetValue("*.*") != null)
                autopropKey.DeleteValue("*.*");            
        }
    }

    [MenuItem("CommonTools/添加svn只读属性", false, 7)]
    public static void AddSvnNeedLock()
    {
        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
        Microsoft.Win32.RegistryKey autopropKey = key.CreateSubKey("Software\\Tigris.org\\Subversion\\Config\\auto-props");
        if (autopropKey != null)
            autopropKey.SetValue("*.*", "svn:needs-lock = *");        
    }
}
