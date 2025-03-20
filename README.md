# UI-System-for-UGUI

一个基于UGUI的Unity UI框架，由本人在游戏项目开发过程中迭代开发，旨在提高UI开发效率，简化UI管理流程。

[English](README.EN.md)

## 系统特点

- **层级管理**：通过UILayer实现多层级UI管理，包括场景层、背景层、普通层、信息层、顶层和提示层
- **MVC结构**：采用View-ViewController-Model架构设计，职责分明，易于维护
- **生命周期管理**：完整的UI生命周期（OnOpen、OnResume、OnPause、OnClose、OnUpdate）
- **数据驱动**：支持IUIData接口进行数据传递，实现UI与数据的解耦
- **编辑器扩展**：提供可视化UI管理工具，快速创建和编辑UI面板

## 核心功能

### 1. UI控制器的自动化生成
- 一键生成与UI面板对应的控制器代码
- 自动创建标准生命周期方法模板

### 2. UI控件绑定流程自动化
- 可视化控件选择与绑定
- 自动生成控件引用代码
- 支持嵌套UI元素管理

### 3. UI面板的管理可视化
- 支持在编辑器中查看、创建和删除UI面板
- 可视化UI层级管理
- 动态预览UI面板结构

### 4. 支持UI上显示3D模型
- 通过UILayer.SceneLayer实现3D模型与UI的混合展示

## 系统架构

```
UI-System-for-UGUI/
├── Runtime/                   # 运行时核心代码
│   ├── UIView.cs              # UI视图基类
│   ├── UIViewController.cs    # UI控制器组件
│   ├── UISystem.cs            # UI系统管理类
│   ├── UILayer.cs             # UI层级枚举定义
│   ├── UIConfig.cs            # UI系统配置
│   └── IUIData.cs             # UI数据接口
├── Editor/                    # 编辑器扩展代码
│   ├── UIViewEditor.cs        # UI视图编辑器
│   └── UIViewManager.cs       # UI管理器窗口
└── Extension/                 # 扩展功能代码
    └── UIViewExtension.cs     # UI视图扩展方法
```

## 使用方法

### 1. 创建UI面板

1. 打开UI管理器窗口：`菜单 -> Framework -> UI -> UIViewManager`
2. 点击"创建新的UI面板"按钮
3. 输入UI面板名称（如`LoginPanel`）并确认
4. 系统将自动创建预制体和对应脚本

### 2. 编辑UI面板

1. 将创建的UI预制体拖入场景
2. 添加所需UI元素（按钮、文本、图像等）
3. 使用UI管理器的控件列表面板进行控件绑定
4. 点击"生成UI绑定代码"生成绑定代码

### 3. 打开UI面板

```csharp
// 打开普通UI面板
UISystem.Instance.OpenPanel<LoginPanel, NoneUIData>(NoneUIData.noneUIData, UILayer.NormalLayer);

// 打开需要数据的UI面板
public class PlayerInfoData : IUIData
{
    public string playerName;
    public int level;
    public float hp;
}

var playerData = new PlayerInfoData { 
    playerName = "Player1", 
    level = 10, 
    hp = 100 
};
UISystem.Instance.OpenPanel<PlayerInfoPanel, PlayerInfoData>(playerData, UILayer.InfoLayer);

// 打开场景UI（如3D模型展示）
UISystem.Instance.OpenViewInScene<CharacterPreviewPanel, CharacterData>(characterData);
```

### 4. 编写UI面板逻辑

```csharp
public partial class LoginPanel : UIView
{
    // 自动生成的绑定字段
    private Button loginButton;
    private InputField usernameInput;
    private InputField passwordInput;
    
    public override void OnOpen()
    {
        // 控件绑定
        BindUI();
        
        // 添加事件监听
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }
    
    public override void OnResume()
    {
        // 当面板从暂停状态恢复时调用
    }
    
    public override void OnPause()
    {
        // 当面板被其他面板覆盖时调用
    }
    
    public override void OnClose()
    {
        // 移除事件监听
        loginButton.onClick.RemoveListener(OnLoginButtonClick);
    }
    
    public override void OnUpdate()
    {
        // 每帧更新逻辑
    }
    
    private void OnLoginButtonClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        
        // 登录逻辑...
    }
}
```

### 5. 关闭UI面板

```csharp
// 关闭顶层面板
UISystem.Instance.CloseTopPanel(UILayer.NormalLayer);

// 关闭场景UI
sceneUIView.CloseInSceneLayer();

// 关闭所有面板
UISystem.Instance.CloseAll();
```

## 最佳实践

1. **UI层级管理**
   - 将不同功能的UI分配到合适的层级
   - 场景层(SceneLayer)：3D模型展示、场景内UI
   - 背景层(BackgroundLayer)：全屏背景、主菜单背景
   - 普通层(NormalLayer)：主要功能面板
   - 信息层(InfoLayer)：信息展示、状态面板
   - 顶层(TopLayer)：弹窗、对话框
   - 提示层(TipLayer)：提示、通知、工具提示

2. **数据驱动**
   - 为每个复杂UI面板创建对应的IUIData实现类
   - 使用NoneUIData实例作为无数据UI面板的参数
   - 数据更新时刷新UI，而不是直接修改UI

3. **UI生命周期**
   - OnOpen：初始化UI、绑定事件
   - OnResume：恢复UI状态、重新订阅事件
   - OnPause：暂停UI状态、取消部分事件订阅
   - OnClose：完全清理资源、取消所有事件订阅
   - OnUpdate：处理需要每帧更新的UI逻辑

## 注意事项

1. UI预制体需要存放在Resources/UIPrefabs目录下
2. UI脚本默认生成在Assets/UI/UIScripts目录下
3. UI绑定代码默认生成在Assets/UI/UIBind目录下
4. 确保场景中存在Canvas或允许系统自动创建
