using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Rewindable : MonoBehaviour
{
    [System.Serializable]
    private struct State
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public float timestamp;
    }

    private List<State> states = new List<State>();
    private float maxRewindTime;
    private CharacterController charController;
    private Rigidbody rb;
    private MonoBehaviour[] scriptsToDisable;

    public void Init(float maxTime)
    {
        maxRewindTime = maxTime;
        charController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        scriptsToDisable = GetComponents<MonoBehaviour>()
            .Where(s => s != this && s != TimeRewind.Instance)
            .ToArray();
    }

    public void RecordState()
    {
        State state = new State
        {
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb != null ? rb.linearVelocity : Vector3.zero,
            timestamp = Time.unscaledTime
        };
        states.Add(state);

        // Remove old states beyond max rewind time
        while (states.Count > 0 && Time.unscaledTime - states[0].timestamp > maxRewindTime)
            states.RemoveAt(0);
    }

    public bool Rewind()
    {
        if (states.Count == 0) return false;

        State state = states[states.Count - 1];
        transform.position = state.position;
        transform.rotation = state.rotation;

        if (rb != null)
        {
            rb.isKinematic = true;
            // Don't set velocity when isKinematic = true (Unity 2023+ fix)
            // rb.velocity = state.velocity;
        }

        states.RemoveAt(states.Count - 1);
        return true;
    }

    public void StartRewind()
    {
        // Disable all other scripts on this object
        foreach (MonoBehaviour script in scriptsToDisable)
            if (script != null) script.enabled = false;

        if (charController != null)
            charController.enabled = false;
        if (rb != null)
            rb.isKinematic = true;
    }

    public void StopRewind()
    {
        // Re-enable disabled scripts
        foreach (MonoBehaviour script in scriptsToDisable)
            if (script != null) script.enabled = true;

        if (charController != null)
            charController.enabled = true;
        if (rb != null)
            rb.isKinematic = false;
    }

    void OnDestroy()
    {
        states.Clear();
    }
}
