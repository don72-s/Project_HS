using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {  get; private set; }
    public AudioSource audioSource;
    public AudioClip[] bgmClips;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void PlayBGMScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Lobby":
                audioSource.clip = bgmClips[0];
                audioSource.volume = 0.8f;
                break;
            case "Room":
                audioSource.clip = bgmClips[1];
                audioSource.volume = 0.8f;
                break;
            case "Game":
                audioSource.clip = bgmClips[2];
                audioSource.volume = 0.2f;
                break;
            default:
                audioSource.clip = bgmClips[0];
                break;
        }
        audioSource.Play();
    }
}
