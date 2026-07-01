using UnityEngine;

public class DeathExplosion : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        gameObject.SetActive(false);   // hide at start
    }

    public void PlayExplosion(Vector3 position)
    {
        transform.position = position; // move explosion to player
        gameObject.SetActive(true);    // show explosion
        anim.Play("DeathGif");         // play animation clip
    }
}
