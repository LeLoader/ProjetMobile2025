using UnityEngine;

public class BouncingPlatform : MonoBehaviour
{
    public GameObject Player;
    public float minJumpHeight = 2f;

    private Rigidbody2D rb;

    private bool isTrackingJump = false;
    private float jumpStartY = 0f;
    private float maxJumpY = 0f;

    void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isTrackingJump && rb.linearVelocity.y > 0)
        {
            isTrackingJump = true;
            jumpStartY = Player.transform.position.y;
            maxJumpY = jumpStartY;
        }

        if (isTrackingJump)
        {
            float currentY = Player.transform.position.y;
            if (currentY > maxJumpY)
                maxJumpY = currentY;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            if (rb.linearVelocity.y <= 0)
            {
                float bounceHeight = minJumpHeight;

                if (isTrackingJump)
                {
                    float computedJumpHeight = maxJumpY - jumpStartY;
                    bounceHeight = (computedJumpHeight >= minJumpHeight) ? computedJumpHeight : minJumpHeight;

                    isTrackingJump = false;
                    jumpStartY = 0f;
                    maxJumpY = 0f;
                }

                float bounceVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * bounceHeight);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceVelocity);
            }
        }
    }
}
