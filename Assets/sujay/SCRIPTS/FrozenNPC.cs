using UnityEngine;

public class FrozenNPC : MonoBehaviour
{
    public string npcName = "Frozen Villager";
    public int rescueScore = 50;
    public GameObject iceBlock;

    private bool isThawed = false;
    private Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
        if (anim == null)
        {
            anim = gameObject.AddComponent<Animation>();
        }

        // Load talking animation from correct path
        AnimationClip clip = Resources.Load<AnimationClip>("source/character/animations/talking(1)");
        if (clip != null)
        {
            anim.AddClip(clip, "talking");
        }

        if (iceBlock != null)
        {
            iceBlock.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isThawed) return;

        if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            Thaw();
        }
    }

    void Thaw()
    {
        isThawed = true;

        if (iceBlock != null)
        {
            iceBlock.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isThawed) return;

        if (other.GetComponent<Player>() != null)
        {
            Rescue();
        }
    }

    void Rescue()
    {
        // Play talking animation
        if (anim != null && anim.GetClip("talking") != null)
        {
            anim.Play("talking");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(rescueScore);
        }

        Destroy(gameObject, 3f);
    }
}
