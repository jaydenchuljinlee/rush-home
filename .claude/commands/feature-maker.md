---
description: feature-maker. 기능 구현 전 브랜치 상태를 점검하고, feature-orchestrator로 구현 파이프라인을 실행한다.
argument-hint: [구현할 기능 설명]
---

# feature-maker

HomeRun Unity 게임 기능 구현 파이프라인을 시작합니다.

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

`docs/PROGRESS.md`에서 진행 중인 Feature(상태가 `완료`가 아닌 것 중 선행이 모두 완료된 Feature)와 현재 `feature/*` 브랜치를 기준으로 판별한다.

---

**경로 A: 진행 중인 Feature가 있다 → 해당 브랜치에서 계속 작업**

진행 중인 Feature가 존재하면:

1. 요청된 기능이 진행 중인 Feature 범위 내인지 확인한다
2. **범위 내** → 현재 `feature/*` 브랜치에서 바로 Step 1 진행
3. **다른 Feature** → 중단:
   ```
   ⚠️ 현재 feature/{브랜치명}에서 F-{번호} 작업이 진행 중입니다.
   진행 중인 Feature를 먼저 완료하세요.
   `/feature-check`로 현재 상태를 확인할 수 있습니다.
   ```

예시:
```
현재: feature/player-character (F-04 진행 중)
요청: "F-04 장애물 시스템 구현" → ✅ 경로 A, 현재 브랜치에서 진행
요청: "F-05 인게임 HUD 구현" → ❌ 중단, F-04 먼저 완료
```

---

**경로 B: 진행 중인 Feature가 없다 → 새 Feature 브랜치 생성**

모든 Feature가 `완료`이거나 현재 브랜치가 `main`이면:

1. 요청된 기능에 해당하는 Feature 번호를 `PROGRESS.md`에서 찾는다
2. 선행 Feature가 모두 완료되었는지 확인한다
3. **선행 미완료** → 중단:
   ```
   ⚠️ F-{번호}의 선행 Feature(F-{선행번호})가 아직 완료되지 않았습니다.
   ```
4. **선행 완료** → 새 브랜치 생성 후 Step 1 진행:
   ```bash
   git checkout main
   git checkout -b feature/{kebab-case-기능명}
   ```

브랜치명은 구현 요청에서 추출한다:
- 예: "장애물 시스템 구현" → `feature/obstacle-system`
- 예: "인게임 HUD" → `feature/ingame-hud`

---

게이트 체크 통과 + 경로 판별 완료 시 Step 1로 진행한다.

---

## Step 1: 기능 구현

`@feature-orchestrator` 에이전트를 실행한다.

- 프롬프트: "$ARGUMENTS 기능을 구현해주세요."

에이전트가 완료 보고서를 반환하면 아래 형식으로 사용자에게 전달합니다.

---

## 구현 완료: {기능명}

### 생성/수정 파일
- `Assets/Scripts/{경로}/{파일명}.cs` -- 설명

### Unity 에디터 수동 설정
- [ ] {프리팹/씬에서 수행할 작업}

### 테스트 결과
- Edit Mode: PASS (N개) / Play Mode: PASS (N개) or 해당 없음

### 검증 결과: PASS

### 회귀 횟수: 테스트 N회 / 검증 N회
### 병렬 분석: 없음 or N건 병렬 분석

---

구현 완료 후 아래를 안내한다:

```
구현이 완료되었습니다.
작업을 정리하려면 `/feature-check`를 실행하세요.
```

---

**형식 규칙**:
- 섹션 순서와 제목을 변경하지 않는다.
- Play Mode 테스트가 없으면 `/ Play Mode: 해당 없음`을 붙인다.
- 병렬 분석이 없으면 `없음`, 있으면 `N건 병렬 분석`으로 표기한다.
- Unity 에디터에서 수동 설정이 필요한 항목은 반드시 체크리스트로 안내한다.
- 에스컬레이션으로 중단된 경우 아래 형식을 사용한다:

## 구현 중단: {기능명}

### 중단 사유: [{카테고리}]
{분석 보고서 요약}

### 현재 Phase: {중단된 Phase}
### 회귀 횟수: 테스트 N회 / 검증 N회
