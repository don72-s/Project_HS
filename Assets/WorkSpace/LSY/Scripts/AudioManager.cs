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

    private void Start()
    {
        DataManager.Instance.OptionDataParams.bgmValueChanged += (val) => 
        { 
            GetComponent<AudioSource>().volume = val;
        };
    }

    public void PlayBGMScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Lobby":
                audioSource.clip = bgmClips[0];
                break;
            case "Room":
                audioSource.clip = bgmClips[1];
                break;
            case "Game":
                audioSource.clip = bgmClips[2];
                break;
            default:
                audioSource.clip = bgmClips[0];
                break;
        }
        audioSource.Play();
    }
}
