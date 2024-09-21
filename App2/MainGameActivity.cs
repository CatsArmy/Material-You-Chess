using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace TicTacToe
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar")]
    public class MainGameActivity : AppCompatActivity
    {
        internal TextView Player1Name;
        internal TextView Player2Name;
        private TextView Player1Score;
        private TextView Player2Score;
        private TextView DrawCounter;
        private TextView RoundCounter;
        private int Rounds = 1;
        private int Draws = 0;
        private int Player1 = 0;
        private int Player2 = 0;

        private readonly Button[] Numpad = new Button[9];
        private readonly PointType[] Board = new PointType[9];

        private PointType Player = PointType.X;
        private bool isGameOver = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            base.SetContentView(Resource.Layout.activity_main_game);

            RoundCounter = base.FindViewById<TextView>(Resource.Id.RoundCounter);
            DrawCounter = base.FindViewById<TextView>(Resource.Id.DrawCounter);
            Player1Score = base.FindViewById<TextView>(Resource.Id.Player1Score);
            Player2Score = base.FindViewById<TextView>(Resource.Id.Player2Score);

            Player1Name = base.FindViewById<TextView>(Resource.Id.Player1Name);
            Player1Name.Text = base.Intent.GetStringExtra(nameof(Player1Name));
            Player2Name = base.FindViewById<TextView>(Resource.Id.Player2Name);
            Player2Name.Text = base.Intent.GetStringExtra(nameof(Player2Name));

            for (int i = 0; i < Numpad.Length; i++)
            {
                //Resource.Id.btn8 == Resource.Id.btn9 - 1 ... Resource.Id.btn1 == Resource.Id.btn2 - 1
                Numpad[i] = base.FindViewById<Button>(Resource.Id.btn1 + i);
                int index = int.Parse($"{i}");
                Numpad[i].Click += (sender, e) => TryPlay(sender, e, index);
            }
            ResetBoard();
        }

        private void ResetBoard()
        {
            for (int i = 0; i < Numpad.Length; i++)
            {
                Board[i] = PointType.Empty;
                Numpad[i].Text = string.Empty;
            }
            isGameOver = false;
            Player = PointType.X;
            Toast.MakeText(base.ApplicationContext, $"{Player1Name.Text} its your turn", ToastLength.Short).Show();
        }



        private void IncrementRoundCounter()
        {
            Rounds++;
            NumberPostfix postfix = (Rounds % 10) switch
            {
                1 => NumberPostfix.st,
                2 => NumberPostfix.nd,
                3 => NumberPostfix.rd,
                _ => NumberPostfix.th,
            };
            RoundCounter.Text = $"{Rounds}{postfix}";
        }

        public void TryPlay(object sender, EventArgs e, int i)
        {
            if (isGameOver)
            {
                IncrementRoundCounter();
                ResetBoard();
                return;
            }

            if (Board[i] != PointType.Empty)
            {
                return;
            }

            Board[i] = Player;
            Numpad[i].Text = Player.GetValue();
            if (Diagonal() || Horizontal() || Vertical())
            {
                isGameOver = true;
                switch (Player)
                {
                    case PointType.X:
                        Toast.MakeText(base.ApplicationContext, $"{Player1Name.Text} won the game", ToastLength.Long).Show();
                        Player1++;
                        Player1Score.Text = $"Wins: {Player1}";
                        break;
                    case PointType.O:
                        Toast.MakeText(base.ApplicationContext, $"{Player2Name.Text} won the game", ToastLength.Long).Show();
                        Player2++;
                        Player2Score.Text = $"Wins: {Player2}";
                        break;
                }
                return;
            }

            if (IsDraw())
            {
                isGameOver = true;
                Toast.MakeText(base.ApplicationContext, "No one won the game (draw)", ToastLength.Long).Show();
                Draws++;
                DrawCounter.Text = $"Draws: {Draws}";
                return;
            }

            switch (Player)
            {
                case PointType.X:
                    Player = PointType.O;
                    break;
                case PointType.O:
                    Player = PointType.X;
                    break;
                default:
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