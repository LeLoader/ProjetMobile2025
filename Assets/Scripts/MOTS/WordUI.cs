using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordUI : MonoBehaviour, IPointerDownHandler
{
    [field: SerializeField] public Text Text { get; set; }
    [field: SerializeReference] public WordModifier WordModifier { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (WordModifier.Owner.LinkedWordBase != null)
        {
            Debug.Log($"Trying to give {WordModifier.GetName()} to {WordModifier.Owner.LinkedWordBase} from {WordModifier.Owner}");
            WordModifier.Owner.GiveObjectTo(WordModifier.Owner.LinkedWordBase, WordModifier);
        }
    }

    public void Link()
    {
        Text.color = Color.red;
    }

    public void Unlink()
    {
        Text.color = Color.white;
    }
}
