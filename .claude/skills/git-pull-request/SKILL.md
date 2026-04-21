---
name: git-pull-request
description: >
  PR을 자동 생성하는 스킬. git log와 diff를 분석해 팀 PR 포맷에 맞는 본문 초안을 만들고,
  사용자 확인 후 gh pr create로 GitHub PR을 생성한다.
  다음 상황에서 반드시 이 스킬을 사용한다:
  - "PR 만들어줘", "PR 작성해줘", "PR 올려줘" 등의 요청
  - "풀리퀘 만들어줘", "PR 올리자", "PR 초안 만들어줘" 등의 발화
  - "/pr", "/git-pull-request" 슬래시 커맨드
  - git-commit-push 완료 후 사용자가 PR 생성을 요청할 때
---

# Git Pull Request Skill

git 변경 이력을 분석해 PR 본문 초안을 생성하고, `gh pr create`로 GitHub에 PR을 생성하는 스킬.

---

## PR 템플릿 규칙

> ⚠️ **STRICT TEMPLATE RULE**
> PR 본문은 반드시 `.github/PULL_REQUEST_TEMPLATE.md` 파일을 읽어 그 구조를 그대로 사용한다.
> - 섹션 제목, 설명 문구, 이모지, 줄바꿈, 항목 순서 모두 **단 한 글자도 변경하지 않는다**
> - 내용이 없는 섹션도 **섹션 자체를 삭제하거나 생략하지 않는다**
> - 설명 문구를 요약하거나 바꾸지 않는다
> - Claude가 채우는 것은 오직 각 섹션의 `-` bullet 내용뿐이다

Claude가 채우는 규칙:

| 섹션 | Claude가 채우는 것 | 절대 하면 안 되는 것 |
|---|---|---|
| 🧩 PR 개요 | `-` 뒤에 한 문장 요약 작성 | 설명 문구 수정, 섹션 삭제 |
| 🔍 주요 변경사항 | `-` bullet 여러 개 작성 | 설명 문구 수정, 섹션 삭제 |
| ⚙️ 추가 변경사항 | 해당 변경 있으면 `-` 작성, 없으면 `-` 빈칸 유지 | **섹션 자체 삭제, "해당 없음" 등 임의 문구 추가** |
| 🧪 테스트 | 체크박스 미체크 상태 유지 (`[  ]`) | 체크박스 자동 체크, 설명 문구 수정 |

---

## 실행 절차

### Step 1: 사전 확인

```bash
git branch --show-current
git remote -v
git log origin/$(git branch --show-current)..HEAD --oneline 2>/dev/null
```

- 현재 브랜치가 `main`, `master`, `develop`, `release` 이면 → 경고 출력 후 중단
  ```
  ⚠️ 현재 브랜치 [브랜치명]은 공용 브랜치입니다.
  feature 브랜치에서 PR을 작성해주세요.
  ```
- 원격에 push되지 않은 커밋이 있으면 → "먼저 push가 필요합니다. push 후 진행할까요?" 확인

### Step 1.5: target 브랜치와 conflict 확인

target 브랜치(기본 `main`)를 fetch한 후 conflict 여부를 확인한다:

```bash
git fetch origin [TARGET]
git merge --no-commit --no-ff origin/[TARGET] 2>&1
```

- **conflict 없음** → `git merge --abort`로 되돌리고 Step 2 진행
- **conflict 있음** → `git merge --abort`로 되돌리고, conflict 파일 목록을 보여준 뒤 안내:
  ```
  ⚠️ target 브랜치(origin/[TARGET])와 conflict가 있습니다.

  충돌 파일:
  - {파일 목록}

  PR 생성 전에 conflict를 해결해야 합니다.
  `git merge origin/[TARGET]`으로 병합 후 conflict를 해결하시겠습니까?
  ```
  사용자 승인 시:
  1. `git merge origin/[TARGET]` 실행
  2. conflict 파일을 읽고 해결
  3. 머지 커밋 생성
  4. push 후 Step 2 진행

### Step 2: target 브랜치 확인

사용자에게 물어본다:

```
📋 PR target 브랜치를 선택해주세요:

현재 브랜치: [브랜치명]

1. develop
2. release
3. master
4. 직접 입력
```

### Step 3: PR 템플릿 읽기 및 정보 분석

`.github/PULL_REQUEST_TEMPLATE.md` 파일을 읽어 템플릿 구조를 파악한다.

```bash
# origin/TARGET 대비 커밋 목록
git log origin/[TARGET]..HEAD --oneline

# 변경 파일 목록
git diff origin/[TARGET]..HEAD --name-status

# 전체 diff
git diff origin/[TARGET]..HEAD
```

origin/TARGET이 로컬에 없으면 먼저 fetch:
```bash
git fetch origin [TARGET]
```

분석 후 채울 내용:

| 섹션 | 생성 기준 |
|---|---|
| PR 제목 | `[브랜치명] - 핵심 변경 내용 요약` 형식 |
| 🧩 PR 개요 `-` | 브랜치명 + 커밋 메시지 흐름으로 **한 문장, 30자 이내**로 요약. 기능 추가 / 버그 수정 / 리팩토링 유형 명시 |
| 🔍 주요 변경사항 `-` | 변경된 파일/클래스/메서드 기준으로 핵심만 bullet |
| ⚙️ 추가 변경사항 `-` | `.yml`, `.yaml`, `migration`, `schema`, `Dockerfile`, `build.gradle` 변경 시 내용 작성. **없어도 섹션 유지, `-` 빈칸** |
| 🧪 테스트 | 체크박스 항상 `[  ]` 미체크 유지 |

diff가 클 때 (변경 파일 30개 이상):
```
⚠️ 변경 파일이 N개로 많습니다. 주요 변경사항을 직접 입력하시겠습니까?
1. 자동 생성 계속
2. 직접 입력
```

### Step 4: 사용자 확인

생성된 초안을 출력하고 확인을 받는다:

```
📝 PR 초안

제목: [feature/order-refund] - 주문 환불 API 및 환불 상태 처리 추가
Target: develop

---
[.github/PULL_REQUEST_TEMPLATE.md 구조 그대로, 각 섹션 내용만 채워서 출력]
---

이대로 PR을 생성할까요?
(수정하려면 원하는 내용을 입력해주세요 / "Local 완료" 입력 시 해당 체크박스 체크)
```

사용자 응답에 따라:
- **승인** ("응", "ㅇㅇ", "올려줘" 등) → Step 5 실행
- **수정 내용 입력** → 반영 후 Step 4 재출력
- **"Local 완료" / "Stage 완료"** → 해당 체크박스 `[O]`로 변경 후 Step 4 재출력
- **취소** → 중단

### Step 5: gh pr create로 PR 생성

```bash
gh pr create \
  --title "[브랜치명] - 요약" \
  --body "$(cat <<'EOF'
[생성된 PR 본문 전체]
EOF
)" \
  --base [TARGET]
```

생성 완료 후 PR URL을 출력한다:

```
✅ PR이 생성되었습니다.

🔗 [PR URL]
```

---

## 주의사항

- PR 본문은 **항상 사용자 확인을 거친다** — 자동으로 바로 생성하지 않는다
- 테스트 체크박스는 **절대 자동으로 체크하지 않는다** — 사용자가 직접 판단
- `gh` CLI가 설치되어 있지 않거나 인증이 안 된 경우 → 오류 메시지를 출력하고 중단
