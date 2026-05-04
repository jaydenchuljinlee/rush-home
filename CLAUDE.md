# RushHome 3D - Claude Rules

## 프로젝트 개요
- Unity 6 LTS / Universal 3D (URP) 프로젝트
- 3D 러너 게임 (rush-home 2D → 3D 변환)
- 프로젝트 경로: `RushHome/`

## CoPlay MCP 사용 규칙

### Editor 스크립트 관리
- `execute_script`로 일회용 스크립트 실행 후 **반드시 즉시 삭제**한다
- Editor 폴더에 스크립트를 남겨두지 않는다 — 코드 변경마다 전체 재컴파일 + 도메인 리로드가 발생하여 RAM이 급증한다
- 디버그/확인 목적의 스크립트도 실행 후 삭제한다

### 불필요한 실행 금지
- 코드만으로 확인 가능한 값(변수값, 수식 계산 등)은 CoPlay 실행 없이 코드를 직접 읽어서 답한다
- 런타임 상태 확인이 꼭 필요한 경우에만 `execute_script`이나 `get_unity_logs`를 사용한다

## 게임 설계

### 조작 방식
- `↑/W` 누르는 동안 전진 (가속도 기반), 놓으면 감속
- `↓/S` 후진 (절반 속도)
- `←/→` 레인 전환 (3레인: -1, 0, 1)
- `Space` 점프, `Left Shift` 슬라이드
- 공중에서는 관성 유지 (airDeceleration으로 천천히 감속)

### 물리 설정
- 플레이어: Capsule, height=2, center=(0,0,0), position.y=1 → 발 y=0 (도로 위)
- 도로 윗면: y=0
- Ground Layer: index 6
- useGravity=true, 상승/하강 모두 추가 중력 적용

> 플레이어 관련 규칙은 `.claude/rules/player.md` 참조

## Game 뷰 해상도 (깨져 보이는 문제)
- **Free Aspect**는 Game 탭의 창 크기 = 렌더링 해상도이므로, 창이 작으면 저해상도로 렌더링되어 깨져 보인다
- **Full HD(1920×1080)** 등 고정 해상도 프리셋을 사용하면 정상 품질로 확인 가능
- 이것은 에디터 표시 문제이지 실제 빌드 품질과 무관하다 — 빌드 시 디바이스 네이티브 해상도로 렌더링됨
- 따라서 "깨져 보인다"는 피드백이 있을 때, 카메라/URP 설정을 건드리기 전에 **먼저 Game 뷰 해상도 프리셋을 확인**한다
