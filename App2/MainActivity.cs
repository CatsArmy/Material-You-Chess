using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Card;
using Google.Android.Material.TextField;

namespace TicTacToe
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextInputEditText Player1NameInput;
        private TextInputEditText Player2NameInput;
        private Button StartGame;
        private MaterialCardView UserLogin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            base.SetContentView(Resource.Layout.activity_main);

            Player1NameInput = base.FindViewById<TextInputEditText>(Resource.Id.TextInputEditText1);
            Player2NameInput = base.FindViewById<TextInputEditText>(Resource.Id.TextInputEditText2);
            StartGame = base.FindViewById<Button>(Resource.Id.StartGame);
            StartGame.Click += StartGame_Click;

            UserLogin = base.FindViewById<MaterialCardView>(Resource.Id.AccountDashboard);

            base.RegisterForContextMenu(UserLogin);

        }

        //#region Context Menu
        //public override void OnCreateContextMenu(Android.Views.IContextMenu menu, Android.Views.View view,
        //    Android.Views.IContextMenuContextMenuInfo menuInfo)
        //{
        //    base.OnCreateContextMenu(menu, view, menuInfo);

        //    MenuInflater.Inflate(Resource.Menu.login_menu, menu);
        //}
        //public override bool OnContextItemSelected(Android.Views.IMenuItem item)
        //{
        //    switch (item.ItemId)
        //    {
        //        case Resource.Id.LoginUser:
        //            Toast.MakeText(this, "You selected the Login user context item", ToastLength.Long).Show();
        //            return true;
        //        case Resource.Id.RegisterUser:
        //            Toast.MakeText(this, "You selected the Register user context item", ToastLength.Long).Show();
        //            return true;
        //        default:
        //            return base.OnContextItemSelected(item);
        //    }
        //}
        //#endregion
        //#region OptionsMenu
        //public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        //{
        //    base.MenuInflater.Inflate(Resource.Menu.login_menu, menu);

        //    return true;
        //}

        //public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        //{
        //    Toast.MakeText(this, $"you selected options item {item.ItemId}", ToastLength.Long);
        //    switch (item.ItemId)
        //    {
        //        case Resource.Id.LoginUser:
        //            Toast.MakeText(this, "You selected the Login user option item", ToastLength.Long).Show();
        //            return true;
        //        case Resource.Id.RegisterUser:
        //            Toast.MakeText(this, "You selected the Register user option item", ToastLength.Long).Show();
        //            return true;
        //        default:
        //            return base.OnOptionsItemSelected(item);
        //    }
        //}
        //#endregion

        private const string Empty = "";
        private void StartGame_Click(object sender, EventArgs e)
        {
            const string Player1 = "Player 1";
            string player1 = Player1NameInput.Text switch
            {
                null => Player1,
                Empty => Player1,
                _ => Player1NameInput.Text,
            };
            const string Player2 = "Player 2";
            string player2 = Player2NameInput.Text switch
            {
                null => Player2,
                Empty => Player2,
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