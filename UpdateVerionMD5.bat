:�ҵ�ע�����unity��λ��
for /f "delims=" %%i in ('REG.EXE QUERY "HKEY_CLASSES_ROOT\com.unity3d.kharma\DefaultIcon" /VE ') do set InstallDir="%%i"
set InstallDir=%InstallDir:~22,-11%
echo Unity3d�İ�װ·����%InstallDir%

cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod ExportScenesManager.UpdatePkgWithMd5

pause