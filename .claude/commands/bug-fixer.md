---
description: 버그를 진단하고 수정하는 파이프라인. 증상을 설명하면 재현 → 진단 → 수정 → 검증을 자동으로 수행한다.
argument-hint: [버그 설명]
---

# bug-fixer

HomeRun Unity 게임 버그 수정 파이프라인을 시작합니다.

버그 보고: $ARGUMENTS

---

## Step 0: Git 브랜치 준비

1. **현재 브랜치 확인**:
   ```bash
   git branch --show-current
   git status --short
   ```

2. **브랜치 유형별 처리**:

   | 현재 브랜치 | 처리 |
   |------------|------|
   | `feature/*` | 이 feature 브랜치에서 bugfix 브랜치를 생성한다 (정상) |
   | `bugfix/*` | 이미 bugfix 중이므로 새 브랜치 없이 현재 브랜치에서 수정 |
   | `main` | **경고** — "feature 브랜치에서 작업하세요. `/feature-check`를 실행하세요." → **중단** |

3. **미커밋 변경사항 확인**:
   변경 파일이 있으면 `/feature-check`로 먼저 정리하도록 안내하고 **중단**한다.

4. **bugfix 브랜치 생성** (현재 `feature/*`인 경우):
   - 상위 feature 브랜치를 기억해둔다 (머지 대상)
   - 브랜치명을 kebab-case로 결정한다.
     - 예: "플레이어 점프가 안 됨" → `bugfix/player-jump`
     - 예: "게임오버 UI가 안 뜸" → `bugfix/gameover-ui`
   ```bash
   git checkout -b bugfix/{브랜치명}
   ```

5. **확인**: `git branch --show-current`로 bugfix 브랜치에 있는지 확인한다.

---

## Step 1: 버그 수정

`@bug-fix-orchestrator` 에이전트를 실행합니다.

- 프롬프트: "$ARGUMENTS 버그를 진단하고 수정해주세요."

## Step 2: 커밋

에이전트가 완료 보고서를 반환하면, 변경사항을 커밋한다.

1. **변경 파일 확인 및 커밋** (bugfix 관련 파일만 명시적으로 add):
   ```bash
   git add {수정된 파일들}
   git commit -m "fix: {버그 수정 요약}"
   ```

2. 사용자에게 완료 보고서를 전달하고, 정리를 안내한다:
   ```
   bugfix가 완료되었습니다.
   `/feature-check`로 상위 feature 브랜치에 머지하세요.
   ```

**중요:** bugfix 브랜치에서는 PR을 생성하지 않는다.
bugfix는 상위 feature 브랜치로 `git merge`하는 것이 정상 흐름이다.

---

아래 형식으로 사용자에게 전달합니다.

---

## 버그 수정 완료: {버그명}

### 근본 원인: [{카테고리}]
{원인 2~3문장 설명}

### 수정 파일
- `Assets/Scripts/{경로}/{파일명}.cs` -- 변경 내용

### 테스트 결과
- 회귀 테스트: PASS (N개) or 해당 없음
- 수정 확인 테스트: PASS (N개)

### 회귀 횟수: N회

### 다음 단계
`/feature-check`로 상위 feature 브랜치에 머지하세요.

---

**형식 규칙**:
- 에스컬레이션으로 중단된 경우 아래 형식을 사용한다:

## 수정 중단: {버그명}

### 중단 사유: [{카테고리}]
{분석 보고서 요약}

### 현재 Phase: {중단된 Phase}
### 회귀 횟수: N회
