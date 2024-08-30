using Unity.Netcode;
using UnityEngine;

public class TitleScreenUIManager : MonoBehaviour
{
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StarNewGame()
    {
        StartCoroutine(GameManager.instance.LoadNewWorld());
    }
    
    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else

        Application.Quit();
#endif
    }
}