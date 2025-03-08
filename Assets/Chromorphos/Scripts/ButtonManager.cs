using NaughtyAttributes;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public GameManager.SCENEPARAMETERS _nameNextScene;
    [Scene]
    public string scene;

    public void OnButtonClicked()
    {
        GameManager.Instance.ChangeScene(_nameNextScene, scene);
    }
}

