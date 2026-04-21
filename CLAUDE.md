# HomeRun 프로젝트 CLAUDE.md

## Coplay MCP 사용 시 주의사항

### 씬 오브젝트에 컴포넌트 부착 확인 필수
- `create_game_object`로 오브젝트를 만든 후 반드시 `add_component`로 스크립트 컴포넌트를 부착한다
- 완료 후 `list_game_objects_in_hierarchy`(onlyPaths: false)로 컴포넌트가 실제로 붙었는지 검증한다
- 실수 사례: GameManager 오브젝트를 만들었으나 GameManager 컴포넌트가 빠져 게임이 동작하지 않음

### save_scene 경로 지정
- `save_scene`의 `scene_name`에 "GameScene"만 넣으면 `Assets/GameScene.unity`로 저장됨
- 올바른 경로: `scene_name: "Scenes/GameScene"` -> `Assets/Scenes/GameScene.unity`로 저장
- 실수 사례: 중복 씬 파일이 생성되어 혼란 발생

### Input System 설정 확인
- Unity 6 LTS 프로젝트는 기본적으로 New Input System이 활성화되어 있을 수 있음
- 기존 코드가 `UnityEngine.Input` (Old API)을 사용하면 `InvalidOperationException` 발생
- Player Settings > Active Input Handling을 "Both"로 설정해야 Old/New 모두 사용 가능
- 프로젝트 초기 또는 첫 Play 테스트 전에 반드시 확인한다

### Play Mode 중 제약
- `save_scene`은 Play Mode 중 호출 불가 -> 반드시 `stop_game` 후 저장
- Play Mode에서 추가한 컴포넌트/변경은 Play Mode 종료 시 사라짐 -> 에디터 모드에서 작업 후 저장

### SerializeField 값 변경 시 씬/프리팹 동시 반영 필수
- 코드에서 `[SerializeField]` 기본값만 변경하면 Inspector에 이미 직렬화된 값이 우선됨
- 반드시 `set_property` 또는 `execute_script`(SerializedObject)로 **씬 오브젝트와 프리팹 에셋 모두** 업데이트한다
- 프리팹이 `Assets/Prefabs/`에 별도 존재하면, 씬 템플릿과 프리팹 양쪽 모두 변경해야 풀 클론에도 반영됨
- 실수 사례: 씬 템플릿만 scale 0.7로 변경했으나 프리팹은 1.0 → 스폰된 클론 크기 불일치

### execute_script 크로스 어셈블리 제약
- `execute_script`로 컴파일된 코드에서 `GameManager.Instance` 등 **static 프로퍼티는 null** 반환 (게임 어셈블리와 별도 어셈블리)
- `Object.FindFirstObjectByType<T>()`는 정상 작동하지만 static 멤버 접근 불가
- **해결법**: 게임 시작이 필요하면 `SuiteAutoStarter` 같은 **MonoBehaviour를 씬에 부착**하여 게임 어셈블리 컨텍스트에서 실행
- execute_script는 데이터 조회/씬 저장 등 **에디터 작업에만** 사용한다

### 프리팹에 컴포넌트 추가 시 씬 + 프리팹 모두 적용
- `add_component`는 대상(씬 오브젝트 vs 프리팹 에셋)에 따라 적용 범위가 다름
- ObstaclePool이 프리팹에서 Instantiate하면, **프리팹 에셋에도 컴포넌트를 추가**해야 클론에 포함됨
- `prefab_path` 파라미터를 명시하여 프리팹 에셋에도 적용한다
- 실수 사례: AirObstacleMover를 씬 오브젝트에만 추가 → 풀 클론에 누락

## 파이프라인 규칙

### 구현 완료 후 반드시 에디터 플레이 검증
- feature-orchestrator Phase 5에서 `play_game` -> `get_unity_logs` -> `capture_scene_object` -> `stop_game` 순서로 실행
- 사용자가 변경사항을 직접 확인할 수 있도록 스크린샷을 캡처하여 전달한다
- 에러 발생 시 즉시 수정하고 재검증 (최대 2회)
