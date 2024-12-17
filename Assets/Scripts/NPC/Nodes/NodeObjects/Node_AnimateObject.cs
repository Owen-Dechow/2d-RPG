using System.Collections;
using System.Collections.Generic;
using Controllers;
using NPC;
using UnityEngine;

public class Node_AnimateObject : ActionNode
{
    [SerializeField] string referenceKey;
    [SerializeField] AnimationClip animationClip;

    public override string MenuLocation => "Animation/Animate Object";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        Animator animator = treeData.GameObjects[referenceKey].GetComponent<Animator>();
        animator.Play(animationClip.name);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animator.name);
    }
}