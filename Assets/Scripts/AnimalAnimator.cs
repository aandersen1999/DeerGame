using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimator : MonoBehaviour
{
    private Animator anim;

    private int SpeedFID;
    private int EatID;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Start()
    {
        SpeedFID = Animator.StringToHash("Speed_f");
        EatID = Animator.StringToHash("Eat_b");
    }

    public void SetFSpeed(float speed)
    {
        speed = Mathf.Clamp01(speed);

        //anim.SetFloat("Speed_f", speed);
        anim.SetFloat(SpeedFID, speed, .1f, Time.deltaTime);
    }

    public void SetEat(bool eat)
    {
        anim.SetBool(EatID, eat);
    }

    public void SetFSpeedImmediate(float speed)
    {
        speed = Mathf.Clamp01(speed);

        anim.SetFloat(SpeedFID, speed);
    }

    public void StopAnimating(bool stop)
    {
        anim.enabled = !stop;
    }
}
