using UnityEngine;

/// <summary>
/// 3겹 배경 파랄랙스 효과를 관리.
/// GroundScroller의 scrollSpeed를 기준으로 각 레이어를 서로 다른 속도로 스크롤.
/// 게임 상태(Playing/GameOver)에 따라 스크롤 시작/정지.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private GroundScroller groundScroller;
    [SerializeField] private ParallaxLayer[] layers;
    [SerializeField] private Camera viewCamera;

    private void Awake()
    {
        if (viewCamera == null)
        {
            viewCamera = Camera.main;
        }

        foreach (ParallaxLayer layer in layers)
        {
            if (layer != null)
            {
                layer.Init(viewCamera);
            }
        }
    }

    private void Update()
    {
        if (groundScroller == null)
        {
            return;
        }

        bool isPlaying = GameManager.IsPlaying;
        float speed = groundScroller.ScrollSpeed;

        foreach (ParallaxLayer layer in layers)
        {
            if (layer != null)
            {
                layer.Tick(speed, isPlaying);
            }
        }
    }
}
