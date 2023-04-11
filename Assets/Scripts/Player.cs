using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float maxLookAngle = 80;

    [SerializeField] private Transform playerCamPOI;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float fVel = 0.0f;
    private Vector2 horizontalVelocity = Vector2.zero;

    private CharacterController cc;
    private InputActions.MainGameActions controller;
    private AnimalAnimator anim;

    public float Hunger { get; private set; }

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<AnimalAnimator>();

        Hunger = 100.0f;
    }

    private void Start()
    {
        GameMaster.Instance.SetPlayer(this);
        controller = GameMaster.Instance.MainGameMap;
    }

    private void Update()
    {
        HandleHunger();

        Vector2 _move = controller.Move.ReadValue<Vector2>();
        Vector2 _look = controller.Look.ReadValue<Vector2>();

        UpdateStick(_move);

        SetForwardVelocity(_move.magnitude * walkSpeed);

        cc.Move(new Vector3(horizontalVelocity.x, -30, horizontalVelocity.y) * Time.deltaTime);

        yaw += _look.x * lookSensitivity;
        pitch -= _look.y * lookSensitivity;

        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);
        playerCamPOI.eulerAngles = new Vector3(pitch, yaw, 0);

        if(transform.position.y <= -100.0f)
        {
            ForcePlayerPosition(Vector3.zero);
        }
        
    }

    private void SetForwardVelocity(float fVel)
    {
        float rad = Mathf.Deg2Rad * transform.eulerAngles.y;

        this.fVel = fVel;

        horizontalVelocity.x = Mathf.Sin(rad) * this.fVel;
        horizontalVelocity.y = Mathf.Cos(rad) * this.fVel;
    }

    private void UpdateStick(Vector2 move)
    {
        float mag = move.sqrMagnitude;
        float intendedYaw;

        if(mag > .1f)
        {
            intendedYaw = Mathf.Atan2(move.x, move.y) + (Mathf.Deg2Rad * playerCamPOI.eulerAngles.y);
            intendedYaw *= Mathf.Rad2Deg;
            anim.SetFSpeed(1f);
        }
        else
        {
            anim.SetFSpeed(0f);
            intendedYaw = transform.eulerAngles.y;
        }

        transform.rotation = Quaternion.Euler(0, intendedYaw, 0);
    }

    private void HandleHunger()
    {
        if(fVel != 0.0f)
        {
            Hunger -= .5f * Time.deltaTime;
        }
        else
        {
            Hunger -= .25f * Time.deltaTime;
        }
    }

    public void ForcePlayerPosition(Vector3 pos)
    {
        cc.enabled = false;
        transform.position = pos;
        cc.enabled = true;
    }

    public void FeedPlayer()
    {
        Hunger = 100.0f;
    }

    private void GameIsPaused()
    {
        
    }
}
