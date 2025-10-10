using Firebase;
using Firebase.Firestore;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Object = UnityEngine.Object;

public class FirestoreDbService
{
    private FirebaseFirestore db;

    public async Task InitializeAsync()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            db = FirebaseFirestore.DefaultInstance;
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
        }
    }

    public async Task UploadHeroDataAsync(HeroData heroData)
    {
        if (db == null)
        {
            Debug.LogError("Upload failed: Firestore not initialized (db is null)");
            return;
        }

        try
        {
            var data = new Dictionary<string, object>
            {
                { "HeroName", heroData.Name },
                { "PlayerId", heroData.PlayerID },
                { "Hp", heroData.Hp },
                { "Power", heroData.Power },
                { "SelectedWeapons", heroData.SelectedWeapons },
                { "tag", "testBuild" }
            };

            var result = await db.Collection("heroes").AddAsync(data);
            Debug.Log($"Hero data uploaded. DocID={result.Id}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Hero data upload failed: {e.Message}\n{e.StackTrace}");
        }
    }
    
    public async Task<List<DocumentSnapshot>> GetRandomHeroAsync(int power, int count = 3)
    {
        var snapshot = await db.Collection("heroes")
            .WhereEqualTo("Power", power)
            .GetSnapshotAsync();

        if (snapshot.Count == 0)
        {
            Debug.Log("No hero found with same power.");
            return new List<DocumentSnapshot>();
        }

        var docs = snapshot.Documents.ToList();
        var shuffled = docs.OrderBy(x => UnityEngine.Random.value).ToList();
        var selected = shuffled.Take(Mathf.Min(count, shuffled.Count)).ToList();

        foreach (var doc in selected)
        {
            Debug.Log($"Hero: DocID={doc.Id}, Data={FirestoreDebugExt.DictToString(doc.ToDictionary())}");
        }

        return selected;
    }

    public async Task DeleteAllHeroesAsync()
    {
        var snapshot = await db.Collection("heroes").GetSnapshotAsync();
        foreach (var doc in snapshot.Documents)
        {
            await doc.Reference.DeleteAsync();
        }
        Debug.Log("All heroes deleted.");
    }

    public async Task DeleteTestBuildHeroesAsync()
    {
        var snapshot = await db.Collection("heroes")
            .WhereEqualTo("tag", "testBuild")
            .GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            await doc.Reference.DeleteAsync();
        }
        Debug.Log("All tesBuild heroes deleted.");
    }

    public async Task GetAllHeroesAsync()
    {
        var snapshot = await db.Collection("heroes").GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            Debug.Log($"DocID: {doc.Id}");
            Debug.Log(doc.ToDictionary().ToDebugString());
            Debug.Log("-------------------");
        }
    }

    public async Task<List<HeroData>> GetAllHeroes()
    {
        var heroesList = new List<HeroData>();
        
        var snapshot = await db.Collection("heroes").GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            var heroData = new HeroData
            {
                Id = doc.Id,
                Name = doc.GetValue<string>("HeroName"),
                PlayerID = doc.GetValue<string>("PlayerId"),
                Hp = (int)doc.GetValue<long>("Hp"),
                Power = (int)doc.GetValue<long>("Power"),
                SelectedWeapons = ConvertListToIntList(doc.GetValue<object>("SelectedWeapons")),
            };
            
            heroesList.Add(heroData);
        }
        
        return heroesList;
    }
    
    private List<int> ConvertListToIntList(object listObject)
    {
        var intList = new List<int>();
        
        if (listObject is System.Collections.IList firebaseList)
        {
            foreach (var item in firebaseList)
            {
                if (item is long longValue)
                {
                    intList.Add((int)longValue);
                }
                else if (item is int intValue)
                {
                    intList.Add(intValue);
                }
            }
        }
        
        return intList;
    }
}