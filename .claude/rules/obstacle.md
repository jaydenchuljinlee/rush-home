# 장애물 시스템 규칙

## 아키텍처 (인터페이스 기반 컴포넌트 조합)
- `ObstacleBase`: 공통 로직 (풀링, 디스폰, 충돌 감지 → Effect/Movement에 위임)
- `IObstacleEffect`: 충돌 시 효과 인터페이스. 새 효과는 이것을 구현하여 컴포넌트로 붙인다
- `IObstacleMovement`: 이동 패턴 인터페이스. 새 패턴은 이것을 구현하여 컴포넌트로 붙인다
- 프리팹에 Effect + Movement 컴포넌트를 조합하여 다양한 장애물을 코드 수정 없이 생성 가능

## isTrigger 규칙
- **기본값은 반드시 false** (모든 물체는 통과 불가)
- isTrigger=false → OnCollisionEnter로 효과 적용 (물리 차단)
- isTrigger=true → OnTriggerEnter로 효과 적용 (통과형, 향후 아이템/버프존 등에 활용)
- ObstacleBase가 양쪽 모두 HandleCollision으로 위임하므로 프리팹 설정만으로 전환 가능

## 프리팹 재생성 시 주의사항
- 스크립트(.cs)를 삭제하면 프리팹에 **missing script 참조가 남는다** → 프리팹 저장 실패, 컴포넌트 추가 불가
- 해결: 프리팹을 삭제 후 완전히 새로 생성한다 (EditPrefabContentsScope로 missing script 제거 불가)
- **프리팹 재생성 후 씬의 SerializedObject 참조(ObstacleSpawner 등)도 반드시 재연결**한다

## 현재 프리팹 구성

| 프리팹 | 실제 색상 | Effect | Movement | 동작 |
|--------|-----------|--------|----------|------|
| Obs_Ground | 주황 | BlockEffect | Stationary | 물리적으로 막힘만 |
| Obs_Air | 노랑 | LethalEffect | ApproachMovement | 다가오면서 게임오버 |
| Obs_Lane | 보라 | LethalEffect | Stationary | 부딪히면 게임오버 |

## 장애물 색상 참고
- 코드상 머티리얼 이름과 실제 보이는 색상이 다를 수 있다
- M_ObstacleGround → 주황, M_ObstacleAir → 노랑, M_ObstacleLane → 보라
