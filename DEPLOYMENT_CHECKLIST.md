# Windows + IIS deployment checklist for GeoForFramework

Use this as the fast operational checklist before deploying the app to IIS on a Windows machine.

## 1) Prepare the project
- Open `GeoForFramework.sln` in Visual Studio on Windows.
- Make sure the target framework matches the installed .NET Framework runtime on the machine.
- Restore NuGet packages if required.
- Build the solution in Debug or Release.

## 2) Prepare the PROJ runtime files
The app expects PROJ runtime files to be available locally in the application folder, not from a system-wide install.

Copy these files into:
- `GeoForFramework/App_Data/proj/`

Required files:
- `proj_9.dll`
- `proj.db`
- any required runtime dependencies (depending on the PROJ version and Windows build)

Notes:
- Do not rely on a machine-wide PROJ install.
- Keep the native files next to the app deployment.
- If the app cannot find the files, the transform will fail or use the wrong CRS.

## 3) Check app-local layout
Your deployed app should include a directory similar to:
- `GeoForFramework/App_Data/proj/`
- `GeoForFramework/App_Data/proj/proj_9.dll`
- `GeoForFramework/App_Data/proj/proj.db`

Verify that the application can resolve this folder at runtime.

## 4) Configure IIS
- Open IIS Manager.
- Create or select an Application Pool using .NET CLR version matching the app.
- Create a site or virtual directory for the app.
- Point it to the build output folder or published folder.
- Ensure the IIS worker process has permission to read the app and `App_Data/proj` contents.

## 5) Application pool / permissions
- Use an identity that can access the folder.
- Grant read access to the published app folder.
- Ensure the PROJ data directory and DLL are readable by IIS.

## 6) Test locally before production
Open the app in a browser and test the conversion with a known Swedish coordinate.

Example values to verify:
- Latitude: `59.3293`
- Longitude: `18.0686`

Expected output should be a valid SWEREF99 TM easting/northing value in EPSG:3006.

## 7) Validate the CRS setup
The app should use:
- Source CRS: `EPSG:4326` (WGS84)
- Target CRS: `EPSG:3006` (SWEREF99 TM)

This is the official Swedish mapping for the project.

## 8) Validate output
- Confirm the page accepts latitude and longitude.
- Confirm the conversion button calls the transform service.
- Confirm the UI displays easting and northing.
- Confirm no custom CRS math is embedded directly in the page code-behind.

## 9) Deployment final check
Before going live:
- [ ] Build succeeds in Visual Studio
- [ ] PROJ DLL is present in `App_Data/proj`
- [ ] `proj.db` is present in `App_Data/proj`
- [ ] IIS app has read access to those files
- [ ] Browser test passes with known Swedish coordinates
- [ ] Conversion output is in SWEREF99 TM (EPSG:3006)

## 10) Quick troubleshooting
If conversion fails:
- Check that the PROJ files are copied to the correct folder
- Check the app pool identity permissions
- Confirm the DLL and `proj.db` are not missing
- Confirm the target CRS is set to `EPSG:3006`
- Confirm the page is calling the transform service and not custom logic directly

## 11) Production rule
This app should use the official PROJ library as the authoritative transformation engine, with the app-local native files deployed with the application.
