@echo off

rem _/_/_/_/_/_/_/_/_/ ���[�U�[�ݒ� _/_/_/_/_/_/_/_/_/

rem �A�v���P�[�V������z�u���Ă���f�B���N�g���̃p�X
set APP_DIR="C:\Dev\VPNQuickControl"

rem �A�v���P�[�V�����̎��s�t�@�C����
set APP_EXE="VPNQuickControl.exe"

rem _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/


if not exist %APP_DIR%\%APP_EXE% (
    echo �K�v�Ȏ��s�t�@�C�������݂��܂���: %APP_DIR%\%APP_EXE%
    pause
    exit
)

cd %APP_DIR%
start "" %APP_EXE%

exit

