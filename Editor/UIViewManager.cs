using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UIViewManager : EditorWindow
{
    private List<GameObject> _prefabList = new List<GameObject>();
    private Vector2 _scrollPos;
    private Dictionary<Transform, bool> _foldouts = new Dictionary<Transform, bool>();
    private GameObject _selectedObject;
    [MenuItem("Framework/UI/UIViewManager")]
    private static void OpenUIViewManager()
    {
        GetWindow<UIViewManager>("UIViewManager");
    }

    private void OnGUI()
    {
      
        List<GameObject> toRemove = new List<GameObject>();

        GUILayout.BeginHorizontal();
        
        GUILayout.BeginVertical("UI面板列表", "window",GUILayout.Width(500));
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        if (GUILayout.Button("创建新的UI面板"))
        {
            UIViewCreateWindow.OpenUIViewCreateWindow();
        }
        
        foreach (var prefab in _prefabList)
        {
            if(_selectedObject == prefab)
            {
                GUI.color = Color.cyan;
            }
            else
            {
                GUI.color = Color.white;
            }
            GUILayout.BeginHorizontal("frameBox");
            //当前行被点击选中时 该行的样式变为蓝色
            if (GUILayout.Button(prefab.name, "Label"))
            {
                _selectedObject = prefab;
            }
            //让下面这一个ObjectField无法编辑
            GUI.enabled = false;
            EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
            GUI.enabled = true;
            
            if (!AssetDatabase.LoadAssetAtPath(UIConfig.UIScriptPath + prefab.name + ".cs", typeof(Object)))
            {
                GUI.color = Color.white;
                EditorGUILayout.HelpBox("没有对应的UIView脚本", MessageType.Error);
            }
            else
            {
                //设置按钮颜色
                GUI.color = Color.green;
                if (GUILayout.Button("编辑UI脚本"))
                {
                    string path = UIConfig.UIScriptPath + prefab.name + ".cs";
                    Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                    if (obj != null)
                    {
                        AssetDatabase.OpenAsset(obj);
                    }
                    else
                    {
                        Debug.LogError("找不到对应的代码文件");
                    }
                }
            }

            GUI.color = Color.red;
            if (GUILayout.Button("删除"))
            {
                //点击按钮先弹出警告弹窗 下方有确认按钮和取消按钮 点击确认进行删除操作
                if (EditorUtility.DisplayDialog("警告", "确定要删除UI面板预制体以及对应脚本吗,这将是一个无法撤销的操作？", "确定", "取消"))
                {
                    //判断对应的脚本文件是否存在，并删除
                    string scriptPath = UIConfig.UIScriptPath + prefab.name + ".cs";
                    if (AssetDatabase.LoadAssetAtPath(scriptPath, typeof(Object)))
                    {
                        AssetDatabase.DeleteAsset(scriptPath);
                    }

                    string path = AssetDatabase.GetAssetPath(prefab);
                    AssetDatabase.DeleteAsset(path);
                    toRemove.Add(prefab);
                }
            }

            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            
           
            
            
            
            
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginVertical("控件列表", "window");
        //使用Foldout显示一个列表(类似Hierarchy窗口)，将子物体按照树形结构排列出来，如果子物体有子物体，也会以树形结构排列出来
        if(_selectedObject!=null){
            DrawChild(_selectedObject.transform);
        }
        GUILayout.EndVertical();
        foreach (var prefab in toRemove)
        {
            _prefabList.Remove(prefab);
        }

        
        //GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
private void DrawChild(Transform child)
{
    if (child.childCount > 0)
    {
        if (!_foldouts.ContainsKey(child))
        {
            _foldouts[child] = false;
        }

        _foldouts[child] = EditorGUILayout.Foldout(_foldouts[child], child.name);

        if (_foldouts[child])
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < child.childCount; i++)
            {
                DrawChild(child.GetChild(i));
            }
            EditorGUI.indentLevel--;
        }
    }
    else
    {
        EditorGUILayout.LabelField(child.name);
    }
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

public class UIViewCreateWindow : EditorWindow
{
    private string _panelName;


    public static void OpenUIViewCreateWindow()
    {
        GetWindowWithRect<UIViewCreateWindow>(new Rect(0, 0, 350, 50), true, "创建新的UI面板");
    }

    private void OnGUI()
    {
        _panelName = EditorGUILayout.TextField("UI面板名字", _panelName);
        if (GUILayout.Button("确认"))
        {
            if (string.IsNullOrEmpty(_panelName))
            {
                EditorUtility.DisplayDialog("警告", "UI面板名字不能为空", "确认");
            }
            if (AssetDatabase.LoadAssetAtPath(UIConfig.PanelPath + "/" + _panelName + ".prefab", typeof(Object)))
            {
                EditorUtility.DisplayDialog("警告", "已存在同名UI面板", "确认");
            }
            else
            {
                string prefabPath = UIConfig.PanelPath + "/" + _panelName + ".prefab";
                string scriptPath = UIConfig.UIScriptPath + _panelName + ".cs";
                if (AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object)))
                {
                    EditorUtility.DisplayDialog("警告", "已存在同名UI面板", "确认");
                }
                else
                {
                    GameObject panel = new GameObject(_panelName);
                    panel.AddComponent<RectTransform>();
                    panel.AddComponent<UIViewController>();
                    PrefabUtility.SaveAsPrefabAsset(panel, prefabPath);
                    string codeContent = $@"public class {_panelName}:UIView
{{
    public override void OnOpen()
    {{

    }}

    public override void OnResume()
    {{

    }}

    public override void OnPause()
    {{

    }}

    public override void OnClose()
    {{

    }}

    public override void OnUpdate()
    {{

    }}
}}";
                    File.WriteAllText(scriptPath, codeContent);
                    AssetDatabase.Refresh();
                    Close();
                }
            }
        }
    }
}