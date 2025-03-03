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
    public bool IsLinked
    {
        get
        {
            return LinkedWordBase != null ? true : false;
        }
    }

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
                        AudioManager.Instance?.PlaySFX(AudioManager.Instance?._mistakeWord2);
                        return;
                    }
                }
                
                target.AddModifier(modifier); // METHOD
                currentModifiers.Remove(currentModifiers.Find(mod => mod.GetType() == modifier.GetType()));
                UpdateWords(ref currentModifiers);
                modifier.Owner = target;
                AudioManager.Instance?.PlaySFX(AudioManager.Instance?._takeWord);
            }
            else
            {
                Debug.LogWarning("Cannot add more modifier to this object");
                AudioManager.Instance?.PlaySFX(AudioManager.Instance?._mistakeWord1);
            }
        }
    }

    virtual public void AddModifier(WordModifier wordModifier)
    {
        currentModifiers.Add(wordModifier);
        UpdateWords(ref currentModifiers);
    }

    protected void UpdateWords(ref List<WordModifier> newModifiers)
    {
        if (Application.IsPlaying(this))
        {
            for (int i = WordWrapper.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(WordWrapper.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            for (int i = WordWrapper.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(WordWrapper.transform.GetChild(i).gameObject);
            }
        }

        //foreach (WordModifier modifier in newModifiers)
        //{
        //    if (Instantiate(WordPrefab, WordWrapper.transform).TryGetComponent<WordUI>(out WordUI wordUI))
        //    {
        //        wordUI.enabled = true;
        //        wordUI.Text.text = modifier.GetName();
        //        if (LinkedWordBase != null)
        //        {
        //            wordUI.Link();
        //        }
        //        wordUI.SetWordModifier(modifier);
        //        modifier.WordUI = wordUI;
        //    }
        //}

        for (int i = 0; i < newModifiers.Count; i++)
        {
            if (Instantiate(WordPrefab, WordWrapper.transform).TryGetComponent<WordUI>(out WordUI wordUI))
            {
                wordUI.enabled = true;
                wordUI.Text.text = newModifiers[i].GetName();
                if (LinkedWordBase != null)
                {
                    wordUI.Link();
                }
                wordUI.SetWordModifier(newModifiers[i]);
                newModifiers[i].WordUI = wordUI;
            }
        }
    }
}
