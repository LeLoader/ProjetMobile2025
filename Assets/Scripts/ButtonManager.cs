using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    [SerializeField] private string _nameNextScene;


    public void OnButtonClicked()
    {
        VolumeSettings.Instance.SaveVolume();
        GameManager.Instance.ChangeScene(_nameNextScene);
    }
}
