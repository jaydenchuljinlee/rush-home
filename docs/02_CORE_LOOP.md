# 2단계 기획 및 구현 문서: Core Loop

## 목표

하나의 테마에 묶이지 않은 기본 러너 재미를 완성한다. 이 단계가 끝나면 에셋이 전부 큐브여도 반복 플레이가 가능해야 한다.

## 핵심 루프

```text
전진 -> 장애물 예측 -> 레인 변경/점프/슬라이드 -> 거리 증가 -> 난이도 상승 -> 더 어려운 패턴 출현
```

## 구현 범위

- 장애물 타입 표준화
- 스폰 규칙 개선
- `PlayerVitals` 기반 피로도/점수 기본 시스템
- 난이도 데이터 확장
- 스테이지 전환을 위한 데이터 구조 준비

## 장애물 타입

| 타입 | 기능 | 현재 기반 | 추가 필요 |
| --- | --- | --- | --- |
| Ground | 점프로 회피 | `groundObstacles` | 스테이지별 프리팹 테이블 |
| Air | 슬라이드로 회피 | `airObstacles` | 높이 기준 통일 |
| LaneBlock | 레인 변경으로 회피 | `laneObstacles` | 연속 배치 제한 |
| SlowZone | 접촉 시 감속/피로 증가 | 없음 | 새 효과 컴포넌트 |
| Lethal | 즉시 실패 또는 큰 피로 | `LethalEffect` | 피로도 시스템과 연결 |

## 스폰 설계

초기 `ObstacleSpawner`는 랜덤 단일 장애물 중심이다. Core Loop 단계에서는 불공정한 패턴을 막기 위해 다음 규칙을 추가한다.

- 같은 Z 위치에서 3개 레인을 모두 막지 않는다.
- 점프 장애물 직후 바로 슬라이드 장애물을 붙이지 않는다.
- 최소 반응 거리를 보장한다.
- 난이도 단계별로 한 번에 등장 가능한 장애물 수를 늘린다.
- 스테이지별 스폰 테이블을 받을 수 있게 구조를 열어둔다.

## 피로도 시스템

구현 기준:

- `PlayerVitals`를 추가한다.
- 값 범위는 0-100.
- 작은 충돌은 +10, 큰 충돌은 +30, 치명 충돌은 즉시 100.
- 시간이 지날수록 아주 천천히 증가하거나, Stage 4에서 감소한다.
- UI는 숫자보다 바 형태를 우선한다.
- 피로도 변경, 충돌 횟수 변경, 탈진 상태는 이벤트로 외부에 알린다.
- 장애물 효과는 `PlayerVitals` 또는 `PlayerController`의 공개 API를 호출하고 UI를 직접 수정하지 않는다.

## 점수 시스템

점수는 플레이어의 귀가 성과를 보여준다.

```text
score = distance * 10 + remainingFatigueBonus - hitPenalty + stageClearBonus
```

초기에는 거리와 생존 시간만 표시해도 충분하다. 결과 화면에서는 피로도, 충돌 횟수, 도착 시간을 함께 보여준다.

## 구현 작업

1. 데이터 구조
   - `StageDefinition` ScriptableObject를 만든다.
   - 최소 필드:
     - `stageId`: 저장/분기용 고유 ID
     - `displayName`: UI 표시명
     - `targetDistance`: 다음 목표 트리거 활성화 거리
     - `speedMultiplier`: 현재 난이도 속도에 곱할 배율
     - `ambientColor`: 조명/안개 기준 색
     - `groundPrefab`: 구간 지면 프리팹
     - `obstaclePatterns`: 스폰 가능한 패턴 목록
     - `goalPrefab`: 스테이지 종료 트리거 프리팹
   - `DifficultyData`는 전체 속도 곡선을 맡고, `StageDefinition`은 테마와 패턴을 맡는다.

2. 스테이지 진행
   - `StageManager`를 추가한다.
   - 현재 `StageDefinition`과 스테이지 시작 Z 위치를 보관한다.
   - 플레이어의 현재 Z와 시작 Z의 차이로 스테이지 진행 거리를 계산한다.
   - `targetDistance`에 도달하면 `goalPrefab` 또는 기존 목표 트리거를 활성화한다.
   - `StageExitTrigger`는 한 번만 발동하고 `StageManager.AdvanceStage()`를 호출한다.

3. 스폰 패턴
   - `ObstaclePattern` 데이터 구조를 만든다.
   - 최소 필드: `patternId`, `weight`, `minDifficultyTier`, `laneSlots`, `obstaclePrefab`, `zOffset`, `requiresJump`, `requiresSlide`.
   - 같은 패턴 안에서도 적어도 하나의 안전 레인을 남긴다.

4. 스폰 개선
   - `ObstacleSpawner`가 현재 스테이지의 장애물 테이블을 참조하게 한다.
   - 단일 랜덤 대신 패턴 단위 스폰을 지원한다.
   - 최소 안전 레인을 계산한다.

5. 피로도 연결
   - 장애물 효과가 `GameManager.GameOver()`를 직접 호출하기보다 플레이어 상태에 영향을 주게 정리한다.
   - 치명 효과는 MVP에서는 기존처럼 즉시 실패를 유지해도 된다.

6. UI
   - 거리
   - 현재 스테이지명
   - 피로도 바
   - 게임오버 결과

7. 밸런싱
   - Easy 0-30초: 단일 장애물 위주
   - Normal 30-60초: 2레인 위협
   - Hard 60-120초: 이동 장애물 또는 연속 패턴
   - Extreme 120초 이후: 반응 거리 축소, 혼합 패턴

## 완료 기준

- 2분 이상 플레이해도 장애물 스폰이 끊기지 않는다.
- 피할 수 없는 패턴이 생성되지 않는다.
- 점프/슬라이드/레인 변경 회피가 각각 명확한 역할을 가진다.
- 피로도나 충돌 규칙이 UI에 반영된다.
- `StageManager`가 거리 기준으로 목표 트리거를 활성화한다.
- `StageExitTrigger`가 한 번만 발동한다.
- 스테이지별 장애물 테이블이 `ObstacleSpawner`에 연결된다.

## 테스트 체크리스트

- 3레인이 동시에 막히는 경우가 없는가
- 스폰 간격이 현재 속도에 비례해 과도하게 좁아지지 않는가
- 같은 타입 장애물이 너무 길게 반복되지 않는가
- 장애물 풀링 오브젝트가 정상 반환되는가
- 피로도가 중복으로 여러 번 증가하지 않는가
- Stage 1 목표 거리 도달 전에는 목표 트리거가 비활성인가
- 목표 트리거 접촉 후 다음 `StageDefinition`이 적용되는가
