using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordObject : MonoBehaviour
{
    [SerializeField] public Text words;
    [SerializeField, Label("1e Type de base pour cet objet")] WORDTYPE wordType;
    [SerializeField, Label("2e Type de base pour cet objet"), Tooltip("A utiliser si jamais il faut 2 fois le même état.")] WORDTYPE wordType1;
    [SerializeReference] List<WordModifier> currentModifiers = new();

    private void Start()
    {
        AddBaseModifiers();
        UpdateUI();
    }

    public void AddModifier(WordModifier wordModifier)
    {
        currentModifiers.Add(wordModifier);
        wordModifier.Apply(transform);
        UpdateUI();
    }

    public List<WordModifier> GetAllModifiers()
    {
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.DebugName();
        }
        return currentModifiers;
    }

    public List<WordModifier> UseAllModifiers()
    {
        foreach(WordModifier modifier in currentModifiers)
        {
            modifier.DeApply(transform);
        }
        List<WordModifier> returnList = new List<WordModifier>(currentModifiers);
        currentModifiers.Clear();
        words.text = "";
        return returnList;
    }

    public WordModifier UseModifier(int index)
    {
        if (index < currentModifiers.Count)
        {
            Debug.Log("Used modifier:" + currentModifiers[index]);
            return currentModifiers[index];
        }
        else
        {
            Debug.LogWarning("Wrong index");
            return null;
        }
    }

    public WordModifier UseModifier(WordModifier wordModifier)
    {
        return currentModifiers.Find(modifier => modifier.GetType() == wordModifier.GetType());
    }

    private void UpdateUI()
    {
        words.text = "";
        for (int i = 0; i < currentModifiers.Count; i++)
        {
            WordModifier wordModifier = currentModifiers[i];
            if (i != 0)
            {
                words.text += "\n";
            }
            words.text += wordModifier.GetName();
        }
    }
    private void AddBaseModifiers()
    {
        if ((int)wordType == 0) return;
        if (wordType.HasFlag(WORDTYPE.SMALL))
            currentModifiers.Add(new SmallModifier());
        if (wordType.HasFlag(WORDTYPE.BIG))
            currentModifiers.Add(new BigModifier());
        if (wordType.HasFlag(WORDTYPE.TALL))
            currentModifiers.Add(new TallModifier());
        if (wordType.HasFlag(WORDTYPE.LONG))
            currentModifiers.Add(new LongModifier());
    }
}
