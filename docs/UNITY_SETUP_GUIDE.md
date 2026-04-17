# Unity 설치 및 개발 환경 세팅 가이드

> HomeRun 프로젝트를 위한 Unity 세팅부터 Claude 연동까지 A to Z

## 📋 준비물

- 컴퓨터 (Windows 10/11 또는 macOS 12+)
- 최소 사양: RAM 8GB, 저장공간 30GB (Unity + 프로젝트)
- 권장 사양: RAM 16GB 이상, SSD
- 인터넷 연결

---

## 1️⃣ Unity Hub 설치

**Unity Hub**는 Unity 에디터 버전을 관리하고 프로젝트를 실행하는 런처입니다. Unity 자체를 직접 설치하는 게 아니라, Hub를 통해서 관리하는 구조예요.

### 설치 방법
1. https://unity.com/download 접속
2. "Download Unity Hub" 클릭 (OS 자동 감지)
3. 다운로드된 설치 파일 실행
4. 기본 옵션으로 설치 완료
5. Unity Hub 실행

### 계정 생성/로그인
- Unity ID로 로그인 필요 (없으면 무료 가입)
- 라이선스: **Personal License** 선택 (개인/소규모 개발자는 무료)
  - 연 매출 $200K 미만이면 무료
  - HomeRun 프로젝트는 여기에 해당

---

## 2️⃣ Unity 에디터 설치

Unity Hub 안에서 실제 Unity 버전을 설치합니다.

### 설치 절차
1. Unity Hub 좌측 메뉴 → **Installs** → **Install Editor** 클릭
2. **Unity 6 LTS** 또는 **Unity 6.2** 선택
   - LTS(Long Term Support)가 안정적이라 초보자에게 추천
   - Unity 6.2는 공식 MCP 지원 (AI 연동 편리)
3. **Add modules** 단계에서 다음 항목 체크:
   - ✅ **Android Build Support** (Android 빌드용)
     - ✅ OpenJDK
     - ✅ Android SDK & NDK Tools
   - ✅ **iOS Build Support** (Mac 사용자, iOS 빌드용)
   - ✅ **Documentation** (오프라인 문서, 선택)
   - ✅ **한국어** (Language Pack, 선택)
4. "Install" 클릭 → 다운로드 (약 10~15GB, 시간 걸림)

### 설치 확인
- Unity Hub → Installs 탭에 설치된 버전이 보이면 완료

---

## 3️⃣ 첫 프로젝트 생성

### 절차
1. Unity Hub → **Projects** → **New Project**
2. 템플릿 선택: **2D (Built-In Render Pipeline)** 또는 **Universal 2D**
   - Universal 2D 추천 (URP 기반, 더 최신)
3. 프로젝트 이름: `HomeRun`
4. 경로: 원하는 폴더 지정 (예: `~/Documents/UnityProjects/HomeRun`)
5. **Create Project** 클릭
6. Unity 에디터가 열리면 완료 (첫 로딩은 1~5분 걸림)

### 한국어 설정 (선택)
- 에디터 상단 `Edit > Preferences > Languages > 한국어`
- 메뉴가 한글로 바뀌지만, 튜토리얼은 대부분 영어 기준이라 영어 유지 추천

---

## 4️⃣ 에디터 레이아웃 이해

Unity 에디터를 처음 열면 여러 패널이 보입니다. 각각 이해해두면 작업이 편해요.

| 패널 | 역할 |
|------|------|
| **Hierarchy** | 현재 씬에 있는 모든 오브젝트 목록 |
| **Scene** | 3D 공간에서 시각적으로 오브젝트 배치 |
| **Game** | 실제 실행 시 보이는 화면 (플레이어 시점) |
| **Inspector** | 선택한 오브젝트의 속성 편집 |
| **Project** | 프로젝트의 모든 에셋(파일) 브라우저 |
| **Console** | 로그, 에러, 경고 출력 |

### 추천 레이아웃
`Window > Layouts > 2 by 3` 선택 → 2D 개발에 적합한 레이아웃

---

## 5️⃣ Git 연동 (버전 관리)

프로토타입이라도 Git은 반드시 쓰세요. 실수로 파일 날리는 일 방지 + 히스토리 추적.

### .gitignore 설정
Unity 프로젝트는 자동 생성 파일이 많아서 전용 `.gitignore`가 필요해요.

```bash
cd HomeRun/
curl -o .gitignore https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore
git init
git add .
git commit -m "Initial commit"
```

