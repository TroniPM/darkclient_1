
:�����鿴��־�Ĺ��ߣ�����ִ��
start DebugLogViewer.exe

:�ȴ�5�룬��־����������Ҫʱ�䣬���ȴ�����ղ�����־��Ϣ
ping 127.0.0.1 -n 5

:�ҵ�ע�����unity��λ��
for /f "delims=" %%i in ('REG.EXE QUERY "HKEY_CLASSES_ROOT\com.unity3d.kharma\DefaultIcon" /VE ') do set InstallDir="%%i"
set InstallDir=%InstallDir:~22,-11%
echo Unity3d�İ�װ·����%InstallDir%

cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.NewConsoleBuild
pause