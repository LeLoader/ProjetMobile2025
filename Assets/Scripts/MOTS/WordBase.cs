using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class WordBase : MonoBehaviour
{
    [SerializeField] public WordUI wordUI;
    [SerializeField, Label("Mot qui l'objet possède au Start")] protected WORDTYPE wordType;
    [SerializeReference, ReadOnly] protected List<WordModifier> currentModifiers = new();

    public WordBase linkedWordBase = null;

    protected virtual void Start()
    {
        wordUI.WordBase = this;
    }

    public void GiveObjectTo(WordBase target)
    {
        if (linkedWordBase != null) {
           // target.currentModifiers.Add(this);
        }
    }
}