### GitHub 리포 연결
```bash
git remote add origin https://github.com/your-username/HomeRun.git
git branch -M main
git push -u origin main
```

### Unity 프로젝트 Git 설정 (중요!)
1. Unity 에디터 → `Edit > Project Settings > Editor`
2. **Version Control** → **Visible Meta Files**
3. **Asset Serialization** → **Force Text**
   - 이 설정이 있어야 Git merge 충돌 해결 가능

### Git LFS (권장)
아트 에셋은 용량이 크니까 LFS 써주는 게 좋아요.

```bash
git lfs install
git lfs track "*.png" "*.psd" "*.wav" "*.mp3" "*.fbx"
git add .gitattributes
git commit -m "Add Git LFS tracking"
```

---

## 6️⃣ IDE 연결 (C# 코드 편집)

Unity에서 스크립트를 더블클릭하면 외부 IDE가 열립니다.

### 옵션 A: Visual Studio (Windows/Mac, 무료)
- Unity Hub 설치 시 기본 제공
- 통합이 가장 매끄러움
- 학생/개인 개발자: Community 에디션 무료

### 옵션 B: JetBrains Rider (유료, 강력 추천)
- Unity 전문 기능이 압도적으로 강력
- 리팩토링, 자동완성이 VS보다 우수
- 개인 라이선스 약 $15/월
- **무료 대안**: GitHub 학생 팩 있으면 무료

