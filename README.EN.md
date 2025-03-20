# UI-System-for-UGUI

A UGUI-based Unity UI framework, developed and refined through game projects, designed to improve UI development efficiency and simplify UI management processes.

[中文](README.md)

## System Features

- **Layer Management**: Multi-level UI management through UILayer, including Scene Layer, Background Layer, Normal Layer, Info Layer, Top Layer, and Tip Layer
- **MVC Architecture**: Designed with View-ViewController-Model architecture, clear responsibilities, easy to maintain
- **Lifecycle Management**: Complete UI lifecycle (OnOpen, OnResume, OnPause, OnClose, OnUpdate)
- **Data-Driven**: Supports IUIData interface for data transmission, decoupling UI and data
- **Editor Extensions**: Provides visual UI management tools for quick creation and editing of UI panels

## Core Functions

### 1. Automated UI Controller Generation
- One-click generation of controller code corresponding to UI panels
- Automatic creation of standard lifecycle method templates

### 2. Automated UI Control Binding Process
- Visual control selection and binding
- Automatic generation of control reference code
- Support for nested UI element management

### 3. Visual UI Panel Management
- Support for viewing, creating, and deleting UI panels in the editor
- Visual UI hierarchy management
- Dynamic preview of UI panel structure

### 4. Support for 3D Models on UI
- Achieve mixed display of 3D models and UI through UILayer.SceneLayer

## System Architecture

```
UI-System-for-UGUI/
├── Runtime/                   # Runtime core code
│   ├── UIView.cs              # UI view base class
│   ├── UIViewController.cs    # UI controller component
│   ├── UISystem.cs            # UI system management class
│   ├── UILayer.cs             # UI layer enum definition
│   ├── UIConfig.cs            # UI system configuration
│   └── IUIData.cs             # UI data interface
├── Editor/                    # Editor extension code
│   ├── UIViewEditor.cs        # UI view editor
│   └── UIViewManager.cs       # UI manager window
└── Extension/                 # Extension code
    └── UIViewExtension.cs     # UI view extension methods
```

## Usage

### 1. Create UI Panel

1. Open the UI manager window: `Menu -> Framework -> UI -> UIViewManager`
2. Click the "Create New UI Panel" button
3. Enter the UI panel name (e.g., `LoginPanel`) and confirm
4. The system will automatically create a prefab and corresponding script

### 2. Edit UI Panel

1. Drag the created UI prefab into the scene
2. Add required UI elements (buttons, text, images, etc.)
3. Use the UI manager's control list panel for control binding
4. Click "Generate UI Binding Code" to generate binding code

### 3. Open UI Panel

```csharp
// Open a normal UI panel
UISystem.Instance.OpenPanel<LoginPanel, NoneUIData>(NoneUIData.noneUIData, UILayer.NormalLayer);

// Open a UI panel that requires data
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

// Open a scene UI (e.g., 3D model display)
UISystem.Instance.OpenViewInScene<CharacterPreviewPanel, CharacterData>(characterData);
```

### 4. Write UI Panel Logic

```csharp
public partial class LoginPanel : UIView
{
    // Automatically generated binding fields
    private Button loginButton;
    private InputField usernameInput;
    private InputField passwordInput;
    
    public override void OnOpen()
    {
        // Control binding
        BindUI();
        
        // Add event listeners
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }
    
    public override void OnResume()
    {
        // Called when the panel resumes from a paused state
    }
    
    public override void OnPause()
    {
        // Called when the panel is covered by another panel
    }
    
    public override void OnClose()
    {
        // Remove event listeners
        loginButton.onClick.RemoveListener(OnLoginButtonClick);
    }
    
    public override void OnUpdate()
    {
        // Per-frame update logic
    }
    
    private void OnLoginButtonClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        
        // Login logic...
    }
}
```

### 5. Close UI Panel

```csharp
// Close the top panel
UISystem.Instance.CloseTopPanel(UILayer.NormalLayer);

// Close a scene UI
sceneUIView.CloseInSceneLayer();

// Close all panels
UISystem.Instance.CloseAll();
```

## Best Practices

1. **UI Layer Management**
   - Assign different UI functions to appropriate layers
   - Scene Layer: 3D model display, in-scene UI
   - Background Layer: Full-screen backgrounds, main menu backgrounds
   - Normal Layer: Main function panels
   - Info Layer: Information display, status panels
   - Top Layer: Pop-ups, dialog boxes
   - Tip Layer: Hints, notifications, tooltips

2. **Data-Driven**
   - Create an IUIData implementation class for each complex UI panel
   - Use NoneUIData instances as parameters for UI panels with no data
   - Update UI when data changes, rather than directly modifying UI

3. **UI Lifecycle**
   - OnOpen: Initialize UI, bind events
   - OnResume: Restore UI state, resubscribe to events
   - OnPause: Pause UI state, unsubscribe from some events
   - OnClose: Completely clean up resources, unsubscribe from all events
   - OnUpdate: Handle UI logic that needs to be updated every frame

## Notes

1. UI prefabs should be stored in the Resources/UIPrefabs directory
2. UI scripts are generated by default in the Assets/UI/UIScripts directory
3. UI binding code is generated by default in the Assets/UI/UIBind directory
4. Ensure a Canvas exists in the scene or allow the system to create one automatically