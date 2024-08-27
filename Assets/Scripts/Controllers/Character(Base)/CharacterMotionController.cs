using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotionController : MonoBehaviour
{
    private CharacterManager _character; 

    [Header("Ustawienia Grawitacji")]
    [SerializeField] protected float gravityForce = -30f;
    [SerializeField] protected LayerMask groundLayer; // Layer dla isGrounding 
    [SerializeField] private float groundCheckSphereRadius = 1f; // Promień sfery, która sprawdza, czy postać stoi na ziemi
    [SerializeField] protected Vector3 yVelocity; // Szybkość ruchu postaci w osi Y (skoki lub spadanie)
    [SerializeField] private float groundedYVelocity = -20f; // Szybkość utrzymywania postaci na ziemi
    [SerializeField] private float fallStartVelocity = -5f; // Początkowa prędkość spadania po oderwaniu od ziemi / zwieksza sie wrac z time
    private bool fallingVelocityHasBeenSet = false; // Czy prędkość spadania została ustawiona??
    private float inAirTimer = 0f; // Czas spędzony w powietrzu?

    protected virtual void Awake()
    {
        _character = GetComponent<CharacterManager>(); 
    }

    protected virtual void Update()
    {
        HandleGroundCheck();
        HandleGroundedMovement();
        ApplyGravityAndMoveCharacter();
    }
    
    private void HandleGroundedMovement()
    {
        if (_character.isGrounded)
        {
            ResetInAirState();
        }
        else
        {
            HandleAirborneState();
        }
    }

    private void ResetInAirState()
    {
        // Jeśli nie próbujemy skakać ani poruszać się w górę
        if (yVelocity.y < 0)
        {
            inAirTimer = 0; // Resetujemy licznik czasu w powietrzu
            fallingVelocityHasBeenSet = false; // Resetujemy flagę ustawienia prędkości spadania
            yVelocity.y = groundedYVelocity; // Ustawiamy prędkość Y na wartość dla postaci uziemionej
        }
    }

    private void HandleAirborneState()
    {
        // Jeśli nie skaczemy i prędkość spadania nie została jeszcze ustawiona
        if (!_character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
        {
            fallingVelocityHasBeenSet = true; // Ustawiamy flagę, że prędkość spadania została ustawiona
            yVelocity.y = fallStartVelocity; // Ustawiamy początkową prędkość spadania
        }

        // Zwiększamy licznik czasu w powietrzu
        inAirTimer += Time.deltaTime;
        _character.animator.SetFloat(_character.characterAnimatorManager._inAirTimer, inAirTimer); // Przekazujemy czas w powietrzu do animacji
    }

    private void ApplyGravityAndMoveCharacter()
    {
        // Dodajemy siłę grawitacji do prędkości Y
        yVelocity.y += gravityForce * Time.deltaTime;

        // Przemieszczamy postać według zaktualizowanej prędkości Y
        _character.characterController.Move(yVelocity * Time.deltaTime);
    }

    protected virtual void HandleGroundCheck()
    {
        // Sprawdzamy, czy postać jest na ziemi, wykorzystując sferę kolizji
        _character.isGrounded = Physics.CheckSphere(_character.transform.position, groundCheckSphereRadius, groundLayer);
    }

    // debug draw
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(_character.transform.position, groundCheckSphereRadius); 
    }
}