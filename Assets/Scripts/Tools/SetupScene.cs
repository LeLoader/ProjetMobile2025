using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

public class SetupScene : MonoBehaviour
{
    [SerializeField] bool DebugMode;

    [SerializeField, ShowIf("DebugMode")] GameObject Player;
    [SerializeField, ShowIf("DebugMode")] GameObject UI;
    [SerializeField, ShowIf("DebugMode")] GameObject Grids;
    [SerializeField, ShowIf("DebugMode")] GameObject Door;
    [SerializeField, ShowIf("DebugMode")] GameObject Camera;

    [SerializeField] int scene_Id = -1;
    [SerializeField] bool CLEAR_ALL_EXISTING_OBJECTS = false;

    [Button]
    void StartSetupingScene()
    {
        if (scene_Id == -1)
        {
            Debug.LogError("INCORRECT SCENE ID: -1");
            return;
        }

        if (CLEAR_ALL_EXISTING_OBJECTS)
        {
            var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects)
            {
                DestroyImmediate(obj);
            }
        }

        PlayerWord player = Instantiate(Player).GetComponent<PlayerWord>();
        Instantiate(UI);
        Instantiate(Grids);
        Instantiate(Door);
        CinemachineCamera camera = Instantiate(Camera).GetComponent<CinemachineCamera>();
        player._camera = camera;
        camera.Follow = player.transform;
    }

    [Button]
    void DeleteSelf()
    {
        Destroy(this);
    }
}
