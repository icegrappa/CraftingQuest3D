using System.Collections.Generic;
using UnityEngine;

public class ObjectRandomizer : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs; // Lista obiektów
    [SerializeField] private Vector2 scaleRange = new(0.5f, 1.5f); //
    [SerializeField] private Vector2 rotationRange = new(0f, 360f); // 
    [SerializeField] private bool randomizeOnAwake; // Flaga określająca czy losowanie ma się odbyć podczas uruchamiania

    private void Awake()
    {
        // Jeśli flaga jest prawdziwa, losuj obiekt na pozycji tego obiektu
        if (randomizeOnAwake) RandomizeObject(transform.position);
    }

    public void RandomizeObject(Vector3? position = null)
    {
        if (prefabs.Count == 0)
        {
            Debug.LogWarning("Brak obiektów do randomizera.");
            return;
        }

        // Użyj podanej pozycji lub domyśl do pozycji tego obiektu
        var spawnPosition = position ?? transform.position;

        // wybierz losowy obiekt z listy 
        var selectedPrefab = prefabs[Random.Range(0, prefabs.Count)];

        // Zainstancjuj obiekt jako dziecko
        var instance = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);

        // Wygeneruj losowy seed lub offset dla obliczeń noise
        float randomSeed = Random.Range(0f, 100f);

        // ROTACJA
        var noiseRotation = Mathf.PerlinNoise(spawnPosition.x + randomSeed, spawnPosition.y + randomSeed);
        var randomRotation = Mathf.Lerp(rotationRange.x, rotationRange.y, noiseRotation);
        instance.transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);

        // SCKALA/snoise
        var noiseScale = Mathf.PerlinNoise(spawnPosition.x + randomSeed, spawnPosition.y + randomSeed);
        var randomScale = Mathf.Lerp(scaleRange.x, scaleRange.y, noiseScale);
        instance.transform.localScale = Vector3.one * randomScale;
    }

    private void OnDrawGizmos()
    {
        // tylko dla podgladu wybieramy 1 z listy i rysujemy mesh 
        if (prefabs.Count > 0 && prefabs[0] != null)
        {
            MeshFilter meshFilter = prefabs[0].GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position, transform.rotation, transform.localScale);
            }
        }
    }
}
