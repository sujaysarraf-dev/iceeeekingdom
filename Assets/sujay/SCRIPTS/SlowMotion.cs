using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [Header("Settings")]
    public float slowFactor = 0.3f;
    public float smoothTransition = 0.5f;

    private bool isSlow = false;
    private float targetTimeScale = 1f;

    void Start()
    {
        Debug.Log("[SlowMotion] Script initialized. Press F to toggle slow motion.");
        Debug.Log("[SlowMotion] Current slowFactor: " + slowFactor);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isSlow = !isSlow;
            targetTimeScale = isSlow ? slowFactor : 1f;

            Debug.Log("[SlowMotion] Toggled! isSlow: " + isSlow + " | Target TimeScale: " + targetTimeScale);
        }

        // Smooth transition
        if (Mathf.Abs(Time.timeScale - targetTimeScale) > 0.01f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, smoothTransition * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else if (Time.timeScale != targetTimeScale)
        {
            Time.timeScale = targetTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Debug.Log("[SlowMotion] TimeScale set to: " + Time.timeScale);
        }
    }

    void OnDestroy()
    {
        // Reset time scale if script is destroyed while in slow motion
        if (Time.timeScale < 1f)
        {
            Debug.Log("[SlowMotion] Script destroyed, resetting TimeScale to 1");
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}