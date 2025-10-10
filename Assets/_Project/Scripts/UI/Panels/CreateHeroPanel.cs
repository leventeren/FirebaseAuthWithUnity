using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateHeroPanel : PanelBase
{
    [Header("Panel UI Elements")]
    [SerializeField] private Button createHeroButton;

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        createHeroButton.onClick.AddListener(CreateHero);
    }
    
    private void OnDisable()
    {
        createHeroButton.onClick.RemoveListener(CreateHero);
    }
    
    private async void CreateHero()
    {
        try
        {
            AuthUIController.DbService = new FirestoreDbService();
            await AuthUIController.DbService.InitializeAsync();
            
            var selectedHeroName = GetRandomHeroName();
        
            var heroData = new HeroData
            {
                Name = selectedHeroName,
                PlayerID = AuthUIController.PlayerSession.Data.Uid,
                Hp = 100,
                Power = 100,
                SelectedWeapons = new List<int> { 1, 2, 3 }
            };
        
            await AuthUIController.DbService.UploadHeroDataAsync(heroData);
            
        }
        catch (Exception e)
        {
            AuthUIController.Message(e.Message);
        }
    }
    
    private string GetRandomHeroName()
    {
        var heroNames = Enum.GetValues(typeof(HeroNames));
        return heroNames.GetValue(UnityEngine.Random.Range(0, heroNames.Length)).ToString();
    }
}