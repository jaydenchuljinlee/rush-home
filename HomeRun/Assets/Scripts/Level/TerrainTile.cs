using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class TerrainTile : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D polygonCollider;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material terrainMaterial;
    [SerializeField] private Color terrainColor = new Color(0.58f, 0.58f, 0.58f, 1f);
    [SerializeField] private float tileWidth = 16f;
    [SerializeField] private float tileHeight = 2f;
    [Tooltip("메시 하단 확장 깊이. 카메라 뷰 아래를 완전히 덮기 위해 충분히 깊게 설정.")]
    [SerializeField] private float bottomExtend = 8f;
    [SerializeField] private float slopeHeightDelta = 0.8f;
    [SerializeField] private int curveSegments = 8;
    [SerializeField] private float curveMagnitudeRatio = 0.3f;

    private TerrainChunkType _currentType = TerrainChunkType.Flat;
    private Mesh _mesh;
    private bool _isInitialized;
    private float _leftTopYOffset;
    private float _rightTopYOffset;

    public TerrainChunkType CurrentType => _currentType;
    public float GroundYOffset => (_leftTopYOffset + _rightTopYOffset) * 0.5f;
    public float LeftTopYOffset => _leftTopYOffset;
    public float RightTopYOffset => _rightTopYOffset;
    public float SlopeHeightDelta => slopeHeightDelta;
    public int CurveSegments => curveSegments;
    public bool HasGround => _currentType != TerrainChunkType.Gap;

    private void Awake()
    {
        InitializeIfNeeded();
        ApplyGeometry();
    }

    private void Reset()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetType(TerrainChunkType type)
    {
        SetType(type, _leftTopYOffset);
    }

    public void SetType(TerrainChunkType type, float leftTopYOffset)
    {
        InitializeIfNeeded();
        _currentType = type;
        _leftTopYOffset = leftTopYOffset;
        _rightTopYOffset = CalculateRightTopYOffset(type, leftTopYOffset);
        ApplyGeometry();
    }

    public float GetGroundYAtLocalX(float localX)
    {
        if (!HasGround)
        {
            return 0f;
        }

        float halfWidth = tileWidth * 0.5f;
        float t = Mathf.InverseLerp(-halfWidth, halfWidth, localX);
        float linearY = Mathf.Lerp(_leftTopYOffset, _rightTopYOffset, t);

        if (_currentType == TerrainChunkType.CurveUp)
        {
            return linearY + Mathf.Sin(t * Mathf.PI) * slopeHeightDelta * curveMagnitudeRatio;
        }

        if (_currentType == TerrainChunkType.CurveDown)
        {
            return linearY - Mathf.Sin(t * Mathf.PI) * slopeHeightDelta * curveMagnitudeRatio;
        }

        return linearY;
    }

    private void InitializeIfNeeded()
    {
        if (_isInitialized)
        {
            return;
        }

        if (polygonCollider == null)
        {
            polygonCollider = GetComponent<PolygonCollider2D>();
        }

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        if (polygonCollider == null)
        {
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        }

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        if (_mesh == null)
        {
            _mesh = new Mesh { name = "TerrainTileMesh" };
            meshFilter.sharedMesh = _mesh;
        }

        if (terrainMaterial == null)
        {
            terrainMaterial = CreateDefaultMaterial();
        }

        meshRenderer.sharedMaterial = terrainMaterial;
        transform.localRotation = Quaternion.identity;
        _isInitialized = true;
    }

    private Material CreateDefaultMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        Material material = new Material(shader);
        material.color = terrainColor;
        return material;
    }

    private float CalculateRightTopYOffset(TerrainChunkType type, float leftTopYOffset)
    {
        switch (type)
        {
            case TerrainChunkType.SlopeUp:
            case TerrainChunkType.CurveUp:
                return leftTopYOffset + slopeHeightDelta;

            case TerrainChunkType.SlopeDown:
            case TerrainChunkType.CurveDown:
                return leftTopYOffset - slopeHeightDelta;

            default:
                return leftTopYOffset;
        }
    }

    private void ApplyGeometry()
    {
        bool visible = HasGround;
        if (meshRenderer != null)
        {
            meshRenderer.enabled = visible;
        }

        if (polygonCollider != null)
        {
            polygonCollider.enabled = visible;
        }

        if (!visible)
        {
            return;
        }

        if (_currentType == TerrainChunkType.CurveUp || _currentType == TerrainChunkType.CurveDown)
        {
            ApplyCurveGeometry();
        }
        else
        {
            ApplyLinearGeometry();
        }
    }

    private void ApplyLinearGeometry()
    {
        float halfWidth = tileWidth * 0.5f;
        float halfHeight = tileHeight * 0.5f;
        float bottomY = -halfHeight - bottomExtend;
        float baseTopY = halfHeight;
        float leftTopY = baseTopY + _leftTopYOffset;
        float rightTopY = baseTopY + _rightTopYOffset;

        Vector3[] vertices =
        {
            new Vector3(-halfWidth, bottomY, 0f),
            new Vector3(-halfWidth, leftTopY, 0f),
            new Vector3(halfWidth, rightTopY, 0f),
            new Vector3(halfWidth, bottomY, 0f)
        };

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.uv = new[]
        {
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f)
        };
        _mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        _mesh.RecalculateBounds();

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, new[]
        {
            new Vector2(-halfWidth, bottomY),
            new Vector2(-halfWidth, leftTopY),
            new Vector2(halfWidth, rightTopY),
            new Vector2(halfWidth, bottomY)
        });
    }

    private void ApplyCurveGeometry()
    {
        int n = Mathf.Max(2, curveSegments);
        float halfWidth = tileWidth * 0.5f;
        float halfHeight = tileHeight * 0.5f;
        float bottomY = -halfHeight - bottomExtend;
        float baseTopY = halfHeight;
        float leftTopY = baseTopY + _leftTopYOffset;
        float rightTopY = baseTopY + _rightTopYOffset;
        float curveBump = slopeHeightDelta * curveMagnitudeRatio;
        bool isUp = _currentType == TerrainChunkType.CurveUp;

        // 꼭짓점: 상단 (n+1)개 + 하단 (n+1)개
        int totalVerts = 2 * (n + 1);
        Vector3[] vertices = new Vector3[totalVerts];
        Vector2[] uvs = new Vector2[totalVerts];
        Vector2[] colliderTop = new Vector2[n + 1];
        Vector2[] colliderBottom = new Vector2[n + 1];

        for (int i = 0; i <= n; i++)
        {
            float t = i / (float)n;
            float x = Mathf.Lerp(-halfWidth, halfWidth, t);
            float linearTop = Mathf.Lerp(leftTopY, rightTopY, t);
            float curveOffset = isUp
                ? Mathf.Sin(t * Mathf.PI) * curveBump
                : -Mathf.Sin(t * Mathf.PI) * curveBump;
            float topY = linearTop + curveOffset;

            // 상단 꼭짓점: 인덱스 i
            vertices[i] = new Vector3(x, topY, 0f);
            uvs[i] = new Vector2(t, 1f);
            colliderTop[i] = new Vector2(x, topY);

            // 하단 꼭짓점: 인덱스 (n+1) + i
            vertices[n + 1 + i] = new Vector3(x, bottomY, 0f);
            uvs[n + 1 + i] = new Vector2(t, 0f);
            colliderBottom[i] = new Vector2(x, bottomY);
        }

        // 삼각형: 각 세그먼트마다 2개 (상단i, 상단i+1, 하단i), (상단i+1, 하단i+1, 하단i)
        int[] triangles = new int[n * 6];
        for (int i = 0; i < n; i++)
        {
            int topLeft = i;
            int topRight = i + 1;
            int botLeft = n + 1 + i;
            int botRight = n + 1 + i + 1;

            triangles[i * 6 + 0] = topLeft;
            triangles[i * 6 + 1] = botLeft;
            triangles[i * 6 + 2] = topRight;
            triangles[i * 6 + 3] = topRight;
            triangles[i * 6 + 4] = botLeft;
            triangles[i * 6 + 5] = botRight;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.uv = uvs;
        _mesh.triangles = triangles;
        _mesh.RecalculateBounds();

        // PolygonCollider2D: 상단 경로(좌→우) + 하단 역순(우→좌)
        Vector2[] colliderPath = new Vector2[2 * (n + 1)];
        for (int i = 0; i <= n; i++)
        {
            colliderPath[i] = colliderTop[i];
        }
        for (int i = 0; i <= n; i++)
        {
            colliderPath[n + 1 + i] = colliderBottom[n - i];
        }

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, colliderPath);
    }
}
