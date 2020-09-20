using System;
using UnityEngine;

/**
 * handles dynamic soundtrack and ambience
 * singleton pattern
 */
public class AudioManager : MonoBehaviour {
    // singleton
    public static AudioManager Instance;

    // sounds
    [SerializeField] private Sound[] sounds = null;

    private void Awake() {
        // enforce singleton
        if (Instance && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // init each audio source
        foreach (Sound s in sounds) {
            s.Init(gameObject.AddComponent<AudioSource>());
        }
    }

    private void Start() {
        Play("note");
        Play("wind");
    }

    // plays a specified audio clip
    public void Play(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s?.Play();
    }

    // stops a specified audio clip
    public void Stop(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s?.Pause();
    }
}