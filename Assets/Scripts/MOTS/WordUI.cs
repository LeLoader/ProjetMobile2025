using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordUI : MonoBehaviour, IPointerDownHandler
{
    [field: SerializeField] public Image Image { get; set; }
    [field: SerializeReference] public WordModifier WordModifier { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (WordModifier.Owner.IsLinked)
        {
            WordModifier.Owner.GiveObjectTo(WordModifier.Owner.LinkedWordBase, WordModifier);
        }
    }

    public void SetWordModifier(WordModifier wordModifier)
    {
        WordModifier = wordModifier;
    }

    public void Link()
    {
        Image.color = new(150, 150, 150, 1);
    }

    public void Unlink()
    {
        Image.color = Color.white;
    }
}
