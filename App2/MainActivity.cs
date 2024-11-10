using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Google.Android.Material.ImageView;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Chess
{

    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private string PlayerName = "Guest2";
        private bool MaterialYouThemePreference = true;
        private PickVisualMediaRequest.Builder pickVisualMediaRequestBuilder;
        private ShapeableImageView MainProfileImageView;
        private ActivityResultLauncher photoPicker;
        private AppPermissions permissions;
        private Android.Net.Uri uri;
        private Button StartGame;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            _ = this.GetMaterialYouThemePreference(out this.MaterialYouThemePreference);

            this.photoPicker = RegisterForActivityResult(new PickVisualMedia(),
                new ActivityResultCallback<Android.Net.Uri>(uri =>
                {
                    //Logic
                    Log.Debug("PhotoPicker", $"{uri}");
                    if (uri != null)
                        this.MainProfileImageView.SetImageURI(uri);
                    this.uri = uri;
                }));

            this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder()
                .SetMediaType(PickVisualMedia.ImageOnly.Instance);

            if (!this.MaterialYouThemePreference)
                base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from layout resource
            base.SetContentView(Resource.Layout.main_activity);
            // Permission request logic
            this.permissions = new AppPermissions();
            this.permissions.RequestPermissions(this);
            //Run our logic
            this.StartGame = base.FindViewById<Button>(Resource.Id.btnStartGame);
            this.StartGame.Click += StartGame_Click;
            this.MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.MainProfileImageView);
            this.photoPicker.Launch(this.pickVisualMediaRequestBuilder.Build());
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ChessActivity))
            .PutExtra(nameof(ChessActivity.MaterialYouThemePreference), $"{this.MaterialYouThemePreference}")
            .PutExtra(nameof(this.PlayerName), this.PlayerName)
            .PutExtra(nameof(this.uri), $"{this.uri}");
            base.StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            // Handle permission requests results
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}