using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using UnityEngine;

public class Node_AnimateObject : ActionNode
{
    [SerializeField] readonly string referenceKey;
    [SerializeField] readonly AnimationClip animationClip;

    public override string MenuLocation => "Animation/Animate Object";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        Animator animator = treeData.GameObjects[referenceKey].GetComponent<Animator>();
        animator.Play(animationClip.name);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animator.name);
    }
}
