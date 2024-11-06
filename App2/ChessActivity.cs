using System;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Card;

namespace TicTacToe
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar")]
    public class ChessActivity : AppCompatActivity
    {
        internal TextView Player1Name;
        internal TextView Player2Name;
        private TextView Player1Score;
        private TextView Player2Score;
        private TextView DrawCounter;
        private TextView RoundCounter;
        private MaterialCardView Player1X;
        private MaterialCardView Player2O;

        private Color ActivePlayerCard;
        private Color InactivePlayerCard;

        private int Rounds = 1;
        private int Draws = 0;
        private int Player1 = 0;
        private int Player2 = 0;

        private readonly Button[] Numpad = new Button[9];
        private readonly PointType[] Board = new PointType[9];

        private PointType Player = PointType.X;
        private bool isGameOver = false;

        public bool MaterialYouThemePreference;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            MaterialYouThemePreference = bool.Parse(base.Intent.GetStringExtra(nameof(MaterialYouThemePreference)));
            if (!MaterialYouThemePreference)
            {
                base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);
            }
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            base.SetContentView(Resource.Layout.main);

            //RoundCounter = base.FindViewById<TextView>(Resource.Id.RoundCounter);
            //DrawCounter = base.FindViewById<TextView>(Resource.Id.DrawCounter);
            //Player1Score = base.FindViewById<TextView>(Resource.Id.Player1Score);
            //Player2Score = base.FindViewById<TextView>(Resource.Id.Player2Score);

            //Player1Name = base.FindViewById<TextView>(Resource.Id.Player1Name);
            //Player1Name.Text = base.Intent.GetStringExtra(nameof(Player1Name));
            //Player2Name = base.FindViewById<TextView>(Resource.Id.Player2Name);
            //Player2Name.Text = base.Intent.GetStringExtra(nameof(Player2Name));

            //Player1X = base.FindViewById<MaterialCardView>(Resource.Id.mcvPlayer1X);
            //Player2O = base.FindViewById<MaterialCardView>(Resource.Id.mcvPlayer2O);

            //ActivePlayerCard = new Color(base.GetColor(Resource.Color.m3_sys_color_dynamic_light_on_surface));
            //InactivePlayerCard = new Color(base.GetColor(Resource.Color.m3_sys_color_dynamic_dark_on_surface));
            //for (int i = 0; i < Numpad.Length; i++)
            //{
            //    //Resource.Id.button8 == Resource.Id.button9 - 1 ... Resource.Id.button1 == Resource.Id.button2 - 1
            //    Numpad[i] = base.FindViewById<Button>(Resource.Id.button1 + i);
            //    int index = int.Parse($"{i}");
            //    Numpad[i].Click += (sender, e) => TryPlay(sender, e, index);
            //}
            //ResetBoard();
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private void UpdateCounters()
        {
            Player1Score.Text = $"Wins: {Player1}";
            DrawCounter.Text = $"Draws: {Draws}";
            Player2Score.Text = $"Wins: {Player2}";
            NumberPostfix postfix = (Rounds % 10) switch
            {
                1 => NumberPostfix.st,
                2 => NumberPostfix.nd,
                3 => NumberPostfix.rd,
                _ => NumberPostfix.th,
            };

            ISpanned a = Html.FromHtml($"{Rounds}<sup>{postfix}</sup>");
            RoundCounter.SetText(a, TextView.BufferType.Spannable);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        private void ResetBoard()
        {
            isGameOver = false;
            TypedValue typedValue = new TypedValue();
            Theme.ResolveAttribute(Resource.Attribute.colorPrimary, typedValue, true);
            for (int i = 0; i < Numpad.Length; i++)
            {
                Board[i] = PointType.Empty;
                Numpad[i].Text = string.Empty;
                Numpad[i].SoundEffectsEnabled = true;
                Numpad[i].SetBackgroundColor(new Color(base.GetColor(typedValue.ResourceId)));
            }
            Player1X.CardBackgroundColor = ColorStateList.ValueOf(ActivePlayerCard);
            Player2O.CardBackgroundColor = ColorStateList.ValueOf(InactivePlayerCard);
            Player = PointType.X;
            Toast.MakeText(this, $"{Player1Name.Text} its your turn", ToastLength.Short).Show();
        }

        public void TryPlay(object sender, EventArgs e, int i)
        {
            if (isGameOver)
            {
                UpdateCounters();
                ResetBoard();
                return;
            }

            if (Board[i] != PointType.Empty)
                return;

            Board[i] = Player;
            Numpad[i].Text = Player.GetValue();
            Numpad[i].SoundEffectsEnabled = false;


            if (Diagonal() || Horizontal() || Vertical())
            {
                isGameOver = true;
                Rounds++;
                switch (Player)
                {
                    case PointType.X:
                        Toast.MakeText(this, $"{Player1Name.Text} won the game", ToastLength.Long).Show();
                        Player1++;
                        break;
                    case PointType.O:
                        Toast.MakeText(this, $"{Player2Name.Text} won the game", ToastLength.Long).Show();
                        Player2++;
                        break;
                }
                CrossOut(Resources.GetColor(Resource.Color.Crossout, Theme));
                return;
            }

            if (IsDraw())
            {
                isGameOver = true;
                Rounds++;
                Toast.MakeText(this, "No one won the game (draw)", ToastLength.Long).Show();
                Draws++;
                return;
            }


            switch (Player)
            {
                case PointType.X:
                    Player2O.CardBackgroundColor = ColorStateList.ValueOf(ActivePlayerCard);
                    Player1X.CardBackgroundColor = ColorStateList.ValueOf(InactivePlayerCard);
                    Player = PointType.O;
                    break;

                case PointType.O:
                    Player1X.CardBackgroundColor = ColorStateList.ValueOf(ActivePlayerCard);
                    Player2O.CardBackgroundColor = ColorStateList.ValueOf(InactivePlayerCard);
                    Player = PointType.X;
                    break;
            }



        }

        public bool IsDraw()
        {
            for (int i = 0; i < Board.Length; i++)
                if (Board[i] == PointType.Empty)
                    return false;
            return true;
        }

        public void CrossOut(Color bg)
        {
            var none = (0, 0, 0);
            var (a1, b1, c1) = Diagonal(false);
            var d1 = (a1, b1, c1);
            var (a2, b2, c2) = Diagonal(true);
            var d2 = (a2, b2, c2);
            if (d1 != none)
            {
                Numpad[a1].SetBackgroundColor(bg);
                Numpad[b1].SetBackgroundColor(bg);
                Numpad[c1].SetBackgroundColor(bg);
            }
            if (d2 != none)
            {
                Numpad[a2].SetBackgroundColor(bg);
                Numpad[b2].SetBackgroundColor(bg);
                Numpad[c2].SetBackgroundColor(bg);
            }
            for (int index = 0; index < 3; index++)
            {
                var (ih, jh, kh) = Horizontal(index);
                var h = (ih, jh, kh);
                if (h != none)
                {
                    Numpad[ih].Background.SetTint(bg);
                    Numpad[jh].Background.SetTint(bg);
                    Numpad[kh].Background.SetTint(bg);
                }
                var (iv, jv, kv) = Vertical(index);
                var v = (iv, jv, kv);
                if (v != none)
                {
                    Numpad[iv].Background.SetTint(bg);
                    Numpad[jv].Background.SetTint(bg);
                    Numpad[kv].Background.SetTint(bg);
                }
            }
        }

        /// <param name="direction">
        /// the <paramref name="direction"/> of the diagonal: <see langword="false"/> <see langword="is"/> left, <see langword="true"/> <see langword="is"/> right
        /// </param>
        public (int, int, int) Diagonal(bool direction)
        {
            var (i, j, k) = (0, 4, 8);
            var playerPointType = (Player, Player, Player);
            if (direction)
            {
                (i, k) = (2, 6);
            }
            var diagonal = (Board[i], Board[j], Board[k]);
            if (playerPointType == diagonal)
                return (i, j, k);

            return (0, 0, 0);
        }

        public (int, int, int) Horizontal(int row = 0)
        {
            var playerPointType = (Player, Player, Player);
            int step = 3 * row;
            var (i, j, k) = (0 + step, 1 + step, 2 + step);

            var horizontal = (Board[i], Board[j], Board[k]);
            if (horizontal == playerPointType)
                return (i, j, k);

            return (0, 0, 0);
        }

        public (int, int, int) Vertical(int column = 0)
        {
            var playerPointType = (Player, Player, Player);
            int step = 1 * column;
            var (i, j, k) = (0 + step, 3 + step, 6 + step);
            var horizontal = (Board[i], Board[j], Board[k]);
            if (horizontal == playerPointType)
                return (i, j, k);
            return (0, 0, 0);
        }

        public bool Diagonal()
        {
            var playerPointType = (Player, Player, Player);
            for (var (i, j, k) = (0, 4, 8); i < j; i += 2, k -= 2)
            {
                var diagonal = (Board[i], Board[j], Board[k]);
                if (diagonal == playerPointType)
                    return true;
            }
            return false;
        }

        public bool Horizontal()
        {
            var playerPointType = (Player, Player, Player);
            for (var (i, j, k) = (0, 1, 2); k < Numpad.Length; (i, j, k) = (i + 3, j + 3, k + 3))
            {
                var horizontal = (Board[i], Board[j], Board[k]);
                if (horizontal == playerPointType)
                    return true;
            }
            return false;
        }

        public bool Vertical()
        {
            var playerPointType = (Player, Player, Player);
            for (var (i, j, k) = (0, 3, 6); k < Numpad.Length; i++, j++, k++)
            {
                var vertical = (Board[i], Board[j], Board[k]);
                if (vertical == playerPointType)
                    return true;
            }
            return false;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}