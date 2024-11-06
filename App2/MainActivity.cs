using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Card;
using Google.Android.Material.Dialog;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;

namespace TicTacToe
{

    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextInputEditText Player1NameInput;
        private TextInputEditText Player2NameInput;
        private MaterialCardView Player1X;
        private MaterialCardView Player2O;
        private Button StartGame;
        private Button HelpButton;

        private bool MaterialYouThemePreference = true;
        bool prevState;

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
            //Resource.Color.m3_sys_color_light_on_surface
            //Resource.Color.m3_sys_color_dark_on_surface
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

            Player1NameInput = base.FindViewById<TextInputEditText>(Resource.Id.Player1NameInput);
            Player2NameInput = base.FindViewById<TextInputEditText>(Resource.Id.Player2NameInput);
            Player1X = base.FindViewById<MaterialCardView>(Resource.Id.mcvPlayer1Card);
            Player2O = base.FindViewById<MaterialCardView>(Resource.Id.mcvPlayer2Card);
            StartGame = base.FindViewById<Button>(Resource.Id.StartGame);
            StartGame.Click += StartGame_Click;

            HelpButton = base.FindViewById<Button>(Resource.Id.HelpButton);
            base.RegisterForContextMenu(HelpButton);

            prevState = MaterialYouThemePreference;
            InitDialog();
            HelpButton.Click += Boo;
        }
        void InitDialog()
        {
            var mTheme = Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons;
            MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this, mTheme);
            builder.SetTitle("Title");
            builder.SetMessage("Message");
            builder.SetPositiveButton("Confirm", (a, b) =>
            {
                if (prevState != MaterialYouThemePreference)
                    Foo();
            });
            builder.SetNegativeButton("Cancel", (a, b) =>
            {
                MaterialYouThemePreference = prevState;
            });
            //builder.SetNeutralButton("Dismiss", (a, b) => { });
            builder.SetIcon(Resources.GetDrawable(Resource.Drawable.outline_share_24, builder.Context.Theme));
            builder.SetView(Resource.Menu.dialog_v1);
            dialog = builder.Create();
        }
        void Boo(object sender, EventArgs e)
        {
            dialog.Show();
            var msSwitch = dialog.FindViewById<MaterialSwitch>(Resource.Id.MaterialYouSwitch);
            msSwitch.Checked = MaterialYouThemePreference;
            int id = msSwitch.Checked switch

            {
                true => Resource.Drawable.outline_circle_24,
                false => Resource.Drawable.outline_close_24,
            };
            msSwitch.SetThumbIconResource(id);
            msSwitch.CheckedChange += (a, b) =>
            {
                MaterialYouThemePreference = msSwitch.Checked;
                int id = msSwitch.Checked switch

                {
                    true => Resource.Drawable.outline_circle_24,
                    false => Resource.Drawable.outline_close_24,
                };
                msSwitch.SetThumbIconResource(id);
                //Foo();
                //InitDialog();
            };
        }
        AndroidX.AppCompat.App.AlertDialog dialog;
        void Foo()
        {
            var a = dialog.FindViewById<MaterialSwitch>(Resource.Id.MaterialYouSwitch);
            ISharedPreferences sharedPref = base.GetPreferences(FileCreationMode.Private);
            var editor = sharedPref.Edit();
            MaterialYouThemePreference = a.Checked;
            prevState = MaterialYouThemePreference;
            editor.PutBoolean(nameof(MaterialYouThemePreference), MaterialYouThemePreference);
            editor.Commit();
            editor.Apply();
            int theme = MaterialYouThemePreference switch
            {

                true => Resource.Style.Theme_Material3_DynamicColors_DayNight_NoActionBar,
                false => Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt,
            };
            base.SetTheme(theme);
            Thread.Sleep(500);
            base.Recreate();
        }

        #region Context Menu
        public override void OnCreateContextMenu(IContextMenu menu, View view, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, view, menuInfo);

            base.MenuInflater.Inflate(Resource.Menu.menu1, menu);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.LoginUser:
                    Toast.MakeText(this, "You selected the Login user context item", ToastLength.Long).Show();
                    return true;
                case Resource.Id.RegisterUser:
                    Toast.MakeText(this, "You selected the Register user context item", ToastLength.Long).Show();
                    return true;
                default:
                    return base.OnContextItemSelected(item);
            }
        }
        #endregion

        #region OptionsMenu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.MenuInflater.Inflate(Resource.Menu.menu1, menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, $"you selected options item {item.ItemId}", ToastLength.Long);
            switch (item.ItemId)
            {
                case Resource.Id.LoginUser:
                    Toast.MakeText(this, "You selected the Login user option item", ToastLength.Long).Show();
                    return true;
                case Resource.Id.RegisterUser:
                    Toast.MakeText(this, "You selected the Register user option item", ToastLength.Long).Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        #endregion

        private void StartGame_Click(object sender, EventArgs e)
        {
            const string Empty = "";
            const string Player1 = "Player 1";
            const string Player2 = "Player 2";

            string player1 = Player1NameInput.Text switch
            {
                null => Player1,
                Empty => Player1,
                _ => Player1NameInput.Text,
            };

            string player2 = Player2NameInput.Text switch
            {
                null => Player2,
                Empty => Player2,
                _ => Player2NameInput.Text,
            };

            Intent intent = new Intent(this, typeof(ChessActivity))
                .PutExtra(nameof(ChessActivity.MaterialYouThemePreference), MaterialYouThemePreference.ToString())
                .PutExtra(nameof(ChessActivity.Player1Name), player1)
                .PutExtra(nameof(ChessActivity.Player2Name), player2);
            base.StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}