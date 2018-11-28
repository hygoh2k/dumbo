# Dumbo
A Web api job scheduler written in C# .NET CORE 
This application is tested in Windows7 x64 with .NET CORE 2.1


## Requirements
required for development mode: .NET Core2.1
optional: Visual Studio 2017


## Configuration
### Port Configuration
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

<imgur_api_token>: The oauth2 access token. See https://apidocs.imgur.com for requesting the tokens from imgur.




## Running the WebAPI:
### In development mode
1) cd to <project_root>\WebAPI
2) execute the command: "dotnet run"
3) the service will be started. 

### In release mode (Currently only available for windows)
1) download the release at https://github.com/hygoh2k/dumbo/releases/tag/
2) unzip the file and go into the folder
3) in command prompt, execute "WebAPI.exe" to start the service


## Supported API


### GET v1/image
Retrieve the uploaded images from imgur account

Example:
https://localhost:5001/v1/image

Returns:
{"uploaded":["https://i.imgur.com/51On245.jpg","https://i.imgur.com/CdoiKIh.jpg","https://i.imgur.com/rROv9C1.jpg","https://i.imgur.com/pDzHKKJ.jpg","https://i.imgur.com/WF2Hviw.jpg","https://i.imgur.com/wOH6E6W.jpg","https://i.imgur.com/xKWoKqk.jpg","https://i.imgur.com/uCGnLze.jpg","https://i.imgur.com/pxk6xSW.jpg","https://i.imgur.com/jlP4Rmh.jpg","https://i.imgur.com/kdeDXUd.jpg","https://i.imgur.com/KKotLmR.jpg","https://i.imgur.com/MRYZSdB.jpg","https://i.imgur.com/PSSehMx.jpg","https://i.imgur.com/Z5NylH1.jpg","https://i.imgur.com/NPu24Qw.jpg","https://i.imgur.com/gck0ieu.jpg"]}




### POST v1/image/upload

Example:
https://localhost:5001/v1/image/upload

Input body in JSON:
{
	"urls" : [
				"https://cptv.org/wp-content/uploads/2017/04/Big-Pacific_ProgramMain.jpg",
				"https://cptv.org/wp-content/uploads/2017/04/Big-Pacific_ProgramMain.jpg",
				"https://www.planwallpaper.com/static/images/computer-desktop-wallpapers-3d.jpg",
				"https://www.nosuchimage.com/static/images/wallpaper-scenic-marcin-guitar-desktop-26218.jpg",
				"https://www.planwallpaper.com/static/images/wallpaper-scenic-marcin-guitar-desktop-26218.jpg"
				]
}

Returns:
{"jobId":"25e90440-5823-4fe9-a40d-0323fbd30079"}


### GET v1/image/job?id=(job_id)

Example:
https://localhost:5001/v1/image/job?id=25e90440-5823-4fe9-a40d-0323fbd30079

Returns:
{"id":"25e90440-5823-4fe9-a40d-0323fbd30079","created":"2018-11-28T15:36:13.3994004Z","finished":"2018-11-28T15:36:38.0064004Z","status":"completed","uploaded":{"pending":[],"complete":["https://i.imgur.com/51On245.jpg","https://i.imgur.com/rROv9C1.jpg","https://i.imgur.com/CdoiKIh.jpg"],"failed":["https://www.nosuchimage.com/static/images/wallpaper-scenic-marcin-guitar-desktop-26218.jpg"]}}
