:找到注册表中unity的位置
for /f "delims=" %%i in ('REG.EXE QUERY "HKEY_CLASSES_ROOT\com.unity3d.kharma\DefaultIcon" /VE ') do set InstallDir="%%i"
set InstallDir=%InstallDir:~22,-11%
echo Unity3d的安装路径：%InstallDir%

:启动查看日志的工具，最先执行
start DebugLogViewer.exe

:等待5秒，日志工具启动需要时间，不等待会接收不到日志消息
ping 127.0.0.1 -n 5

:创建一个新的版本对应的文件夹，比如0.2.0.0
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CreateUpVersionFolder

:拷贝资源文件，这个过程非常长，一般超过一个小时或者几个小时
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuild

:拷贝一个version.xml文件到Resources目录
cd %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyVersionXml2Resources

:下面三个，执行一个空函数，编译csharp工程到一个dll，混淆代码并拷贝到对应的版本目录中
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.EmptyFunc

cd /d %~dp0
C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\msbuild.exe Assembly-CSharp.csproj /t:rebuild 

cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ExportMogoLib

:生成xml文件对应的hmf文件，并拷贝对应的版本目录
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuildHmf

:生成pkg文件，内容包括：meta.xml、meta.xml中对应的文件、mogores、data
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.BuildFirstExport

:剪切已经打包到pkg文件中到pkgtemp临时文件，export中对应的文件
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CutFirstExportToPkgDir

:生成resourceindex,并拷贝到对应版本目录
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuildFileIndex
 
:拷贝版本的整个目录到publish工程的asset/StreamingAssets目录,publish的目录要在BuildConfig.xml文件中配置
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyResourceToPublishStreamingAsset

pause