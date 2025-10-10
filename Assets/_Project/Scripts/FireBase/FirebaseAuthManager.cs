using System;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;

    public FirebaseUser CurrentUser => user;

    private async void Start()
    {
        try
        {
            Environment.SetEnvironmentVariable("USE_AUTH_EMULATOR", "false");
        
            await FirebaseApp.CheckAndFixDependenciesAsync();
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase initialization error: {e.Message}");
        }
    }

    public async Task Register(string email, string password, string playerName = null, Action onSuccess = null, Action onError = null)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            user = result.User;
            
            if (string.IsNullOrEmpty(playerName))
            {
                var n = UnityEngine.Random.Range(1000, 9999);
                playerName = $"DemoPlayer{n}";
            }
            
            var userData = new Dictionary<string, object>
            {
                { "uid", user.UserId },
                { "name", playerName },
                { "email", user.Email }
            };
            await db.Collection("users").Document(user.UserId).SetAsync(userData);

            Debug.Log($"Registered User ID : {user.UserId}, Player Name : {playerName}, Email : {user.Email}");
            
            onSuccess?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            onError?.Invoke();
            throw;
        }
    }

    public async Task Login(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = result.User;
            Debug.Log("CurrentUser ID : " + CurrentUser.UserId);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    public void Logout()
    {
        auth.SignOut();
        user = null;
    }

    public async Task<string> GetPlayerNameById(string uid)
    {
        try
        {
            var doc = await FirebaseFirestore.DefaultInstance
                .Collection("users")
                .Document(uid)
                .GetSnapshotAsync();

            if (doc.Exists && doc.ContainsField("name"))
            {
                return doc.GetValue<string>("name");
            }

            Debug.LogWarning($"No name found for UID={uid}");
            return "Unknown Player";
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get name for UID={uid}: {e.Message}");
            return "Unknown Player";
        }
    }
}
