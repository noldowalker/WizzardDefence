using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public const int UP = 0;
    public const int UP_LEFT = 1;
    public const int LEFT = 2;
    public const int DOWN_LEFT = 3;
    public const int DOWN = 4;
    public const int DOWN_RIGHT = 5;
    public const int RIGHT = 6;
    public const int UP_RIGHT = 7;

    private Animator animator;
    private int lastDirectionValue;

    public static int DirectionToIndex(Vector2 dir, int sliceCount)
    {
        Vector2 normDir = dir.normalized;
        float step = 360f / sliceCount;
        float halfstep = step / 2;
        float angle = Vector2.SignedAngle(Vector2.up, normDir);
        angle += halfstep;

        if (angle < 0)
        {
            angle += 360;
        }

        float stepCount = angle / step;

        return Mathf.FloorToInt(stepCount);
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void trowCharacterCurrentMovement(Vector2 direction)
    {
        int directionValue = DirectionToIndex(direction, 8);
        bool isRunning = false;
        if (direction.magnitude > .01f)
        {
            Debug.Log("Direction :" + directionValue.ToString());
            this.lastDirectionValue = directionValue;
            isRunning = true;
            
        };

        this.setCurrentDirection(lastDirectionValue);
        this.setRunningState(isRunning);
    }

    private void setCurrentDirection(int directionValue)
    {
        animator.SetInteger("Direction", directionValue);
    }

    private void setRunningState(bool isRunning)
    {
        animator.SetBool("isRunning", isRunning);
    }
}
