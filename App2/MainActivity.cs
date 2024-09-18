using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Color;

namespace App2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView Player1Score;
        private TextView Player2Score;

        private readonly Button[] Numpad = new Button[9];
        private readonly PointType[] Board = new PointType[9];

        private PointType Player = PointType.X;
        private bool isGameOver = false;
        private static bool isRecreated = false;
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
            Player1Score = base.FindViewById<TextView>(Resource.Id.Player1Score);
            Player2Score = base.FindViewById<TextView>(Resource.Id.Player2Score);
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
            Player = PointType.X;
            Toast.MakeText(base.ApplicationContext, "Player(X) 1 its your turn", ToastLength.Short);
            isGameOver = false;
        }

        public void TryPlay(object? sender, EventArgs e, int i)
        {
            if (isGameOver)
            {
                ResetBoard();
                return;
            }

            if (Board[i] != PointType.Empty)
            {
                return;
            }

            Board[i] = Player;
            Numpad[i].Text = $"{Player}";
            if (Diagonal() || Horizontal() || Vertical())
            {
                isGameOver = true;
                int player = 0;
                switch (Player)
                {
                    case PointType.X:
                        player = 1;
                        Player1Score.Text = $"{int.Parse(Player1Score.Text) + 1}";
                        break;
                    case PointType.O:
                        Player2Score.Text = $"{int.Parse(Player2Score.Text) + 1}";
                        player = 2;
                        break;
                }

                string playerWon = $"Player {player} won the game";
                Toast.MakeText(base.ApplicationContext, playerWon, ToastLength.Long).Show();
                return;
            }

            if (IsDraw())
            {
                isGameOver = true;
                Player = PointType.X;
                Toast.MakeText(base.ApplicationContext, "The game is a draw", ToastLength.Long).Show();
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
            bool isDraw = true;
            for (int i = 0; i < Board.Length; i++)
            {
                if (Board[i] == PointType.Empty)
                {
                    isDraw = false;
                }
            }
            return isDraw;
        }

        public bool Diagonal()
        {
            var (i, j, k) = (0, 4, 8);
            var (i2, k2) = (2, 6);
            var diagonalLeft = (Numpad[i].GetValue(), Numpad[j].GetValue(), Numpad[k].GetValue());
            var diagonalRight = (Numpad[i2].GetValue(), Numpad[j].GetValue(), Numpad[k2].GetValue());
            var playerPointType = (Player, Player, Player);
            if (diagonalLeft == playerPointType || diagonalRight == playerPointType)
                return true;
            return false;
        }

        public bool Horizontal()
        {

            var playerPointType = (Player, Player, Player);
            for (var (i, j, k) = (0, 1, 2); k < Numpad.Length; (i, j, k) = (i + 3, j + 3, k + 3))
            {
                var horizontal = (Numpad[i].GetValue(), Numpad[j].GetValue(), Numpad[k].GetValue());

                if (horizontal == playerPointType)
                    return true;
            }

            return false;
        }

        public bool Vertical()
        {
            var playerPointType = (Player, Player, Player);
            for (var (i, j, k) = (0, 3, 6); k < Numpad.Length; i++)
            {
                var vertical = (Numpad[i].GetValue(), Numpad[j].GetValue(), Numpad[k].GetValue());
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