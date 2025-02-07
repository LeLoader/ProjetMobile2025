using NaughtyAttributes;
using NUnit.Framework;
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
    [SerializeField] Collider2D coll;
    [SerializeField] float distanceCheck;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float speedForce = 5f;

    int orientX = 1;

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
    }

    private bool IsTouchingGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * ((coll.bounds.size.y / 2) + distanceCheck), Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK));
        return (hit.collider != null);
    }


    private void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * speedForce);
            Unlink();

        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * speedForce);
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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * orientX, interactionDistance, (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK));
        
        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.right * orientX * interactionDistance, Color.green, 10f);
            if (hit.transform.TryGetComponent<WordObject>(out WordObject wordObject)){
                Link(wordObject);
            };
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.right * orientX * interactionDistance, Color.red, 10f);
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
