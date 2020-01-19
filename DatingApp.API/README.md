# Microsoft .Net Core 3.1 Web API

This is the details procedure of preparing project and necessary extension for Microsoft .Net Core 3.1 Web API 


## Developing Environment Setup
- [Microsoft .Net Core download](https://dotnet.microsoft.com/download/dotnet-core)
- [NodeJs](https://nodejs.org/en/)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Postman](https://www.getpostman.com/)
- [SQLite](https://www.sqlite.org/index.html)
#### VSCode Extensions
- C# by Microsoft
- C# Extensions by jchannon
- NuGet Package Manager by jmrog
- Prettier- Code formatter by Esben Peterson
- TSLint by Microsoft
- Material Icon Themes by Phillipp Kief
- Debugger for Chrome by Microsoft
- Bracket Pair Colorizer 2 by Coenraads

#### Exclude Object extension (.obj) files from vscode track:
- File -> Preference -> settings -> Exclude -> **/obj

## Create files and folders
In the Command Prompt:
- `dotnet -h` :  Get help with dotnet command
- `dotnet new webapi -n NameOfTheProject` : To create a new project
