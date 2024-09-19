using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Color;

//< !--com.google.android.material.card.MaterialCardView-- >

namespace App2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView Player1Score;
        private TextView Player2Score;
        private TextView DrawCounter;
        private TextView RoundCounter;
        private int Rounds = 1;
        private int Draws = 0;
        private int Player1 = 0;
        private int Player2 = 0;
        string Player1Name = "Player 1";
        string Player2Name = "Player 2";

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
            base.SetContentView(Resource.Layout.dashboard);

            //Apply Material You
            if (!isRecreated)
            {
                isRecreated = true;
                DynamicColors.ApplyToActivitiesIfAvailable(base.Application);
                base.Recreate();
            }
            return;
            Player1Score = base.FindViewById<TextView>(Resource.Id.Player1Score);
            Player2Score = base.FindViewById<TextView>(Resource.Id.Player2Score);
            RoundCounter = base.FindViewById<TextView>(Resource.Id.RoundCounter);
            DrawCounter = base.FindViewById<TextView>(Resource.Id.DrawCounter);
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
            Toast.MakeText(base.ApplicationContext, "Player 1 its your turn", ToastLength.Short);
            isGameOver = false;
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

        public void TryPlay(object? sender, EventArgs e, int i)
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
                int player = 0;
                switch (Player)
                {
                    case PointType.X:
                        player = 1;
                        Player1++;
                        Player1Score.Text = $"Wins: {Player1}";
                        break;
                    case PointType.O:
                        player = 2;
                        Player2++;
                        Player2Score.Text = $"Wins: {Player2}";
                        break;
                }

                string playerWon = $"Player {player} won the game";
                Toast.MakeText(base.ApplicationContext, playerWon, ToastLength.Long).Show();
                return;
            }

            if (IsDraw())
            {
                isGameOver = true;
                Draws++;
                DrawCounter.Text = $"Draws: {Draws}";
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