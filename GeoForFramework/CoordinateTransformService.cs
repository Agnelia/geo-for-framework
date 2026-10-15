using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;

namespace GeoForFramework
{
    public class CoordinateTransformService
    {
        public bool TryTransformWgs84ToSweref99Tm(double latitude, double longitude, out double easting, out double northing)
        {
            easting = 0.0;
            northing = 0.0;

            if (double.IsNaN(latitude) || double.IsNaN(longitude) || latitude < -90.0 || latitude > 90.0 || longitude < -180.0 || longitude > 180.0)
            {
                return false;
            }

            var projDirectory = ProjNative.ResolveProjectDirectory();
            if (!ProjNative.TryPrepareAppLocalNativeEnvironment(projDirectory))
            {
                return false;
            }

            IntPtr context = IntPtr.Zero;
            IntPtr transform = IntPtr.Zero;

            try
            {
                context = ProjNative.proj_context_create();
                if (context == IntPtr.Zero)
                {
                    return false;
                }

                transform = ProjNative.proj_create_crs_to_crs(context, "EPSG:4326", "EPSG:3006", null);
                if (transform == IntPtr.Zero)
                {
                    return false;
                }

                var coordinate = new ProjNative.PJ_COORD { v1 = longitude, v2 = latitude, v3 = 0.0, v4 = 0.0 };
                var result = ProjNative.proj_trans(transform, ProjNative.PJ_FWD, coordinate);

                easting = result.v1;
                northing = result.v2;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (transform != IntPtr.Zero)
                {
                    ProjNative.proj_destroy(transform);
                }

                if (context != IntPtr.Zero)
                {
                    ProjNative.proj_context_destroy(context);
                }
            }
        }
    }

    internal static class ProjNative
    {
        internal const int PJ_FWD = 0;

        internal static string ResolveProjectDirectory()
        {
            var appBase = HttpRuntime.AppDomainAppPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(appBase, "App_Data", "proj"),
                Path.Combine(appBase, "proj")
            };

            foreach (var candidate in candidates)
            {
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            return candidates[0];
        }

        internal static bool TryPrepareAppLocalNativeEnvironment(string projDirectory)
        {
            if (string.IsNullOrWhiteSpace(projDirectory) || !Directory.Exists(projDirectory))
            {
                return false;
            }

            Environment.SetEnvironmentVariable("PROJ_DATA", projDirectory);
            Environment.SetEnvironmentVariable("PROJ_LIB", projDirectory);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (!NativeMethods.SetDllDirectory(projDirectory))
                {
                    return false;
                }

                return true;
            }

            var libraryPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? string.Empty;
            var paths = libraryPath.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

            if (Array.Exists(paths, path => string.Equals(path, projDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            Environment.SetEnvironmentVariable("LD_LIBRARY_PATH",\n                string.IsNullOrEmpty(libraryPath)
                    ? projDirectory
                    : projDirectory + Path.PathSeparator + libraryPath);

            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PJ_COORD
        {
            public double v1;
            public double v2;
            public double v3;
            public double v4;
        }

        [DllImport("proj_9.dll", EntryPoint = "proj_context_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr proj_context_create_windows();

        [DllImport("libproj.so", EntryPoint = "proj_context_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr proj_context_create_unix();

        [DllImport("proj_9.dll", EntryPoint = "proj_create_crs_to_crs", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr proj_create_crs_to_crs_windows(IntPtr context, string source, string target, string options);

        [DllImport("libproj.so", EntryPoint = "proj_create_crs_to_crs", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr proj_create_crs_to_crs_unix(IntPtr context, string source, string target, string options);

        [DllImport("proj_9.dll", EntryPoint = "proj_trans", CallingConvention = CallingConvention.Cdecl)]
        private static extern PJ_COORD proj_trans_windows(IntPtr transform, int direction, PJ_COORD point);

        [DllImport("libproj.so", EntryPoint = "proj_trans", CallingConvention = CallingConvention.Cdecl)]
        private static extern PJ_COORD proj_trans_unix(IntPtr transform, int direction, PJ_COORD point);

        [DllImport("proj_9.dll", EntryPoint = "proj_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void proj_destroy_windows(IntPtr obj);

        [DllImport("libproj.so", EntryPoint = "proj_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void proj_destroy_unix(IntPtr obj);

        [DllImport("proj_9.dll", EntryPoint = "proj_context_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void proj_context_destroy_windows(IntPtr obj);

        [DllImport("libproj.so", EntryPoint = "proj_context_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void proj_context_destroy_unix(IntPtr obj);

        internal static IntPtr proj_context_create()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? proj_context_create_windows()
                : proj_context_create_unix();
        }

        internal static IntPtr proj_create_crs_to_crs(IntPtr context, string source, string target, string options)
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? proj_create_crs_to_crs_windows(context, source, target, options)
                : proj_create_crs_to_crs_unix(context, source, target, options);
        }

        internal static PJ_COORD proj_trans(IntPtr transform, int direction, PJ_COORD point)
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                ? proj_trans_windows(transform, direction, point)
                : proj_trans_unix(transform, direction, point);
        }

        internal static void proj_destroy(IntPtr obj)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                proj_destroy_windows(obj);
                return;
            }

            proj_destroy_unix(obj);
        }

        internal static void proj_context_destroy(IntPtr obj)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                proj_context_destroy_windows(obj);
                return;
            }

            proj_context_destroy_unix(obj);
        }
    }

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetDllDirectory(string lpPathName);
    }
}
