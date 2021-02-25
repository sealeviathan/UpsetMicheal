using UnityEngine.Audio;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public AudioClip clip;
    
    public string name;
    [Range(0,1)]
    public float volume = 0.5f;
    [Range(0,3f)]
    public float pitch = 1f;
    public AudioSource source;
    public bool loop;

}
