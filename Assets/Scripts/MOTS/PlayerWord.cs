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

public class PlayerWord : WordBase
{
    [Header("General")]
    [SerializeField] float interactionDistance = 5;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundCheckers;
    [SerializeField] Transform leftCheckers;
    [SerializeField] Transform rightCheckers;
    [SerializeField] Transform topCheckers;
    [SerializeField] Transform interactionCheckers;
    [SerializeField] Transform orientSign;
    [SerializeField] float distanceCheck;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float speedForce = 5f;
    [SerializeField] bool IsLink = false;
    [SerializeField] CinemachineCamera _camera;
    [SerializeField] float duration = 2f;
    [SerializeField] bool IsStick;
    [SerializeField] bool CanMove;

    int orientX = 1;

    const int GROUND_LAYERMASK = 3;
    const int WORDOBJECT_LAYERMASK = 7;
    const int MAP_LAYERMASK = 8;
    ContactFilter2D contactFilter = new();

    float _cameraUnlink = 7f;
    float _currentCamera;
    float _cameraLink = 3f;

    private void Start()
    {

        contactFilter.layerMask = (int)Mathf.Pow(2, WORDOBJECT_LAYERMASK);
        contactFilter.useLayerMask = true;
        CanMove = true;

        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);
    }

    void Update()
    {
        Use();
        Move();
        Jump();
        UpdateOrientation();
        IsStick = PlayerIsOnSticky();
        UpdateGravity();
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
                WordObject _block = hit.collider?.GetComponent<WordObject>();
                if (_block != null && _block.BlockIsSticky)
                {
                    speedForce = 3;
                    jumpForce  = 2;
                }
                return true;
            }
        }
        speedForce = 7;
        jumpForce = 5;
        return false;
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
                if(_block != null && _block.BlockIsSticky)
                {
                    //appeler la fonction qui colle le joueur à GAUCHE
                    this.transform.SetParent(hit.transform, true);
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
                    //appeler la fonction qui colle le joueur à DROITE
                    this.transform.SetParent(hit.transform, true);
                    return true;
                }
            }
        }
        this.transform.SetParent(null, true);
        return false;
    }

    private bool HeadIsSticky()
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
                    //appeler la fonction qui colle le joueur à GAUCHE
                    this.transform.SetParent(hit.transform, true);
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateGravity()
    {
        if(!IsTouchingGround() && PlayerIsOnSticky())
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.A) && CanMove)
        {
            rb.AddForce(Vector2.left * speedForce);
            orientX = -1;
            Unlink();

        }
        if (Input.GetKey(KeyCode.D) && CanMove)
        {
            rb.AddForce(Vector2.right * speedForce);
            orientX = 1;
            Unlink();
        }
    }

    private void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || (!IsTouchingGround() && !IsStick)) return;

        if (IsStick && !IsTouchingGround())
        {
            JumpOnSticky();
        }
        else
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        Unlink();
    }

    private void JumpOnSticky()
    {
        if(orientX > 0)
        {
            rightCheckers.gameObject.SetActive(false);
            IsStick = false;
            CanMove = false;
            UpdateGravity();
            this.transform.SetParent(null, true);
            Invoke("ReactivateRightCheckers", 0.5f);
            rb.AddForce(new Vector2(-10, 20) * speedForce * 4);
            orientX = -1;
        }
        else
        {
            leftCheckers.gameObject.SetActive(false);
            CanMove = false;
            IsStick = false;
            UpdateGravity();
            this.transform.SetParent(null, true);
            Invoke("ReactivateLeftCheckers", 0.5f);
            rb.AddForce(new Vector2(10, 20) * speedForce * 4);
            orientX = 1;
        }
    }

    private void Use()
    {
        if (!Input.GetKeyDown(KeyCode.E) || !IsTouchingGround() || IsLink) return;

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
        IsLink = true;
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
            IsLink = false;
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


    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawLine(transform.position, transform.position + interactionDistance * orientX * Vector3.right);
    }
}
