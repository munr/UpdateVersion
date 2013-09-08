Option Explicit

'-----------------------------------------------------------------------
'Script settings
'-----------------------------------------------------------------------
Dim UpdateAppPath:	UpdateAppPath = "C:\Utilities\UpdateVersion.exe"
Dim UpdateAppArgs:	UpdateAppArgs = "--build YearDayOfYear --revision Increment"


'-----------------------------------------------------------------------
'Initialise objects
'-----------------------------------------------------------------------
Dim oFs: Set oFs = CreateObject("Scripting.FileSystemObject")
Dim oShell: Set oShell = WScript.CreateObject("WScript.Shell")

'-----------------------------------------------------------------------
'Main()
'-----------------------------------------------------------------------
If (WScript.Arguments.Count = 0) Then
	WScript.Echo ("Folder must be specified as command line parameter")
	Call WScript.Quit(0)
End If

Dim singleFolder: singleFolder = WScript.Arguments.Item(0)
Call UpdateAssembly(singleFolder)

'-----------------------------------------------------------------------
'Clean up
'-----------------------------------------------------------------------
Set oShell = Nothing
Set oFs = Nothing



'-----------------------------------------------------------------------
'Helper Methods
'-----------------------------------------------------------------------
Sub UpdateAssembly(folder)
	Dim filepath: filePath = folder & "\AssemblyInfo.cs"
	If (oFs.FileExists(filePath)) Then
		Dim command: command = UpdateAppPath & " " & UpdateAppArgs & " --inputfile """ & filePath & """ --outputfile """ & filePath & """"
		Call oShell.Run(command)
	End If
End Sub