:�ҵ�ע�����unity��λ��
for /f "delims=" %%i in ('REG.EXE QUERY "HKEY_CLASSES_ROOT\com.unity3d.kharma\DefaultIcon" /VE ') do set InstallDir="%%i"
set InstallDir=%InstallDir:~22,-11%
echo Unity3d�İ�װ·����%InstallDir%

:��������shader��resources
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod ExportScenesManager.CopyBuildInShaders

:����shaderת��
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod ExportScenesManager.HandleBuildInShaders

:�л�Ϊiosƽ̨
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.Switch2IosTarget

:����̿�����Դ�ļ���������̷ǳ�����һ�㳬��һ��Сʱ���߼���Сʱ
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.NewConsoleBuild

:����һ��version.xml�ļ���ResourcesĿ¼
cd %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyVersionXml2Resources

:����xml�ļ���Ӧ��hmf�ļ�����������Ӧ�İ汾Ŀ¼
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuildHmf

:����pkg�ļ������ݰ�����meta.xml��meta.xml�ж�Ӧ���ļ���mogores��data,,ios��û��mogores
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.BuildFirstExport

:�����Ѿ������pkg�ļ��е�pkgtemp��ʱ�ļ���export�ж�Ӧ���ļ�
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CutFirstExportToPkgDir

:����resourceindex,����������Ӧ�汾Ŀ¼
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuildFileIndex
 
:�����汾������Ŀ¼��publish���̵�asset/StreamingAssetsĿ¼,publish��Ŀ¼Ҫ��BuildConfig.xml�ļ�������
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyResourceToPublishStreamingAsset

:����assets/script��publish/assets/script��ios��֧�ֶ�̬����dll,������Ҫȫ��Դ��
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyScript2IosPublish

pause