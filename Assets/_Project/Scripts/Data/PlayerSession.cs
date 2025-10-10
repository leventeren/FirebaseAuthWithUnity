using System;
using System.IO;
using UnityEngine;

public class PlayerSession
{
    private readonly string _filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
    public PlayerData Data { get; private set; }

    public void SetData(string uid, string name, string email)
    {
        if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(name)) return;

        Data = new PlayerData(uid, name, email);
        SaveLocal();
    }

    private void SaveLocal()
    {
        if (Data == null) return;

        try
        {
            var json = JsonUtility.ToJson(Data);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Local Save Error: " + e.Message);
        }
    }

    public PlayerData LoadLocalData()
    {
        if (!File.Exists(_filePath))
            return null;
    
        try
        {
            var json = File.ReadAllText(_filePath);
            var loadedData = JsonUtility.FromJson<PlayerData>(json);
            Data = loadedData;
            return loadedData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load Local Error: {e.Message}");
            Data = null;
            return null;
        }
    }

    public void ClearData()
    {
        try
        {
            if (ExistsOnDisk()) File.Delete(_filePath);
            Data = null;
        }
        catch (Exception e)
        {
            Debug.LogError("Clear Data Error: " + e.Message);
        }
    }

    public bool HasData()
    {
        return Data != null;
    }

    private bool ExistsOnDisk()
    {
        return File.Exists(_filePath);
    }
}