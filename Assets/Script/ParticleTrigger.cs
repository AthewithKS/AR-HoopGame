using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    public ParticleSystem particle0;
    public ParticleSystem particle1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            StartCoroutine(PlayParticle());
        }
    }
    IEnumerator PlayParticle()
    {
        particle0.Play();
        particle1.Play();
        yield return new WaitForSeconds(4f);
        particle0.Stop();
        particle1.Stop();
    }
}
