using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathScene : MonoBehaviour
{
    public float delayBeforeRedirect = 3f;

    void Start()
    {
        Debug.Log("[DeathScene] Player died! Redirecting to boss scene...");
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(RedirectToBossScene());
    }

    IEnumerator RedirectToBossScene()
    {
        yield return new WaitForSeconds(delayBeforeRedirect);
        Debug.Log("[DeathScene] Loading boss scene...");
        SceneManager.LoadScene("boss");
    }
}
