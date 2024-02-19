
    public static class UIViewExtension
    {
        public static void CloseInSceneLayer(this UIView self)
        {
            if(UISystem.Instance.IsInSceneLayer(self))
                UISystem.Instance.CloseInSceneLayer(self);
        }
    }