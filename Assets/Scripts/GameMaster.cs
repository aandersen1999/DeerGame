using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniStorm;

public class GameMaster : Singleton<GameMaster>
{
    public event Action OnTimeChange;
    public event Action DeerCountChanged;
    public event Action<bool> OnGamePause;
    //public event Action OnGameResume;

    [SerializeField] private WeatherType[] weatherTypes;
    [SerializeField] private TimeScale timeScale = TimeScale.Slow;
    public TimeScale TimeScale { get { return timeScale; } }
    [SerializeField] private WeatherTypes currentType;

    [SerializeField] private int spawnDeer = 15;
    [SerializeField] private GameObject deerPrefab;

    public InputActions Actions { get; private set; }
    public InputActions.MainGameActions MainGameMap { get; private set; }

    public Player PlayerInstance { get; private set; }
    public int DeerCount { get; private set; }
    public bool GameIsPaused { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Actions = new InputActions();
        MainGameMap = Actions.MainGame;
    }

    private void Start()
    {
        SpawnDeer();
        UpdateTimeScale(timeScale);
        currentType = WeatherTypes.Clear;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        
        Actions.Enable();
        UniStormSystem.Instance.OnHourChangeEvent.AddListener(() => UpdateWeather());
    }

    private void OnDisable()
    {
        if (Actions != null)
        {
            Actions.Disable();
        }

        UniStormSystem.Instance.OnHourChangeEvent.RemoveListener(() => UpdateWeather());
    }

    private void OnDestroy()
    {
        if(Actions != null)
        {
            Actions.Dispose();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            switch (timeScale)
            {
                case TimeScale.Slow:
                    timeScale = TimeScale.Medium;
                    break;
                case TimeScale.Medium:
                    timeScale = TimeScale.Fast;
                    break;
                case TimeScale.Fast:
                    timeScale = TimeScale.Slow;
                    break;
            }
            UpdateTimeScale(timeScale);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();

        if(DeerCount <= 0)
        {
            SpawnDeer();
        }
    }

    private void UpdateTimeScale(TimeScale newScale)
    {
        switch (newScale)
        {
            case TimeScale.Slow:
                UniStormManager.Instance.SetDayLength(5);
                UniStormManager.Instance.SetNightLength(5);
                break;
            case TimeScale.Medium:
                UniStormManager.Instance.SetDayLength(2);
                UniStormManager.Instance.SetNightLength(2);
                break;
            case TimeScale.Fast:
                UniStormManager.Instance.SetDayLength(1);
                UniStormManager.Instance.SetNightLength(1);
                break;
        }
        OnTimeChange?.Invoke();
    }

    private void UpdateWeather()
    {
        float rand = UnityEngine.Random.value;

        if(rand <= .5f)
        {
            rand = UnityEngine.Random.value;
            if(rand <= .25f)
            {
                currentType = WeatherTypes.Clear;
            }
            else if(rand <= .5f)
            {
                currentType = WeatherTypes.Cloudy;
            }
            else if(rand <= .75f)
            {
                currentType = WeatherTypes.Blue;
            }
            else
            {
                currentType = WeatherTypes.Red;
            }
        }
        else if(rand <= .85f)
        {
            rand = UnityEngine.Random.value;
            if(rand <= .33f)
            {
                currentType = WeatherTypes.Foggy;
            }
            else if(rand <= .67f)
            {
                currentType = WeatherTypes.MostlyCloudy;
            }
            else
            {
                currentType = WeatherTypes.OverCast;
            }
        }
        else
        {
            rand = UnityEngine.Random.value;
            if(rand <= .25f)
            {
                currentType = WeatherTypes.Rain;
            }
            else if(rand <= .5f)
            {
                currentType = WeatherTypes.ThunderStorm;
            }
            else if(rand <= .75f)
            {
                currentType = WeatherTypes.Snow;
            }
            else
            {
                currentType = WeatherTypes.ThunderSnow;
            }
        }
        UniStormManager.Instance.ChangeWeatherInstantly(weatherTypes[(int)currentType]);
    }

    public void SetPlayer(Player player)
    {
        PlayerInstance = player;
    }

    public void AddDear()
    {
        DeerCount++;
        DeerCountChanged?.Invoke();
    }

    public void KillDear()
    {
        DeerCount--;
        DeerCountChanged?.Invoke();
    }

    public void PauseGame()
    {
        if (!GameIsPaused)
        {
            Time.timeScale = 0.0f;
            Actions.Disable();
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1.0f;
            Actions.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }
        GameIsPaused = !GameIsPaused;
        OnGamePause?.Invoke(GameIsPaused);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SpawnDeer()
    {
        for(int i = 0; i < spawnDeer; i++)
        {
            float xCoord = UnityEngine.Random.Range(-450.0f, 450.0f);
            float zCoord = UnityEngine.Random.Range(-450.0f, 450.0f);
            Vector3 position = new(xCoord, 0, zCoord);
            Instantiate(deerPrefab, position, Quaternion.identity);
        }
    }
}

public enum TimeScale
{
    Slow,
    Medium,
    Fast
}

public enum WeatherTypes
{
    Clear,
    Cloudy,
    Blue,
    Red,
    MostlyCloudy,
    Foggy,
    OverCast,
    Rain,
    ThunderStorm,
    Snow,
    ThunderSnow,
}