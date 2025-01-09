using Android.Content.PM;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Bumptech.Glide;
using Chess.Game;
using Chess.Util;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class ChessActivity : AppCompatActivity
{
    public bool MaterialYouThemePreference;
    private ShapeableImageView? p1MainProfileImageView;
    private ShapeableImageView? p2MainProfileImageView;
    private TextView? p1MainUsername;
    private TextView? p2MainUsername;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra(nameof(this.MaterialYouThemePreference)), out this.MaterialYouThemePreference);
        if (hasValue && !this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        // Permission request logic
        _ = new PermissionsRequester(this);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        //Run our logic
        this.p1MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is Android.Net.Uri)
            Glide.With(this).Load(FirebaseStorage.Instance.Reference.Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png")).Error(Resource.Drawable.outline_account_circle_24)
                .Into(this.p1MainProfileImageView!);

        this.p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.p2MainUsername = base.FindViewById<TextView>(Resource.Id.p2MainUsername);

        this.p1MainUsername!.Text = (FirebaseAuth.Instance?.CurrentUser == null) switch
        {
            true => "Guest",
            false => FirebaseAuth.Instance?.CurrentUser?.DisplayName,
        };

        //TODO implement En Passant difficulty Medium--
        //TODO Add Castling difficulty Medium
        //TODO Add Promotion difficulty Easy+ / Medium-
        //TODO More?

        //FIX why are the captured pieces not captured??
        ChessGame game = new(base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard));
    }
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
