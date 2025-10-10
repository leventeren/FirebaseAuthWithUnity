using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    [SerializeField] private Hero heroPrefab;
    [SerializeField, ReadOnly] private List<Hero> heroes = new();
    
    private FirestoreDbService _dbService;
    private const int RefreshIntervalSeconds = 10;
    
    private void Start()
    {
        InitializeAndStartLoop().Forget();
    }

    private async UniTaskVoid InitializeAndStartLoop()
    {
        _dbService = new FirestoreDbService();
        await _dbService.InitializeAsync();
        
        await FetchAndSpawnHeroes();
        
        while (this != null)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(RefreshIntervalSeconds), cancellationToken: this.GetCancellationTokenOnDestroy());
            await FetchAndSpawnHeroes();
        }
    }
    
    private async UniTask FetchAndSpawnHeroes()
    {
        try
        {
            var allHeroes = await _dbService.GetAllHeroes();
            var existingID = heroes.Select(h => h.Id).ToHashSet();

            foreach (var heroData in allHeroes)
            {
                if (existingID.Contains(heroData.Id))
                    continue;

                var randomPos = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));
                SpawnHero(heroData, randomPos);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

    private void SpawnHero(HeroData data, Vector3 position)
    {
        var hero = Instantiate(heroPrefab, position, Quaternion.identity, transform);
        hero.gameObject.name = $"Hero {data.Name}";
        hero.Init(data);
        
        heroes.Add(hero);
    }
}