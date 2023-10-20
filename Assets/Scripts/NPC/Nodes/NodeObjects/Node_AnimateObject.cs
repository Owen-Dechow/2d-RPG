using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_AnimateObject : ActionNode
{
    [SerializeField] GameObject _gameObject;
    [SerializeField] AnimationClip animationClip;

    protected override IEnumerator Execute(Npc npc)
    {
        Animator animator = _gameObject.GetComponent<Animator>();
        animator.Play(animationClip.name);
        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animator.name) {

            yield return new WaitForEndOfFrame();
        }
    }
}