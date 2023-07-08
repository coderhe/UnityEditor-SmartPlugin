using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class OpenFileInBrowser : EditorWindow
{
    private string mFilePath;
    [MenuItem("Tools/OpenFileInBrowser")]
    public static void OpenFile()
    {
        //创建窗口
        Rect window = new Rect(300, 300, 300, 300);
        OpenFileInBrowser setting = (AtlasMaxSizeSettingWindow)EditorWindow.GetWindowWithRect(typeof(OpenFileInBrowser), window, false, "");
        setting.Show();
    }

    private void OnGUI()
    {
        mFilePath = EditorGUILayout.TextField("File Path", mFilePath);

        if (IsInWinOS)
        {
            OpenInWin(path);
        }
        else if (IsInMacOS)
        {
            OpenInMac(path);
        }
        else
        {
            OpenInWin(path);
            OpenInMac(path);
        }
    }

    public static bool IsInMacOS
    {
        get
        {
            return UnityEngine.SystemInfo.operatingSystem.IndexOf("Mac OS") != -1;
        }
    }

    public static bool IsInWinOS
    {
        get
        {
            return UnityEngine.SystemInfo.operatingSystem.IndexOf("Windows") != -1;
        }
    }

    public static void OpenInMac(string path)
    {
        bool openInsidesOfFolder = false;

        // try mac
        string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

        if (System.IO.Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        if (!macPath.StartsWith("\""))
        {
            macPath = "\"" + macPath;
        }

        if (!macPath.EndsWith("\""))
        {
            macPath = macPath + "\"";
        }

        string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

        try
        {
            System.Diagnostics.Process.Start("open", arguments);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
    }

    public static void OpenInWin(string path)
    {
        path = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
    }
}
