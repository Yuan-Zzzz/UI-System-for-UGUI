using System.Linq;
using UnityEngine;

public abstract class UIView
{
    protected GameObject _panelObject; //Panel对应的预制体
   
    protected UILayer _uiLayer;//ui的层级
    public IUIData _uiData;//对应的数据
    public GameObject PanelObject
    {
        get => _panelObject;
        set => _panelObject = value;
    }
    
    public UILayer UILayer
    {
        get => _uiLayer;
        set => _uiLayer = value;
    }
    
    public abstract void OnOpen();
    public abstract void OnResume();
    public abstract void OnPause();
    public abstract void OnClose();
    public abstract void OnUpdate();
    
    protected virtual T GetOrAddComponentInChildren<T>(string childName) where T : Component
    {
        return _panelObject.GetComponentsInChildren<T>().FirstOrDefault(child => child.name == childName);
    }

   public void SetData<T>(T uiData) where T : class, IUIData
   {
       _uiData = uiData;
   }
}
public abstract class UIView<T>:UIView where T: class, IUIData
{
    
    public T UIData
    {
        get => _uiData as T;
        set => _uiData = value;
    }
    
}