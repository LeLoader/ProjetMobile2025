using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimBottomUI : MonoBehaviour
{

    [SerializeField] Animator Left;
    [SerializeField] Animator Right;
    [SerializeField] Animator Seringue;
    [SerializeField] Animator Jump;


    void Start()
    {

    }

    public IEnumerator PlaySequence()
    {
        Left.SetTrigger("Start");
        yield return new WaitForSeconds(0.1f);

        Right.SetTrigger("Start");
        yield return new WaitForSeconds(0.2f);

        Seringue.SetTrigger("Start");
        yield return new WaitForSeconds(0.2f);

        Jump.SetTrigger("Start");
        yield return new WaitForSeconds(0.2f);
    }
}
