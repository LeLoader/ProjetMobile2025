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
    [SerializeField] float interactionRadius = 5;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float speedForce = 5f;

    const int WORDOBJECT_LAYERMASK = 7;
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
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Use()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        List<Collider2D> results = new(); 
 
        if (Physics2D.OverlapCircle(transform.position, interactionRadius, contactFilter, results) > 0)
        {
            if (results[0].TryGetComponent<WordObject>(out WordObject wordObject)){
                Link(wordObject);
            };
        }
    }

    private void Link(WordObject wordObject)
    {
        wordObject.Link(this);
        LinkedWordBase = wordObject;
    }

    private void Unlink()
    {
        if (LinkedWordBase != null)
        {
            ((WordObject)LinkedWordBase).Unlink();
            LinkedWordBase = null;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, interactionRadius);
    }
}
