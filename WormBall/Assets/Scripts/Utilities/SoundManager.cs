using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SingletonBehaviour<SoundManager> singleton = new SingletonBehaviour<SoundManager>();

    // Sets many sfx or songs could potentially be playing at once
    [SerializeField]
    int soundPoolSize = 3;
    [SerializeField]
    int songPoolSize = 2;

    // Put all clips in these then you'll be able to refer to them by name
    [SerializeField]
    AudioClip[] sounds;
    [SerializeField]
    AudioClip[] songs;

    string soundTag = "SoundSource";
    string songTag = "SongSource";

    GameObject sounderHolder;
    GameObject songerHolder;

    GameObject[] sounders;
    GameObject[] songers;

    int currentSounder = 0;
    int currentSonger = 0;

    Dictionary<string, AudioClip> soundList = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> songList = new Dictionary<string, AudioClip>();

	[SerializeField] bool destroyOnLoad;

    // Use this for initialization
    void Awake()
    {
		if(!destroyOnLoad)
		{
        	singleton.DontDestroyElseKill(this);
		}

        // Setup dictionaries
        foreach (AudioClip clip in sounds)
            soundList.Add(clip.name, clip);

        foreach (AudioClip clip in songs)
            songList.Add(clip.name, clip);

        //Set up AudioSource pools
        SetupSounderPool();
        SetupSongerPool();
    }

    void SetupSounderPool()
    {
        // Setup sounder sounder pool
        sounders = new GameObject[soundPoolSize];

        // Create object to hold sounders
        sounderHolder = new GameObject("Sounders");
        sounderHolder.transform.parent = transform;

        //Create sounders and put them in a pool
        for (int i = 0; i < soundPoolSize; i++)
        {
            sounders[i] = new GameObject("Sounder" + i);
            sounders[i].AddComponent<AudioSource>();
            sounders[i].GetComponent<AudioSource>().playOnAwake = false;
            sounders[i].transform.parent = sounderHolder.transform;
        }
    }

    void SetupSongerPool()
    {
        // Setup sounder songer pool
        songers = new GameObject[songPoolSize];

        // Create obj to hold songers
        songerHolder = new GameObject("Songers");
        songerHolder.transform.parent = transform;

        // Create songers and put them in the pool
        for (int i = 0; i < songPoolSize; i++)
        {
            songers[i] = new GameObject("Songer" + i);
            songers[i].AddComponent<AudioSource>();
            songers[i].GetComponent<AudioSource>().playOnAwake = false;
            songers[i].transform.parent = songerHolder.transform;
        }
    }

    public AudioSource PlaySoundAtPosition(string clip, Vector3 position, float volume = 1.0f, bool loop = false)
    {
        // Put at location
        sounders[currentSounder].transform.position = position;

        // Set up audio source
        return SetupSounder(clip, volume, loop);
    }

    public AudioSource PlaySoundAndFollow(string clip, Transform target, float volume = 1.0f, bool loop = false)
    {
        // Attach to target
        sounders[currentSounder].transform.position = target.position;
        sounders[currentSounder].transform.parent = target;

        // Set up audio source
        return SetupSounder(clip, volume, loop);
    }

    public AudioSource Play2DSong(string clip, float volume = 1.0f, bool loop = false)
    {
        // Set up audio source
        return SetupSonger(clip, volume, loop);
    }

    public AudioSource PlaySongAtPosition(string clip, Vector3 position, float volume = 1.0f, bool loop = false)
    {
        // Put at location
        songers[currentSonger].transform.position = position;

        // Set up audio source
        return SetupSonger(clip, volume, loop);
    }

    public AudioSource PlaySongAndFollow(string clip, Transform target, float volume = 1.0f, bool loop = false)
    {
        // Attach to target
        songers[currentSonger].transform.position = target.position;
        songers[currentSonger].transform.parent = target;

        // Set up audio source
        return SetupSonger(clip, volume, loop);
    }

    public void KillAllSounders(bool resetSounderPool)
    {
        for (int i = 0; i < sounders.Length; i++)
        {
            GameObject sounder = sounders[i];
            sounders[i] = null;
            Destroy(sounder);
        }

        if (resetSounderPool)
            SetupSounderPool();

    }

    public void KillAllSongers(bool resetSongerPool)
    {
        for (int i = 0; i < songers.Length; i++)
        {
            GameObject songer = songers[i];
            songers[i] = null;
            Destroy(songer);
        }

        if (resetSongerPool)
            SetupSongerPool();
    }

    AudioSource SetupSounder(string clip, float volume, bool loop = false)
    {
        AudioSource sounder = sounders[currentSounder].GetComponent<AudioSource>();

        // Set up audio source
        sounder.clip = soundList[clip];
        sounder.volume = volume;
        sounder.loop = loop;
        sounder.Play();

        // Tag object as sound
        sounders[currentSounder].tag = soundTag;

        // If this sound will loop forever, then replace it in the pool
        if (loop)
        {
            sounders[currentSounder].transform.parent = null;
            sounders[currentSounder].name = clip + "Sounder";

            sounders[currentSounder] = new GameObject("Sounder" + currentSounder);
            sounders[currentSounder].AddComponent("AudioSource");
            sounders[currentSounder].GetComponent<AudioSource>().playOnAwake = false;
            sounders[currentSounder].transform.parent = sounderHolder.transform;
        }
        else
            StartCoroutine(ReturnSourceToPool(sounders[currentSounder].transform, sounderHolder.transform, clip.Length));

        // Go to next sounder
        IncrementSounder();

        return sounder;
    }

    AudioSource SetupSonger(string clip, float volume, bool loop = false)
    {
        AudioSource songer = songers[currentSonger].GetComponent<AudioSource>();

        // Set up audio source
        songer.clip = songList[clip];
        songer.volume = volume;
        songer.loop = loop;
        songer.Play();

        // Tag object as song
        songers[currentSonger].tag = songTag;

        // If this sound will loop forever, then replace it in the pool
        if (loop)
        {
            songers[currentSonger].transform.parent = null;
            songers[currentSonger].name = clip + "Songer";

            songers[currentSonger] = new GameObject("Songer" + currentSonger);
            songers[currentSonger].AddComponent("AudioSource");
            songers[currentSonger].GetComponent<AudioSource>().playOnAwake = false;
            songers[currentSonger].transform.parent = songerHolder.transform;
        }
        else
            StartCoroutine(ReturnSourceToPool(songers[currentSonger].transform, songerHolder.transform, clip.Length));

        // Go to next sounder
        IncrementSonger();

        return songer;
    }

    void IncrementSounder()
    {
        currentSounder++;

        if (currentSounder >= sounders.Length)
            currentSounder = 0;
    }

    void IncrementSonger()
    {
        currentSonger++;

        if (currentSonger >= songers.Length)
            currentSonger = 0;
    }

    IEnumerator ReturnSourceToPool(Transform source, Transform parent, float time)
    {
        yield return new WaitForSeconds(time);

        source.position = parent.position;
        source.parent = parent;
    }
}