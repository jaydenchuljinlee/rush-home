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
    [SerializeField] private float slopeHeightDelta = 0.8f;

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
        return Mathf.Lerp(_leftTopYOffset, _rightTopYOffset, t);
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
                return leftTopYOffset + slopeHeightDelta;

            case TerrainChunkType.SlopeDown:
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

        float halfWidth = tileWidth * 0.5f;
        float halfHeight = tileHeight * 0.5f;
        float bottomY = -halfHeight;
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
}
