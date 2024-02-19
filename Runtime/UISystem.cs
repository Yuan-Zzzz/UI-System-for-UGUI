using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public interface IUISystem
{
    UIView<TUIData> OpenPanel<T,TUIData>(TUIData uiData, UILayer uiLayer = UILayer.NormalLayer) where T : UIView, new()  where TUIData : class, IUIData;
    UIView<TUIData> OpenViewInScene<T,TUIData>(TUIData uiData) where T : UIView, new() where TUIData : class, IUIData;
    void CloseInSceneLayer(UIView uiView);
    UIView GetInSceneLayer<T>();
    bool IsInSceneLayer(UIView uiView);
    void CloseTopPanel(UILayer uiLayer);
    void CloseAll();
    void Reset();
    UIView GetTopPanel(UILayer layer);
    Canvas GetOrAddPanelCanvas();
}

public class UISystem : IUISystem
{
    private Dictionary<string, GameObject> _panelPrefabDict = new Dictionary<string, GameObject>();
    private List<UIView> _sceneLayerPanelList = new List<UIView>();
    private Dictionary<UILayer,Stack<UIView>> _panelStack  = new Dictionary<UILayer, Stack<UIView>>();
    private Canvas _activeCanvas; //面板挂载的Canvas

    #region 通过单例调用 后续可改为其他

    private static UISystem _instance;

    public static UISystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UISystem();
                _instance.OnInit(); // 执行初始化操作
            }

            return _instance;
        }
    }

    private UISystem()
    {
        // 私有构造函数，确保只能通过Instance属性访问
    }

    #endregion


    //初始化
    public void OnInit()
    {
        //TODO:通过资源加载系统加载所有Panel的预制体
        foreach (var newPanel in Resources.LoadAll<GameObject>("UIPrefabs"))
        {
            _panelPrefabDict.Add(newPanel.name, newPanel);
        }
    }

    public UIView<TUIData> OpenPanel<T,TUIData>(TUIData uiData , UILayer uiLayer = UILayer.NormalLayer) where T : UIView, new() where TUIData : class, IUIData
    {
        if (uiLayer == UILayer.SceneLayer)
        {
            Debug.LogError("场景中的UI请使用OpenViewInScene方法");
        }
        T newPanel = CreateAndInitializePanel<T,TUIData>(uiData, uiLayer);
        
        if (!_panelStack.ContainsKey(uiLayer))
        {
            _panelStack.Add(uiLayer,new Stack<UIView>());
        }
        _panelStack[uiLayer].Push(newPanel);
        return newPanel as UIView<TUIData>;
    }
    

    public UIView<TUIData> OpenViewInScene<T, TUIData>(TUIData uiData) where T : UIView, new() where TUIData : class, IUIData
    {
        var newPanel = CreateAndInitializePanel<T,TUIData>(uiData, UILayer.SceneLayer);
        _sceneLayerPanelList.Add(newPanel);
        return newPanel as UIView<TUIData>;
    }

    private T CreateAndInitializePanel<T,TUIData>(TUIData uiData, UILayer uiLayer) where T : UIView, new() where TUIData : class, IUIData
    {
        //判断场景中是否有画布
        if (_activeCanvas == null) _activeCanvas = GetOrAddPanelCanvas();

        var newPanelObject = Object.Instantiate(_panelPrefabDict[typeof(T).ToString()], _activeCanvas.transform);
        newPanelObject.name = typeof(T).ToString();
        var newPanel = new T
        {
            PanelObject = newPanelObject
        };
        newPanel.UILayer = uiLayer;
        newPanel.PanelObject.GetComponent<UIViewController>().onUpdate += newPanel.OnUpdate;
        
        //传入数据
     
        newPanel.SetData(uiData);
        newPanel.OnOpen();
        newPanel.OnResume();

        return newPanel;
    }
    
    public  UIView GetInSceneLayer<T>()
    {
        foreach (var view in _sceneLayerPanelList)
        {
            if (view.GetType() == typeof(T))
            {
                return view;
            }
        }

        return null;
    }

    public bool IsInSceneLayer(UIView uiView)
    {
        return _sceneLayerPanelList.Contains(uiView);
    }

    public void CloseTopPanel(UILayer uiLayer)
    {
        if (_panelStack[uiLayer].Count == 0) return;
        var topPanel = GetTopPanel(uiLayer);
        Object.Destroy(topPanel.PanelObject);
        topPanel.OnPause();
        topPanel.OnClose();
        _panelStack[uiLayer].Pop();
        if (_panelStack[uiLayer].Count == 0) return;
        GetTopPanel(uiLayer).PanelObject.GetComponent<UIViewController>().onUpdate += GetTopPanel(uiLayer).OnUpdate;
        GetTopPanel(uiLayer).OnResume();
    }

    public void CloseAll()
    {
        foreach (var layer in _panelStack.Keys)
        {
            CloseTopPanel(layer);
            
        }
    }

    public void CloseInSceneLayer(UIView uiView)
    {
        var view = uiView;
        Object.Destroy(view.PanelObject);
        if (_sceneLayerPanelList.Contains(view))
        {
            _sceneLayerPanelList.Remove(view);
        }
        view.OnPause();
        view.OnClose();
    }

    public void Reset()
    {
        CloseAll();
        _activeCanvas = null;
    }

    public UIView GetTopPanel(UILayer layer)
    {
        if (_panelStack[layer].Count > 0)
            return _panelStack[layer].Peek();
        Debug.LogError("UI栈中无任何面板");
        return null;
    }

    public Canvas GetOrAddPanelCanvas()
    {
        _activeCanvas = GameObject.Find("OverlayCanvas").GetComponent<Canvas>();
        if (_activeCanvas != null)
            return _activeCanvas;

        var panelCanvasGameObject = new GameObject("OverlayCanvas");

        var canvas = panelCanvasGameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = panelCanvasGameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var graphicRaycaster = panelCanvasGameObject.AddComponent<GraphicRaycaster>();

        // 判断场景中是否存在EventSystem
        if (!Object.FindObjectOfType<EventSystem>())
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        var uIRoot = new GameObject("UIRoot");
        panelCanvasGameObject.transform.SetParent(uIRoot.transform);
        Object.FindObjectOfType<EventSystem>().transform.SetParent(uIRoot.transform);
        return canvas;
    }

    // public T GetOrAddComponent<T>() where T : Component
    // {
    //     return GetTopPanel().PanelObject.GetComponent<T>();
    // }
    //
    // public T GetOrAddComponentInChildren<T>(string childName) where T : Component
    // {
    //     return GetTopPanel().PanelObject.GetComponentsInChildren<T>().FirstOrDefault(child => child.name == childName);
    // }
}