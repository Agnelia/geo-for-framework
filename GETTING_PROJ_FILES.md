# Getting the PROJ files for this application

This application needs the **PROJ runtime library** and its data database. Put
them in `GeoForFramework/App_Data/proj/` before building or deploying the app.
This guide follows the [official PROJ installation and build requirements](https://proj.org/en/stable/install.html#build-requirements).

> You do not need to build PROJ just to run the application. A compatible
> Windows runtime package is enough. The steps below show how to build PROJ
> yourself when you need to produce or control that package.

## Build PROJ on Windows

1. **Install the build prerequisites.** PROJ's current build requirements are:
   - A C99 compiler and a C++17 compiler.
   - CMake 3.22.1 or newer.
   - SQLite 3.11 or newer, including headers and libraries for the target
     architecture, plus the `sqlite3` executable.
   - nlohmann/json 3.7.0 or newer. PROJ can use its vendored copy if no
     external copy is found.
   - libtiff 4.0 or newer is optional but recommended for GeoTIFF grids.
   - curl 7.29.0 or newer is optional but recommended for downloading remote
     grid files.

   Visual Studio's C++ build tools can provide the compilers. If you leave
   PROJ's tests enabled (they are enabled by default), also install GoogleTest
   1.8.1 or newer and Python 3.7 or newer. This guide disables tests to avoid
   those additional build-time requirements.

2. **Download and extract a PROJ source release.** Use the release link on the
   [official PROJ download page](https://proj.org/en/stable/download.html).
   Open a Developer Command Prompt for Visual Studio and change to the
   extracted source directory.

3. **Configure a release build.** For PROJ 9.5 or newer, configure the Windows
   library name to match this application's P/Invoke declaration (`proj_9.dll`):

   ```bat
   cmake -S . -B build -DBUILD_TESTING=OFF -DPROJ_OUTPUT_NAME=proj_9 -DAPPEND_SOVERSION=OFF
   ```

   The library-name options were added in PROJ 9.5. For an older PROJ release,
   use a compatible runtime package that provides `proj_9.dll`.

4. **Build and install PROJ.** These commands install the runtime into
   `C:\proj-install`:

   ```bat
   cmake --build build --config Release
   cmake --install build --config Release --prefix C:\proj-install
   ```

5. **Check the installed files.** The install should contain the PROJ DLL under
   `C:\proj-install\bin` and the PROJ data under `C:\proj-install\share\proj`.
   Confirm that `proj.db` exists in the data directory. PROJ also documents
   downloading available resource files with `projsync --system-directory
   --all`; if you use it, locate the resulting data directory and copy the
   required resources with `proj.db`.

6. **Copy the runtime files into the application.** Create
   `GeoForFramework\App_Data\proj` if it does not exist. Copy `proj_9.dll` and
   `proj.db` there. Also copy any dependent runtime DLLs from the PROJ install
   that `proj_9.dll` needs; keep them beside `proj_9.dll`. Do not copy build tools
   or the `projsync` executable.

7. **Check the application folder.** At minimum, it should contain:

   ```text
   GeoForFramework/
     App_Data/
       proj/
         proj_9.dll
         proj.db
         (any required PROJ runtime dependency DLLs)
   ```

8. **Build and test the app.** Build the solution in Visual Studio and run a
   coordinate conversion. For IIS deployment, ensure the application pool
   process architecture matches the PROJ DLL architecture and has read access
   to `App_Data\proj`.

The app looks for PROJ in this application-local folder rather than relying on
a machine-wide installation. See [SETUP_WINDOWS_IIS.md](SETUP_WINDOWS_IIS.md)
for the rest of the Windows development and deployment steps.

## Using a prebuilt Windows package instead

The official PROJ installation guide describes installing Windows packages
through [OSGeo4W](https://trac.osgeo.org/osgeo4w/). This avoids compiling PROJ,
but you must still copy the compatible `proj_9.dll`, `proj.db`, and any
required runtime dependency DLLs into the application's `App_Data\proj` folder.
Check the DLL's actual filename and architecture; do not rename another PROJ
version's DLL to `proj_9.dll`.
