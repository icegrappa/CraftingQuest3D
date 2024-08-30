using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentGenerator : MonoBehaviour
{
    // EnvironmentGenerator generujący środowisko gry w sposób proceduralny. 
    // Tworzy teren za pomoca systemu grid, umieszcza na niej ściany, drzewa i zasoby, 
    // a także przechowuje te obiekty w dictionary, aby ułatwić zarządzanie.
    public static EnvironmentGenerator instance { get; private set; }

    [Header("Terrain Settings")] [SerializeField]
    private GameObject[] terrainPrefabs; // prefaby terenu

    [SerializeField] private int terrainSize = 50; // Rozmiar pojedynczego segmentu terenu (zakładamy kwadrat 50x50) taki mam model: D
    [SerializeField] private int gridWidth = 5; // Szerokość siatki terenu x
    [SerializeField] private int gridHeight = 5; // Wysokość siatki terenu z
    [SerializeField] private Transform terrainParent; // Rodzic dla wszystkich segmentów terenu

    [Header("Wall Settings")] [SerializeField]
    private GameObject wallPrefab; // prefaby sciany , primitywny cube

    [SerializeField] private float wallHeight = 30f; // Wysokość ściany
    [SerializeField] private float wallThickness = 1f; // Grubość ściany

    [Header("Tree Settings")] [SerializeField]
    private GameObject[] treePrefabs; // DRZEWA

    [SerializeField] private int treeDensity = 10; // DENSITY ROZMIESZCZENIA DRZE
    [SerializeField] private LayerMask terrainLayerMask; // LAYER TERENU

    [Header("Resource Settings")] [SerializeField]
    private ResourceData[] resources; // zasoby

    [SerializeField] private float resourceDistanceFromTree = 2f; // Minimalna odległość od drzewa czyli jak daleko zasob musi szukac od drzwa

    private readonly Dictionary<Vector2Int, List<GameObject>>
        spawnedObjectsGrid = new(); // Przechowuje obiekty według pozycji w gridzie

    private Vector3 terrainStart; // Pozycja początkowa całego terenu
    private Vector3 terrainEnd; // Pozycja końcowa całego terenu / po przekatenej

    [Serializable]
    public struct ResourceData
    {
        public GameObject resourcePrefab;
        public int resourceDensity; // Liczba zasobów na segment terenu
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        // Generowanie terenu, obliczanie jego granic i dodanie elementów takich jak ściany, drzewa i zasoby
        GenerateTerrain();
        CalculateTerrainBounds();
        PlaceWalls();
        SpawnTrees();
        SpawnResources();
    }

    // Generowanie  terenu na podstawie zadanych wymiarów i prefabów
    private void GenerateTerrain()
    {
        for (var x = 0; x < gridWidth; x++)
        for (var z = 0; z < gridHeight; z++)
        {
            var position = new Vector3(x * terrainSize, 0, z * terrainSize); // Obliczanie pozycji dla danego segmentu

            var selectedPrefab = terrainPrefabs[Random.Range(0, terrainPrefabs.Length)]; // losowy prefab terenu aby zapewnic roznorodnosc
            var terrainTile =
                Instantiate(selectedPrefab, position, Quaternion.identity, terrainParent); // spwanienie tilu terenu z prefaba 

            terrainTile.isStatic = true; // Ustawianie segmentu jako obiektu statycznego // dla optymalizacji
        }
    }

    // Obliczanie pozycji startowej i końcowej całego terenu 
    private void CalculateTerrainBounds()
    {
        terrainStart = new Vector3(0, 0, 0);
        terrainEnd = new Vector3(gridWidth * terrainSize, 0, gridHeight * terrainSize);

        Debug.Log("Terrain Start: " + terrainStart);
        Debug.Log("Terrain End: " + terrainEnd);
    }

    // Umieszczanie ścian wokół terenu gora/ dol /prawo /lewo
    private void PlaceWalls()
    {
        float terrainWidth = gridWidth * terrainSize;
        float terrainHeight = gridHeight * terrainSize;

        var bottomWallPosition = new Vector3(terrainWidth / 2 - terrainSize / 2, wallHeight / 2, -terrainSize / 2);
        CreateWall(bottomWallPosition, terrainWidth, wallThickness);

        var topWallPosition = new Vector3(terrainWidth / 2 - terrainSize / 2, wallHeight / 2,
            terrainHeight - terrainSize / 2);
        CreateWall(topWallPosition, terrainWidth, wallThickness);

        var leftWallPosition = new Vector3(-terrainSize / 2, wallHeight / 2, terrainHeight / 2 - terrainSize / 2);
        CreateWall(leftWallPosition, wallThickness, terrainHeight);

        var rightWallPosition = new Vector3(terrainWidth - terrainSize / 2, wallHeight / 2,
            terrainHeight / 2 - terrainSize / 2);
        CreateWall(rightWallPosition, wallThickness, terrainHeight);
    }

    // Tworzenie ściany na podstawie pozycji, długości i grubości
    private void CreateWall(Vector3 position, float length, float thickness)
    {
        if (wallPrefab != null)
        {
            var wall = Instantiate(wallPrefab, position, Quaternion.identity, terrainParent);
            wall.transform.localScale = new Vector3(length, wallHeight, thickness); // Skalowanie ściany
            wall.isStatic = true; // Ustawianie ściany jako obiektu statycznego
        }
        else
        {
            Debug.LogWarning("Prefab ściany nie został przypisany.");
        }
    }

    // Rozstawianie drzew na terenie na podstawie zadanego zagęszczenia
    private void SpawnTrees()
    {
        for (var x = 0; x < gridWidth; x++)
        for (var z = 0; z < gridHeight; z++)
        for (var i = 0; i < treeDensity; i++)
        {
            // Losowa pozycja w ramach segmentu terenu, początkowo na wysokości 100 (zostanie to zmienione po rzucie raycast)
            float randomX = Random.Range(0, terrainSize);
            float randomZ = Random.Range(0, terrainSize);
            var randomPosition = new Vector3(x * terrainSize + randomX, 100, z * terrainSize + randomZ);

            // Rzutowanie promienia w dół w celu znalezienia powierzchni terenu
            if (Physics.Raycast(randomPosition, Vector3.down, out var hit, Mathf.Infinity, terrainLayerMask))
            {
                var selectedTreePrefab =
                    treePrefabs[Random.Range(0, treePrefabs.Length)]; // Losowy wybór prefabrykatu drzewa
                var tree = Instantiate(selectedTreePrefab, hit.point, Quaternion.identity,
                    terrainParent); // Umieszczenie drzewa
                tree.transform.up = hit.normal; // Dopasowanie drzewa do normalnej powierzchni terenu

                AddObjectToGrid(
                    new Vector2Int(Mathf.FloorToInt(hit.point.x / terrainSize),
                        Mathf.FloorToInt(hit.point.z / terrainSize)),
                    tree); // Dodanie drzewa do dictionary według pozycji w gridzie
            }
        }
    }

    // Rozstawianie zasobów na terenie na podstawie zadanego zagęszczenia
    private void SpawnResources()
    {
        foreach (var resource in resources)
            for (var x = 0; x < gridWidth; x++)
            for (var z = 0; z < gridHeight; z++)
            for (var i = 0; i < resource.resourceDensity; i++)
            {
                Vector3 randomPosition;
                var validPosition = false;
                var attempts = 0;
                var maxAttempts = 100; // Ograniczenie liczby prób losowania pozycji

                // Pętla próbująca znaleźć odpowiednią pozycję dla zasobu
                do
                {
                    float randomX = Random.Range(0, terrainSize);
                    float randomZ = Random.Range(0, terrainSize);
                    randomPosition = new Vector3(x * terrainSize + randomX, 100, z * terrainSize + randomZ);

                    // Rzutowanie promienia w dół w celu znalezienia powierzchni terenu
                    // Rzutowanie promienia w dół w celu znalezienia powierzchni terenu
                    if (Physics.Raycast(randomPosition, Vector3.down, out var hit, Mathf.Infinity, terrainLayerMask))
                    {
                        // Sprawdzenie, czy zasób nie znajduje się zbyt blisko drzewa
                        validPosition = true;

                        var gridPos = new Vector2Int(Mathf.FloorToInt(hit.point.x / terrainSize),
                            Mathf.FloorToInt(hit.point.z / terrainSize));
                        var objectsInGrid = GetObjectsInGrid(gridPos);

                        foreach (var obj in objectsInGrid)
                            // Jeżeli zasób znajduje się zbyt blisko drzewa, odrzucamy tę pozycję
                            if (Vector3.Distance(hit.point, obj.transform.position) < resourceDistanceFromTree)
                            {
                                validPosition = false;
                                break;
                            }

                        // Jeżeli pozycja jest poprawna, instancjujemy zasób na terenie
                        if (validPosition)
                        {
                            var resourceObj = Instantiate(resource.resourcePrefab, hit.point, Quaternion.identity,
                                terrainParent);
                            AddObjectToGrid(gridPos, resourceObj); // dodaje resource wedlug pozycji do dicitonary
                        }
                    }

                    attempts++; // ile prob?
                } while
                    (!validPosition &&
                     attempts <
                     maxAttempts); // szukamy do pozytwnej pozycji albo gdy excced dla optymalizacji
            }
    }

    // Dodaje obiekt do dictionary na podstawie pozycji w gridzie
    private void AddObjectToGrid(Vector2Int gridPos, GameObject obj)
    {
        if (!spawnedObjectsGrid.ContainsKey(gridPos))
            spawnedObjectsGrid[gridPos] =
                new List<GameObject>(); // Jeśli dany klucz jeszcze nie istnieje, tworzymy nową listę

        spawnedObjectsGrid[gridPos].Add(obj); // Dodajemy obiekt do listy pod odpowiednim kluczem
    }

    // Pobiera obiekty znajdujące się w danej pozycji w siatce
    private List<GameObject> GetObjectsInGrid(Vector2Int gridPos)
    {
        if (spawnedObjectsGrid.ContainsKey(gridPos))
            return spawnedObjectsGrid[gridPos]; // Zwraca listę obiektów dla danej pozycji
        return new List<GameObject>(); // Jeśli nie ma obiektów, zwraca pustą listę
    }

    
    public (Vector3 start, Vector3 end) GetTerrainBounds()
    {
        return (terrainStart, terrainEnd); 
    }
}