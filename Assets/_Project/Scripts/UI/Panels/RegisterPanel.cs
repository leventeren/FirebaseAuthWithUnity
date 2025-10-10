using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : PanelBase
{
    [Header("Panel UI Elements")]
    [SerializeField] private TMP_InputField emailRegisterInput;
    [SerializeField] private TMP_InputField passwordRegisterInput;
    [SerializeField] private TMP_InputField nameRegisterInput;
    [SerializeField] private Button registerButton;

    private void OnEnable()
    {
        registerButton.onClick.AddListener(Register);
    }
    
    private void OnDisable()
    {
        registerButton.onClick.RemoveListener(Register);
    }

    private async void Register()
    {
        try
        {
            var userEmail = emailRegisterInput.text.Trim();
            var userPassword = passwordRegisterInput.text.Trim();
            var userName = nameRegisterInput.text.Trim();
            
            await AuthUIController.HandleAuthResult(AuthUIController.AuthManager.Register(userEmail, userPassword, userName, 
                OnRegisterSuccess, OnRegisterFailed), ProcessType.Register);
        }
        catch (Exception e)
        {
            AuthUIController.Message(e.Message);
        }
    }
    
    private void OnRegisterSuccess()
    {
        AuthUIController.Message("Registration successful! Please select team.");
        AuthUIController.HidePanel<RegisterPanel>();
        AuthUIController.ShowPanel<CreateHeroPanel>();
    }

    private void OnRegisterFailed()
    {
        AuthUIController.Message("Registration failed. Please try again.");
    }
}