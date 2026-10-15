using System;
using System.Globalization;
using System.Web.UI;

namespace GeoForFramework
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResultLabel.Text = string.Empty;
            }
        }

        protected void ConvertButton_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(LatitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude) ||
                !double.TryParse(LongitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
            {
                ResultLabel.Text = "Please enter valid numeric latitude and longitude values.";
                EastingTextBox.Text = string.Empty;
                NorthingTextBox.Text = string.Empty;
                return;
            }

            if (!CoordinateConverter.TryConvertWgs84ToSweref99Tm(latitude, longitude, out var easting, out var northing))
            {
                ResultLabel.Text = "Latitude and longitude values are out of range for WGS84.";
                EastingTextBox.Text = string.Empty;
                NorthingTextBox.Text = string.Empty;
                return;
            }

            EastingTextBox.Text = easting.ToString("0.###", CultureInfo.InvariantCulture);
            NorthingTextBox.Text = northing.ToString("0.###", CultureInfo.InvariantCulture);
            ResultLabel.Text = string.Format(CultureInfo.InvariantCulture, "Converted result: {0} m E, {1} m N", easting.ToString("0.###", CultureInfo.InvariantCulture), northing.ToString("0.###", CultureInfo.InvariantCulture));
        }
    }
}