using System.Collections;
using UnityEngine;

public class histoire : MonoBehaviour
{

    [SerializeField] private GameObject Image1;
    [SerializeField] private GameObject Image2;
    [SerializeField] private GameObject Image3;
    [SerializeField] private GameObject Image4;

    [SerializeField] Animator animator1;
    [SerializeField] Animator animator2;
    [SerializeField] Animator animator3;
    [SerializeField] Animator animator4;

    private void Awake()
    {
        if (SaveSystem._instance._sawIntro)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        animator1 = Image1.GetComponent<Animator>();
        animator2 = Image2.GetComponent<Animator>();
        animator3 = Image3.GetComponent<Animator>();
        animator4 = Image4.GetComponent<Animator>();

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        animator1.SetTrigger("Start");
        yield return new WaitForSeconds(7f);

        animator2.SetTrigger("Start");
        yield return new WaitForSeconds(7f);

        animator3.SetTrigger("Start");
        yield return new WaitForSeconds(7f);

        animator4.SetTrigger("Start");
        yield return new WaitForSeconds(8.5f);

        Destroy(gameObject);
    }
}
