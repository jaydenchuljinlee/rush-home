---
description: Unity 게임을 플레이하고 결과를 확인한다. 모드를 지정하여 다양한 플레이를 수행할 수 있다.
argument-hint: [모드] [옵션] — 모드: check, watch, test, repro, free, suite
allowed-tools: mcp__coplay-mcp__play_game, mcp__coplay-mcp__stop_game, mcp__coplay-mcp__get_unity_logs, mcp__coplay-mcp__capture_scene_object, mcp__coplay-mcp__capture_ui_canvas, mcp__coplay-mcp__check_compile_errors, mcp__coplay-mcp__list_game_objects_in_hierarchy, mcp__coplay-mcp__get_game_object_info, mcp__coplay-mcp__get_unity_editor_state, mcp__coplay-mcp__save_scene, mcp__coplay-mcp__add_component, mcp__coplay-mcp__set_property, mcp__coplay-mcp__execute_script, Bash, Read, Glob, Grep
model: sonnet
---

# HomeRun Unity Play 스킬

Unity 에디터에서 게임을 플레이하고 결과를 확인합니다.

인자: $ARGUMENTS

---

## 플레이 모드 결정

$ARGUMENTS를 파싱하여 아래 5가지 모드 중 하나를 선택한다.
인자가 비어 있거나 모드가 명시되지 않으면 `check` 모드로 실행한다.

| 모드 | 키워드 | 설명 |
|------|--------|------|
| `check` | check, 확인, 체크, (빈 인자) | 컴파일 에러 확인 + 3초 플레이 + 에러 로그 확인 + 스크린샷 |
| `watch` | watch, 관찰, N초 | 지정 시간(기본 10초) 동안 플레이하며 로그와 화면을 관찰 |
| `test` | test, 테스트, 검증 + 기능명 | 특정 기능이 동작하는지 플레이로 검증 |
| `repro` | repro, 재현, 버그 | 버그 재현을 위해 플레이하고 증상을 캡처 |
| `free` | free, 자유, 프리 | 플레이만 시작하고 사용자가 직접 조작 (stop하지 않음) |
| `suite` | suite, 전체, 통합, 전체테스트 | 구현된 모든 기능을 순차 검증 (play-suite.md 기반) |

---

## 공통 사전 단계

모든 모드에서 플레이 전 아래를 먼저 수행한다:

### 1. 컴파일 에러 확인
```
mcp__coplay-mcp__check_compile_errors
```
에러가 있으면 에러 내용을 사용자에게 보고하고 **중단**한다. 자동 수정하지 않는다.

### 2. 에디터 상태 확인
```
mcp__coplay-mcp__get_unity_editor_state
```
이미 Play Mode 중이면 `stop_game` 후 진행한다.

---

## 모드별 실행

### check 모드 (기본)

빠른 스모크 테스트. 게임이 에러 없이 실행되는지 확인한다.

1. `play_game` 실행
2. 3초 대기
3. `get_unity_logs` (show_errors: true, show_warnings: true, limit: 20)
4. `capture_scene_object` -- 게임 화면 스크린샷
5. `stop_game`

**출력:**
```
## Play: check

### 에러: 없음 / N건
{에러가 있으면 에러 메시지 나열}

### 스크린샷
{캡처된 화면}

### 결과: PASS / FAIL
```

---

### watch 모드

지정 시간 동안 플레이하며 변화를 관찰한다. 시간 경과에 따른 동작을 확인할 때 사용.

인자에서 숫자를 추출하여 플레이 시간으로 사용한다 (기본 10초, 최대 60초).

1. `play_game` 실행
2. 총 플레이 시간을 3등분하여 각 구간마다:
   - `get_unity_logs` (show_errors: true, limit: 10)
   - `capture_scene_object` -- 화면 캡처
3. `stop_game`

**출력:**
```
## Play: watch ({N}초)

### 구간별 관찰
| 시점 | 스크린샷 | 에러 |
|------|----------|------|
| {T1}초 | (캡처) | 없음 |
| {T2}초 | (캡처) | 없음 |
| {T3}초 | (캡처) | 없음 |

### 관찰 요약
{시간에 따른 변화 설명 -- 속도 변화, 장애물 등장 패턴, 상태 전이 등}

### 결과: PASS / FAIL
```

---

### test 모드

특정 기능의 동작을 검증한다. 인자에서 기능명/검증 대상을 추출한다.

1. `list_game_objects_in_hierarchy` (onlyPaths: false) -- 관련 오브젝트 확인
2. `play_game` 실행
3. 5초 대기
4. `get_unity_logs` (show_errors: true, show_warnings: true, limit: 30)
5. 검증 대상 오브젝트가 있으면 `get_game_object_info`로 상태 확인
6. `capture_scene_object` -- 전체 화면
7. 검증 대상 오브젝트가 있으면 `capture_scene_object` (gameObjectPath 지정) -- 대상 오브젝트
8. `stop_game`

