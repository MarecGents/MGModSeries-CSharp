@echo off
rem ============================================================
rem  MGModSeries-CSharp build menu (double-click entry)
rem  Navigate with Up/Down arrows, Enter to run.
rem  NOTE: keep this file pure ASCII (cmd parses in ANSI).
rem ============================================================
chcp 65001 >nul
title MGModSeries-CSharp Build Menu
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-menu.ps1"
echo.
echo ============================================
echo  Build finished. Press any key to close.
echo ============================================
pause >nul
