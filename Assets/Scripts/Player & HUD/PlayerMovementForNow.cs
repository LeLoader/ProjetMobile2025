using UnityEngine;

public class PlayerMovementForPC : MonoBehaviour
{
    public float move = 5f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(moveInput * move, rb.linearVelocity.y);
    }
}
