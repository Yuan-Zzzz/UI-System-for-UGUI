using System;
using UnityEngine;

public class UIViewController : ViewController
{
    public Action onUpdate;

    private void Update()
    {
        onUpdate?.Invoke();
    }

    private void OnDisable()
    {
        onUpdate = null;
    }
}