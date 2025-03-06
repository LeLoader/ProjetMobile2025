using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WordObject : WordBase
{
    [SerializeField] bool ShouldWaitUntilGroundToApply;
    [SerializeField] float distanceCheck;
    [SerializeField] float applySpeed;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject prefabUI;
    [SerializeField, ReadOnly, OnValueChanged("ObjectUIChanged")] ObjectUI objectUI;

    public bool BlockIsSticky;
    public bool BlockIsBouncy;

    bool DuringSetup = true;

    public Vector3 TargetScale { get; set; } = Vector3.one;
    protected Vector3 realTargetScale = Vector3.one;

    const int MAP_LAYERMASK = 8;

    Collider2D coll;

    [Header("Default")]
    [SerializeField] BoxCollider2D defaultCollider;

    [Header("Stairs")]
    [SerializeField] PolygonCollider2D stairsCollider;

    [Header("Ball")]
    [SerializeField] CapsuleCollider2D ballCollider;

    private void Awake()
    {
        Debug.Log(objectUI);
    }

    private void Start()
    {
        coll = FindActiveCollider();
        ApplyAllModifiers();

        CreateObjectUI(false); // Can work even if already setup

        if (currentModifiers.Count == 0 && wordType != 0) // Only do setup if not already setup
        {
            WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
            UpdateModifiers();
        }

        DuringSetup = false;
    }

    private void FixedUpdate()
    {
        ApplyScale();
    }

    private void ApplyAllModifiers()
    {
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.Apply(this);
        }
    }

    private Collider2D FindActiveCollider()
    {
        if (ballCollider.enabled == true)
            return ballCollider;
        else if (stairsCollider.enabled == true)
            return stairsCollider;
        else
            return defaultCollider;
    }

    private void SetShape()
    {
        if (!Application.IsPlaying(this))
        {
            coll = FindActiveCollider();
        }

        coll.enabled = false;
        //transform.rotation = Quaternion.identity;
        if (currentModifiers.Count == 0)
        {
            coll = defaultCollider;
            rb.freezeRotation = true;
            rb.mass = 10000f;
            transform.rotation = Quaternion.identity;
            objectUI.rotationConstraint.constraintActive = false;
            objectUI.transform.rotation = Quaternion.identity;
        }

        foreach (WordModifier modifier in currentModifiers)
        {
            if (modifier is StairsModifier)
            {
                coll = stairsCollider;
                rb.freezeRotation = true;
                rb.mass = 10000f;
                if (!DuringSetup)
                {
                    if (FindAnyObjectByType<PlayerWord>().xOrient < 0)
                    {
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        transform.rotation = Quaternion.identity;
                    }
                }
                objectUI.rotationConstraint.constraintActive = false;
                objectUI.transform.rotation = Quaternion.identity;
                break;
            }
            else if (modifier is BallModifier)
            {
                coll = ballCollider;
                rb.freezeRotation = false;
                rb.mass = 1f; // PARAM
                transform.rotation = Quaternion.identity;
                objectUI.rotationConstraint.constraintActive = true;
                break;
            }
            else
            {
                coll = defaultCollider;
                rb.freezeRotation = true;
                rb.mass = 10000f;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                objectUI.rotationConstraint.constraintActive = false;
                objectUI.transform.rotation = Quaternion.identity;

            }
        }

        coll.enabled = true;
    }

    private void ApplySkin()
    {
        if (currentModifiers.Count == 0)
        {
            spriteRenderer.sprite = spriteSettings.emptySprite;
            return;
        }

        foreach (WordModifier modifier in currentModifiers) // Stops when a non scale modifier is found
        {
            if (modifier is NonScaleModifier)
            {
                if (modifier is BouncyModifier)
                    spriteRenderer.sprite = spriteSettings.bouncySprite;
                else if (modifier is StickyModifier)
                    spriteRenderer.sprite = spriteSettings.stickySprite;
                else if (modifier is StairsModifier)
                    spriteRenderer.sprite = spriteSettings.stairsSprite;
                else if (modifier is BallModifier)
                    spriteRenderer.sprite = spriteSettings.ballSprite;

                break;
            }
            else
            {
                spriteRenderer.sprite = spriteSettings.scaleSprite;
            }
        }
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
        Debug.DrawLine(transform.position + new Vector3(-0.5f, 0, 0),  transform.position + Vector3.up + new Vector3(-0.5f, 0, 0) * ((coll.bounds.size.y / 2) + distanceCheck), Color.magenta);
        Debug.DrawLine(transform.position + new Vector3(0.5f, 0, 0), transform.position + Vector3.up + new Vector3(0.5f, 0, 0) * ((coll.bounds.size.y / 2) + distanceCheck), Color.blue);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.up, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK));
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0)  , Vector2.up, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK));
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), Vector2.up, (coll.bounds.size.y / 2) + distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK));
        return (hit1.collider != null || hit2.collider != null || hit3.collider != null);
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
            modifier.WordUI.enabled = true;
            modifier.WordUI.Link();
        }
    }

    public void Unlink() // Bind to move action when linked
    {
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.WordUI.Unlink();
            modifier.WordUI.enabled = false;
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

    protected override void UpdateUI(ref List<WordModifier> newModifiers)
    {
        base.UpdateUI(ref newModifiers);

        objectUI.gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
          
        //if (TargetScale.y >= TargetScale.x)
        //{
        //    objectUI.gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
        //}
        //else
        //{
        //    objectUI.gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        //}
    }

    private void CreateObjectUI(bool destructive)
    {
        if (objectUI != null && destructive)
        {
            if (!Application.IsPlaying(this))
            {
                DestroyImmediate(objectUI.gameObject);
            }
            else
            {
                Destroy(objectUI.gameObject);
            }
        }

        if (objectUI == null) 
        {
            objectUI = Instantiate(prefabUI).GetComponent<ObjectUI>();
            objectUI.gameObject.name = $"{gameObject.name}_UI";
            UnityEngine.Animations.ConstraintSource constraintSource = new()
            {
                sourceTransform = transform,
                weight = 1
            };
            objectUI.positionConstraint.AddSource(constraintSource);
            objectUI.transform.position = transform.position;
            objectUI.positionConstraint.constraintActive = true;
            objectUI.rotationConstraint.AddSource(constraintSource);
            objectUI.rotationConstraint.constraintActive = false;
            WordWrapper = objectUI.wrapper;
        }

        if (destructive)
        {
            UpdateModifiers();
        }
    }

    private void UpdateModifiers()
    {
        ResetObject();
        ApplyAllModifiers();
        foreach (WordModifier modifier in currentModifiers)
        {
            if (modifier is ScaleModifier scaleModifier) // Maybe use a reset method in WordModifier
            {
                scaleModifier.appliedTimer = 0;
            }
        }
        UpdateUI(ref currentModifiers);
        SetShape();
        ApplySkin();
    }

    [Button]
    private void SetupEveryObjectsOfTheLevel()
    {
        WordObject[] objs = FindObjectsByType<WordObject>(FindObjectsSortMode.None);
        foreach (WordObject obj in objs)
        {
            obj.SetupObject();
        }
    }

    [Button]
    private void SetupObject()
    {
        CreateObjectUI(true);

        if (!Application.IsPlaying(this)) // Clear old wrong children just in case
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        AddBaseModifier();
        ForceUpdateWord();
    }

    private void AddBaseModifier()
    {
        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        wordType = 0;
    }

    private void ForceUpdateWord()
    {
        UpdateModifiers();
        UpdateUI(ref currentModifiers);
        ApplySkin();
    }

    [Button]
    private void ForceScale()
    {
        Vector3 finalScale = Vector3.one;
        foreach (WordModifier modifier in currentModifiers)
        {
            if (modifier is ScaleModifier scaleModifier)
            {
                scaleModifier.appliedTimer = 1;
                finalScale.Scale(scaleModifier.GetScale());
            }
        }

        transform.localScale = finalScale;
    }

    private bool HasAStairs()
    {
        return currentModifiers.Exists(mod => mod is StairsModifier);
    }

    private bool HasAStickyOrBouncy()
    {
        return currentModifiers.Exists(mod => mod is EffectModifier);
    }

    [Button, EnableIf("HasAStairs")]
    private void ForceRotate()
    {
        transform.rotation = Quaternion.Euler(0, 180, 0) * transform.rotation;
    }

    [Button, EnableIf("HasAStickyOrBouncy")]
    private void ForceStuck()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
    }
}
