#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor
{
    public class CheckSimilarTextEditor : EditorWindow
    {
        public static CheckSimilarTextEditor Window;
        private Dictionary<string, List<string>> versionContens = new Dictionary<string, List<string>>();
        
        [MenuItem("Tools/CheckSimilarText")]
        public static void ShowWindow()
        {
            if (Window == null)
            {
                Window = CreateInstance<CheckSimilarTextEditor>();
            }
            Window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            if (GUILayout.Button("Compare"))
            {
                ToCheck();
            }
            
            EditorGUILayout.BeginVertical();
        }

        private void ToCheck()
        {
            string[] destnationText = File.ReadAllLines("D:\\xxx\\destnation.txt");
            string[] originalText = File.ReadAllLines("D:\\xxx\\original.txt");
            List<string> contents = null;

            for (int i = 4; i < originalText.Length; ++i)
            {
                if(originalText[i].Contains(":"))
                {
                    contents = new List<string>();
                    String[] content = originalText[i].Split(':');
                    for(int x = 0; x < content.Length; ++x)
                    {
                        contents.Add(content[x]);
                    }
                    versionContens.Add(content[0], contents);
                }
            }

            int nHeavy = 0;
            List<string> lstTemps = new List<string>();
            for (int j = 4; j < destnationText.Length; ++j)
            {
                if (destnationText[j].Contains(":"))
                {
                    lstTemps.Clear();                    
                    String[] sContents = destnationText[j].Split(':');
                    string fixedTxt = sContents[0];
                    if (versionContens.TryGetValue(sContents[0], out lstTemps))
                    {
                        List<string> details = versionContens[sContents[0]];
                        nHeavy += int.Parse(lstTemps[1]);
                        for (int y = 1; y < sContents.Length; ++y)
                        {
                            if (y < sContents.Length)
                                fixedTxt += ":";

                            fixedTxt += details[y];
                        }
                        destnationText[j] = fixedTxt;
                    }
                }
            }
            destnationText[2] = nHeavy.ToString();
            File.WriteAllLines("D:\\xxx\\destnation.txt", destnationText);

            EditorUtility.DisplayDialog("提示", "检查完成, 总大小: " + nHeavy.ToString() + "KB", "确定");
        }
    }   
}
#endif
