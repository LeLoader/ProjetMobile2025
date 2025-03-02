using System.Collections;
using UnityEngine;

public class Porte : MonoBehaviour
{

    [SerializeField] private GameObject _canva;
    [SerializeField] private PlayerWord _player;
    [SerializeField] private ParticleSystem _Leftparticule;
    [SerializeField] private ParticleSystem _Rightparticule;

    private void Start()
    {
        _Leftparticule = GameObject.FindGameObjectWithTag("LeftConfetti")?.GetComponent<ParticleSystem>();
        _Rightparticule = GameObject.FindGameObjectWithTag("RightConfetti")?.GetComponent<ParticleSystem>();
        _canva = GameObject.FindGameObjectWithTag("CanvaFin");
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
            GameManager.Instance.FinishLevel();
        }
        _canva.SetActive(true);
    }
}
