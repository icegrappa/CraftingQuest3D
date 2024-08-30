using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    private PlayerManager _player;
    protected override void Awake()
    {
        base.Awake();

        _player = GetComponent<PlayerManager>();

    }

    public void PlayPickUpItemAnimation()
    {
        //animacja interakcji
        PlayActionAnimation("PickItem", true, false, false, false);
    }

    private void OnAnimatorMove()
    {
        // Jeśli gracz ma włączony Root Motion, to:
        if (_player.applyRootMotion)
        {
            // Przypisuję prędkość do wektora na podstawie deltaPosition z Animatora
            Vector3 velocity = _player.animator.deltaPosition;
            
            // Przesuwam postać zgodnie z prędkością uzyskaną z Animatora
            _player.characterController.Move(velocity);
            
            // Obracam gracza zgodnie z deltaRotation z Animatora
            // Zmieniam rotację gracza bezpośrednio, mnożąc obecną rotację przez deltaRotation
            _player.transform.rotation *= _player.animator.deltaRotation;
        }
    }


}
