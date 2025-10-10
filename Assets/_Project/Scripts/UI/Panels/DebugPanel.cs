using TMPro;
using UnityEngine;

public class DebugPanel : PanelBase
{
    [SerializeField] private TextMeshProUGUI messageText;
    
    public override void SetMessage(string message)
    {
        messageText.text = message;
    }
}