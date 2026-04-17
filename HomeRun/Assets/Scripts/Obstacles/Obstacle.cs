using UnityEngine;

/// <summary>
/// 기본 장애물. 왼쪽으로 이동하며, 화면 밖으로 나가면 자신을 파괴.
/// Collider2D를 Trigger로 설정하고, Tag를 "Obstacle"로 지정할 것.
/// </summary>
public class Obstacle : MonoBehaviour
{
    [SerializeField] private float destroyX = -15f;

    private float _scrollSpeed;

    public void Initialize(float scrollSpeed)
    {
        _scrollSpeed = scrollSpeed;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        transform.position += Vector3.left * _scrollSpeed * Time.deltaTime;

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}
