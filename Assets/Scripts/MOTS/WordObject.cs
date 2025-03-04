using NaughtyAttributes;
using System.Collections.Generic;
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
    [SerializeField] GameObject prefabUI;
    [SerializeField, ReadOnly] ObjectUI objectUI;

    public bool BlockIsSticky;
    public bool BlockIsBouncy;

    bool DuringSetup = true;

    public Vector3 TargetScale { get; set; } = Vector3.one;
    protected Vector3 realTargetScale = Vector3.one;

    const int MAP_LAYERMASK = 8;

    Collider2D coll;

    [Header("Empty")]
    [SerializeField] Sprite emptySprite;
    [SerializeField] BoxCollider2D defaultCollider;

    [Header("Scale")]
    [SerializeField] Sprite scaleSprite;

    [Header("Sticky")]
    [SerializeField] Sprite stickySprite;

    [Header("Bouncy")]
    [SerializeField] Sprite bouncySprite;

    [Header("Stairs")]
    [SerializeField] Sprite stairsSprite;
    [SerializeField] PolygonCollider2D stairsCollider;

    [Header("Ball")]
    [SerializeField] Sprite ballSprite;
    [SerializeField] CapsuleCollider2D ballCollider;

    private void Start()
    {
        coll = FindActiveCollider();
        ApplyAllModifiers();

        if (currentModifiers.Count == 0) // Only do setup if not already setup
        {
            WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
            UpdateWords(ref currentModifiers);
            UpdateModifiers();
            ApplySkin();
            CreateObjectUI();
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

    public void SetShape(WORDTYPE type)
    {
        if (!Application.IsPlaying(this))
        {
            coll = FindActiveCollider();
        }

        coll.enabled = false;
        //transform.rotation = Quaternion.identity;

        if (type.HasFlag(WORDTYPE.STAIRS))
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
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
        else if (type.HasFlag(WORDTYPE.BALL))
        {
            coll = ballCollider;
            rb.freezeRotation = false;
            rb.mass = 1f; // PARAM
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (type == WORDTYPE.NONE)
        {
            coll = defaultCollider;
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

    private void ApplySkin()
    {
        if (currentModifiers.Count == 0)
        {
            spriteRenderer.sprite = emptySprite;
            return;
        }

        foreach (WordModifier modifier in currentModifiers) // Stops when a non scale modifier is found
        {
            if (modifier is NonScaleModifier)
            {
                if (modifier is BouncyModifier)
                    spriteRenderer.sprite = bouncySprite;
                else if (modifier is StickyModifier)
                    spriteRenderer.sprite = stickySprite;
                else if (modifier is StairsModifier)
                    spriteRenderer.sprite = stairsSprite;
                else if (modifier is BallModifier)
                    spriteRenderer.sprite = ballSprite;

                break;
            }
            else
            {
                spriteRenderer.sprite = scaleSprite;
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

    protected override void UpdateWords(ref List<WordModifier> newModifiers)
    {
        CreateObjectUI();
        base.UpdateWords(ref newModifiers);
    }

    private void CreateObjectUI()
    {
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
            objectUI.positionConstraint.enabled = true;
            WordWrapper = objectUI.wrapper;
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
        if (!currentModifiers.Exists(mod => mod is ShapeModifier)) // If no shape modifier is found, then set shape to default using NONE
        {
            SetShape(WORDTYPE.NONE);
        }
        ApplySkin();
    }

    [Button]
    void DeleteAllChildOfWordObject()
    {
        WordObject[] wordObjects = FindObjectsByType<WordObject>(FindObjectsSortMode.None);

        foreach (WordObject wordObject in wordObjects)
        {
            if (!Application.IsPlaying(this))
            {
                for (int i = wordObject.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }
    }

    [Button]
    private void SetupObject()
    {
        if (!Application.IsPlaying(this))
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
        UpdateWords(ref currentModifiers);
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
