using UnityEngine;
using System.Collections;

public class Hoop : MonoBehaviour
{
    private Animator animator;

    private string currentAnimation = string.Empty;

    private bool isScoring = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (isScoring)
        {
            return;
        }

        // animator.Play("Default");
    }

    public void TriggerScoreAnimation()
    {
        if (isScoring) return;
        StartCoroutine(PlayScoreAnimation());
    }

    private IEnumerator PlayScoreAnimation()
    {
        isScoring = true;
        animator.Play("Score", 0, 0);
        yield return null;
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.Play("Default");
        isScoring = false;
    }

    public bool IsScoring()
    {
        return isScoring;
    }
}
