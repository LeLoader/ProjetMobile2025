using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordUI : MonoBehaviour, IPointerDownHandler
{
    [field: SerializeField] public Text Text { get; set; }
    public WordBase WordBase { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (WordBase != null)
        {
            WordBase.GiveObjectTo(WordBase.linkedWordBase);
            Debug.Log("Give me to someone else");
        }
    }

    public void UpdateUI(List<WordModifier> currentModifiers)
    {
        Text.text = "";
        for (int i = 0; i < currentModifiers.Count; i++)
        {
            WordModifier wordModifier = currentModifiers[i];
            if (i != 0)
            {
                Text.text += "\n";
            }
            Text.text += wordModifier.GetName();
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
