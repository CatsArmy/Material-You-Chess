using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Color;
using Google.Android.Material.TextField;

namespace TicTacToe
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextInputEditText Player1NameInput;
        private TextInputEditText Player2NameInput;
        private Button StartGame;
        private static bool isRecreated = false;
        private const string Empty = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            base.SetContentView(Resource.Layout.activity_main);
            //Apply Material You
            if (!isRecreated)
            {
                isRecreated = true;
                DynamicColors.ApplyToActivitiesIfAvailable(base.Application);
                base.Recreate();
            }

            Player1NameInput = base.FindViewById<TextInputEditText>(Resource.Id.TextInputEditText1);
            Player2NameInput = base.FindViewById<TextInputEditText>(Resource.Id.TextInputEditText2);
            StartGame = base.FindViewById<Button>(Resource.Id.StartGame);
            StartGame.Click += StartGame_Click;
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            string player1 = Player1NameInput.Text switch
            {
                null => "Player 1",
                Empty => "Player 1",
                _ => Player1NameInput.Text,
            };
            string player2 = Player1NameInput.Text switch
            {
                null => "Player 2",
                Empty => "Player 2",
                _ => Player2NameInput.Text,
            };

            Intent intent = new Intent(this, typeof(MainGameActivity))
                .PutExtra(nameof(MainGameActivity.Player1Name), player1)
                .PutExtra(nameof(MainGameActivity.Player2Name), player2);
            base.StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}