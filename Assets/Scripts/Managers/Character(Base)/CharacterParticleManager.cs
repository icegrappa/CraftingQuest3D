using System.Collections;
using UnityEngine;

public class CharacterParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject successParticlePrefab;
    [SerializeField] private GameObject failureParticlePrefab;

    public void PlaySuccessParticle()
    {
        StartCoroutine(PlayParticleAndDestroy(successParticlePrefab));
    }
    
    public void PlayFailureParticle()
    {
        StartCoroutine(PlayParticleAndDestroy(failureParticlePrefab));
    }

    private IEnumerator PlayParticleAndDestroy(GameObject particlePrefab)
    {
        if (particlePrefab != null)
        {
            var instantiatedParticle = Instantiate(particlePrefab, transform.position, Quaternion.identity);


            var particleSystem = instantiatedParticle.GetComponent<ParticleSystem>();
            if (particleSystem != null)
                particleSystem.Play();
           

            yield return new WaitForSeconds(2f);

            Destroy(instantiatedParticle);
        }
       
    }
}