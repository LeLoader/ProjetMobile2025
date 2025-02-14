using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WordObject : WordBase
{
    [SerializeField] bool ShouldWaitUntilGroundToApply;
    [SerializeField] float distanceCheck;
    [SerializeField] float applySpeed;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;

    public bool BlockIsSticky;
    public bool BlockIsBouncy;
   
    public Vector3 TargetScale { get; set; } = Vector3.one;
    protected Vector3 realTargetScale = Vector3.one;

    const int MAP_LAYERMASK = 8;

    Collider2D coll;

    [Header("Default")]
    [SerializeField] Sprite defaultSprite;
    [SerializeField] BoxCollider2D defaultCollider;
    [Header("Stairs")]
    [SerializeField] Sprite stairsSprite;
    [SerializeField] PolygonCollider2D stairsCollider;
    [Header("Ball")]
    [SerializeField] Sprite ballSprite;
    [SerializeField] CapsuleCollider2D ballCollider;

    private void Start()
    {
        coll = defaultCollider;
        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);
        UpdateModifiers();
    }

    private void FixedUpdate()
    {
        ApplyScale();
    }

    public void SetShape(WORDTYPE type)
    {
        coll.enabled = false;
        transform.rotation = Quaternion.identity;

        if (type.HasFlag(WORDTYPE.STAIRS))
        {
            coll = stairsCollider;
            spriteRenderer.sprite = stairsSprite;
            rb.freezeRotation = true;
            rb.mass = 10000f;
            if (FindAnyObjectByType<PlayerWord>().xOrient < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else if (type.HasFlag(WORDTYPE.BALL))
        {
            coll = ballCollider;
            spriteRenderer.sprite = ballSprite;
            rb.freezeRotation = false;
            rb.mass = 1f; // PARAM
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (type == WORDTYPE.NONE)
        {
            coll = defaultCollider;
            spriteRenderer.sprite = defaultSprite;
            rb.freezeRotation = true;
            rb.mass = 10000f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            Debug.LogWarning("Wrong shape type passed");
        }

        coll.enabled = true;
    }

    private void ApplyScale()
    {
        if (LinkedWordBase) return;
        if (!ShouldWaitUntilGroundToApply || (ShouldWaitUntilGroundToApply && IsTouchingGround()))
        {
            realTargetScale = Vector3.one;
            foreach (WordModifier wordModifier in currentModifiers)
            {
                if (wordModifier is ScaleModifier modifier)
                {
                    if (IsTouchingTop() && (modifier.IsGreatScaleY() /*|| TargetScale.y > 1*/))
                    {

                    }
                    else if (IsStuckOnSide() && (modifier.IsGreatScaleX() /*|| TargetScale.x > 1*/))
                    {

                    }
                    else
                    {
                        modifier.appliedTimer += Time.fixedDeltaTime;
                    }

                    realTargetScale.Scale(Vector3.Lerp(Vector3.one, modifier.GetScale(), modifier.appliedTimer));
                } 
            }

            if (TargetScale.x <= transform.localScale.x && TargetScale.y <= transform.localScale.y)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, TargetScale, Time.fixedDeltaTime);
            }
            else
            {
                transform.localScale = realTargetScale;
            }
        }
    }

    private bool IsStuckOnSide()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.left * ((coll.bounds.size.x / 2) + distanceCheck), Color.blue);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * ((coll.bounds.size.x / 2) + distanceCheck), Color.blue);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, (coll.bounds.size.x / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK));
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, (coll.bounds.size.x / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK));
        return !(leftHit.collider == null || rightHit.collider == null);
    }

    private bool IsTouchingTop()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.up * ((coll.bounds.size.y / 2) + distanceCheck), Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK));
        return (hit.collider != null);
    }

    private bool IsTouchingGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * ((coll.bounds.size.y / 2) + distanceCheck), Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK));
        return (hit.collider != null);
    }

    public void Link(PlayerWord player)
    {
        LinkedWordBase = player;
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.WordUI.Link();
        }
    }

    public void Unlink() // Bind to move action when linked
    {
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.WordUI.Unlink();
        }
        LinkedWordBase = null;
        UpdateModifiers();
    }

    private void ResetObject()
    {
        TargetScale = Vector3.one;
        BlockIsBouncy = false;
        BlockIsSticky = false;
    }

    private void UpdateModifiers()
    {
        ResetObject();
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.Apply(this);
            if (modifier is ScaleModifier scaleModifier) // Maybe use a reset method in WordModifier
            {
                scaleModifier.appliedTimer = 0;
            }
        }
        if (!currentModifiers.Exists(mod => mod is ShapeModifier)) // If no shape modifier is found, then set shape to default using NONE
        {
            SetShape(WORDTYPE.NONE);
        }
    }

    [Button]
    private void ForceUpdateWord()
    {
        UpdateModifiers();
        UpdateWords(currentModifiers);
    }
}
