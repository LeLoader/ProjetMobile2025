using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordObject : WordBase
{
    private void Start()
    {
        WordModifier.AddBaseModifiers(wordType, ref currentModifiers, this);
        UpdateWords(currentModifiers);
        UpdateModifiers();
    }

    public void Link(PlayerWord player)
    {
        LinkedWordBase = player;
        foreach(WordModifier modifier in currentModifiers)
        {
            modifier.WordUI.Link();
        }
    }

    public void Unlink() // Bind to move action when linked
    {
        LinkedWordBase = null;
        UpdateModifiers();
    }

    private void ResetObject()
    {
        transform.localScale = Vector3.one;
    }

    private void UpdateModifiers()
    {
        ResetObject();
        foreach (WordModifier modifier in currentModifiers)
        {
            modifier.Apply(transform);
        }
    }

    [Button]
    private void ForceUpdateWord()
    {
        UpdateModifiers();
        UpdateWords(currentModifiers);
    }
}