### 옵션 C: Visual Studio Code (무료, 가벼움)
- 확장 기능 설치 필요 (C#, Unity Tools)
- 가볍지만 Unity 통합이 Rider/VS보다 약함

### 설정 방법
Unity 에디터 → `Edit > Preferences > External Tools > External Script Editor`에서 선택

---

## 7️⃣ Claude 연동 (Claude Code + MCP)

AI 페어 프로그래밍을 위해 Claude를 Unity와 연결합니다.

### 7-1. Claude Code 설치

```bash
npm install -g @anthropic-ai/claude-code
```

설치 후 확인:
```bash
claude --version
```

처음 실행 시 Anthropic 계정 로그인 필요. Claude Pro 구독이 있으면 구독 기반으로 사용 가능.

### 7-2. Coplay MCP 설치 (Unity ↔ Claude 브릿지)

**Step 1**: Claude Code에 MCP 서버 추가
```bash
claude mcp add --scope user --transport stdio coplay-mcp \
  --env MCP_TOOL_TIMEOUT=720000 \
  -- uvx --python ">=3.11" coplay-mcp-server@latest
```

**Step 2**: Unity 프로젝트에 Coplay 패키지 설치
1. Unity 에디터 → `Window > Package Manager`
2. 좌상단 `+` → `Add package from git URL...`
3. URL 입력: `https://github.com/CoplayDev/unity-plugin.git#beta`
4. 설치 완료되면 Unity 메뉴에 Coplay 항목 추가됨

**Step 3**: 동작 확인
1. Unity 에디터를 켠 상태에서
2. 터미널에서 `claude` 실행 → Unity 프로젝트 폴더에서
3. 프롬프트에서 "What can you do with Unity through Coplay MCP?" 입력
4. 응답이 오면 성공

### 7-3. 사용 예시

Claude Code에서 자연어로 Unity 조작:
```
> Player라는 GameObject를 만들고 Rigidbody2D, BoxCollider2D를 붙여줘
> Assets/Scripts/Player 폴더에 PlayerController.cs를 만들고 점프 기능을 구현해줘
> Console의 현재 에러 로그를 분석해줘
```

### 7-4. 문제 해결
- **MCP 서버 인식 안 됨**: 터미널 재시작, `claude mcp list`로 등록 확인
- **Unity 연결 안 됨**: Unity 에디터가 켜져 있는지, Coplay 패키지가 활성화됐는지 확인
- **권한 에러 (macOS)**: `chmod +x` 필요할 수 있음

---

## 8️⃣ Asset Store 활용

Unity 공식 에셋 마켓에서 무료/유료 에셋 구매 가능.

### 접근 방법
1. 브라우저에서 https://assetstore.unity.com 접속 (Unity 계정으로 로그인)
2. 원하는 에셋 "Add to My Assets" 클릭
3. Unity 에디터 → `Window > Package Manager` → 상단 드롭다운을 **My Assets**로 변경
4. 구매한 에셋 목록에서 Download → Import

### HomeRun용 추천 검색 키워드
- `2D runner template`
- `2D character pack`
- `pixel art city`
- `parallax background 2D`
- `mobile UI pack`

### 시작용 무료 에셋
- **Kenney.nl** 에셋들 (프로토타입에 최고)
- Unity 공식 **Starter Assets - Third Person Character**
- **2D Platformer Microgame** (Unity Learn 무료 템플릿)

---

## 9️⃣ PlayFab 계정 세팅 (서버)

나중에 랭킹 붙일 때 쓸 거지만, 미리 계정만 만들어두세요.

### 절차
1. https://playfab.com 접속 → Sign Up
2. 로그인 후 "Create a title" → 제목: `HomeRun`
3. 대시보드에서 **Title ID** 확인 (나중에 Unity에서 사용)
4. Unity SDK 다운로드: https://github.com/PlayFab/UnitySDK
   - 또는 Asset Store에서 "PlayFab" 검색해서 무료 설치

지금은 여기까지만. 실제 연동은 Phase 4에서 진행.

---

## 🔟 학습 경로 (Unity 처음이라면)

### Day 1~2: 에디터 익숙해지기
- [ ] Unity Hub, 에디터 실행, 간단한 씬 만들기
- [ ] 공식 튜토리얼: https://learn.unity.com/project/2d-platformer-template (2시간)

### Day 3~7: C# 기본
- [ ] 서버 개발자라 문법은 금방 적응
- [ ] `MonoBehaviour` 생명주기 (`Start`, `Update`, `FixedUpdate`) 이해
- [ ] `Transform`, `Rigidbody2D`, `Collider2D` 기본 개념

### Week 2: 작은 프로토타입
- [ ] 박스가 좌우로 움직이는 기본 씬
- [ ] 점프 구현
- [ ] 장애물에 부딪히면 게임 오버

### 추천 튜토리얼 (한국어)
- 골드메탈 유튜브 채널 - 2D 게임 시리즈
- 고박사의 유니티 노트 - 기초부터 차근차근
- 인프런 "C#과 유니티로 만드는 MMORPG" (중급)

### 추천 튜토리얼 (영어)
- Brackeys - 2D Game Tutorials (자막 좋음)
- Code Monkey - Unity 심화
- Sebastian Lague - 알고리즘적 접근

---

## ✅ 최종 체크리스트

세팅이 다 끝났는지 확인해보세요:

- [ ] Unity Hub 설치됨
- [ ] Unity 6 LTS 설치됨 (Android/iOS 모듈 포함)
- [ ] HomeRun 프로젝트 생성됨
- [x] Git 연동 + .gitignore 설정됨 (2026-04-17 완료 -- .gitignore, .gitattributes, Git LFS)
- [x] IDE (VS / Rider / VSCode) 연결됨 (JetBrains Rider 설치 확인)
- [x] Claude Code 설치됨 (v2.1.42)
- [ ] Coplay MCP 연동됨 (uvx 설치 완료, MCP 서버 등록 대기)
- [ ] PlayFab 계정 생성됨
- [ ] 공식 튜토리얼 1개 이상 완료

---

## 🆘 자주 생기는 문제

### Unity가 너무 느려요
- Unity 에디터는 원래 무겁습니다. SSD 필수.
- `Edit > Preferences > General`에서 `Auto Refresh` 끄면 조금 빨라짐

### 빌드할 때 Android SDK 오류
- Unity Hub → Installs → 해당 버전 옆 톱니바퀴 → Add Modules
- Android SDK & NDK Tools 재설치

### 스크립트 더블클릭해도 IDE가 안 열림
- `Edit > Preferences > External Tools > External Script Editor` 설정 확인
- 해당 IDE에서 "Regenerate project files" 실행

### Git에 수백 MB 이상 커밋됨
- Library/, Temp/, Logs/ 폴더는 .gitignore에 포함돼야 함
- 이미 커밋됐다면 `git rm -r --cached Library` 후 재커밋

---

## 📚 참고 링크

- Unity 공식 문서: https://docs.unity3d.com
- Unity Learn: https://learn.unity.com
- Coplay MCP: https://github.com/CoplayDev/unity-mcp
- PlayFab 문서: https://learn.microsoft.com/en-us/gaming/playfab/
- Claude Code 문서: https://docs.claude.com/en/docs/claude-code

---

*마지막 업데이트: 2026-04-17*
