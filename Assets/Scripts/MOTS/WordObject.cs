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
    [SerializeField] Collider2D coll;
    [SerializeField] bool ShouldWaitUntilGroundToApply;
    [SerializeField] float distanceCheck;
    [SerializeField] float applySpeed;

    public Vector3 TargetScale { get; set; } = Vector3.one;
    protected Vector3 realTargetScale = Vector3.one;

    const int MAP_LAYERMASK = 8;

    bool wasStuckOnSide = false;
    bool wasStuckOnTop = false;

    Vector3 baseScale;

    private void Start()
    {
        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);
        UpdateModifiers();
    }

    private void FixedUpdate()
    {
        CheckStuck();
        ApplyScale();
    }

    private void CheckStuck()
    {
        if (!wasStuckOnSide && IsStuckOnSide())
        {
            wasStuckOnSide = true;
            CalculateNewTargetXScale();
        }

        if (!wasStuckOnTop && IsTouchingTop())
        {
            wasStuckOnTop = true;
            CalculateNewTargetYScale();
        }
    }

    private void CalculateNewTargetXScale()
    {
        float maxWidth = transform.localScale.x;
    }

    private void CalculateNewTargetYScale()
    {
        float maxHeight = transform.localScale.y;
    }

    private void ApplyScale()
    {
        if (LinkedWordBase) return;
        if (!ShouldWaitUntilGroundToApply || (ShouldWaitUntilGroundToApply && IsTouchingGround()))
        {
            realTargetScale = Vector3.one;
            foreach (ScaleModifier modifier in currentModifiers)
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


            transform.localScale = realTargetScale;
            //transform.localScale = Vector3.MoveTowards(transform.localScale, realTargetScale, applySpeed * Time.fixedDeltaTime);

            // Vector3 tempScale = Vector3.MoveTowards(transform.localScale, TargetScale, applySpeed * Time.fixedDeltaTime);
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
        baseScale = transform.localScale; 
    }

    private void UpdateModifiers()
    {
        ResetObject();
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.Apply(this);
            ((ScaleModifier)modifier).appliedTimer = 0;
        }
    }

    [Button]
    private void ForceUpdateWord()
    {
        UpdateModifiers();
        UpdateWords(currentModifiers);
    }
}
