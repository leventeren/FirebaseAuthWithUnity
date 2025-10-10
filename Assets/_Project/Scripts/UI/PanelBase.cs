using System;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AuthUIController authUIController;
    
    protected AuthUIController AuthUIController => authUIController;

    public void Show(Action onShow = null)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        onShow?.Invoke();
    }

    public void Hide(Action onHide = null)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        onHide?.Invoke();
    }

    public virtual void SetMessage(string message)
    {
        
    }
    
    public virtual void SetUserLoggedInfo(string userInfo)
    {
        
    }
}