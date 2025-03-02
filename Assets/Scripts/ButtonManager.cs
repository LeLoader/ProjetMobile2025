using NaughtyAttributes;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public string _nameNextScene;
    public void OnButtonClicked()
    {
        GameManager.Instance.ChangeScene(_nameNextScene);
    }
}
