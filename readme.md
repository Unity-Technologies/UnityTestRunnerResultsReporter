# Unity Test Runner Results Reporter
The Unity Test Runner Results Reporter is a .NET Core assmebly that, when invoked with the correct parameters, parses test results from Unity Test Runner results .xml file formate (e.g. Unity.exe -runTests ...) and generates an html report.

## Usage

`--resultsPath=<Path to a directory where test result XML file and Unity log file can be find>`

`[--resultXMLName="Name of the test result XML file, reporter will look for TestResults.xml if no value"]`

`[--unityLogName="Name of the unity log file, reporter will look for UnityLog.txt if no value"]`

`[--reportdirpath="Path to where the report will be written"]`

**Example 1:**

`// Run Unity Test Runner tests with default result file names`

`Unity.exe -projectPath G:\MyUnityTest -logFile G:\MyUnityTest\Results\UnityLog.txt -testPlatform Android -testResults G:\MyUnityTest\Results\TestResults.xml -buildTarget Android -runTests`

`// Run reporter for test results`

`dotnet UnityTestRunnerResultsReporter.dll --resultsPath=G:\MyUnityTest\Results`


**Example 2:**

`// Run Unity Test Runner tests with customized result file names`

`Unity.exe -projectPath G:\MyUnityTest -logFile G:\MyUnityTest\Results\myLog.txt -testPlatform Android -testResults G:\MyUnityTest\Results\results.xml -buildTarget Android -runTests`


`// Run reporter for test results`

`dotnet UnityTestRunnerResultsReporter.dll --resultsPath=G:\MyUnityTest\Results --resultXMLName=results.xml --unityLogName=myLog.txt`


**Options:**

  `-?, --help, -h             Prints out the options.`
  
 `     --resultsPath=VALUE // REQUIRED - Path to a directory where test result XML file and Unity log file can be find.`
 
 `     --resultXMLName, --XMLName[=VALUE] // OPTIONAL - Name of test result XML file name.`
 
  `    --unityLogName, --LogName[=VALUE] // OPTIONAL - Name of Unity log file name.`

 `     --report, --reportdirpath[=VALUE] // OPTIONAL - Path to where the report will be`
