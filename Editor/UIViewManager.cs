//编写一个UI框架的自定义编辑器窗口，实现该功能:
//获取并显示PanelPath路径下，所有挂载了UIViewController脚本的预制体

//提示:使用ObjectField显示

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIViewManager : EditorWindow
{
    private List<GameObject> _prefabList = new List<GameObject>();
    private Vector2 _scrollPos;

    [MenuItem("Tools/UIViewManager")]
    private static void OpenUIViewManager()
    {
        GetWindow<UIViewManager>("UIViewManager");
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        foreach (var prefab in _prefabList)
        {
            EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnEnable()
    {
        _prefabList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab.GetComponent<UIViewController>())
            {
                _prefabList.Add(prefab);
            }
        }
    }
}
