@echo off
rem ============================================================
rem  MGModSeries-CSharp 构建菜单（双击入口）
rem  打开后按 ↑/↓ 选择一键编译或单独编译某项目，Enter 执行
rem ============================================================
chcp 65001 >nul
title MGModSeries-CSharp Build Menu
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-menu.ps1"
echo.
echo ============================================
echo  构建流程已结束。按任意键关闭窗口。
echo ============================================
pause >nul
