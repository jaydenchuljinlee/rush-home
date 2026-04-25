using UnityEngine;

/// <summary>
/// 현재 난이도 티어에 맞는 다음 지형 타입을 선택한다.
/// Easy는 평지만, 이후 티어부터 경사와 Gap을 점진적으로 허용한다.
/// </summary>
public class TerrainTypeSequencer : MonoBehaviour
{
    [SerializeField] private float hardGapChance = 0.15f;
    [SerializeField] private float extremeGapChance = 0.25f;

    private DifficultyTier _currentTier = DifficultyTier.Easy;
    private TerrainChunkType _lastType = TerrainChunkType.Flat;

    public DifficultyTier CurrentTier => _currentTier;

    public void SetDifficultyTier(DifficultyTier tier)
    {
        _currentTier = tier;
    }

    public TerrainChunkType GetNextType()
    {
        TerrainChunkType nextType;

        switch (_currentTier)
        {
            case DifficultyTier.Normal:
                nextType = PickConnectedNonGapType();
                break;

            case DifficultyTier.Hard:
                nextType = PickConnectedHardOrExtremeType(hardGapChance);
                break;

            case DifficultyTier.Extreme:
                nextType = PickConnectedHardOrExtremeType(extremeGapChance);
                break;

            default:
                nextType = TerrainChunkType.Flat;
                break;
        }

        _lastType = nextType;
        return nextType;
    }

    public void SetLastType(TerrainChunkType type)
    {
        _lastType = type;
    }

    private TerrainChunkType PickConnectedNonGapType()
    {
        switch (_lastType)
        {
            case TerrainChunkType.SlopeUp:
                return Random.value < 0.5f ? TerrainChunkType.SlopeUp : TerrainChunkType.Flat;

            case TerrainChunkType.SlopeDown:
                return Random.value < 0.5f ? TerrainChunkType.SlopeDown : TerrainChunkType.Flat;

            case TerrainChunkType.Gap:
                return TerrainChunkType.Flat;

            default:
                return PickFlatStartNonGapType();
        }
    }

    private TerrainChunkType PickConnectedHardOrExtremeType(float gapChance)
    {
        if (_lastType != TerrainChunkType.Flat)
        {
            return PickConnectedNonGapType();
        }

        return Random.value < gapChance ? TerrainChunkType.Gap : PickFlatStartNonGapType();
    }

    private static TerrainChunkType PickFlatStartNonGapType()
    {
        int roll = Random.Range(0, 3);
        return roll switch
        {
            1 => TerrainChunkType.SlopeUp,
            2 => TerrainChunkType.SlopeDown,
            _ => TerrainChunkType.Flat
        };
    }
}
