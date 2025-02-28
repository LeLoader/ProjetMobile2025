using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordUI : MonoBehaviour, IPointerDownHandler
{
    [field: SerializeField] public Text Text { get; set; }
    [field: SerializeReference] public WordModifier WordModifier { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (WordModifier.Owner.LinkedWordBase != null)
        {
            WordModifier.Owner.GiveObjectTo(WordModifier.Owner.LinkedWordBase, WordModifier);
            AudioManager.Instance.PlaySFX(AudioManager.Instance._SeringuePlantée);
        }
    }

    public void SetWordModifier(WordModifier wordModifier)
    {
        WordModifier = wordModifier;
    }

    public void Link()
    {
        Text.color = Color.red;
    }

    public void Unlink()
    {
        Text.color = Color.black;
    }
}
