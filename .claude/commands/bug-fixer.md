---
description: 버그를 진단하고 수정하는 파이프라인. 증상을 설명하면 재현 → 진단 → 수정 → 검증을 자동으로 수행한다.
argument-hint: [버그 설명]
---

# bug-fixer

RushHome Unity 게임 버그 수정 파이프라인을 시작합니다.

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
   | `feature/*` | 이 feature 브랜치에서 bugfix 브랜치를 생성한다 |
   | `bugfix/*` | 현재 브랜치에서 수정 |
   | `main` | **경고** — "feature 브랜치에서 작업하세요." → **중단** |

3. **미커밋 변경사항 확인**:
   변경 파일이 있으면 `/feature-check`로 먼저 정리하도록 안내하고 **중단**.

4. **bugfix 브랜치 생성** (현재 `feature/*`인 경우):
   ```bash
   git checkout -b bugfix/{브랜치명}
   ```

---

## Step 1: 버그 수정

`@bug-fix-orchestrator` 에이전트를 실행합니다.

- 프롬프트: "$ARGUMENTS 버그를 진단하고 수정해주세요."

## Step 2: 커밋

에이전트가 완료 보고서를 반환하면, 변경사항을 커밋한다.

```bash
git add {수정된 파일들}
git commit -m "fix: {버그 수정 요약}"
```

**중요:** bugfix 브랜치에서는 PR을 생성하지 않는다.
bugfix는 상위 feature 브랜치로 `git merge`하는 것이 정상 흐름이다.

---

## 버그 수정 완료: {버그명}

### 근본 원인: [{카테고리}]
{원인 2~3문장 설명}

### 수정 파일
- `RushHome/Assets/Scripts/{경로}/{파일명}.cs` -- 변경 내용

### 테스트 결과
- 회귀 테스트: PASS (N개) or 해당 없음
- 수정 확인 테스트: PASS (N개)

### 회귀 횟수: N회

### 다음 단계
`/feature-check`로 상위 feature 브랜치에 머지하세요.
