using UnityEngine;

public class CameraRotationController
{
    private Transform cameraTransform;
    private Transform cameraPivot;
    private float leftAndRightRotationSpeed;
    private float upAndDownRotationSpeed;
    private float minimumPivot;
    private float maximumPivot;
    private float leftAndRightLookAngle;
    private float upAndDownLookAngle;

    public CameraRotationController(Transform cameraTransform, Transform cameraPivot, float leftAndRightRotationSpeed, float upAndDownRotationSpeed, float minimumPivot, float maximumPivot)
    {
        this.cameraTransform = cameraTransform;
        this.cameraPivot = cameraPivot;
        this.leftAndRightRotationSpeed = leftAndRightRotationSpeed;
        this.upAndDownRotationSpeed = upAndDownRotationSpeed;
        this.minimumPivot = minimumPivot;
        this.maximumPivot = maximumPivot;
    }

    public void UpdateCameraRotation()
    {
        // Obracamy kamerę w lewo/prawo na podstawie inputu
        leftAndRightLookAngle += PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed * Time.deltaTime;
    
        // Obracamy kamerę w gore/dol na podstawie inputu
        upAndDownLookAngle -= PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed * Time.deltaTime;
    
        // Ograniczamy kat obrotu kamery żeby nie przekroczyć max/min
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        // Ustawiamy rotacje kamery
        cameraTransform.rotation = Quaternion.Euler(upAndDownLookAngle, leftAndRightLookAngle, 0f);

        // Jeśli jest pivot ustawiamy jego rotację
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(upAndDownLookAngle, 0f, 0f);
    }
    
    public void UpdateRotationSettings(float newLeftAndRightRotationSpeed, float newUpAndDownRotationSpeed, float newMinimumPivot, float newMaximumPivot)
    {
        leftAndRightRotationSpeed = newLeftAndRightRotationSpeed;
        upAndDownRotationSpeed = newUpAndDownRotationSpeed;
        minimumPivot = newMinimumPivot;
        maximumPivot = newMaximumPivot;
    }



    public float LeftAndRightLookAngle => leftAndRightLookAngle;  // aktulny kat lewo prawo
    public float UpAndDownLookAngle => upAndDownLookAngle;  // aktulny kat gora dol
}