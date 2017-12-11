
:启动查看日志的工具，最先执行
start DebugLogViewer.exe

:等待5秒，日志工具启动需要时间，不等待会接收不到日志消息
ping 127.0.0.1 -n 5

:找到注册表中unity的位置
for /f "delims=" %%i in ('REG.EXE QUERY "HKEY_CLASSES_ROOT\com.unity3d.kharma\DefaultIcon" /VE ') do set InstallDir="%%i"
set InstallDir=%InstallDir:~22,-11%
echo Unity3d的安装路径：%InstallDir%

cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.NewConsoleBuild
pause