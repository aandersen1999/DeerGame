using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Text timeDisplay;
    [SerializeField] private Text deerDisplay;
    [SerializeField] private Text FPSDisplay;
    [SerializeField] private Slider hungerMeter;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private float FPSDisplayTimer = .5f;

    private int UIDeerCount = 0;

    private IEnumerator FPSCounterRoutine;

    protected override void Awake()
    {
        base.Awake();
        FPSCounterRoutine = FPSCounter();
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        GameMaster.Instance.OnTimeChange += UpdateTimeChange;
        GameMaster.Instance.OnGamePause += OnPause;
        //GameMaster.Instance.DeerCountChanged += UpdateDeerCount;
        StartCoroutine(FPSCounterRoutine);
    }

    private void OnDisable()
    {
        if (!GameMaster.InstanceIsNull())
        {
            GameMaster.Instance.OnTimeChange -= UpdateTimeChange;
            GameMaster.Instance.OnGamePause -= OnPause;
            //GameMaster.Instance.DeerCountChanged -= UpdateDeerCount;
            StopCoroutine(FPSCounterRoutine);
        }
    }

    private void Update()
    {
        if(UIDeerCount != GameMaster.Instance.DeerCount)
        {
            UIDeerCount = GameMaster.Instance.DeerCount;
            deerDisplay.text = $"Deer: {GameMaster.Instance.DeerCount}";
        }
        hungerMeter.value = GameMaster.Instance.PlayerInstance.Hunger;
    }

    private void UpdateTimeChange()
    {
        timeDisplay.text = GameMaster.Instance.TimeScale.ToString();
    }

    private void OnPause(bool paused)
    {
        if (paused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }

    private void UpdateDeerCount()
    {
        deerDisplay.text = $"Deer: {GameMaster.Instance.DeerCount}";
    }

    private IEnumerator FPSCounter()
    {
        float deltaTime = 0.0f;
        while (true)
        {
            deltaTime += (Time.deltaTime - deltaTime) * .1f;
            float display = 1.0f / deltaTime;
            FPSDisplay.text = $"FPS: {Mathf.Ceil(display)}";
            yield return new WaitForSeconds(FPSDisplayTimer);
        }
    }
}
