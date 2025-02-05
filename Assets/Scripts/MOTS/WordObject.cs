using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordObject : WordBase
{
    protected override void Start()
    {
        base.Start();
        WordModifier.AddBaseModifiers(wordType, ref currentModifiers);
        UpdateModifiers();
    }

    public void Link(PlayerWord player)
    {
        Debug.Log("Linked WordObject with: " + player.name);
        linkedWordBase = player;
        wordUI.Link();
    }

    public void Unlink() // Bind to move action when linked
    {
        linkedWordBase = null;
        UpdateModifiers();
    }

    public void AddModifier(WordModifier wordModifier)
    {
        currentModifiers.Add(wordModifier);
        wordUI.UpdateUI(currentModifiers);
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

        wordUI.UpdateUI(currentModifiers);
    }
}
