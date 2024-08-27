using UnityEngine;

public class CameraCollisionController
{
    private Transform cameraTransform;
    private Transform cameraPivotTransform;
    private float cameraZPosition;
    private float targetCameraZPosition;
    private float cameraCollisionRadius;
    private LayerMask collideWithLayers;
    
    public float CameraZPosition => cameraZPosition;
    public float TargetCameraZPosition => targetCameraZPosition;

    public CameraCollisionController(Transform cameraTransform, Transform cameraPivotTransform, float cameraCollisionRadius, LayerMask collideWithLayers)
    {
        this.cameraTransform = cameraTransform;
        this.cameraPivotTransform = cameraPivotTransform;
        this.cameraCollisionRadius = cameraCollisionRadius;
        this.collideWithLayers = collideWithLayers;
        this.cameraZPosition = cameraTransform.localPosition.z;
    }

    public void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition; // ustawiamy domyślną pozycję Z kamery

        RaycastHit hit;
        // obliczam kierunek dla sprawdzenia kolizji, normalizuję go
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        // sprawdzam czy jest obiekt przed naszą kamerą w kierunku ruchu
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            // jeśli jest, obliczam dystans od obiektu i aktualizuję pozycję Z kamery
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // jeśli targetowa pozycja Z jest za blisko, resetujemy ją do minimalnej odległości
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        // finalnie, lerpujemy pozycję kamery, aby płynnie przesuwała się do nowej pozycji Z
        Vector3 cameraObjectPosition = cameraTransform.localPosition;
        cameraObjectPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraTransform.localPosition = cameraObjectPosition;
    }
    
    public void UpdateCollisionSettings(float newCollisionRadius, LayerMask newLayerMask)
    {
        // kolizja
        cameraCollisionRadius = newCollisionRadius;
        collideWithLayers = newLayerMask;
    }


    
    
    // TODO: dodac shader lub zanikanie jak obiekt jest zbyt blisko kamery
    // ewentualnie zrobić obiekty niewidzialne gdy kamera się odsuwa
    
}