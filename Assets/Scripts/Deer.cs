using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float runSpeed = 9.0f;
    [SerializeField] private float viewPlayerDist = 20.0f;
    [SerializeField] private float rotSpeed = 1.0f;
    [SerializeField] private float rotWhenRunningSpeed = 3.0f;
    //For efficiency purposes
    private float viewPlayerDistSquared;
    private float viewPlayerDistWhileEatingSqr;

    [Header("Times")]
    [SerializeField] private float minWanderTime = 10.0f;
    [SerializeField] private float maxWanderTime = 30.0f;
    [SerializeField] private float minIdleTime = 3.0f;
    [SerializeField] private float maxIdleTime = 8.0f;
    [SerializeField] private float minEatTime = 10.0f;
    [SerializeField] private float maxEatTime = 20.0f;
    [SerializeField] private float decayTime = 5.0f;

    

    private DeerState state = DeerState.Wander;

    private CharacterController cc;
    private AnimalAnimator anim;

    private float idleTimer = 0.0f;
    private float rotationTimer = 0.0f;
    private float intendedYaw = 0.0f;

    private readonly Vector3 gravity = Vector3.down * 30;

    #region Monobehavior
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<AnimalAnimator>();
    }

    private void Start()
    {
        state = DeerState.Wander;
        idleTimer = Random.Range(minWanderTime, maxWanderTime);
        SetNewYaw();
        transform.rotation = Quaternion.Euler(0, intendedYaw, 0);

        SetViewDistance(viewPlayerDist);
    }

    private void OnValidate()
    {
        SetViewDistance(viewPlayerDist);
    }

    private void OnEnable()
    {
        GameMaster.Instance.AddDear();
    }

    private void OnDisable()
    {
        if (!GameMaster.InstanceIsNull())
        {
            GameMaster.Instance.KillDear();
        }    
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewPlayerDist);
    }

    private void Update()
    {
        Vector3 move;

        switch (state)
        {
            case DeerState.Idle:
                if (idleTimer <= 0.0f)
                {
                    int coin = Random.Range(0, 4);
                    if(coin == 0)
                    {
                        state = DeerState.Eat;
                        idleTimer = Random.Range(minEatTime, maxEatTime);
                    }
                    else
                    {
                        state = DeerState.Wander;
                        idleTimer = Random.Range(minWanderTime, maxWanderTime);
                    }
                }
                anim.SetFSpeed(0);
                anim.SetEat(false);
                goto default;
            case DeerState.Wander:
                if (rotationTimer <= 0.0f)
                {
                    SetNewYaw();
                }

                if (idleTimer <= 0.0f)
                {
                    state = DeerState.Idle;
                    idleTimer = Random.Range(minIdleTime, maxIdleTime);
                }
                move = transform.TransformDirection(Vector3.forward) * walkSpeed;
                Quaternion yaw = transform.rotation;
                Quaternion intendedRot = Quaternion.Euler(0, intendedYaw, 0);
                transform.rotation = Quaternion.Slerp(yaw, intendedRot, rotSpeed * Time.deltaTime);
                move += gravity;
                anim.SetFSpeed(.5f);
                anim.SetEat(false);
                cc.Move(move * Time.deltaTime);
                goto default;
            case DeerState.Eat:
                if(idleTimer <= 0.0f)
                {
                    state = DeerState.Idle;
                    idleTimer = Random.Range(minIdleTime, maxIdleTime);
                }
                anim.SetFSpeed(0);
                anim.SetEat(true);
                goto default;
            case DeerState.Run:
                Vector3 diff = transform.position - GameMaster.Instance.PlayerInstance.transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diff), rotWhenRunningSpeed * Time.deltaTime);
                transform.eulerAngles = new(0, transform.eulerAngles.y, 0);

                move = transform.TransformDirection(Vector3.forward) * runSpeed;
                move += gravity;
                anim.SetFSpeed(1.0f);
                anim.SetEat(false);
                cc.Move(move * Time.deltaTime);
                if (!PlayerIsInRange(viewPlayerDistSquared))
                {
                    state = DeerState.Wander;
                }
                break;

            case DeerState.Dead:
                break;
            default:
                idleTimer -= Time.deltaTime;
                rotationTimer -= Time.deltaTime;

                if (rotationTimer <= 0.0f)
                {
                    SetNewYaw();
                }

                if (PlayerIsInRange(state != DeerState.Eat ? viewPlayerDistSquared : viewPlayerDistWhileEatingSqr))
                {
                    state = DeerState.Run;
                }
                break;
        }

        if(transform.position.y <= -100.0f)
        {
            Destroy(gameObject);
        }

        if (PlayerIsInRange(9f))
        {
            KillDeer();
        }
        
    }
    #endregion

    public void KillDeer()
    {
        cc.enabled = false;
        state = DeerState.Dead;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 90);
        anim.SetFSpeedImmediate(0);
        anim.StopAnimating(true);
        enabled = false;
        GameMaster.Instance.PlayerInstance.FeedPlayer();
        Destroy(gameObject, decayTime);
    }

    private void SetNewYaw()
    {
        intendedYaw = Random.Range(0, 360);
        rotationTimer = Random.Range(.5f, 3.5f);
    }

    private void SetViewDistance(float viewDistance)
    {
        viewPlayerDist = viewDistance;
        viewPlayerDistSquared = viewDistance * viewDistance;
        viewPlayerDistWhileEatingSqr = viewDistance / 2;
        viewPlayerDistWhileEatingSqr *= viewPlayerDistWhileEatingSqr;
    }

    private bool PlayerIsInRange(float rangeSqr)
    {
        float dist = Vector3.SqrMagnitude(transform.position - GameMaster.Instance.PlayerInstance.transform.position);

        return dist <= rangeSqr;
    }
}

public enum DeerState
{
    Idle,
    Wander,
    Eat,
    Run,
    Dead
}