using UnityEngine;

/// <summary>
/// 청크 프리팹과 메타데이터를 묶어 관리하는 ScriptableObject.
/// 난이도 필터링에 사용한다.
/// </summary>
[CreateAssetMenu(fileName = "ChunkData", menuName = "HomeRun/Chunk Data")]
public class ChunkData : ScriptableObject
{
    [Tooltip("청크 프리팹 (지면 + 장애물 배치 포함)")]
    [SerializeField] private GameObject _prefab;

    [Tooltip("이 청크가 등장 가능한 최소 경과 시간(초)")]
    [SerializeField] private float _minAppearTime = 0f;

    [Tooltip("이 청크의 폭 (다음 청크 배치 기준점)")]
    [SerializeField] private float _chunkWidth = 20f;

    public GameObject Prefab => _prefab;
    public float MinAppearTime => _minAppearTime;
    public float ChunkWidth => _chunkWidth;
}
