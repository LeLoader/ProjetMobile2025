using UnityEditor;
using UnityEngine;
using ReadOnlyAttribute = NaughtyAttributes.ReadOnlyAttribute;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

public class PlayerWord : WordBase
{
    [Header("General")]
    [SerializeField] float interactionDistance = 5;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CapsuleCollider2D playerCollider;
    [SerializeField] CircleCollider2D groundChecker;
    [SerializeField] Transform leftCheckers;
    [SerializeField] Transform rightCheckers;
    [SerializeField] Transform topCheckers;
    [SerializeField] Transform interactionCheckers;
    [SerializeField] SpriteRenderer orientSign;
    [SerializeField] float distanceCheck = 0.01f;
    [SerializeField] float slopeHorizontalDistanceCheck = 1f;
    [SerializeField] float slopeVerticalDistanceCheck = 1f;
    [SerializeField] float sideSlopeOffset = 0.01f;

    [SerializeField] public CinemachineCamera _camera;
    [SerializeField] float duration = 2f;

    [Header("Action")]
    [SerializeField] InputActionReference jumpAction;
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference useAction;

    [Header("Movement")]
    [SerializeField, ReadOnly] bool OnGround;
    [SerializeField, ReadOnly] bool HeadIsStick;
    [SerializeField, ReadOnly] bool IsStick;
    [SerializeField, ReadOnly] bool IsJumping;
    [SerializeField, ReadOnly] bool IsOnBouncy;
    [SerializeField, ReadOnly] public bool CanMove;
    [SerializeField, ReadOnly] bool OnSlope;
    [SerializeField, ReadOnly] bool OnSideSlope;

    Vector2 downSlopeNormalPerp;
    Vector2 sideSlopeNormalPerp;
    float lastSlopeAngle;
    float slopeDownAngle;
    float slopeSideAngle;

    private Coroutine _zoomCoroutine;

    public float AccelerationForce
    {
        get
        {
            return GetAccelerationForce();
        }
    }

    public float DecelerationForce
    {
        get
        {
            return GetDecelerationForce();
        }
    }

    public float MaxSpeed
    {
        get
        {
            return GetMaxSpeed();
        }
    }

    public float JumpHeight
    {
        get
        {
            return GetJumpHeight();
        }
    }

    [SerializeField] private float maxPositionYValue;

    [Header("Movement | Default")]
    [SerializeField, Tooltip("m/s²")] float defaultAccelerationForce = 150f;
    [SerializeField, Tooltip("m/s²")] float defaultDecelerationForce = 75f;
    [SerializeField, Tooltip("m/s")] float defaultMaxSpeed = 5f;
    [SerializeField, Tooltip("m")] float defaultJumpHeight = 1.25f;

    [Header("Movement | Air")]
    [SerializeField, Tooltip("m/s²")] float airAccelerationForce = 150f;
    [SerializeField, Tooltip("m/s²")] float airDecelerationForce = 75f;
    [SerializeField, Tooltip("m/s")] float airMaxSpeed = 5f;

    [Header("Movement | Sticked")]
    [SerializeField, Tooltip("m/s²")] float stickedAccelerationForce = 50f;
    [SerializeField, Tooltip("m/s²")] float stickedDecelerationForce = 75f;
    [SerializeField, Tooltip("m/s")] float stickedMaxSpeed = 3f;
    [SerializeField, Tooltip("m")] float stickedJumpHeight = 1f;

    [Header("Movement | Bouncy")]
    [SerializeField, Tooltip("m")] float defaultBouncyJumpHeight = 2.5f;

    //[Header("Tools")]
    //[SerializeField] private GameObject reglage;

    private float lastVelY;

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
        UpdateWords(ref currentModifiers);

