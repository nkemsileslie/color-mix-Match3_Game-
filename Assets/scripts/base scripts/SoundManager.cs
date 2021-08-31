using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public void PlayRandomDestroyNoise()
    {
        //Choose a random number
        int ClipToPlay = Random.Range(0, destroyNoise.Length);
        //Play the clip
        destroyNoise[ClipToPlay].Play();
    }
   
   
}
