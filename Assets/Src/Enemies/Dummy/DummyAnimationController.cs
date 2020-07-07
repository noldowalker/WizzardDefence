using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAnimationController : BasicAnimationController
{   
 

    public float PlayHitedAnimation(float animationSpeed = 1f)
    {
        string[] animatorsNamesList = new string[] { "DummyTorso" };
        return playAnimations(animatorsNamesList, "Hited", animationSpeed);
    }

    public float PlayDeathAnimation(float animationSpeed = 1f)
    {
        string[] animatorsNamesList = new string[]{"DummyTorso"};
        return playAnimations(animatorsNamesList, "Death", animationSpeed);
    }
}
