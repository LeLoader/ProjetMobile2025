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
            FinishLevel1.Instance.FinishLevel();
            _canva.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
        }
    }
}