**출력:**
```
## Play: test — {기능명}

### 검증 대상
- {오브젝트명}: {컴포넌트 목록}

### 오브젝트 상태
{get_game_object_info 결과 요약}

### 에러: 없음 / N건

### 스크린샷
{전체 화면 + 대상 오브젝트 캡처}

### 검증 결과: PASS / FAIL
{기능이 정상 동작하는지 판단 및 근거}
```

---

### repro 모드

버그 재현을 위해 플레이하고 증상을 기록한다. 인자에서 버그 증상을 추출한다.

1. `list_game_objects_in_hierarchy` (onlyPaths: false) -- 씬 구성 기록
2. `play_game` 실행
3. 5초 대기
4. `get_unity_logs` (show_errors: true, show_warnings: true, limit: 50) -- 로그 전수 확인
5. `capture_scene_object` -- 현재 화면
6. 관련 오브젝트가 있으면 `get_game_object_info`로 런타임 상태 확인
7. `stop_game`

**출력:**
```
## Play: repro — {버그 증상}

### 씬 구성
{관련 오브젝트 + 컴포넌트 목록}

### 로그 (전체)
{에러/경고 메시지 전문}

### 런타임 상태
{관련 오브젝트의 Transform, 프로퍼티 값}

### 스크린샷
{캡처된 화면}

### 재현 결과
- 재현 여부: 재현됨 / 재현 안됨
- 관찰된 증상: {설명}
- 추정 원인: {로그와 상태 기반 분석}
```

---

### free 모드

게임을 시작만 하고 사용자가 직접 Unity 에디터에서 조작한다. 자동 종료하지 않는다.

1. `play_game` 실행
2. "게임이 시작되었습니다. Unity 에디터에서 직접 플레이하세요." 메시지 출력
3. **stop_game을 호출하지 않는다**

**출력:**
```
## Play: free

게임이 시작되었습니다. Unity 에디터에서 직접 플레이하세요.
종료하려면 `/play stop` 또는 Unity 에디터에서 정지 버튼을 누르세요.
```

---

### suite 모드

구현된 모든 기능을 한 번의 플레이 세션에서 순차 검증한다.
`.claude/play-suite.md` 파일을 읽어 등록된 기능 목록과 검증 기준을 가져온다.

**실행 절차:**

1. `.claude/play-suite.md` 파일을 읽어 검증 항목 목록을 파싱한다
2. `list_game_objects_in_hierarchy` (onlyPaths: false) -- 씬 전체 구성 확인
3. `play_game` 실행
4. **게임 시작 트리거**: 1초 대기 후 `execute_script`로 GameManager.Instance.StartGame() 호출
   ```csharp
   // execute_script로 실행할 코드
   GameManager.Instance.StartGame();
   ```
   이렇게 하면 Ready → Playing 상태로 자동 전환된다.
5. 각 기능 항목을 순서대로 검증한다:
   - 해당 항목의 "대기" 시간만큼 `sleep` (누적 대기)
   - `get_unity_logs` (show_errors: true, limit: 10) -- 에러 확인
   - 해당 항목의 "오브젝트"에 대해 `get_game_object_info` -- 런타임 상태 확인
   - `capture_scene_object` -- 스크린샷 캡처
   - "판정" 기준에 따라 PASS/FAIL 결정
6. `stop_game`

**판정 기준 해석:**
- "에러 로그 0건" -- 해당 구간에서 새 에러가 없으면 PASS
- "X 위치가 변화" -- get_game_object_info의 Transform 비교 (play 전후)
- "Y 위치 >= N" -- get_game_object_info의 localPosition.y 확인
- "인스턴스 N개 이상 존재" -- list_game_objects_in_hierarchy로 해당 이름 검색

**에러 발생 시:** 해당 항목을 FAIL 처리하고, 나머지 항목은 계속 진행한다 (중단하지 않음).

**중요: suite 중 직접 수정 금지.**
FAIL이 발생해도 코드, 설정, play-suite.md를 직접 수정하지 않는다.
리포트만 작성하고 suite를 끝까지 완료한 뒤, `/play fix`를 안내한다.

**FAIL 항목 버그 리포트 작성:**

FAIL이 1건 이상 발생하면, 각 FAIL 항목마다 `.claude/bugs/` 에 리포트 파일을 작성한다.

파일명: `{F번호}-{kebab-case-요약}.md` (예: `F-03-player-sinking.md`)

```markdown
---
feature: F-{번호}
status: open
severity: {critical / major / minor}
created: {YYYY-MM-DD}
---

# {버그 제목}

## 증상
{관찰된 동작 설명}

## 에러 로그
{에러 메시지 및 스택 트레이스 전문}

## 오브젝트 상태
{get_game_object_info 결과 — Transform, 컴포넌트 프로퍼티 등}

## 스크린샷
{캡처 시점 및 경로}

## 판정 기준 위반
- 기대: {play-suite.md의 판정 기준}
- 실제: {관찰된 결과}
```

