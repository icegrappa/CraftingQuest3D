using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
   [SerializeField] public AudioSource playerSource;
   [SerializeField] public AudioClip sucessSound;
   [SerializeField] public AudioClip failureSound;
    
    protected virtual void Awake()
    {
        playerSource = GetComponent<AudioSource>();
    }
    
    public void PlaySucessSound()
    {
        playerSource.PlayOneShot(sucessSound);
    }
    
    public void PlayFailureSound()
    {
        playerSource.PlayOneShot(failureSound);
    }

}
