using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;
    public static AudioManager instance;
    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach(Sound sound in Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        Play("Song1");
    }

    // Update is called once per frame
    public void Play(string soundName)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == soundName);
        if(s != null)
        {
            s.source.Play();
        }
    }
    public void PlayRandomFromList(String[] soundList)
    {
        int ranChoice = UnityEngine.Random.Range(0, soundList.Length);
        string soundToPlay = soundList[ranChoice];
        Play(soundToPlay);
    }
    public void Pause(string soundName)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == soundName);
        if(s != null)
        {
            s.source.Stop();
        }
    }
    public Sound GetSound(string soundName)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == soundName);
        if(s != null)
        {
            return s;
        }
        else
        {
            Debug.LogError("No sound by name " + soundName);
            return null;
        }
    }
    public void DelayPlay(string soundName, float delay)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == soundName);
        if(s != null)
        {
            s.source.PlayDelayed(delay);
        }
    }
}
