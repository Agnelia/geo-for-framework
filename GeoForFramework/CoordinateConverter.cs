using System;

namespace GeoForFramework
{
    public static class CoordinateConverter
    {
        private const double SemiMajorAxis = 6378137.0;
        private const double Flattening = 1.0 / 298.257223563;
        private const double SemiMinorAxis = SemiMajorAxis * (1.0 - Flattening);
        private const double EccentricitySquared = 1.0 - (SemiMinorAxis * SemiMinorAxis) / (SemiMajorAxis * SemiMajorAxis);
        private const double CentralMeridian = Math.PI * (15.0 / 180.0);
        private const double FalseEasting = 500000.0;
        private const double FalseNorthing = 0.0;
        private const double ScaleFactor = 0.9996;
        private const double OriginLatitude = 0.0;

        public static bool TryConvertWgs84ToSweref99Tm(double latitude, double longitude, out double easting, out double northing)
        {
            easting = 0;
            northing = 0;

            if (double.IsNaN(latitude) || double.IsNaN(longitude))
            {
                return false;
            }

            var lat = latitude.ToRadians();
            var lon = longitude.ToRadians();

            var sinLat = Math.Sin(lat);
            var cosLat = Math.Cos(lat);
            var tanLat = Math.Tan(lat);

            var n = (SemiMajorAxis * (1.0 - EccentricitySquared)) /
                    Math.Pow(1.0 - EccentricitySquared * sinLat * sinLat, 1.5);

            var v = SemiMajorAxis / Math.Sqrt(1.0 - EccentricitySquared * sinLat * sinLat);

            var x = lon - CentralMeridian;
            var a = x * cosLat;
            var m = SemiMajorAxis * ((1.0 - EccentricitySquared / 4.0 - 3.0 * EccentricitySquared * EccentricitySquared / 64.0 - 5.0 * Math.Pow(EccentricitySquared, 3.0) / 256.0) * lat
                - (3.0 * EccentricitySquared / 8.0 + 3.0 * EccentricitySquared * EccentricitySquared / 32.0 + 45.0 * Math.Pow(EccentricitySquared, 3.0) / 1024.0) * Math.Sin(2.0 * lat)
                + (15.0 * EccentricitySquared * EccentricitySquared / 256.0 + 45.0 * Math.Pow(EccentricitySquared, 3.0) / 1024.0) * Math.Sin(4.0 * lat)
                - (35.0 * Math.Pow(EccentricitySquared, 3.0) / 3072.0) * Math.Sin(6.0 * lat));

            var eastingUnscaled = FalseEasting + ScaleFactor * v * (a + (1.0 - tanLat * tanLat + EccentricitySquared * cosLat * cosLat) * Math.Pow(a, 3.0) / 6.0 + (5.0 - 18.0 * tanLat * tanLat + tanLat * tanLat * tanLat * tanLat + 14.0 * EccentricitySquared * cosLat * cosLat - 58.0 * EccentricitySquared * sinLat * sinLat) * Math.Pow(a, 5.0) / 120.0);
            var northingUnscaled = FalseNorthing + ScaleFactor * (m + v * tanLat * (Math.Pow(a, 2.0) / 2.0 + (5.0 - tanLat * tanLat + 9.0 * EccentricitySquared * cosLat * cosLat + 4.0 * EccentricitySquared * EccentricitySquared * cosLat * cosLat * cosLat * cosLat) * Math.Pow(a, 4.0) / 24.0 + (61.0 - 58.0 * tanLat * tanLat + tanLat * tanLat * tanLat * tanLat + 270.0 * EccentricitySquared * cosLat * cosLat - 330.0 * EccentricitySquared * sinLat * sinLat) * Math.Pow(a, 6.0) / 720.0));

            easting = eastingUnscaled;
            northing = northingUnscaled;

            return true;
        }
    }

    public static class DoubleExtensions
    {
        public static double ToRadians(this double value)
        {
            return value * Math.PI / 180.0;
        }
    }
}
