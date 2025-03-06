using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Porte : MonoBehaviour
{

    [SerializeField] private GameObject _canva;
    [SerializeField] private PlayerWord _player;
    [SerializeField] private ParticleSystem _Leftparticule;
    [SerializeField] private ParticleSystem _Rightparticule;
    public GameObject _nextLevel;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerWord>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerWord _player = other.GetComponent<PlayerWord>();
        if (_player != null)
        {
            _Leftparticule.Play();
            _Rightparticule.Play();
            _player.CanMove = false;
            StartCoroutine(DelayBeforeEndCanva());
        }
    }

    private IEnumerator DelayBeforeEndCanva()
    {
        yield return new WaitForSeconds(3f);
        _player.CanMove = false;
        _Leftparticule.Stop();
        _Rightparticule.Stop();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinishLevel(_nextLevel);
        }
        _canva.SetActive(true);
        _canva.GetComponent<Animator>().SetTrigger("Start");
        SaveSystem._instance.SaveData();
    }
}
