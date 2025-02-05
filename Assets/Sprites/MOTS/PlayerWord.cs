using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlyAttribute = NaughtyAttributes.ReadOnlyAttribute;

public class PlayerWord : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float interactionRadius = 5;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float speedForce = 5f;



    const int WORDOBJECT_LAYERMASK = 7;
    public ContactFilter2D contactFilter = new();

    [Header("Word")]
    [SerializeField] GameObject WordWrapper;
    [SerializeField] GameObject WordPrefab;
    [SerializeField, Label("1e Type de base pour cet objet")] WORDTYPE wordType;
    [SerializeField, Label("2e Type de base pour cet objet"), Tooltip("A utiliser si jamais il faut 2 fois le même état.")] WORDTYPE wordType1;
    [SerializeReference, ReadOnly] List<WordModifier> currentModifiers = new();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //contactFilter.layerMask = WORDOBJECT_LAYERMASK;
        contactFilter.useLayerMask = true;

        AddBaseModifiers(wordType);
        AddBaseModifiers(wordType1);
        UpdateWords(currentModifiers);
    }

    // Update is called once per frame
    void Update()
    {
        UseWord();
        Move();
        Jump();
    }

    private void AddBaseModifiers(WORDTYPE type)
    {
        if ((int)type == 0) return;
        if (type.HasFlag(WORDTYPE.SMALL))
            currentModifiers.Add(new SmallModifier());
        if (type.HasFlag(WORDTYPE.BIG))
            currentModifiers.Add(new BigModifier());
        if (type.HasFlag(WORDTYPE.TALL))
            currentModifiers.Add(new TallModifier());
        if (type.HasFlag(WORDTYPE.LONG))
            currentModifiers.Add(new LongModifier());
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * speedForce);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * speedForce);
        }
    }

    private void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void UseWord()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        List<Collider2D> results = new(); 
 
        if (Physics2D.OverlapCircle(transform.position, interactionRadius, contactFilter, results) > 0)
        {
            if (results[0].TryGetComponent<WordObject>(out WordObject wordObject)){
                // HOLDING MODIFIER
                if (currentModifiers.Count > 0) 
                {
                    UseAllModifiers(wordObject);
                    UpdateWords(currentModifiers);
                }
                // NO MODIFIER
                else
                {
                    currentModifiers = wordObject.UseAllModifiers();
                    UpdateWords(currentModifiers);
                }
            };
        }
    }

    private void UseAllModifiers(WordObject wordObject)
    {
        foreach (var modifier in currentModifiers.ToList())
        {
            wordObject.AddModifier(modifier);
            currentModifiers.Remove(modifier);
        }
        UpdateWords(currentModifiers);
    }

    private void UpdateWords(List<WordModifier> newModifiers)
    {
        for (int i = 0; i < WordWrapper.transform.childCount; i++) {
            Destroy(WordWrapper.transform.GetChild(0).gameObject);
        }

        foreach(WordModifier wordModifier in newModifiers)
        {
            if (Instantiate(WordPrefab, WordWrapper.transform).TryGetComponent<WordUI>(out WordUI wordUI)){
                wordUI.text.text = wordModifier.GetName();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(transform.position, Vector3.forward, Vector3.up, 360, interactionRadius);
    }
}
