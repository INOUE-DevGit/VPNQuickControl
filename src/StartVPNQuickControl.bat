@echo off

rem _/_/_/_/_/_/_/_/_/ ユーザー設定 _/_/_/_/_/_/_/_/_/

rem アプリケーションを配置しているディレクトリのパス
set APP_DIR="C:\Dev\VPNQuickControl"

rem アプリケーションの実行ファイル名
set APP_EXE="VPNQuickControl.exe"

rem _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/


if not exist %APP_DIR%\%APP_EXE% (
    echo 必要な実行ファイルが存在しません: %APP_DIR%\%APP_EXE%
    pause
    exit
)

cd %APP_DIR%
start "" %APP_EXE%

exit

