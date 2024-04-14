using UnityEngine;
using UnityEngine.UIElements;

public class soundController : MonoBehaviour
{
    public AudioClip[] click = new AudioClip[4];
    public AudioClip[] snowStep = new AudioClip[3];
    public AudioClip theClick;
    public AudioClip deathSound;

    public AudioSource soundSource;
    public AudioSource backgroundMusic;

    public void playRandomClick() {
        int randomClick = (int)Random.Range(0, 3);
        soundSource.clip = click[randomClick];
        soundSource.Play();
    }
    public void playTheClick() {
        soundSource.clip = theClick;
        soundSource.Play();
    }
    public void playRandomStep()
    {
        int randomStep = (int)Random.Range(0, 3);
        soundSource.clip = snowStep[randomStep];
        soundSource.Play();
    }
    public void playDeathSound() {
        soundSource.clip = deathSound;
        soundSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundMusic.pitch = Random.Range(0.2f, 1f);
        backgroundMusic.Play();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
