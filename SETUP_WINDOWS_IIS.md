# Windows + IIS Express setup for geo-for-framework

This project is a .NET Framework WebForms app that uses PROJ as the authoritative geodetic engine for WGS84 -> SWEREF99 TM.

Important:
- This setup is for Windows + Visual Studio + IIS Express only.
- No separate local IIS installation is required for development.
- Do not use WSL.
- The PROJ native files must be bundled with the app in `GeoForFramework/App_Data/proj`.

## 1. Prerequisites

Install on the developer machine:
- Visual Studio 2022 with:
  - ASP.NET and web development
  - .NET desktop development
  - .NET Framework 4.8.1 targeting pack
- PROJ runtime binaries for Windows

No full IIS Manager setup is needed for local development. IIS Express is included with Visual Studio and is the supported local runtime.

Download the Windows PROJ runtime from the official PROJ project binaries or a trusted Windows distribution package, then extract it and copy the native files into the app folder before running locally:
- `proj.dll`
- `proj.db`

A typical setup is:
1. Download the official Windows PROJ binary package from the PROJ release page or a trusted Windows package source.
2. Extract the archive to a temporary folder.
3. Locate the files in the extracted package.
   - `proj.dll` is usually under a `bin` folder.
   - `proj.db` is usually under a `share\proj` or similar data folder.
4. Copy both files into `GeoForFramework/App_Data/proj` so the app can load them from the project folder.

The folder structure should look like this:

`GeoForFramework/App_Data/proj/`
- `proj.dll`
- `proj.db`

If the package contains the files in nested folders, keep only the required two files and place them directly in `App_Data/proj`.

## 2. Open the project in Visual Studio

1. Open `GeoForFramework.sln` in Visual Studio.
2. Set the solution configuration to `Debug` or `Release`.
3. Ensure the target framework is `.NET Framework 4.8.1`.
4. Restore NuGet packages if prompted.
5. Build the solution.

## 3. Local development on Windows with IIS Express

1. Make sure the app has the `App_Data/proj` folder with the native PROJ files.
2. In Visual Studio, set the project to run with IIS Express.
3. Start debugging with F5.
4. Open the site in the browser.
5. Enter a WGS84 latitude/longitude and convert to SWEREF99 TM.

There is no special need for a separate local IIS site or IIS Manager setup when developing locally. IIS Express is sufficient for this project.

## 4. IIS deployment on Windows Server

This section is only relevant when deploying to a Windows server outside the local development workflow.

1. Build the project in `Release` configuration.
2. Copy the output to the IIS site folder, for example:
   - `C:\inetpub\wwwroot\geo-for-framework`
3. Ensure the app folder contains:
   - the compiled ASP.NET WebForms files
   - `App_Data\proj\proj.dll`
   - `App_Data\proj\proj.db`
4. In IIS Manager:
   - Create or select an IIS site
   - Point it to the deployment folder
   - Set the application pool to an ASP.NET 4.x compatible pool
5. Ensure the app pool identity has read access to the deployment folder and to `App_Data\proj`.
6. Start the site and browse to it.

## 5. Required runtime behavior

The app uses the PROJ native library and the database from the application folder, not from a global Windows install.

This is important because:
- the app should run consistently without needing a machine-wide PROJ installation
- the same deployment bundle can be moved to another server without extra manual native setup

## 6. Troubleshooting

If the app returns an error when converting coordinates:

- Check that `proj.dll` exists in `GeoForFramework/App_Data/proj`
- Check that `proj.db` exists in the same folder
- Confirm the app pool has read permissions to that folder
- Confirm the app is really running the output folder that contains those files
- Confirm the transformation is using EPSG:4326 -> EPSG:3006

## 7. Supported CRS mapping

- Source CRS: `EPSG:4326` (WGS84)
- Target CRS: `EPSG:3006` (SWEREF99 TM)

This is the approved and maintained mapping for Swedish national grid conversion.

## 8. No WSL requirement

This project should be run directly on Windows with:
- Visual Studio
- IIS Express
- local Windows development

Do not rely on WSL for the app runtime or local development flow.
