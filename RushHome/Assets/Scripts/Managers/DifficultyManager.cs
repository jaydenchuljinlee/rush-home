using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] DifficultyData difficultyData;

    public float CurrentSpeed { get; private set; }
    public DifficultyData.DifficultyTier CurrentTier { get; private set; }
    public float SpawnIntervalMin { get; private set; }
    public float SpawnIntervalMax { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateDifficulty(0f);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            UpdateDifficulty(GameManager.Instance.ElapsedTime);
        }
    }

    void UpdateDifficulty(float time)
    {
        CurrentSpeed = difficultyData.GetSpeedForTime(time);
        var settings = difficultyData.GetSettingsForTime(time);
        CurrentTier = settings.tier;
        SpawnIntervalMin = settings.spawnIntervalMin;
        SpawnIntervalMax = settings.spawnIntervalMax;
    }
}
