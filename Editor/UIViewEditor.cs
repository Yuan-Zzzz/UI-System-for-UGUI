using System.IO;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UIViewController))]
public class UIViewEditor : UnityEditor.Editor
{
    private bool _isGenerator = false;

    public override void OnInspectorGUI()
    {
        UIViewController generator = (UIViewController)target;

        // 判断文件是否存在
        _isGenerator = File.Exists(UIConfig.UIScriptPath + generator.gameObject.name + ".cs");

        //TODO:生成代码文件
        if (!_isGenerator && GUILayout.Button("生成UIView代码"))
        {
            // 生成代码内容
            string codeContent =
@"public class " + generator.gameObject.name + @":UIView
{
    public override void OnOpen()
    {
           
    }

    public override void OnResume()
    {
            
    }

    public override void OnPause()
    {
           
    }

    public override void OnClose()
    {
            
    }

    public override void OnUpdate()
    {
            
    }
}";
            // 写入代码文件
            File.WriteAllText(UIConfig.UIScriptPath + generator.gameObject.name + ".cs", codeContent);

            // 刷新项目资源，使生成的代码被Unity自动检测并编译
            UnityEditor.AssetDatabase.Refresh();
        }

        if (_isGenerator)
        {
            GUILayout.Label($"已生成与面板名同名的代码文件:{generator.gameObject.name}.cs");
            if (GUILayout.Button("打开"))
            {
                EditorUtility.OpenWithDefaultApp(UIConfig.UIScriptPath + generator.gameObject.name + ".cs");
            }
            //TODO UI绑定逻辑
        }

        DrawDefaultInspector();
    }
}