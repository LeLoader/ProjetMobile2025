using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlyAttribute = NaughtyAttributes.ReadOnlyAttribute;

public class PlayerWord : WordBase
{
    [Header("General")]
    [SerializeField] float interactionDistance = 5;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundCheckers;
    [SerializeField] Transform interactionCheckers;
    [SerializeField] Transform orientSign;
    [SerializeField] float distanceCheck;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float accelerationForce = 5f;
    [SerializeField] float maxSpeed = 3f;

    int orientX = 1;

    const int GROUND_LAYERMASK = 3;
    const int WORDOBJECT_LAYERMASK = 7;
    const int MAP_LAYERMASK = 8;
    ContactFilter2D contactFilter = new();

    private void Start()
    {
        contactFilter.layerMask = (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK);
        contactFilter.useLayerMask = true;

        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);
    }

    void Update()
    {
        Use();
        Move();
        Jump();
        UpdateOrientation();
    }

    private void UpdateOrientation()
    {
        if (orientX > 0)
        {
            orientSign.rotation = Quaternion.Euler(0, 0, -90);
        }
        if (orientX < 0)
        {
            orientSign.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    private bool IsTouchingGround()
    {
        for (int i = 0; i < groundCheckers.childCount; i++)
        {
            Transform t = groundCheckers.GetChild(i).transform;
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));
            if (hit.collider != null)
            {
                return true;
            }
        }
            
        return false;
    }


    private void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * accelerationForce);
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
            orientX = -1;
            Unlink();

        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * accelerationForce);
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
            orientX = 1;
            Unlink();
        }
    }

    private void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || !IsTouchingGround()) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Unlink();
    }

    private void Use()
    {
        if (!Input.GetKeyDown(KeyCode.E) || !IsTouchingGround()) return;

        for (int i = 0; i < interactionCheckers.childCount; i++)
        {
            Transform t = interactionCheckers.GetChild(i).transform;
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.right * orientX, interactionDistance, (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK));
            if (hit.collider != null)
            {
                if (hit.transform.TryGetComponent<WordObject>(out WordObject wordObject))
                {
                    Link(wordObject);
                    return;
                };
            }
        }
    }

    private void Link(WordObject wordObject)
    {
        wordObject.Link(this);
        LinkedWordBase = wordObject;
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.WordUI.Link();
        }
    }

    private void Unlink()
    {
        if (LinkedWordBase != null)
        {
            foreach (WordModifier modifier in currentModifiers)
            {
                modifier.WordUI.Unlink();
            }
            ((WordObject)LinkedWordBase).Unlink();
            LinkedWordBase = null;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawLine(transform.position, transform.position + interactionDistance * orientX * Vector3.right);
    }
}
