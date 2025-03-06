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

    private void Start()
    {
        if (SaveSystem._instance._sawIntro)
        {
            Destroy(gameObject);
        }
        else
        {
            animator1 = Image1.GetComponent<Animator>();
            animator2 = Image2.GetComponent<Animator>();
            animator3 = Image3.GetComponent<Animator>();
            animator4 = Image4.GetComponent<Animator>();
            Debug.Log($"Animator1: {animator1}, Animator2: {animator2}, Animator3: {animator3}, Animator4: {animator4}");
        }
    }

    public void LaunchHistory()
    {
        StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence()
    { 
        animator1.SetTrigger("Start");
        yield return new WaitForSeconds(5f);

        animator2.SetTrigger("Start");
        yield return new WaitForSeconds(5f);

        animator3.SetTrigger("Start");
        yield return new WaitForSeconds(5f);

        animator4.SetTrigger("Start");
        yield return new WaitForSeconds(7f);

        Destroy(gameObject);
    }
}
