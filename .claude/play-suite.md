# Play Suite - 기능별 플레이 검증 체크리스트

> feature-orchestrator가 기능 구현 완료 시 자동으로 항목을 추가한다.
> `/play suite` 실행 시 이 파일을 읽어 순차 검증한다.

---

## 전제 조건
- play_game 실행 후 1초 대기
- `execute_script`로 아래 2가지를 실행:
  1. **무적 모드 활성화**: 리플렉션으로 `PlayerController.OnPlayerHit` 이벤트 구독자를 제거하여 장애물 충돌 시 GameOver 방지. 물리 충돌은 정상 유지됨.
     ```csharp
     var field = typeof(PlayerController).GetField("OnPlayerHit",
         BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
     field.SetValue(null, null); // 이벤트 구독자 제거
     ```
  2. **게임 시작**: `GameManager.Instance.StartGame()` 호출하여 Playing 상태 전환
- Playing 상태 전환 후부터 각 항목의 대기 시간 적용

---

## 검증 구조

Suite는 **2 Phase**로 나뉜다:

### Phase 1: 무적 모드 (기능 동작 검증)
- 무적 모드 활성화 상태에서 F-01 ~ F-04, F-06을 검증한다.
- 시간 경과가 필요한 항목(F-06)은 **구간별로 멈추고 상태를 검증**한다.
- 무적 모드이므로 장애물 충돌 없이 장시간 플레이 가능.

### Phase 2: 일반 모드 (실패 케이스 검증)
- Phase 1 완료 후 `stop_game` → `play_game`으로 새 세션 시작 (무적 모드 **없이**).
- F-05 GameOver 흐름을 자연스러운 충돌로 검증한다.
- GameOver UI 표시, 타이머 정지, RestartButton 존재를 확인한다.

---

## Phase 1: 무적 모드

### F-01: 씬 기초 세팅
- 오브젝트: GameManager, MainCamera
- 검증: 게임 시작 시 에러 없이 Playing 상태 전환
- 대기: 2초
- 판정: 에러 로그 0건

### F-02: 달리는 지면
- 오브젝트: GroundScroller
- 검증: Playing 상태에서 지면 타일이 왼쪽으로 스크롤
- 대기: 3초
- 판정: GroundScroller 하위 오브젝트의 X 위치가 변화

### F-03: 플레이어 캐릭터
- 오브젝트: Player
- 검증: 플레이어가 지면 위에 서 있고, 가라앉지 않음
- 대기: 0초 (F-02 시점에서 이미 확인 가능)
- 판정: Player Y 위치 >= 지면 높이, NullReferenceException 없음

### F-04: 장애물 시스템
- 오브젝트: ObstacleSpawner, ObstaclePool
- 검증: 장애물이 스폰되어 왼쪽으로 이동 (플레이어 무적이므로 통과)
- 대기: 5초 (누적 ~10초)
- 판정: 씬에 GroundObstacle 또는 AirObstacle 인스턴스 1개 이상 존재

### F-06: 난이도 곡선 (시간 구간별 검증)
- 오브젝트: DifficultyManager, GroundScroller, ObstacleSpawner
- 검증 방법: **`execute_script`로 구간별 상태를 검증**한다.
  - `execute_script`에서 DifficultyData를 리플렉션으로 접근하여 단계별 반환값 확인.
  - 런타임 적용 검증: 현재 ElapsedTime에 해당하는 ScrollSpeed와 SpawnInterval이 실제 적용되었는지 확인.
- 검증 항목:

  | 구간 | 시간 | 속도 | 스폰 간격 (min~max) |
  |------|------|------|---------------------|
  | Easy | 0~29초 | 6 | 1.5 ~ 3.0 |
  | Normal | 30~59초 | 8 | 1.2 ~ 2.5 |
  | Hard | 60~119초 | 11 | 0.9 ~ 2.0 |
  | Extreme | 120초~ | 15 | 0.6 ~ 1.5 |

- 실행 절차:
  1. F-04 검증 시점(~10초)에서 **Easy 런타임 값 확인**: ScrollSpeed == 6, SpawnInterval == 1.5~3.0
  2. `execute_script`로 **DifficultyData 단계별 반환값 검증** (GetSpeedForTime, GetSpawnIntervalForTime)
  3. 30초까지 대기 후 **Normal 런타임 값 확인**: ScrollSpeed == 8, SpawnInterval == 1.2~2.5
  4. 스크린샷 캡처로 시각적 변화(속도 증가) 확인
