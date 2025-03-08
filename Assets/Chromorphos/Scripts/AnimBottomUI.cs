using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimBottomUI : MonoBehaviour
{

    [SerializeField] Animator Left;
    [SerializeField] Animator Right;
    [SerializeField] Animator Seringue;
    [SerializeField] Animator Jump;

    [SerializeField] GameObject right;
    [SerializeField] GameObject left;
    [SerializeField] GameObject seringue;
    [SerializeField] GameObject jump;

    private void Start()
    {
        StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(1.5f);
        right.SetActive(true);
        left.SetActive(true);
        seringue.SetActive(true);   
        jump.SetActive(true);
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
