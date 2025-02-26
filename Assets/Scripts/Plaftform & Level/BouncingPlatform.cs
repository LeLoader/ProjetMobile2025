using UnityEngine;

public class BouncingPlatform : MonoBehaviour
{
    public GameObject Player;
    public float minJumpHeight = 2f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool isTrackingJump = false;
    [SerializeField] private float jumpStartY = 0f;
    [SerializeField] private float maxJumpY = 0f;

    void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb.linearVelocity.y > 0)
        {
            if (!isTrackingJump)
            {
                isTrackingJump = true;
                jumpStartY = Player.transform.position.y;
                maxJumpY = jumpStartY;
            }
            else
            {
                float currentY = Player.transform.position.y;
                if (currentY > maxJumpY)
                {
                    maxJumpY = currentY;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ResetJumpData();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player && rb.linearVelocity.y <= 0)
        {
            float computedJumpHeight = maxJumpY - jumpStartY;
            float bounceHeight = Mathf.Max(computedJumpHeight, minJumpHeight);

            float bounceVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * bounceHeight);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceVelocity);

            ResetJumpData();
        }
    }

    void ResetJumpData()
    {
        isTrackingJump = false;
        jumpStartY = 0f;
        maxJumpY = 0f;
    }
}
