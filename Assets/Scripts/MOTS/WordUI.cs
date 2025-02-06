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
            Debug.Log($"Giving {WordModifier.GetName()} to {WordModifier.Owner.LinkedWordBase} from {WordModifier.Owner}");
            WordModifier.Owner.GiveObjectTo(WordModifier.Owner.LinkedWordBase, WordModifier);
        }
    }

    public void Link()
    {
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    public void Unlink()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
}
