using Unity.VisualScripting;
using UnityEngine;

public class BouncingPlatform : MonoBehaviour
{
    public GameObject Player;
    public float jumpForce = 5f;
    private Rigidbody2D rb;

    public void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player.SetActive(true);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}
