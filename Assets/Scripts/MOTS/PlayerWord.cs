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
using Unity.Cinemachine;
using System.Collections;
using Unity.Android.Gradle;
using Unity.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine.InputSystem;
using Unity.Mathematics.Geometry;

public class PlayerWord : WordBase
{
    [Header("General")]
    [SerializeField] float interactionDistance = 5;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D groundChecker;
    [SerializeField] Transform leftCheckers;
    [SerializeField] Transform rightCheckers;
    [SerializeField] Transform topCheckers;
    [SerializeField] Transform interactionCheckers;
    [SerializeField] SpriteRenderer orientSign;
    [SerializeField] float distanceCheck;
    [SerializeField] float scale = 1f;


    [SerializeField] public CinemachineCamera _camera;
    [SerializeField] float duration = 2f;

    [Header("Action")]
    [SerializeField] InputActionReference jumpAction;
    [SerializeField] InputActionReference moveAction;

    [Header("Movement")]
    [SerializeField, ReadOnly] bool HeadIsStick;
    [SerializeField, ReadOnly] bool IsStick;
    [SerializeField, ReadOnly] bool CanMove;
    [SerializeField, ReadOnly] bool OnSlope;
    [SerializeField]
    public float AccelerationForce
    {
        get
        {
            return GetAccelerationForce();
        }
    }
    [SerializeField]
    public float MaxSpeed
    {
        get
        {
            return GetMaxSpeed();
        }
    }
    [SerializeField]
    public float JumpHeight
    {
        get
        {
            return GetJumpHeight();
        }
    }

    [Header("Movement | Default")]
    [SerializeField, Tooltip("m/s²")] float defaultAccelerationForce = 150f;
    [SerializeField, Tooltip("m/s²")] float defaultDecelerationForce = 75f;
    [SerializeField, Tooltip("m/s")] float defaultMaxSpeed = 5f;
    [SerializeField, Tooltip("m")] float defaultJumpHeight = 1.25f;

    [Header("Movement | Sticked")]
    [SerializeField, Tooltip("m/s²")] float stickedAccelerationForce = 50f;
    [SerializeField, Tooltip("m/s²")] float stickedDecelerationForce = 75f;
    [SerializeField, Tooltip("m/s")] float stickedMaxSpeed = 3f;
    [SerializeField, Tooltip("m")] float stickedJumpHeight = 1f;

    [Header("Movement | Bouncy")]
    [SerializeField, Tooltip("m")] float bouncyJumpHeight = 2f;

    public int xOrient = 1;
    float xInput = 0;

    const int GROUND_LAYERMASK = 3;
    const int WORDOBJECT_LAYERMASK = 7;
    const int MAP_LAYERMASK = 8;
    ContactFilter2D contactFilter = new();

    [Header("Camera")]
    [SerializeField] float _cameraUnlink = 7f;
    float _currentCamera;
    [SerializeField] float _cameraLink = 3f;

    private void Start()
    {

        contactFilter.layerMask = (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK);
        contactFilter.useLayerMask = true;
        CanMove = true;

        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);

