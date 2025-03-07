using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimBottomUI : MonoBehaviour
{

    [SerializeField] Animator Left;
    [SerializeField] Animator Right;
    [SerializeField] Animator Seringue;
    [SerializeField] Animator Jump;

    private void Start()
    {
        StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(0.3f);
        Left.SetBool("Start", true);
        Right.SetBool("Start", true);
        Seringue.SetBool("Start", true);
        Jump.SetBool("Start", true);
        yield return new WaitForSeconds(1f);
        Left.SetBool("Start", false);
        Right.SetBool("Start", false);
        Seringue.SetBool("Start", false);
        Jump.SetBool("Start", false);
    }
}
