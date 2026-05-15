using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TimeRewind : MonoBehaviour
{
    [Header("Settings")]
    public float maxRewindTime = 5f;
    public KeyCode rewindKey = KeyCode.R;
    public string rewindableTag = "Rewindable";

    private bool isRewinding = false;
    private List<Rewindable> rewindables = new List<Rewindable>();

    public static TimeRewind Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[TimeRewind] Manager initialized. Max rewind: " + maxRewindTime + "s. Press " + rewindKey + " to toggle.");
    }

    void Start()
    {
        RefreshRewindableList();
        Debug.Log("[TimeRewind] Found " + rewindables.Count + " rewindable objects.");
    }

    void Update()
    {
        // DISABLED - was causing physics errors
        return;
        
        if (Input.GetKeyDown(rewindKey))
        {
            if (!isRewinding) StartRewind();
            else StopRewind();
        }

        if (!isRewinding)
        {
            foreach (Rewindable rewindable in rewindables)
                rewindable?.RecordState();
        }
        else
        {
            bool anyRewinding = false;
            foreach (Rewindable rewindable in rewindables)
                if (rewindable?.Rewind() == true) anyRewinding = true;

            if (!anyRewinding) StopRewind();
        }
    }

    void RefreshRewindableList()
    {
        rewindables.Clear();
        GameObject[] rewindableObjects = GameObject.FindGameObjectsWithTag(rewindableTag);
        foreach (GameObject obj in rewindableObjects)
        {
            Rewindable rewindable = obj.GetComponent<Rewindable>();
            if (rewindable == null)
                rewindable = obj.AddComponent<Rewindable>();
            rewindable.Init(maxRewindTime);
            rewindables.Add(rewindable);
        }
    }

    void StartRewind()
    {
        if (rewindables.Count == 0)
        {
            Debug.LogWarning("[TimeRewind] No rewindable objects! Tag objects with '" + rewindableTag + "'.");
            return;
        }

        isRewinding = true;
        foreach (Rewindable rewindable in rewindables)
            rewindable?.StartRewind();

        Debug.Log("[TimeRewind] Rewind started. Objects: " + rewindables.Count);
    }

    void StopRewind()
    {
        isRewinding = false;
        foreach (Rewindable rewindable in rewindables)
            rewindable?.StopRewind();

        Debug.Log("[TimeRewind] Rewind stopped.");
    }

    void OnDestroy()
    {
        rewindables.Clear();
    }
}
