using UnityEngine;
using UnityEngine.Video;

public class Porte : MonoBehaviour
{

    [SerializeField] private GameObject _canva;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerWord _player = other.GetComponent<PlayerWord>();
        if (_player != null)
        {
            GameManager.Instance.FinishLevel();
            _canva.SetActive(true);
        }
    }
}
