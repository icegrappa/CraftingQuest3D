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
}