using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace Chess
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar")]
    public class ChessActivity : AppCompatActivity
    {
        public bool MaterialYouThemePreference;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MaterialYouThemePreference = bool.TryParse(base.Intent.GetStringExtra(nameof(MaterialYouThemePreference)), out MaterialYouThemePreference);
            if (!MaterialYouThemePreference)
                base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            base.SetContentView(Resource.Layout.chess_activity);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}