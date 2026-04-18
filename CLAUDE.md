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

## 파이프라인 규칙

### 구현 완료 후 반드시 에디터 플레이 검증
- feature-orchestrator Phase 5에서 `play_game` -> `get_unity_logs` -> `capture_scene_object` -> `stop_game` 순서로 실행
- 사용자가 변경사항을 직접 확인할 수 있도록 스크린샷을 캡처하여 전달한다
- 에러 발생 시 즉시 수정하고 재검증 (최대 2회)
