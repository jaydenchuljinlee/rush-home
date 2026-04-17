# 빌드 환경

- **엔진**: Unity 6 LTS (2D URP)
- **언어**: C#
- **IDE**: JetBrains Rider / Visual Studio / VS Code
- **버전 관리**: Git + GitHub (Git LFS for assets)

## Unity 프로젝트 경로

프로젝트 루트에서 Unity 프로젝트는 별도 하위 폴더에 위치할 수 있음.
실제 Unity 프로젝트 폴더는 `Assets/`, `ProjectSettings/`, `Packages/`가 있는 위치.

## 주요 명령어

```bash
# Unity 배치모드 빌드 (Android)
Unity -batchmode -quit -projectPath . \
  -executeMethod BuildScript.BuildAndroid \
  -logFile build.log

# Unity 배치모드 빌드 (iOS)
Unity -batchmode -quit -projectPath . \
  -executeMethod BuildScript.BuildiOS \
  -logFile build.log

# Edit Mode 테스트 실행
Unity -batchmode -runTests -testPlatform EditMode \
  -projectPath . -testResults results.xml

# Play Mode 테스트 실행
Unity -batchmode -runTests -testPlatform PlayMode \
  -projectPath . -testResults results.xml
```

## C# 스크립트 작성 시 참고

- Unity 6 LTS는 .NET Standard 2.1 또는 .NET Framework 호환
- `async/await` 사용 가능 (Unity 메인스레드 주의)
- Nullable reference types 사용 가능 (프로젝트 설정에 따라)

## Git LFS 추적 대상

```
*.png, *.psd, *.wav, *.mp3, *.fbx, *.prefab, *.unity, *.asset
```

## .gitignore 필수 제외 항목

```
Library/
Temp/
Logs/
obj/
Build/
Builds/
UserSettings/
```