        jumpAction.action.started += Jump;
        moveAction.action.performed += GetInput;
        moveAction.action.canceled += GetInput;
    }

    void FixedUpdate()
    {
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, 0, defaultDecelerationForce * Time.fixedDeltaTime); // Method
        Use();
        Move();
        UpdateOrientation();
        IsStick = PlayerIsOnSticky();
        HeadIsStick = HeadIsSticky();
        UpdateGravity();
    }

    private float GetAccelerationForce()
    {
        if (IsStick)
        {
            return stickedAccelerationForce;
        }
        else
        {
            return defaultAccelerationForce;
        }
    }

    private float GetMaxSpeed()
    {
        if (IsStick)
        {
            return stickedMaxSpeed;
        }
        else
        {
            return defaultMaxSpeed;
        }
    }

    private float GetJumpHeight()
    {
        if (false) // if on bouncy
        {
            return bouncyJumpHeight;
        }
        else if (IsStick)
        {
            return stickedJumpHeight;
        }
        else
        {
            return defaultJumpHeight;
        }
    }

    private void UpdateOrientation()
    {
        if (xOrient > 0)
        {
            orientSign.flipY = false;
        }
        if (xOrient < 0)
        {
            orientSign.flipY = true;
        }
    }

    private bool IsTouchingGround()
    {
        /*for (int i = 0; i < groundCheckers.childCount; i++)
        {
            Transform t = groundCheckers.GetChild(i).transform;
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));
            if (hit.collider != null)
            {
                WordObject _block = hit.collider?.GetComponent<WordObject>();
                return true;
            }
        }
        return false;*/

        int layerMask = (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK);
        return Physics2D.OverlapCircle(groundChecker.transform.position, groundChecker.radius, layerMask);
    }

    private bool PlayerIsOnSticky()
    {
        for (int i = 0; i < leftCheckers.childCount; i++)
        {
            Transform t = leftCheckers.GetChild(i).transform;
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.left, distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));
            if (hit.collider != null)
            {
                WordObject _block = hit.collider?.GetComponent<WordObject>();
                if (_block != null && _block.BlockIsSticky)
                {
                    //appeler la fonction qui colle le joueur � GAUCHE
                    this.transform.SetParent(hit.transform, true);
                    if(!IsTouchingGround())
                    {
                        rb.linearVelocity = new Vector2(0, 0);
                    }
                    return true;
                }
            }
        }
        for (int i = 0; i < rightCheckers.childCount; i++)
        {
            Transform t = rightCheckers.GetChild(i).transform;
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.right, distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));
            if (hit.collider != null)
            {
                WordObject _block = hit.collider?.GetComponent<WordObject>();
                if (_block != null && _block.BlockIsSticky)
                {
                    //appeler la fonction qui colle le joueur � DROITE
                    this.transform.SetParent(hit.transform, true);
                    this.transform.localScale = new Vector2(0.5f, 0.5f);
                    if (!IsTouchingGround())
                    {
                        rb.linearVelocity = new Vector2(0, 0);
                    }
                    return true;
                }
            }
        }
        this.transform.SetParent(null, true);
        this.transform.localScale = Vector3.one;
        return false;
    }

    private bool HeadIsSticky()
    {
        for (int i = 0; i < topCheckers.childCount; i++)
        {
            Transform t = topCheckers.GetChild(i).transform;
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.up, distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));
            if (hit.collider != null)
            {
                WordObject _block = hit.collider?.GetComponent<WordObject>();
                if (_block != null && _block.BlockIsSticky)
                {
                    //appeler la fonction qui colle le joueur � GAUCHE
                    this.transform.SetParent(hit.transform, true);
                    CanMove = false;
                    rb.linearVelocity = new Vector2(0, 0);
                    return true;
                }
            }
        }
        CanMove = true;
        return false;
    }

    private void UpdateGravity()
    {
        if(!IsTouchingGround() && (PlayerIsOnSticky() || HeadIsSticky()))
        {
            rb.gravityScale = 0;
        }
        else if (IsTouchingGround())
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    private void GetInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
        if (Mathf.Sign(xInput) != Mathf.Sign(xOrient))
        {
            xOrient *= 1;
        }

    }

    private void Move()
    {
        if (CanMove)
        {
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX + xInput * AccelerationForce * Time.fixedDeltaTime, -MaxSpeed, MaxSpeed);
            Unlink();
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (IsStick && !IsTouchingGround())
        {
            JumpOnSticky();
        }
        else if (HeadIsStick && !IsTouchingGround())
        {
            FallAfterSticky();
        }
        else if (IsStick && IsTouchingGround())
        {
            rightCheckers.gameObject.SetActive(false);
            leftCheckers.gameObject.SetActive(false);
            Invoke("ReactivateRightCheckers", 1);
            Invoke("ReactivateLeftCheckers", 1);
            if (xOrient < 0)
            {
                rb.AddForce(Vector2.right, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.left, ForceMode2D.Impulse);
            }
        }
        else if (IsTouchingGround())
        {
            float yForce = Mathf.Sqrt(defaultJumpHeight * 2 * Physics2D.gravity.magnitude /** rb.gravityScale*/); //Gravity scale 0 or 1
            rb.AddForce(Vector2.up * yForce, ForceMode2D.Impulse);
        }

        Unlink();
    }

    private void JumpOnSticky()
    {
        if (xOrient > 0)
        {
            rightCheckers.gameObject.SetActive(false);
            IsStick = false;
            CanMove = false;
            UpdateGravity();
            this.transform.SetParent(null, true);
            Invoke("ReactivateRightCheckers", 1f);
            rb.AddForce(new Vector2(-10, 20) * JumpHeight);
            xOrient = -1;
        }
        else
        {
            leftCheckers.gameObject.SetActive(false);
            CanMove = false;
            IsStick = false;
            UpdateGravity();
            this.transform.SetParent(null, true);
            Invoke("ReactivateLeftCheckers", 1f);
            rb.AddForce(new Vector2(10, 20) * JumpHeight);
            xOrient = 1;
        }
    }

    private void FallAfterSticky()
    {
        topCheckers.gameObject.SetActive(false);
        HeadIsStick = false;
        UpdateGravity();
        Invoke("ReactivateTopCheckers", 2f);
        rb.AddForce(new Vector2(0, -5) * 12);
    }

    private void Use()
    {
        if (!Input.GetKeyDown(KeyCode.E) || !IsTouchingGround() || LinkedWordBase != null) return;

        for (int i = 0; i < interactionCheckers.childCount; i++)
        {
            Transform t = interactionCheckers.GetChild(i).transform;
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.right * xOrient, interactionDistance, (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK));
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
        StartCoroutine(TransitionCamera(_cameraUnlink, _cameraLink, duration));
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
            StartCoroutine(TransitionCamera(_cameraLink, _cameraUnlink, duration));
        }
    }

    IEnumerator TransitionCamera(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            _camera.Lens.OrthographicSize = _currentCamera;
            _currentCamera = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _currentCamera = endValue;
    }

    private void ReactivateLeftCheckers()
    {
        leftCheckers.gameObject.SetActive(true);
        CanMove = true;
    }

    private void ReactivateRightCheckers()
    {
        rightCheckers.gameObject.SetActive(true);
        CanMove = true;
    }
    
    private void ReactivateTopCheckers()
    {
        topCheckers.gameObject.SetActive(true);
        CanMove = true;
    }

    private void OnDestroy()
    {
        jumpAction.action.started -= Jump;
        moveAction.action.started -= GetInput;
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawLine(transform.position, transform.position + interactionDistance * xOrient * Vector3.right);
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.TextField(rb.linearVelocity.ToString());
        GUILayout.EndVertical();
    }
}