severity 기준:
- `critical`: 게임 크래시, 무한 에러 루프
- `major`: 기능 미동작 (장애물 안 나옴, 점프 안 됨 등)
- `minor`: 동작하지만 비정상 (위치 약간 어긋남, 경고 로그 등)

**출력:**
```
## Play: suite

### 검증 결과

| # | 기능 | 오브젝트 | 결과 | 비고 |
|---|------|----------|------|------|
| F-01 | 씬 기초 세팅 | GameManager | PASS | |
| F-02 | 달리는 지면 | GroundScroller | PASS | |
| F-03 | 플레이어 캐릭터 | Player | PASS | |
| F-04 | 장애물 시스템 | ObstacleSpawner | FAIL | 장애물 미스폰 |

### 스크린샷 (구간별)
{각 검증 시점의 캡처 이미지}

### 에러 로그
{FAIL 항목의 에러 상세 내용}

### 버그 리포트
{FAIL 항목이 있으면}
- `.claude/bugs/F-04-obstacle-not-spawning.md` 작성 완료

### 요약: N/M PASS
```

**suite 완료 후 FAIL이 있으면:**

사용자에게 아래 메시지를 출력한다:

```
버그 N건이 `.claude/bugs/`에 리포트되었습니다.
순차 수정을 시작하려면 `/play fix`를 실행하세요.
```

---

## 특수 인자

| 인자 | 동작 |
|------|------|
| `stop`, `종료`, `멈춰` | 현재 Play Mode를 종료한다 (`stop_game` 호출) |
| `log`, `로그` | Play Mode 중 로그만 확인한다 (`get_unity_logs`) |
| `shot`, `스크린샷`, `캡처` | Play Mode 중 스크린샷만 캡처한다 (`capture_scene_object`) |
| `fix`, `수정` | `.claude/bugs/`의 open 리포트를 순차 수정한다 (아래 참조) |

---

## fix 모드

`.claude/bugs/` 폴더의 `status: open` 리포트를 읽어 순차적으로 bug-fixer를 호출한다.

**실행 절차:**

1. `.claude/bugs/` 폴더에서 `*.md` 파일 목록을 읽는다 (`.gitkeep` 제외)
2. 각 파일의 frontmatter에서 `status: open`인 것만 필터링한다
3. severity 우선순위로 정렬한다: `critical` > `major` > `minor`
4. 사용자에게 수정 대상 목록을 보여주고 확인을 요청한다:
   ```
   수정 대상 버그 N건:
   1. [critical] F-03-player-sinking.md — 플레이어 지면 가라앉음
   2. [major] F-04-obstacle-not-spawning.md — 장애물 미스폰

   순차 수정을 진행할까요? (y/n)
   ```
5. 승인 시, 각 리포트를 순서대로 처리한다:
   a. 리포트 파일 전문을 읽는다
   b. `@bug-fix-orchestrator` 에이전트를 실행한다
      - 프롬프트: 리포트 파일의 증상, 에러 로그, 오브젝트 상태를 포함하여 전달
   c. 수정 완료 시 리포트 파일의 `status: open` → `status: fixed` 로 업데이트
   d. 수정 실패(에스컬레이션) 시 `status: open` → `status: blocked` 로 업데이트
   e. 다음 리포트로 진행
6. 전체 완료 후 요약을 출력한다

**출력:**
```
## Play: fix

### 수정 결과

| # | 리포트 | severity | 결과 |
|---|--------|----------|------|
| 1 | F-03-player-sinking.md | critical | fixed |
| 2 | F-04-obstacle-not-spawning.md | major | blocked |

### 요약: N/M 수정 완료

blocked된 항목은 수동 확인이 필요합니다.
```

**주의사항:**
- 각 bug-fixer 호출은 리포트 파일을 컨텍스트로 전달하므로, 대화 컨텍스트에 의존하지 않는다
- 수정 후 자동으로 `/play check`를 실행하지 않는다 (사용자가 원하면 `/play suite`로 재검증)
- `.claude/bugs/` 폴더의 리포트는 수동 삭제 전까지 유지된다 (이력 관리 용도)

---

## 에러 발생 시 처리

플레이 중 에러가 발견되면:
1. 에러 내용을 **그대로 사용자에게 보고**한다
2. 자동 수정은 하지 않는다 (사용자가 `/bug-fixer`로 별도 처리)
3. 단, `[INPUT]` 에러(Input System 설정)는 `execute_script`로 자동 수정 후 재시도 1회 허용

## 주의사항

- Play Mode 중 `save_scene` 호출 금지
- 스크린샷 캡처 실패 시 무시하고 진행 (로그 결과로 판단)
- watch 모드에서 대기 시간은 `Bash`의 `sleep` 명령어로 구현
