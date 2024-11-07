using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;

namespace Chess
{

    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private bool MaterialYouThemePreference = true;
        private Button StartGame;
        MaterialButtonToggleGroup a;

        private ISharedPreferences InitializeMaterialYouThemePreference()
        {
            ISharedPreferences sharedPref = base.GetPreferences(FileCreationMode.Private);
            if (sharedPref.Contains(nameof(MaterialYouThemePreference)))
            {
                MaterialYouThemePreference = sharedPref.GetBoolean(nameof(MaterialYouThemePreference), MaterialYouThemePreference);
                return sharedPref;
            }
            var editor = sharedPref.Edit();
            editor.PutBoolean(nameof(MaterialYouThemePreference), MaterialYouThemePreference);
            editor.Commit();
            editor.Apply();
            return sharedPref;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ISharedPreferences sharedPref = InitializeMaterialYouThemePreference();
            if (!MaterialYouThemePreference)
            {
                base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);
            }

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            base.SetContentView(Resource.Layout.activity_main);


            StartGame = base.FindViewById<Button>(Resource.Id.btnStartGame);
            StartGame.Click += StartGame_Click;
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ChessActivity))
                //.PutExtra(nameof(ChessActivity.MaterialYouThemePreference), MaterialYouThemePreference)
                ;
            base.StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}