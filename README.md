# Dumbo
A Web api job scheduler written in C$ .NET CORE 
This application is tested in Windows7 x64 with .NET CORE 2.1

## Requirements
required: .NET Core2.1
optional: Visual Studio 2017


## Configuration
### Port Congiguration
<project_root>\WebAPI\Properties\launchSettings.json:
Setup the port in applicationUrl. Example:

"WebAPI": {
      "commandName": "Project",
      "launchBrowser": false,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }

### imgur Congiguration
<project_root>\WebAPI\Properties\imgur_setting.json:

{
  "BaseUrl": "https://api.imgur.com",
  "Token": "<imgur_api_token>",
  "GetImageUrl": "3/account/me/images",
  "PostImageUrl": "3/image"
} 

<imgur_api_token>: See https://apidocs.imgur.com on requesting the tokens from imgur

## Running the WebAPI:
### In development mode
1) cd to <project_root>\WebAPI
2) execute the command: "dotnet run"
3) the service will be started. 

### In release mode (Currently only available for windows)
1) download https://github.com/hygoh2k/dumbo/releases/tag/vx.x
2) unzip the file and go into the folder
3) in command prompt, execute "WebAPI.exe" to start the service


