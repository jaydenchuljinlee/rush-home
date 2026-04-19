---
description: 현재 작업 상황을 점검하고, 미반영 변경사항을 브랜치 전략에 맞게 정리한다. bugfix는 상위 브랜치로 머지, feature 완료 시 PR 생성을 안내한다.
argument-hint: (인자 없음)
allowed-tools: Bash, Read, Glob, Grep, Edit, Write
model: sonnet
---

# Feature Check — 작업 상황 점검 및 정리

현재 브랜치의 작업 상황을 점검하고, 브랜치 전략에 맞게 변경사항을 정리합니다.

---

## 브랜치 전략

```
main
 └── feature/{기능명}        ← 기능 단위 브랜치. 완료 시 main으로 PR
      └── bugfix/{버그명}    ← 버그 수정. 완료 후 상위 feature 브랜치로 머지 (PR 불필요)
```

**핵심 규칙:**
- `bugfix/*` → 상위 `feature/*` 브랜치로 `git merge` (PR 불필요)
- `feature/*` → `main`으로 PR (기능이 완전히 완료되었을 때만)
- PR 단위는 Feature 완료 단위 (F-01~F-04 등 관련 Feature 묶음)

---

## Step 1: 현재 상태 수집

아래 명령어를 **병렬로** 실행한다:

```bash
# 1-1. 현재 브랜치
git branch --show-current

# 1-2. 변경 파일 목록 (staged + unstaged + untracked)
git status --short

# 1-3. 전체 브랜치 목록
git branch --list

# 1-4. 현재 브랜치의 커밋 히스토리 (main 이후)
git log --oneline main..HEAD

# 1-5. 로드맵 진행 현황
cat docs/PROGRESS.md
```

## Step 2: 브랜치 유형 판별 및 보고

현재 브랜치 이름에서 유형을 판별한다:
- `bugfix/*` → bugfix 브랜치
- `feature/*` → feature 브랜치
- `main` → 메인 브랜치

### 출력 형식:

```
## Feature Check

### 현재 브랜치: {브랜치명} ({유형})
### 상위 브랜치: {bugfix면 관련 feature 브랜치 추정, feature면 main}
### 커밋 현황: main 이후 N개 커밋

### 미반영 변경사항
| 구분 | 파일 수 | 주요 내용 |
|------|---------|----------|
| Modified | N개 | {요약} |
| Untracked | N개 | {요약} |

### 변경사항 분류
| 카테고리 | 파일 | 관련 |
|----------|------|------|
| 기능 구현 | {목록} | F-{번호} |
| 테스트 | {목록} | F-{번호} |
| 인프라/도구 | {목록} | 도구 |

### 로드맵 현황
- F-01: {상태}
- F-02: {상태}
- ...
```

## Step 3: 브랜치 유형별 정리 방안

### A. bugfix 브랜치인 경우

1. **미커밋 변경 분류**:
   - 이 bugfix와 관련된 변경 → 현재 브랜치에 커밋
   - bugfix와 무관한 변경 (feature 구현, 도구 등) → stash 또는 상위 브랜치에서 커밋

2. **bugfix 커밋 완료 후**:
   ```
   bugfix 커밋이 완료되었습니다.

   정리 방안:
   1. feature/{상위} 브랜치로 이동
   2. bugfix/{현재} 브랜치를 머지
   3. 남은 미커밋 변경사항을 feature 브랜치에서 커밋

   진행할까요?
   ```

3. **머지 실행** (사용자 승인 후):
   ```bash
   git checkout feature/{상위}
   git merge bugfix/{현재}
   ```

4. **bugfix 브랜치 정리**:
   머지 후 bugfix 브랜치 삭제 여부를 물어본다.

### B. feature 브랜치인 경우

1. **미커밋 변경 커밋**:
   카테고리별로 분리 커밋을 제안한다.
   ```
   제안:
   1. feat: F-04 장애물 시스템 구현
   2. chore: /play 스킬 및 suite 인프라 추가
   ```

2. **Feature 완료 여부 판단**:
   아래 기준으로 판단한다:
   - 관련 Feature의 코드가 모두 구현되었는가?
   - `/play suite`에서 PASS를 받았는가?
   - 미해결 버그 리포트(`.claude/bugs/`에 `status: open`)가 없는가?

3. **Feature 완료 시 PR 안내**:
   ```
   feature/{브랜치}의 모든 작업이 완료되었습니다.

   PR을 생성하려면 `/git-pull-request`를 실행하세요.
   대상: feature/{브랜치} → main
   ```

4. **Feature 미완료 시**:
   ```
   아직 완료되지 않은 항목:
   - [ ] F-06 난이도 곡선 (미구현)
   - [ ] .claude/bugs/F-04-xxx.md (status: open)

   변경사항을 커밋하고 작업을 계속하세요.
   ```

### C. main 브랜치인 경우

```
⚠️ main 브랜치에서 직접 작업 중입니다.
feature/{기능명} 브랜치를 생성하여 작업하는 것을 권장합니다.
```

## Step 4: 사용자 승인 후 실행

사용자가 방안을 승인하면:

1. 관련 파일을 `git add`로 스테이징 (파일별로 명시적 추가, `git add -A` 금지)
2. Conventional Commits 형식으로 커밋
3. `git push -u origin {브랜치명}`

## Step 5: PROGRESS.md 동기화

Feature가 완료된 것으로 판단되면,
`docs/PROGRESS.md`의 해당 Feature 상태를 `완료`로 업데이트할지 물어본다.

---

## 주의사항

- 민감 파일 (.env, *.pem, *.key, secrets.*) 커밋 여부를 반드시 확인한다
- `git add -A`를 사용하지 않는다 — 파일을 명시적으로 추가한다
- 사용자 승인 없이 커밋/푸시/머지하지 않는다
- `settings.local.json` 등 로컬 설정 파일은 커밋 대상에서 제외한다
- bugfix 머지 시 `--no-ff` 옵션은 사용하지 않는다 (fast-forward 허용)
