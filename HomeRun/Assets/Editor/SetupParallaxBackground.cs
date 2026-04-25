using UnityEngine;
using UnityEditor;

/// <summary>
/// 파랄랙스 배경 씬 설정 스크립트.
/// ParallaxBackground 오브젝트에 3개 레이어를 구성하고 각 레이어에 타일 2개를 배치한다.
/// </summary>
public class SetupParallaxBackground
{
    public static void Execute()
    {
        // 레이어 설정 (이름, speedMultiplier, tileWidth, Y위치, Z순서, 색상)
        var layers = new (string name, float speed, float width, float y, float z, Color color)[]
        {
            ("ParallaxLayer_Sky",  0.1f, 22f,  1.5f, -3f, new Color(0.53f, 0.81f, 0.98f)),   // 하늘: 연한 파란색
            ("ParallaxLayer_Far",  0.3f, 22f,  0.5f, -2f, new Color(0.56f, 0.74f, 0.56f)),   // 먼 산: 연한 초록
            ("ParallaxLayer_Near", 0.6f, 22f, -0.5f, -1f, new Color(0.40f, 0.40f, 0.45f)),   // 가까운 건물: 회색
        };

        // ParallaxBackground 루트 오브젝트 찾기
        GameObject root = GameObject.Find("ParallaxBackground");
        if (root == null)
        {
            Debug.LogError("[SetupParallax] ParallaxBackground 오브젝트를 찾을 수 없습니다.");
            return;
        }

        ParallaxBackground pb = root.GetComponent<ParallaxBackground>();
        if (pb == null)
        {
            Debug.LogError("[SetupParallax] ParallaxBackground 컴포넌트가 없습니다.");
            return;
        }

        // GroundScroller 참조 연결
        GameObject groundScrollerGo = GameObject.Find("GroundScroller");
        if (groundScrollerGo != null)
        {
            GroundScroller gs = groundScrollerGo.GetComponent<GroundScroller>();
            if (gs != null)
            {
                var so = new SerializedObject(pb);
                so.FindProperty("groundScroller").objectReferenceValue = gs;
                so.ApplyModifiedProperties();
                Debug.Log("[SetupParallax] GroundScroller 참조 연결 완료");
            }
        }

        // Camera 참조 연결
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            var so = new SerializedObject(pb);
            so.FindProperty("viewCamera").objectReferenceValue = mainCam;
            so.ApplyModifiedProperties();
            Debug.Log("[SetupParallax] Camera 참조 연결 완료");
        }

        // ParallaxLayer 배열 구성
        ParallaxLayer[] layerComponents = new ParallaxLayer[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            var cfg = layers[i];

            // 레이어 오브젝트 찾기
            Transform layerTf = root.transform.Find(cfg.name);
            if (layerTf == null)
            {
                Debug.LogError($"[SetupParallax] {cfg.name} 오브젝트를 찾을 수 없습니다.");
                return;
            }

            GameObject layerGo = layerTf.gameObject;
            layerGo.transform.localPosition = new Vector3(0f, cfg.y, cfg.z);

            ParallaxLayer pl = layerGo.GetComponent<ParallaxLayer>();
            if (pl == null)
            {
                Debug.LogError($"[SetupParallax] {cfg.name}에 ParallaxLayer 컴포넌트가 없습니다.");
                return;
            }

            // speedMultiplier, tileWidth 설정
            var plSo = new SerializedObject(pl);
            plSo.FindProperty("speedMultiplier").floatValue = cfg.speed;
            plSo.FindProperty("tileWidth").floatValue = cfg.width;
            plSo.ApplyModifiedProperties();

            // 기존 자식 타일 제거
            for (int c = layerTf.childCount - 1; c >= 0; c--)
            {
                Object.DestroyImmediate(layerTf.GetChild(c).gameObject);
            }

            // 타일 2개 생성
            for (int t = 0; t < 2; t++)
            {
                GameObject tile = new GameObject($"Tile_{t}");
                tile.transform.parent = layerTf;
                tile.transform.localPosition = new Vector3(t * cfg.width, 0f, 0f);
                tile.transform.localScale = new Vector3(cfg.width, 3f, 1f);

                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = GetWhiteSprite();
                sr.color = cfg.color;
                sr.sortingOrder = -(i + 1) * 10;  // Sky=-10, Far=-20, Near=-30 (뒤에서 앞 순)
            }

            layerComponents[i] = pl;
            Debug.Log($"[SetupParallax] {cfg.name} 구성 완료 (speed={cfg.speed}, width={cfg.width})");
        }

        // layers 배열 할당
        var pbSo2 = new SerializedObject(pb);
        var layersProp = pbSo2.FindProperty("layers");
        layersProp.arraySize = layerComponents.Length;
        for (int i = 0; i < layerComponents.Length; i++)
        {
            layersProp.GetArrayElementAtIndex(i).objectReferenceValue = layerComponents[i];
        }
        pbSo2.ApplyModifiedProperties();

        EditorUtility.SetDirty(root);
        Debug.Log("[SetupParallax] 파랄랙스 배경 씬 설정 완료!");
    }

    private static Sprite GetWhiteSprite()
    {
        // Unity 기본 흰색 스프라이트 생성
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
