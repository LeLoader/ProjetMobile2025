using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordObject : WordBase
{
    [SerializeField] Collider2D coll;
    [SerializeField] bool ShouldWaitUntilGroundToApply;
    [SerializeField] float distanceCheck;
    [SerializeField] float applySpeed;
    public Vector3 TargetScale { get; set; }

    const int MAP_LAYER = 8;

    private void Start()
    {
        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);
        UpdateModifiers();
    }

    private void Update()
    {
        ApplyScale();
    }

    private void ApplyScale()
    {
        if ((ShouldWaitUntilGroundToApply && IsTouchingGround()) || !ShouldWaitUntilGroundToApply)
        {
            if (!IsTouchingTop() && !IsStuckOnSide())
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, TargetScale, applySpeed * Time.deltaTime);
            }
        }
    }

    private bool IsStuckOnSide()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.left * ((coll.bounds.size.x / 2) + distanceCheck), Color.blue);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * ((coll.bounds.size.x / 2) + distanceCheck), Color.blue);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, (coll.bounds.size.x / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYER));
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, (coll.bounds.size.x / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYER));
        return !(leftHit.collider == null && rightHit.collider == null);
    }

    private bool IsTouchingTop()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.up * ((coll.bounds.size.y / 2) + distanceCheck), Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYER));
        return (hit.collider != null);
    }

    private bool IsTouchingGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * ((coll.bounds.size.y / 2) + distanceCheck), Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYER));
        return (hit.collider != null);
    }

    public void Link(PlayerWord player)
    {
        LinkedWordBase = player;
        foreach(WordModifier modifier in currentModifiers)
        {
            modifier.WordUI.Link();
        }
    }

    public void Unlink() // Bind to move action when linked
    {
        LinkedWordBase = null;
        UpdateModifiers();
    }

    private void ResetObject()
    {
        transform.localScale = Vector3.one;
    }

    private void UpdateModifiers()
    {
        ResetObject();
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.Apply(this);
        }
    }

    [Button]
    private void ForceUpdateWord()
    {
        UpdateModifiers();
        UpdateWords(currentModifiers);
    }
}
