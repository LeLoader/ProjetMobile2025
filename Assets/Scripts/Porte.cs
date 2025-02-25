using System.Collections;
using UnityEngine;
using UnityEngine.Video;

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerWord _player = other.GetComponent<PlayerWord>();
        if (_player != null)
        {
            _Leftparticule.Play();
            _Rightparticule.Play();
            ParticleSystem.EmissionModule em = GetComponent<ParticleSystem>().emission;
            StartCoroutine(DelayBeforeEndCanva());
        }
    }

    private IEnumerator DelayBeforeEndCanva()
    {
        yield return new WaitForSeconds(3f);
        _player.CanMove = false;
        _Leftparticule.Stop();
        _Rightparticule.Stop();
        GameManager.Instance.FinishLevel();
        _canva.SetActive(true);
    }
}
