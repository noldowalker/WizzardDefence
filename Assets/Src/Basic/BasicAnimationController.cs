using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimationController : MonoBehaviour
{
    protected Dictionary<string, Animator> animators;
    protected Dictionary<string, float> animationTimings;

    void Awake()
    {
        Animator[] childrens = GetComponentsInChildren<Animator>();

        animators = new Dictionary<string, Animator>();
        animationTimings = new Dictionary<string, float>();

        foreach (Animator animator in childrens)
        {
            animators.Add(animator.gameObject.name, animator);

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                animationTimings.Add(clip.name, clip.length);
            }
        }
    }

    protected float playAnimations(string[] animatorsList, string animation, float animationSpeed = 1f)
    {
        float timing = 0f;
        foreach (string animatiorName in animatorsList)
        {
            Animator animator = animators[animatiorName];
            if (animator != null)
            {
                animator.speed = animationSpeed;
                animator.Play(animatiorName + "_"+ animation);
                timing = animationTimings[animatiorName + "_" + animation] / animationSpeed;
            }            
        }

        return timing;
    }
}
