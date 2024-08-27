using UnityEngine;

public class CameraPositionController
{
    private Transform cameraTransform;
    private Transform playerTransform;
    private float cameraSmoothSpeed;
    private Vector3 cameraVelocity;

    public CameraPositionController(Transform cameraTransform, Transform playerTransform, float cameraSmoothSpeed)
    {
        this.cameraTransform = cameraTransform;
        this.playerTransform = playerTransform;
        this.cameraSmoothSpeed = cameraSmoothSpeed;
    }

    public void UpdateCameraPosition()
    {
        // Skaluje czasu wygładzania przez deltaTime dla spójności klatek (lepiej przed obliczaniem pozycji?)
        var smoothTime = cameraSmoothSpeed * Time.deltaTime;
        
        // Obliczanie docelowej pozycji kamery z płynnym przejściem w stronę pozycji gracza
        var targetCameraPosition = Vector3.SmoothDamp(
            cameraTransform.position,
            playerTransform.position,
            ref cameraVelocity,
            smoothTime
        );
        
        // Aktualizuje transform kamery dla nowej pozycji
        cameraTransform.position = targetCameraPosition;
    }
    
    public void UpdatePositionSettings(float newSmoothSpeed)
    {
        cameraSmoothSpeed = newSmoothSpeed;
    }


    public Vector3 CameraVelocity => cameraVelocity;  // aktualna predkosc kamery
}