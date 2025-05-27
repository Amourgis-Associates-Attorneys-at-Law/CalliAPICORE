
[Setup]
AppName=CalliAPI
AppVersion={#AppVersion}
DefaultDirName={pf}\CalliAPI
DefaultGroupName=CalliAPI
OutputDir=dist
OutputBaseFilename={#OutputBaseFilename}
Compression=lzma
SolidCompression=yes

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\CalliAPI"; Filename: "{app}\CalliAPI.exe"
Name: "{group}\Uninstall CalliAPI"; Filename: "{uninstallexe}"
