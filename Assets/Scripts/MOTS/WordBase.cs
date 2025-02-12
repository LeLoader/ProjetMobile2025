using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class WordBase : MonoBehaviour
{
    [SerializeField, Label("Mot(s) qu'il possède au Start")] protected WORDTYPE wordType;
    [SerializeReference] protected List<WordModifier> currentModifiers = new();
    [SerializeField] protected GameObject WordWrapper;
    [SerializeField] protected GameObject WordPrefab;


    public WordBase LinkedWordBase { get; protected set; }

    public void GiveObjectTo(WordBase target, WordModifier modifier)
    {
        if (LinkedWordBase != null) {
            if (target.currentModifiers.Count < 2)
            {
                if (target is WordObject && modifier is NonScaleModifier) // Player can hold every time of mod
                {
                    if (target.currentModifiers.Exists(mod => mod is NonScaleModifier))
                    {
                        Debug.LogWarning("Cannot add more NonScaleModifier to this object");
                        return;
                    }
                }
                
                target.AddModifier(modifier); // METHOD
                currentModifiers.Remove(modifier);
                UpdateWords(currentModifiers);
                modifier.Owner = target;
            }
            else
            {
                Debug.LogWarning("Cannot add more modifier to this object");
            }
        }
    }

    virtual public void AddModifier(WordModifier wordModifier)
    {
        currentModifiers.Add(wordModifier);
        UpdateWords(currentModifiers);
    }

    protected void UpdateWords(List<WordModifier> newModifiers)
    {
        for (int i = 0; i < WordWrapper.transform.childCount; i++)
        {
            Destroy(WordWrapper.transform.GetChild(i).gameObject);
        }

        foreach (WordModifier modifier in newModifiers)
        {
            if (Instantiate(WordPrefab, WordWrapper.transform).TryGetComponent<WordUI>(out WordUI wordUI))
            {
                wordUI.Text.text = modifier.GetName();
                if (LinkedWordBase != null)
                {
                    wordUI.Link();
                }
                wordUI.WordModifier = modifier;
                modifier.WordUI = wordUI;
            }
        }
    }
}
