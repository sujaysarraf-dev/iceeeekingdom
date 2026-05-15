using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersist : MonoBehaviour
{
    private static PlayerPersist instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = null;

        if (scene.name == "dungeon1")
        {
            spawn = GameObject.Find("DungeonSpawn");
        }
        else if (scene.name == "kingdom")
        {
            spawn = GameObject.Find("IceSpawn");
        }

        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }
}