        jumpAction.action.started += Jump;
        moveAction.action.performed += GetInput;
        moveAction.action.canceled += GetInput;
        useAction.action.started += Use;
    }

    void FixedUpdate()
    {
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, 0, DecelerationForce * Time.fixedDeltaTime); // Method
        //if (IsJumping)
        //{
        //    rb.linearVelocityY = Mathf.MoveTowards(rb.linearVelocityY, 0, DecelerationForce * Time.fixedDeltaTime); // Method
        //}
        CheckIsFalling();

        Move();

        IsTouchingGround();
        BlockIsBouncy();
        SlopeCheck();
        IsStick = PlayerIsOnSticky();
        HeadIsStick = HeadIsSticky();
        UpdateGravity();

        UpdateOrientation();
    }

    private void Update()
    {
        GetMaxLastJump();
    }

    private void BlockIsBouncy()
    {
        WordObject wo = IsTouchingWordObject(); // Update all state at once?
        if (wo)
        {
            if (wo.BlockIsBouncy)
            {
                IsOnBouncy = true;
                AudioManager.Instance.PlaySFX(AudioManager.Instance._BouncySFX1);
                Jump();
            }
            else
            {
                IsOnBouncy = false;
            }
        }
        else
        {
            IsOnBouncy = false;
        }
    }

    private void GetMaxLastJump()
    {
        if (OnGround)
        {
            maxPositionYValue = transform.position.y;
        }
        if (transform.position.y > maxPositionYValue)
        {
            maxPositionYValue = transform.position.y;
        }
    }

    private float GetAccelerationForce()
    {
        if (IsStick)
        {
            return stickedAccelerationForce;
        }
        else if (!OnGround)
        {
            return airAccelerationForce;
        }
        else
        {
            return defaultAccelerationForce;
        }
    }

    private float GetDecelerationForce()
    {
        if (IsStick)
        {
            return stickedDecelerationForce;
        }
        else if (!OnGround)
        {
            return airDecelerationForce;
        }
        else
        {
            return defaultDecelerationForce;
        }
    }

    private float GetMaxSpeed()
    {
        if (IsStick)
        {
            return stickedMaxSpeed;
        }
        else if (!OnGround)
        {
            return airMaxSpeed;
        }
        else
        {
            return defaultMaxSpeed;
        }
    }

    private float GetJumpHeight()
    {
        if (IsOnBouncy)
        {
            Debug.Log(maxPositionYValue - transform.position.y);
            return Mathf.Max(maxPositionYValue - transform.position.y, defaultBouncyJumpHeight);
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

#nullable enable
    private Collider2D? IsTouchingGround()
    {
        int layerMask = (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK);
        Collider2D coll = Physics2D.OverlapCircle(groundChecker.transform.position, groundChecker.radius * groundChecker.transform.lossyScale.x, layerMask);

        if (coll != null)
        {
            OnGround = true;
            return coll;
        }
        OnGround = false;
        return null;
    }

    private WordObject? IsTouchingWordObject()
    {
        Collider2D? coll = IsTouchingGround();
        if (coll != null)
        {
            if (coll.TryGetComponent<WordObject>(out WordObject wo))
            {
                return wo;
            }
        }
        return null;
    }
#nullable disable

    private bool PlayerIsOnSticky()
    {
        return (CheckSticky(rightCheckers, Vector2.right) || CheckSticky(leftCheckers, Vector2.left));
    }

    private bool CheckSticky(Transform checkers, Vector2 dir)
    {
        for (int i = 0; i < checkers.childCount; i++)
        {
            if (checkers.gameObject.activeInHierarchy)
            {
                Transform t = checkers.GetChild(i).transform;
                RaycastHit2D hit = Physics2D.Raycast(t.position, dir, distanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));
                if (hit.collider != null)
                {
                    WordObject _block = hit.collider?.GetComponent<WordObject>();
                    if (_block != null && _block.BlockIsSticky)
                    {
                        //appeler la fonction qui colle le joueur
                        if (!OnGround)
                        {
                            CanMove = false;
                            rb.linearVelocity = new Vector2(0, 0);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool HeadIsSticky()
    {
        return CheckSticky(topCheckers, Vector2.up);

    }

    private void UpdateGravity()
    {
        if (!OnGround && (PlayerIsOnSticky() || HeadIsSticky()))
        {
            rb.gravityScale = 0;
        }
        else if (OnSideSlope || OnSlope)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    private void CheckIsFalling()
    {
        if (lastVelY >= 0 && 0 >= rb.linearVelocityY)
        {
            IsJumping = false;
        }

        lastVelY = rb.linearVelocityY;
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, playerCollider.size.y / 2 * transform.localScale.y + sideSlopeOffset);

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeHorizontalDistanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeHorizontalDistanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));

        Debug.DrawRay(checkPos, transform.right * slopeHorizontalDistanceCheck, Color.yellow);
        Debug.DrawRay(checkPos, -transform.right * slopeHorizontalDistanceCheck, Color.yellow);

        if (slopeHitFront)
        {
            OnSideSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            sideSlopeNormalPerp = Vector2.Perpendicular(slopeHitFront.normal).normalized;
            Debug.DrawRay(slopeHitFront.point, sideSlopeNormalPerp, Color.blue);
        }
        else if (slopeHitBack)
        {
            OnSideSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            sideSlopeNormalPerp = Vector2.Perpendicular(slopeHitBack.normal).normalized;
            Debug.DrawRay(slopeHitBack.point, sideSlopeNormalPerp, Color.blue);
        }
        else
        {
            slopeSideAngle = 0.0f;
            OnSideSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeVerticalDistanceCheck, (int)Mathf.Pow(2, MAP_LAYERMASK) + (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK) + (int)Mathf.Pow(2, GROUND_LAYERMASK));

        if (hit)
        {
            downSlopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (Mathf.Approximately(slopeDownAngle, 0))
            {
                OnSlope = false;
                lastSlopeAngle = 0;
            }
            else
            {
                lastSlopeAngle = slopeDownAngle;
                OnSlope = true;
            }

            Debug.DrawRay(hit.point, downSlopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        else
        {
            if (OnSlope == true && !IsJumping)
            {
                rb.linearVelocityY = 0;
            }
            OnSlope = false;
            lastSlopeAngle = 0;
        }
    }

    private void GetInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
        if (xInput != 0)
        {
            Unlink();
        }

        if (!IsStick)
        {
            if (xInput == -xOrient || -xInput == xOrient)
            {
                xOrient *= -1;
            }
        }
    }

    private void Move()
    {
        if (CanMove)
        {
            if (OnGround && !IsJumping) // GROUND MOVEMENT
            {
                if (OnSlope && !OnSideSlope) // On top of a peak or on weak slope
                {
                    rb.linearVelocity = new Vector3(-xInput * 5 * downSlopeNormalPerp.x,
                                                    -xInput * 5 * downSlopeNormalPerp.y);
                }
                else if (!OnSlope && OnSideSlope) // About to go on slope or on hard slope
                {
                    rb.linearVelocity = new Vector3(-xInput * 5 * sideSlopeNormalPerp.x,
                                                    -xInput * 5 * sideSlopeNormalPerp.y);
                }
                else
                {
                    rb.linearVelocity = new Vector3(-xInput * 5 * downSlopeNormalPerp.x,
                                                    -xInput * 5 * downSlopeNormalPerp.y);
                }

                //if (OnSlope)
                //{
                //    //rb.linearVelocity = new Vector3(-xInput * 5 * slopeNormalPerp.x,
                //    //                                -xInput * 5 * slopeNormalPerp.y);
                //    
                //}
                //else
                //{
                //    rb.linearVelocity = new Vector3(rb.linearVelocityX + xInput * AccelerationForce * Time.fixedDeltaTime,
                //                                    rb.linearVelocityY);
                //}
            }
            else // AIR MOVEMENT
            {
                //if (OnSlope)
                //{
                //    rb.linearVelocity = new Vector3(-xInput * 5 * downSlopeNormalPerp.x,
                //                                    -xInput * 5 * downSlopeNormalPerp.y);
                //}
                //else
                //{
                //    rb.linearVelocityX = rb.linearVelocityX + xInput * AccelerationForce * Time.fixedDeltaTime;
                //}
                rb.linearVelocityX = rb.linearVelocityX + xInput * AccelerationForce * Time.fixedDeltaTime;
            }

            if (OnGround && !IsJumping)
            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, MaxSpeed);
            }
            else // Don't clamp Y vel when in air or jumping
            {
                rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -MaxSpeed, MaxSpeed);
            }
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Jump();
    }

    private void Jump()
    {


        if (IsJumping) return;

        IsJumping = true;
        if (IsStick && !OnGround)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance._JumpSFX);
            JumpOnSticky();
        }
        else if (HeadIsStick && !OnGround)
        {
            FallAfterSticky();
        }
        else if (IsStick && OnGround)
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
        else if (OnGround)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance._JumpSFX);
            rb.linearVelocityY = 0;
            float yForce = Mathf.Sqrt(JumpHeight * 2 * Physics2D.gravity.magnitude);
            rb.AddForce(Vector2.up * yForce, ForceMode2D.Impulse);
        }
        Unlink();
    }

    private void JumpOnSticky()
    {

        if (xOrient > 0)
        {
            rightCheckers.gameObject.SetActive(false);
            Invoke("ReactivateRightCheckers", 1f);
        }
        else
        {
            leftCheckers.gameObject.SetActive(false);
            Invoke("ReactivateLeftCheckers", 1f);
        }
        xOrient *= -1;
        Debug.Log("JumpOnSticky");
        IsStick = false;
        CanMove = false;
        rb.AddForce(new Vector2(5 * xOrient, 5), ForceMode2D.Impulse);
    }

    private void FallAfterSticky()
    {
        Debug.Log("FallAfterSticky");
        topCheckers.gameObject.SetActive(false);
        HeadIsStick = false;
        CanMove = true;
        Invoke("ReactivateTopCheckers", 0.5f);
        rb.AddForce(new Vector2(0, -5) * 12);
    }

    private void Use(InputAction.CallbackContext context)
    {
        if (!OnGround || LinkedWordBase != null) return;

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
        AudioManager.Instance.PlaySFX(AudioManager.Instance._SeringuePlantée);
        _camera.Target.TrackingTarget = wordObject?.transform;
        StartZoom(_cameraUnlink, _cameraLink, duration);
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
            if (transform != null)
            {
                _camera.Target.TrackingTarget = transform;
            }
            StartZoom(_cameraLink, _cameraUnlink, duration);
        }
    }

    private void StartZoom(float startValue, float endValue, float duration)
    {
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
        _zoomCoroutine = StartCoroutine(TransitionCamera(startValue, endValue, duration));
    }

    private IEnumerator TransitionCamera(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _currentCamera = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            _camera.Lens.OrthographicSize = _currentCamera;
            yield return null;
        }

        _currentCamera = endValue;
        _camera.Lens.OrthographicSize = _currentCamera;

        _zoomCoroutine = null;
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
    }

    private void OnDestroy()
    {
        jumpAction.action.started -= Jump;
        moveAction.action.performed -= GetInput;
        moveAction.action.canceled -= GetInput;
        useAction.action.started -= Use;
    }

    private void OnDrawGizmos()
    {
        //Handles.color = Color.blue;
        //Handles.DrawLine(transform.position, transform.position + interactionDistance * xOrient * Vector3.right);
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.TextField(rb.linearVelocity.ToString());
        GUILayout.TextField("Ground:" + OnGround.ToString());
        GUILayout.TextField("Slope:" + OnSlope.ToString());
        GUILayout.TextField("SideSlope:" + OnSideSlope.ToString());
        GUILayout.EndVertical();
    }
}
