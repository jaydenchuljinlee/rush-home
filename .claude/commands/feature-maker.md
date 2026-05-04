---
description: feature-maker. 기능 구현 전 브랜치 상태를 점검하고, feature-orchestrator로 구현 파이프라인을 실행한다.
argument-hint: [구현할 기능 설명]
---

# feature-maker

RushHome Unity 게임 기능 구현 파이프라인을 시작합니다.

구현 요청: $ARGUMENTS

---

## Step 0: 사전 점검

```bash
git branch --show-current
git status --short
cat docs/PROGRESS.md
```

### 게이트 체크 (하나라도 걸리면 중단)

| 조건 | 처리 |
|------|------|
| `git status`에 변경 파일 있음 | **중단** — "`/feature-check`로 먼저 정리하세요." |
| 현재 브랜치가 `bugfix/*` | **중단** — "`/feature-check`로 bugfix를 머지하세요." |

### 경로 판별: A 또는 B

`docs/PROGRESS.md`에서 진행 중인 Feature와 현재 `feature/*` 브랜치를 기준으로 판별한다.

---

**경로 A: 진행 중인 Feature가 있다 → 해당 브랜치에서 계속 작업**

1. 요청된 기능이 진행 중인 Feature 범위 내인지 확인
2. **범위 내** → 현재 `feature/*` 브랜치에서 바로 Step 1 진행
3. **다른 Feature** → 중단

---

**경로 B: 진행 중인 Feature가 없다 → 새 Feature 브랜치 생성**

1. 요청된 기능에 해당하는 Feature 번호를 `PROGRESS.md`에서 찾는다
2. 선행 Feature가 모두 완료되었는지 확인
3. **선행 완료** → 새 브랜치 생성 후 Step 1 진행:
   ```bash
   git checkout main
   git checkout -b feature/{kebab-case-기능명}
   ```

---

## Step 1: 기능 구현

`@feature-orchestrator` 에이전트를 실행한다.

- 프롬프트: "$ARGUMENTS 기능을 구현해주세요."

에이전트가 완료 보고서를 반환하면 아래 형식으로 사용자에게 전달합니다.

---

## 구현 완료: {기능명}

### 생성/수정 파일
- `RushHome/Assets/Scripts/{경로}/{파일명}.cs` -- 설명

### Unity 에디터 수동 설정
- [ ] {프리팹/씬에서 수행할 작업}

### 테스트 결과
- Edit Mode: PASS (N개) / Play Mode: PASS (N개) or 해당 없음

### 검증 결과: PASS

### 회귀 횟수: 테스트 N회 / 검증 N회

---

구현 완료 후 아래를 안내한다:

```
구현이 완료되었습니다.
작업을 정리하려면 `/feature-check`를 실행하세요.
```

---

**형식 규칙**:
- 에스컬레이션으로 중단된 경우:

## 구현 중단: {기능명}

### 중단 사유: [{카테고리}]
{분석 보고서 요약}

### 현재 Phase: {중단된 Phase}
### 회귀 횟수: 테스트 N회 / 검증 N회
