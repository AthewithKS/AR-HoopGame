using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum audio
{
    BallGroundPich,BallHoophit,ScorerSound
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] auidoClip;
    private void Awake()
    {
        instance = this;
    }
    public static void PlayAudio(audio clip)
    {
        instance.audioSource.PlayOneShot(instance.auidoClip[(int)clip], Random.Range(0.8f, 1.2f)); // Vary pitch slightly for realism
    }

}
