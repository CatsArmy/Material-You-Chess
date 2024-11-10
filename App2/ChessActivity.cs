using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.ImageView;

namespace Chess
{

    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar")]
    public class ChessActivity : AppCompatActivity
    {
        public bool MaterialYouThemePreference;
        private string PlayerName = "Guest2";
        private AppPermissions permissions;
        private Android.Net.Uri uri;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            bool hasValue = bool.TryParse(base.Intent.GetStringExtra(nameof(this.MaterialYouThemePreference)), out this.MaterialYouThemePreference);
            if (hasValue && !this.MaterialYouThemePreference)
                base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Permission request logic
            this.permissions = new AppPermissions();
            this.permissions.RequestPermissions(this);

            //Set our view
            base.SetContentView(Resource.Layout.chess_activity);
            //Run our logic
            this.PlayerName = base.Intent.GetStringExtra(nameof(this.PlayerName));
            this.uri = Android.Net.Uri.Parse(base.Intent.GetStringExtra(nameof(uri)));

            ShapeableImageView p1MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
            if (this.uri != null)
                p1MainProfileImageView.SetImageURI(this.uri);
            TextView p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
            p1MainUsername.Text = this.PlayerName;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            // Handle permission requests results
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}