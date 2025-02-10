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

public class PlayerWord : WordBase
{
    [Header("General")]
    [SerializeField] float interactionDistance = 5;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundCheckers;
    [SerializeField] Transform interactionCheckers;
    [SerializeField] Transform orientSign;
    [SerializeField] float distanceCheck;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float speedForce = 5f;
    [SerializeField] bool IsLink = false;
    [SerializeField] CinemachineCamera _camera;
    [SerializeField] float duration = 2f;

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

        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);
    }

    void Update()
    {
        Use();
        Move();
        Jump();
        UpdateOrientation();
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
                return true;
            }
        }
            
        return false;
    }


    private void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * speedForce);
            orientX = -1;
            Unlink();

        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * speedForce);
            orientX = 1;
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
        StartCoroutine(DecreaseValueCoroutine(_cameraUnlink, _cameraLink, duration));
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
            StartCoroutine(DecreaseValueCoroutine(_cameraLink, _cameraUnlink, duration));
        }
    }

    IEnumerator DecreaseValueCoroutine(float startValue, float endValue, float duration)
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


    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawLine(transform.position, transform.position + interactionDistance * orientX * Vector3.right);
    }
}
