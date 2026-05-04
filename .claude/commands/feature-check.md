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

---

## Step 1: 현재 상태 수집

```bash
# 1-1. 현재 브랜치
git branch --show-current

# 1-2. 변경 파일 목록
git status --short

# 1-3. 전체 브랜치 목록
git branch --list

# 1-4. 현재 브랜치의 커밋 히스토리 (main 이후)
git log --oneline main..HEAD

# 1-5. 로드맵 진행 현황
cat docs/PROGRESS.md
```

## Step 2: 브랜치 유형 판별 및 보고

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

### 로드맵 현황
- F-01: {상태}
- ...
```

## Step 3: 브랜치 유형별 정리 방안

### A. bugfix 브랜치인 경우

1. bugfix 관련 변경 → 현재 브랜치에 커밋
2. 커밋 완료 후 상위 feature 브랜치로 머지 안내
3. 머지 실행 (사용자 승인 후):
   ```bash
   git checkout feature/{상위}
   git merge bugfix/{현재}
   ```

### B. feature 브랜치인 경우

1. 카테고리별 분리 커밋 제안
2. Feature 완료 여부 판단
3. **완료 시**: PR 안내 (`/git pr`)
4. **미완료 시**: 남은 항목 안내

### C. main 브랜치인 경우

```
⚠️ main 브랜치에서 직접 작업 중입니다.
feature/{기능명} 브랜치를 생성하여 작업하는 것을 권장합니다.
```

## Step 4: 사용자 승인 후 실행

1. 관련 파일을 `git add`로 스테이징 (파일별로 명시적 추가)
2. Conventional Commits 형식으로 커밋
3. `git push -u origin {브랜치명}`

---

## 주의사항

- 민감 파일 (.env, *.pem, *.key, secrets.*) 커밋 여부를 반드시 확인
- `git add -A`를 사용하지 않는다
- 사용자 승인 없이 커밋/푸시/머지하지 않는다
- `settings.local.json` 등 로컬 설정 파일은 커밋 대상에서 제외
