using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 청크를 이어 붙여 무한 맵을 생성하는 스포너.
/// DifficultyData를 참조해 현재 난이도에 맞는 청크를 선택한다.
/// GroundScroller를 대체하거나 병행할 수 있다.
/// </summary>
public class ChunkSpawner : MonoBehaviour
{
    [Header("청크 설정")]
    [SerializeField] private ChunkData[] _chunkDataList;
    [SerializeField] private DifficultyData _difficultyData;

    [Header("스폰 위치")]
    [SerializeField] private float _spawnX = 15f;
    [SerializeField] private float _spawnY = 0f;

    [Header("풀 설정")]
    [SerializeField] private int _poolInitialSize = 4;
    [SerializeField] private int _poolMaxSize = 8;

    private float _nextSpawnX;

    // ChunkData별 풀을 딕셔너리로 관리
    private Dictionary<ChunkData, ObjectPool<Chunk>> _pools;

    // 스폰된 청크 추적 (디버그용)
    private readonly List<Chunk> _activeChunks = new List<Chunk>();

    private void Awake()
    {
        _pools = new Dictionary<ChunkData, ObjectPool<Chunk>>();

        foreach (ChunkData data in _chunkDataList)
        {
            if (data == null || data.Prefab == null) continue;

            Chunk prefabChunk = data.Prefab.GetComponent<Chunk>();
            if (prefabChunk == null)
            {
                Debug.LogWarning($"[ChunkSpawner] {data.name} 프리팹에 Chunk 컴포넌트가 없습니다.");
                continue;
            }

            _pools[data] = new ObjectPool<Chunk>(prefabChunk, _poolInitialSize, _poolMaxSize, transform);
        }

        _nextSpawnX = _spawnX;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        // 시작 시 화면을 채울 청크를 미리 스폰
        PrewarmChunks();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        // 다음 청크 스폰 위치가 화면 안으로 들어오면 새 청크를 이어 붙임
        if (_nextSpawnX < _spawnX + 5f)
        {
            SpawnNextChunk();
        }
    }

    /// <summary>시작 시 화면을 채울 청크를 이어 붙인다.</summary>
    private void PrewarmChunks()
    {
        float fillUntilX = _spawnX + 20f;
        _nextSpawnX = -_spawnX;

        while (_nextSpawnX < fillUntilX)
        {
            SpawnNextChunk();
        }
    }

    private void SpawnNextChunk()
    {
        ChunkData selectedData = SelectChunkData();
        if (selectedData == null) return;

        if (!_pools.TryGetValue(selectedData, out ObjectPool<Chunk> pool)) return;

        Chunk chunk = pool.Get();
        if (chunk == null) return;

        float elapsedTime = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : 0f;
        float scrollSpeed = _difficultyData != null
            ? _difficultyData.GetScrollSpeed(elapsedTime)
            : 8f;

        chunk.transform.position = new Vector3(_nextSpawnX, _spawnY, 0f);
        chunk.Initialize(scrollSpeed, (c) => ReturnChunk(selectedData, c));

        _nextSpawnX += selectedData.ChunkWidth;
        _activeChunks.Add(chunk);
    }

    private void ReturnChunk(ChunkData data, Chunk chunk)
    {
        _activeChunks.Remove(chunk);

        if (_pools.TryGetValue(data, out ObjectPool<Chunk> pool))
            pool.Return(chunk);
        else
            Destroy(chunk.gameObject);
    }

    /// <summary>현재 경과 시간 기준으로 사용 가능한 ChunkData 중 하나를 무작위 선택한다.</summary>
    private ChunkData SelectChunkData()
    {
        if (_chunkDataList == null || _chunkDataList.Length == 0) return null;

        float elapsed = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : 0f;

        List<ChunkData> candidates = new List<ChunkData>();
        foreach (ChunkData data in _chunkDataList)
        {
            if (data != null && elapsed >= data.MinAppearTime)
                candidates.Add(data);
        }

        if (candidates.Count == 0) return _chunkDataList[0];

        return candidates[Random.Range(0, candidates.Count)];
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            // 스크롤 속도 동기화는 각 Chunk가 매 Update에서 처리
        }
    }
}
