:�ҵ�ע�����unity��λ��
for /f "delims=" %%i in ('REG.EXE QUERY "HKEY_CLASSES_ROOT\com.unity3d.kharma\DefaultIcon" /VE ') do set InstallDir="%%i"
set InstallDir=%InstallDir:~22,-11%
echo Unity3d�İ�װ·����%InstallDir%

:�����鿴��־�Ĺ��ߣ�����ִ��
start DebugLogViewer.exe

:�ȴ�5�룬��־����������Ҫʱ�䣬���ȴ�����ղ�����־��Ϣ
ping 127.0.0.1 -n 5

:����һ���µİ汾��Ӧ���ļ��У�����0.2.0.0
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CreateUpVersionFolder

:������Դ�ļ���������̷ǳ�����һ�㳬��һ��Сʱ���߼���Сʱ
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuild

:����һ��version.xml�ļ���ResourcesĿ¼
cd %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyVersionXml2Resources

:����������ִ��һ���պ���������csharp���̵�һ��dll���������벢��������Ӧ�İ汾Ŀ¼��
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.EmptyFunc

cd /d %~dp0
C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\msbuild.exe Assembly-CSharp.csproj /t:rebuild 

cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ExportMogoLib

:����xml�ļ���Ӧ��hmf�ļ�����������Ӧ�İ汾Ŀ¼
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuildHmf

:����pkg�ļ������ݰ�����meta.xml��meta.xml�ж�Ӧ���ļ���mogores��data
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

pause