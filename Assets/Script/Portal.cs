using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad = "dungeon1"; // EXACT scene name

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered portal: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER detected → loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("NOT player");
        }
    }
}