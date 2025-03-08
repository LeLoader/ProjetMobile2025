using NaughtyAttributes;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class WordBase : MonoBehaviour
{
    [SerializeField, Label("Mot(s) qu'il possède au Start")] protected WORDTYPE wordType;
    [SerializeReference] public List<WordModifier> currentModifiers = new();
    [SerializeField] protected GameObject WordWrapper;
    [SerializeField] protected GameObject WordPrefab;

    [SerializeField] protected WordObjectSettings spriteSettings;

    public WordBase LinkedWordBase { get; protected set; }
    public bool IsLinked
    {
        get
        {
            return LinkedWordBase != null ? true : false;
        }
    }

    virtual public void GiveObjectTo(WordBase target, WordModifier modifier)
    {
        if (LinkedWordBase != null) {
            if (target.currentModifiers.Count < 2)
            {
                if (target is WordObject && modifier is NonScaleModifier) // Player can hold every type of mod
                {
                    if (target.currentModifiers.Exists(mod => mod is NonScaleModifier))
                    {
                        Debug.LogWarning("Cannot add more NonScaleModifier to this object");
                        AudioManager.Instance?.PlaySFX(AudioManager.Instance?._mistakeWord2);
                        return;
                    }
                }

                WordModifier toRemove = currentModifiers.Find(mod => mod.GetType() == modifier.GetType());
                target.AddModifier(toRemove);
                currentModifiers.Remove(toRemove);
                toRemove.Owner = target;
                UpdateUI(ref currentModifiers);
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
        UpdateUI(ref currentModifiers);
    }

    protected virtual void UpdateUI(ref List<WordModifier> newModifiers)
    {
        if (WordWrapper == null)
        {
            Debug.LogWarning("WordWrapper est null, UpdateUI annulé.");
            return;
        }

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

                //pas bô
                if (newModifiers[i] is BigModifier)
                    wordUI.Image.sprite = spriteSettings.bigWordSprite;
                else if (newModifiers[i] is SmallModifier)
                    wordUI.Image.sprite = spriteSettings.smallWordSprite;
                else if (newModifiers[i] is TallModifier)
                    wordUI.Image.sprite = spriteSettings.tallWordSprite;
                else if (newModifiers[i] is LongModifier)
                    wordUI.Image.sprite = spriteSettings.longWordSprite;
                else if (newModifiers[i] is BouncyModifier)
                    wordUI.Image.sprite = spriteSettings.bouncyWordSprite;
                else if (newModifiers[i] is StickyModifier)
                    wordUI.Image.sprite = spriteSettings.stickyWordSprite;
                else if (newModifiers[i] is StairsModifier)
                    wordUI.Image.sprite = spriteSettings.stairsWordSprite;
                else if (newModifiers[i] is BallModifier)
                    wordUI.Image.sprite = spriteSettings.ballWordSprite;

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
