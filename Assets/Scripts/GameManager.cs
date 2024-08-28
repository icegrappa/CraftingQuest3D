using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] int worldSceneIndex = -1;

    // Define the delegate and event
    public delegate void WorldSceneLoadedDelegate();
    public event WorldSceneLoadedDelegate OnWorldSceneLoaded;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    //https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html

    public IEnumerator LoadNewWorld()
    {
        Debug.Log("ładuje scene o indexie " + worldSceneIndex);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(worldSceneIndex);

        if (asyncLoad == null)
        {
            Debug.LogError("Nie udało się rozpocząć ładowania sceny AsyncOperation jest null.");
            yield break;
        }

        // Czekamy, aż ładowanie osiągnie 90%
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Postep ładowania: " + (asyncLoad.progress * 100) + "%");
            yield return null;
        }

        // Po osiągnięciu 90% informujemy, że scena jest prawie gotowa
        Debug.Log("Ładowanie osiągnęło 90% wywołuję OnWorldSceneLoaded...");
        OnWorldSceneLoaded?.Invoke();

        // Czekamy na zakończenie ładowania
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Ładowanie sceny zakończone i w pełni aktywowane!");
    }



    
    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}