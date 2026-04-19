# Play Suite - 기능별 플레이 검증 체크리스트

> feature-orchestrator가 기능 구현 완료 시 자동으로 항목을 추가한다.
> `/play suite` 실행 시 이 파일을 읽어 순차 검증한다.

---

## 전제 조건
- play_game 실행 후 1초 대기
- `execute_script`로 `GameManager.Instance.StartGame()` 호출하여 Playing 상태 전환
- Playing 상태 전환 후부터 각 항목의 대기 시간 적용

---

## F-01: 씬 기초 세팅
- 오브젝트: GameManager, MainCamera
- 검증: 게임 시작 시 에러 없이 Ready 상태 대기
- 대기: 2초
- 판정: 에러 로그 0건

## F-02: 달리는 지면
- 오브젝트: GroundScroller
- 검증: Playing 상태에서 지면 타일이 왼쪽으로 스크롤 (GroundScroller의 Transform 변화 확인)
- 대기: 3초
- 판정: GroundScroller 하위 오브젝트의 X 위치가 변화

## F-03: 플레이어 캐릭터
- 오브젝트: Player
- 검증: 플레이어가 지면 위에 서 있고, 가라앉지 않음
- 대기: 3초
- 판정: Player Y 위치 >= 지면 높이, NullReferenceException 없음

## F-04: 장애물 시스템
- 오브젝트: ObstacleSpawner, ObstaclePool
- 검증: 장애물이 스폰되어 왼쪽으로 이동
- 대기: 5초
- 판정: 씬에 GroundObstacle 또는 AirObstacle 인스턴스 1개 이상 존재
