using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAnimationController : BasicAnimationController
{
    public float PlayAttackAnimation(float animationSpeed = 1f)
    {
        string[] animatorsNamesList = new string[] { "MageMagic", "MagePalms", "MageHead", "MageHands"};
        return playAnimations(animatorsNamesList, "Attack", animationSpeed);
    }
}
