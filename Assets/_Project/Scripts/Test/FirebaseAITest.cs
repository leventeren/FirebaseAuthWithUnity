using System.Threading.Tasks;
using UnityEngine;
using Firebase.AI;
using UnityEngine.UI;

public class FirebaseAITest : MonoBehaviour
{
    /*[SerializeField] private Button generateButton;
    
    private Chat _chatModel;

    private void OnEnable()
    {
        generateButton.onClick.AddListener(()=> { _ = GenerateImage(); });
    }
    
    private void OnDisable()
    {
        generateButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        var ai = FirebaseAI.DefaultInstance.GetGenerativeModel(modelName: "gemini-2.5-flash-image");
        _chatModel = ai.StartChat();
    }

    private async Task GenerateImage()
    {
        var response = await _chatModel.SendMessageAsync("hi ai! generate an image of a sunset over a mountain range");
        Debug.Log(response.Text);
    }*/
}
