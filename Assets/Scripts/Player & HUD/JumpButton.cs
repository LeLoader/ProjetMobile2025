using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JumpButton : MonoBehaviour
{
    public GameObject Player;
    public Transform groundCheck;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    private Rigidbody2D rb;

    public Button jumpButton;

    void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
        jumpButton.onClick.AddListener(TaskOnClick);
    }

    public void TaskOnClick()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
