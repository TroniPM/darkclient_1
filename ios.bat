:找到注册表中unity的位置
for /f "delims=" %%i in ('REG.EXE QUERY "HKEY_CLASSES_ROOT\com.unity3d.kharma\DefaultIcon" /VE ') do set InstallDir="%%i"
set InstallDir=%InstallDir:~22,-11%
echo Unity3d的安装路径：%InstallDir%

:导入内置shader到resources
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod ExportScenesManager.CopyBuildInShaders

:内置shader转换
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod ExportScenesManager.HandleBuildInShaders

:切换为ios平台
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.Switch2IosTarget

:多进程拷贝资源文件，这个过程非常长，一般超过一个小时或者几个小时
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.NewConsoleBuild

:拷贝一个version.xml文件到Resources目录
cd %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyVersionXml2Resources

:生成xml文件对应的hmf文件，并拷贝对应的版本目录
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.ConsoleBuildHmf

:生成pkg文件，内容包括：meta.xml、meta.xml中对应的文件、mogores、data,,ios版没有mogores
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

:拷贝assets/script到publish/assets/script，ios不支持动态加载dll,所以需要全部源码
cd /d %InstallDir%
Unity.exe -batchmode -quit -projectPath %~dp0 -executeMethod BuildProjectExWizard.CopyScript2IosPublish

pause