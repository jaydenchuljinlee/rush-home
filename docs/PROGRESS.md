# HomeRun 프로젝트 진행 현황

> 마지막 업데이트: 2026-04-18

---

## 현재 환경

| 항목 | 상태 |
|---|---|
| macOS | 15.7.4 (Sequoia), Apple Silicon (arm64) |
| RAM | 16 GB |
| 디스크 여유 | 약 145 GB |
| Git | 2.39.5 |
| Git LFS | 3.7.1 |
| Homebrew | 설치됨 |
| Node.js | v24.10.0 |
| Claude Code | 2.1.42 |
| gh CLI | 2.50.0 |
| IDE | JetBrains Rider |
| Python | 3.11.8 (Anaconda) |
| uvx | 0.11.7 |
| Unity Hub | 설치 완료 |
| Unity Editor | Unity 6 LTS 설치 완료 |
| Coplay MCP | 연동 완료 |

---

## 개발 환경 세팅 진행 상황

### 완료

- [x] GitHub 리포지토리 생성 (`rush-home`)
- [x] Git LFS 설치 및 초기화 (`brew install git-lfs && git lfs install`)
- [x] uvx 설치 (`brew install uv` -- Coplay MCP 의존성)
- [x] Unity 전용 `.gitignore` 생성 (Library/, Temp/, Builds/ 등 제외)
- [x] Git LFS `.gitattributes` 생성 (png, wav, mp3, fbx 등 바이너리 에셋 추적)
- [x] `.claude/` 에이전트/규칙/커맨드를 Unity 프로젝트에 맞게 재구성
  - agents: feature-planner, feature-implementer, test-runner, code-validator, feature-orchestrator, test-failure-analyzer
  - rules: architecture.md, implementation.md, testing.md, build.md
  - commands: feature-maker.md
- [x] `.claude/plans/` 디렉토리 생성

### 대기 (수동 설치 필요)

- [x] **Unity Hub 설치** -- brew install --cask unity-hub
- [x] **Unity 6 LTS 설치** -- Unity Hub > Installs > Install Editor
- [x] **HomeRun Unity 프로젝트 생성** -- Unity Hub > New Project > Universal 2D
- [x] **Rider 연결** -- Unity > Settings > External Tools > Rider
- [x] **Coplay MCP 등록**
- [ ] **Unity에 Coplay 패키지 설치** -- Package Manager > Add from git URL:
  ```
  https://github.com/CoplayDev/unity-plugin.git#beta
  ```
- [ ] **PlayFab 계정 생성** -- https://playfab.com

---

## 로드맵 진행 상황 (기획서 기준)

| Phase | 내용 | 상태 |
|---|---|---|
| Phase 1 | 프로토타입 (이동/점프/장애물/게임오버) | 완료 |
| Phase 2 | 핵심 시스템 (청크 맵, 난이도, 오브젝트 풀링) | 완료 |
| Phase 3 | 아트 & UX (에셋, 애니메이션, UI, 사운드) | 대기 |
| Phase 4 | 서버 연동 (PlayFab 인증/리더보드) | 대기 |
| Phase 5 | 폴리싱 & 출시 준비 | 대기 |

---

## 다음 할 일

1. Phase 3 시작 -- 캐릭터 애니메이션 (달리기/점프/슬라이딩/데스)
2. 배경 파랄랙스 스크롤링
3. UI 개선 (메인메뉴/게임오버/결과 화면)
4. 사운드 추가 (BGM, SFX)
