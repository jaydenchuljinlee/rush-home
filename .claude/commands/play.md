---
description: Unity 게임을 플레이하고 결과를 확인한다. 모드를 지정하여 다양한 플레이를 수행할 수 있다.
argument-hint: [모드] [옵션] — 모드: check, watch, test, repro, free, suite
allowed-tools: mcp__coplay-mcp__play_game, mcp__coplay-mcp__stop_game, mcp__coplay-mcp__get_unity_logs, mcp__coplay-mcp__capture_scene_object, mcp__coplay-mcp__capture_ui_canvas, mcp__coplay-mcp__check_compile_errors, mcp__coplay-mcp__list_game_objects_in_hierarchy, mcp__coplay-mcp__get_game_object_info, mcp__coplay-mcp__get_unity_editor_state, mcp__coplay-mcp__save_scene, mcp__coplay-mcp__add_component, mcp__coplay-mcp__set_property, mcp__coplay-mcp__execute_script, Bash, Read, Glob, Grep
model: sonnet
---

# RushHome Unity Play 스킬

Unity 에디터에서 게임을 플레이하고 결과를 확인합니다.

인자: $ARGUMENTS

---

## 플레이 모드 결정

$ARGUMENTS를 파싱하여 아래 모드 중 하나를 선택한다.
인자가 비어 있거나 모드가 명시되지 않으면 `check` 모드로 실행한다.

| 모드 | 키워드 | 설명 |
|------|--------|------|
| `check` | check, 확인, 체크, (빈 인자) | 컴파일 에러 확인 + 3초 플레이 + 에러 로그 확인 + 스크린샷 |
| `watch` | watch, 관찰, N초 | 지정 시간(기본 10초) 동안 플레이하며 로그와 화면을 관찰 |
| `test` | test, 테스트, 검증 + 기능명 | 특정 기능이 동작하는지 플레이로 검증 |
| `repro` | repro, 재현, 버그 | 버그 재현을 위해 플레이하고 증상을 캡처 |
| `free` | free, 자유, 프리 | 플레이만 시작하고 사용자가 직접 조작 (stop하지 않음) |
| `suite` | suite, 전체, 통합 | 구현된 모든 기능을 순차 검증 (play-suite.md 기반) |

---

## 공통 사전 단계

### 1. 컴파일 에러 확인
```
mcp__coplay-mcp__check_compile_errors
```
에러가 있으면 에러 내용을 사용자에게 보고하고 **중단**한다.

### 2. 에디터 상태 확인
```
mcp__coplay-mcp__get_unity_editor_state
```
이미 Play Mode 중이면 `stop_game` 후 진행한다.

---

## 모드별 실행

### check 모드 (기본)

1. `play_game` 실행
2. 3초 대기
3. `get_unity_logs` (show_errors: true, show_warnings: true, limit: 20)
4. `capture_scene_object` -- 게임 화면 스크린샷
5. `stop_game`

**출력:**
```
## Play: check

### 에러: 없음 / N건
### 스크린샷
### 결과: PASS / FAIL
```

---

### watch 모드

인자에서 숫자를 추출하여 플레이 시간으로 사용 (기본 10초, 최대 60초).

1. `play_game` 실행
2. 총 시간을 3등분하여 각 구간마다 로그 + 스크린샷
3. `stop_game`

---

### test 모드

1. `list_game_objects_in_hierarchy` (onlyPaths: false)
2. `play_game` 실행
3. 5초 대기
4. `get_unity_logs` + `get_game_object_info` + `capture_scene_object`
5. `stop_game`

---

### repro 모드

1. `list_game_objects_in_hierarchy` (onlyPaths: false)
2. `play_game` 실행
3. 5초 대기
4. `get_unity_logs` (limit: 50) + `get_game_object_info` + `capture_scene_object`
5. `stop_game`

---

### free 모드

1. `play_game` 실행
2. "게임이 시작되었습니다. Unity 에디터에서 직접 플레이하세요." 출력
3. **stop_game을 호출하지 않는다**

---

### suite 모드

`.claude/play-suite.md`를 읽어 등록된 기능을 순차 검증.

1. `play-suite.md` 파싱
2. `list_game_objects_in_hierarchy`
3. `play_game` + `execute_script`로 GameManager.Instance.StartGame() 호출
4. 각 항목별 검증 (대기 → 로그 → 상태 → 스크린샷 → 판정)
5. `stop_game`

**FAIL 시**: `.claude/bugs/`에 리포트 작성. 직접 수정 금지.

---

## 특수 인자

| 인자 | 동작 |
|------|------|
| `stop`, `종료` | `stop_game` 호출 |
| `log`, `로그` | `get_unity_logs` |
| `shot`, `캡처` | `capture_scene_object` |
| `fix`, `수정` | `.claude/bugs/`의 open 리포트를 순차 수정 |

---

## fix 모드

`.claude/bugs/`의 `status: open` 리포트를 severity 순으로 `@bug-fix-orchestrator`에 위임.
수정 완료 시 `status: fixed`, 실패 시 `status: blocked`로 업데이트.

---

## 에러 발생 시 처리

- 에러 내용을 **그대로 사용자에게 보고**
- 자동 수정은 하지 않음 (`/bug-fixer`로 별도 처리)
- `[INPUT]` 에러만 `execute_script`로 자동 수정 후 재시도 1회 허용

## 주의사항

- Play Mode 중 `save_scene` 호출 금지
- 스크린샷 캡처 실패 시 무시하고 진행
- watch 모드 대기는 `Bash`의 `sleep`으로 구현
