using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    [SerializeField] private string _nameNextScene;


    public void OnButtonClicked()
    {
        GameManager.Instance.ChangeScene(_nameNextScene);
    }
}
