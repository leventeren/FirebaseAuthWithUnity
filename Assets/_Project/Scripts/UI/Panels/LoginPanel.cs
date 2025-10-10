using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : PanelBase
{
    [Header("Panel UI Elements")]
    [SerializeField] private TMP_InputField emailLoginInput;
    [SerializeField] private TMP_InputField passwordLoginInput;
    [SerializeField] private Button loginButton;

    private void OnEnable()
    {
        loginButton.onClick.AddListener(Login);
    }
    
    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(Login);
    }

    private async void Login()
    {
        try
        {
            AuthUIController.Message("Logging in...");
            var userEmail = emailLoginInput.text.Trim();
            var userPassword = passwordLoginInput.text.Trim();

            emailLoginInput.text = "";
            passwordLoginInput.text = "";
            
            await AuthUIController.HandleAuthResult(AuthUIController.AuthManager.Login(userEmail, userPassword), ProcessType.Login);
        }
        catch (Exception e)
        {
            AuthUIController.Message(e.Message);
        }
    }
}