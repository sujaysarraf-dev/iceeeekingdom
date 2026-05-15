using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    void Start()
    {
        StartCoroutine(RedirectToBoss());
    }

    IEnumerator RedirectToBoss()
    {
        for (int i = 5; i > 0; i--)
        {
            if (countdownText != null)
                countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("boss");
    }
}

