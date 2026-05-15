using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu UI")]
    public GameObject menuCanvas;

    [Header("Audio")]
    public AudioClip bgMusic; // Background music (looping)

    [Header("Scene")]
    public string gameSceneName = "Play";

    private AudioSource audioSource;

    void Start()
    {
        // Show cursor in menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }

        // Setup background music
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (bgMusic != null)
        {
            audioSource.clip = bgMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = true;
            audioSource.Play();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