- 판정: 모든 구간의 런타임 값이 기대값과 일치

---

## F-DEBUG: 에디터 전용 무적 토글 (DebugCheatManager)
- 오브젝트: DebugManager
- 검증: F1 키 입력 시 OnPlayerHit 구독자 제거 (무적 ON), 재입력 시 복원 (무적 OFF)
- 대기: 2초
- 판정: 무적 ON 후 OnPlayerHit backing field == null, 무적 OFF 후 != null. DebugManager 오브젝트에 DebugCheatManager 컴포넌트 존재.

---

## Phase 2: 일반 모드 (실패 케이스)

### F-05: 인게임 HUD / 게임오버
- 오브젝트: Canvas, Canvas/ReadyPanel, Canvas/GameOverPanel, Canvas/TimeText
- 실행 절차:
  1. Phase 1의 `stop_game` 후, `play_game`으로 **새 세션 시작** (무적 모드 없음)
  2. `execute_script`로 `GameManager.Instance.StartGame()` 호출
  3. **Playing 상태 검증**: ReadyPanel 비활성화 + TimeText 값 변화 확인
  4. 장애물 충돌까지 대기 (최대 15초, 1초 간격으로 GameState 폴링)
  5. **GameOver 검증**:
     - GameOverPanel 활성화 확인
     - TimeText 비활성화 확인
     - FinalTimeText에 경과 시간 표시 확인
     - RestartButton 존재 확인
  6. 15초 내 GameOver 미발생 시: `execute_script`로 수동 `GameManager.Instance.GameOver()` 후 검증
- 판정:
  - Playing 중 ReadyPanel 비활성화
  - GameOver 후 GameOverPanel 활성화 + FinalTimeText 값 > 0 + RestartButton 존재

---

## F-AIR: AirObstacle 좌우 이동
- 오브젝트: ObstacleSpawner, AirObstacle (인스턴스)
- 검증: 30초 이후 AirObstacle에 AirObstacleMover 컴포넌트가 부착되어 있으면 Y축 왕복 이동 활성화
- 대기: 5초 (누적 ~35초, Normal 구간 진입 후)
- 판정: AirObstacleMover 컴포넌트가 있는 오브젝트에서 IsMoving == true, 에러 로그 없음

---

## F-13: 레벨 패턴 다양화
- 오브젝트: DifficultyManager, ObstacleSpawner, PatternSpawner (선택), TerrainChunkSpawner (선택)
- 검증: DifficultyManager가 씬에 존재하고, PatternSpawner/TerrainChunkSpawner 컴포넌트가 씬에 추가되면 난이도에 따라 패턴 풀 및 청크 타입이 변경됨
- 대기: 3초
- 판정: DifficultyManager 오브젝트 존재 + 컴파일 에러 없음 + 게임 시작 후 에러 로그 없음. Inspector에서 PatternSpawner/TerrainChunkSpawner를 DifficultyManager에 연결하면 패턴 스폰 활성화됨.

---

## F-15: 코드 구조 리팩토링
- 오브젝트: GroundScroller, GameManager
- 검증: 게임 시작 시 에러 없이 동작, GroundScroller tileWidth=16 적용 상태에서 지면 타일이 정상 순환
- 대기: 3초
- 판정: 에러 로그 0건 + GroundScroller 하위 타일 X 위치가 순환(화면 밖 → 오른쪽 재배치)

---

## F-14: 지형 변화 (경사/곡선/틈새)
- 오브젝트: GroundScroller, GroundTile_0, GroundTile_1, GroundTile_2, TerrainTypeSequencer
- 검증: TerrainTypeSequencer를 Normal 이상 티어로 설정 시 CurveUp/CurveDown/SlopeUp/SlopeDown/Gap 타입이 타일에 적용되며, 인접 타일 접합부 높이가 연속됨
- 대기: 3초
- 판정: ForceTerrainCurve 스크립트로 강제 적용 시 각 TerrainTile의 vertexCount == 18 (CurveUp/CurveDown), 접합부 RightTopYOffset == 다음 타일 LeftTopYOffset (오차 0.001 이내), 에러 로그 없음
