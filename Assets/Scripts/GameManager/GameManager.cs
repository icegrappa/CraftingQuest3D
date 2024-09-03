using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int mainMenuSceneIndex = -1;
    [SerializeField] private int worldSceneIndex = -1;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask collisionLayer;

    [SerializeField] public bool worldIsSpawned;

    private Transform playerTransform;
    
    // event który określa czy załadowana główną scene
    public delegate void WorldSceneLoadedDelegate();

    public event WorldSceneLoadedDelegate OnWorldSceneLoaded;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    ///     Instancjuje jeden lub więcej obiektów w świecie na podanej pozycji.
    /// </summary>
    /// <param name="position">Centralna pozycja w świecie, w której ma być instancjowany obiekt/y.</param>
    /// <param name="itemPrefab">Prefab obiektu do instancjowania.</param>
    /// <param name="multipleObjects">Czy instancjować wiele obiektów.</param>
    /// <param name="amount">Liczba obiektów do instancjowania, jeśli multipleObjects jest true.</param>
    /// <param name="radius">Promieńw którym losowo rozmieszczane są obiekty wokół centralnej pozycji.</param>
    public void InstantiateItemInWorld(Vector3 position, GameObject itemPrefab, bool multipleObjects = false,
        int amount = 1, float radius = 1f)
    {
        if (itemPrefab == null) return;

        for (var i = 0; i < amount; i++)
        {
            var spawnPosition = position;

            if (multipleObjects)
            {
                // Oblicz losową pozycję w promieniu od centralnej pozycji
                var randomCircle = Random.insideUnitCircle * radius;
                spawnPosition += new Vector3(randomCircle.x, 0f, randomCircle.y); // Utrzymanie stałej osi Y
            }

            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    ///     Zwraca pozycję w świecie na podstawie bieżącej pozycji myszy / upewnia sie że jest ona Na Layer
    /// </summary>
    /// <returns>Pozycja w świecie po dostosowaniu do powierzchni obiektu trafionego przez promień.</returns>
    public Vector3 GetMouseWorldPosition()
    {
        if (Camera.main == null) return playerTransform.position + playerTransform.forward * 2f;

        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundLayer))
        {
            Debug.Log(
                $"Trafiono {hitInfo.collider.gameObject.name} na warstwie {LayerMask.LayerToName(hitInfo.collider.gameObject.layer)}");

            // Dostosowanie pozycji trafienia, aby była na powierzchni, poprzez przesunięcie jej nieco wzdłuż normalnej
            var surfacePosition = hitInfo.point + hitInfo.normal * 0.1f; // 0.1f to mały offset wzdłuż normalnej

            // Sprawdzanie odległości od gracza
            if (Vector3.Distance(playerTransform.position, surfacePosition) > 4f)
            {
                Debug.Log("Pozycja jest za daleko od gracza.");
                return playerTransform.position + playerTransform.forward * 2f;
            }

            return surfacePosition;
        }

        Debug.Log("Brak trafienia na określonej warstwie.");
        return playerTransform.position + playerTransform.forward * 2f; // Zwraca pozycję gracza z offsetem, jeśli brak trafienia
    }


    //https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html

    public IEnumerator LoadNewWorld()
    {
        Debug.Log("ładuje scene o indexie " + worldSceneIndex);

        var asyncLoad = SceneManager.LoadSceneAsync(worldSceneIndex);

        if (asyncLoad == null)
        {
            Debug.LogError("Nie udało się rozpocząć ładowania sceny AsyncOperation jest null.");
            yield break;
        }

        // Czekamy, aż ładowanie osiągnie 90%
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Postep ładowania: " + asyncLoad.progress * 100 + "%");
            yield return null;
        }

        // Po osiągnięciu 90% informujemy, że scena jest prawie gotowa
        Debug.Log("Ładowanie osiągnęło 90% wywołuję OnWorldSceneLoaded...");
        OnWorldSceneLoaded?.Invoke();

        // Czekamy na zakończenie ładowania
        while (!asyncLoad.isDone) yield return null;

        Debug.Log("Ładowanie sceny zakończone i w pełni aktywowane!");
    }

    public void LoadMainMenuScene()
    {
        StartCoroutine(LoadMainMenu());
    }

    public IEnumerator LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);

        yield return null;

        // Debug.Log("Ładowanie sceny głównego menu zakończone i w pełni aktywowane!");
    }


    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

    public LayerMask GetTerrainMask()
    {
        return groundLayer;
    }

    public LayerMask GetCollisionMask()
    {
        return collisionLayer;
    }

    public void UpdatePlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}