using System;
using System.Collections.Generic;
using Firebase;
using System.Threading.Tasks;
using UnityEngine;

public enum ProcessType
{
    None,
    Register,
    Login
}

public class AuthUIController : MonoBehaviour
{
    [SerializeField] private List<PanelBase> panels;

    private FirebaseAuthManager authManager;
    private FirestoreDbService _dbService;
    private PlayerSession playerSession;
    private PlayerData playerData;
    private HeroManager heroManager;

    #region PROPERTIES
    public FirebaseAuthManager AuthManager => authManager;
    public FirestoreDbService DbService
    {
        get => _dbService;
        set => _dbService = value;
    }
    public PlayerSession PlayerSession => playerSession;
    #endregion
    
    private PanelBase _currentPanel;

    private void Start()
    {
        authManager = FindFirstObjectByType<FirebaseAuthManager>();
        heroManager = FindFirstObjectByType<HeroManager>();
        
        playerSession = new PlayerSession();
        playerData = playerSession.LoadLocalData();
        
        if (playerData != null)
        {
            Debug.Log($"Name : {playerSession.Data.Name} - UID : ({playerSession.Data.Uid}) - Email : {playerSession.Data.Email}");
            HidePanel<RegisterPanel>();
            HidePanel<LoginPanel>();
            
            ShowPanel<LoggedPanel>(() =>
            {
                var loggedPanel = panels.Find(p => p is LoggedPanel) as LoggedPanel;
                if (loggedPanel != null)
                {
                    loggedPanel.UpdateUserInfo(playerSession.Data.Uid);
                }
            });
        }
        else
        {
            Debug.Log("Local player data not found.");
            ShowPanel<RegisterPanel>();
            
            HidePanel<LoginPanel>();
            HidePanel<LoggedPanel>();
            HidePanel<CreateHeroPanel>();
        }
    }
    
    private async void GetRandomHero()
    {
        try
        {
            _dbService = new FirestoreDbService();
            await _dbService.InitializeAsync();
            
            var opponents = await _dbService.GetRandomHeroAsync(
                power: 100,
                count: 3
            );

            Debug.Log($"Total Hero Count : {opponents.Count}");

            await _dbService.GetAllHeroesAsync();
        }
        catch (Exception e)
        {
            Message(e.Message);
        }
    }
    
    private async void DeleteTestBuildHeroes()
    {
        try
        {
            _dbService = new FirestoreDbService();
            await _dbService.InitializeAsync();
        
            await _dbService.DeleteTestBuildHeroesAsync();
        }
        catch (Exception e)
        {
            Message(e.Message);
        }
    }

    public async Task HandleAuthResult(Task task , ProcessType processType)
    {
        try
        {
            await task;
            if (authManager.CurrentUser != null)
            {
                switch (processType)
                {
                    case ProcessType.Register:
                        var uid = authManager.CurrentUser.UserId;
                        var email = authManager.CurrentUser.Email ?? "NoEmail";
                        var playerName = await authManager.GetPlayerNameById(uid);
                        playerSession.SetData(uid, playerName, email);
                        
                        HidePanel<RegisterPanel>();
                        
                        break;
                    case ProcessType.Login:
                        HidePanel<LoginPanel>();
                        break;
                    case ProcessType.None:
                        break;
                }
                
                ShowPanel<LoggedPanel>(() =>
                {
                    var loggedPanel = panels.Find(p => p is LoggedPanel) as LoggedPanel;
                    if (loggedPanel != null)
                    {
                        loggedPanel.UpdateUserInfo(authManager.CurrentUser.UserId);
                    }
                });
                
            }
        }
        catch (FirebaseException fe)
        {
            Message(fe.Message);
        }
        catch (Exception e)
        {
            Message(e.Message);
        }
    }

    public void Message(string message)
    {
        var debugPanel = panels.Find(p => p is DebugPanel) as DebugPanel;
        if (debugPanel != null)
        {
            debugPanel.SetMessage(message);
        }
    }
    
    public void ShowPanel<T>(Action onShow = null) where T : PanelBase
    {
        var panel = panels.Find(p => p is T);
        if (panel == null) return;
        
        _currentPanel = panel;
        _currentPanel.Show(onShow);
    }
    
    public void HidePanel<T>(Action onHide = null) where T : PanelBase
    {
        var panel = panels.Find(p => p is T);
        if (panel == null) return;
        
        _currentPanel = panel;
        _currentPanel.Hide(onHide);
    }

    public void HideAllPanels()
    {
        foreach (var panel in panels)
        {
            if (panel is DebugPanel) continue;
            panel.Hide();
        }
    }
}
