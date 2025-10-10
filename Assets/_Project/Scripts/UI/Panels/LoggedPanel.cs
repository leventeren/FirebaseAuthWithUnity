using System;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoggedPanel : PanelBase
{
    [Header("Panel UI Elements")]
    [SerializeField] private TextMeshProUGUI userLoggedInfoText;
    [SerializeField] private Button logoutButton;

    private void OnEnable()
    {
        logoutButton.onClick.AddListener(Logout);
    }
    
    private void OnDisable()
    {
        logoutButton.onClick.RemoveListener(Logout);
    }
    
    public async void UpdateUserInfo(string uid)
    {
        try
        {
            var snapshot = await FirebaseFirestore.DefaultInstance
                .Collection("users")
                .Document(uid)
                .GetSnapshotAsync();
            
            string GetFieldOrUid(string field) => snapshot.Exists && snapshot.ContainsField(field) ? snapshot.GetValue<string>(field) : uid;
            
            var playerName = GetFieldOrUid("name");
            var email = GetFieldOrUid("email");
            
            SetUserLoggedInfo($"{playerName} logged [{email}]");
            
            AuthUIController.PlayerSession.SetData(uid, playerName, email);
        }
        catch (Exception e)
        {
            AuthUIController.Message(e.Message);
        }
    }
    public override void SetUserLoggedInfo(string userInfo)
    {
        userLoggedInfoText.text = userInfo;
    }

    private void Logout()
    {
        AuthUIController.AuthManager.Logout();
        AuthUIController.HideAllPanels();
        AuthUIController.ShowPanel<LoginPanel>();
    }
}