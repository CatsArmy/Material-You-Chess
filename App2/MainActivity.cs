using System;
using System.Collections.Generic;
using System.Linq;
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
        // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private TextView InputResult;
        private TextView OutputResult;

        private readonly Button[] Numpad = new Button[10];

        private Button FlipPolarity;
        private Button AllClear;

        private Button Modulus;
        private Button Dot;

        private Button Divide;
        private Button Multiply;
        private Button Subtraction;
        private Button Addition;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private Button Equals;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        private readonly List<Operation> operations = new List<Operation>();
        private readonly List<double> numbers = new List<double>();
        private bool hasDot = false;
        private bool dot = false;
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
                Recreate();
            }
            InputResult = base.FindViewById<TextView>(Resource.Id.tvInputResult);
            OutputResult = base.FindViewById<TextView>(Resource.Id.tvOutputResult);
            numbers.Clear();
            operations.Clear();
            numbers.Add(operations.Count);
            InputResult.Text = Evaluate();
            OutputResult.Text = string.Empty;

            for (int i = 0; i < Numpad.Length; i++)
            {
                //Resource.Id.btn0 is a int and btn1 is the same int + 1 and so on
                Numpad[i] = base.FindViewById<Button>(Resource.Id.btn0 + i);
                int number = int.Parse($"{i}");
                Numpad[i].Click += (sender, e) => OnClick_Numpad(sender, e, number);
            }

            FlipPolarity = base.FindViewById<Button>(Resource.Id.btnFlipPolarity);
            FlipPolarity.Click += OnClick_FlipPolarity;
            AllClear = base.FindViewById<Button>(Resource.Id.btnAllClear);
            AllClear.Click += OnClick_AllClear!;
            Modulus = base.FindViewById<Button>(Resource.Id.btnModulus);
            Modulus.Click += (sender, e) => OnClick_Operation(sender, e, Operation.Modulus);
            Divide = base.FindViewById<Button>(Resource.Id.btnDivide);
            Divide.Click += (sender, e) => OnClick_Operation(sender, e, Operation.Division);
            Dot = base.FindViewById<Button>(Resource.Id.btnDot)!;
            Dot.Click += OnClick_Dot;
            Multiply = base.FindViewById<Button>(Resource.Id.btnMultiply);
            Multiply.Click += (sender, e) => OnClick_Operation(sender, e, Operation.Multiplication);
            Subtraction = base.FindViewById<Button>(Resource.Id.btnSubtraction);
            Subtraction.Click += (sender, e) => OnClick_Operation(sender, e, Operation.Subtraction);
            Addition = base.FindViewById<Button>(Resource.Id.btnAddition);
            Addition.Click += (sender, e) => OnClick_Operation(sender, e, Operation.Addition);
            Equals = base.FindViewById<Button>(Resource.Id.btnEquals);
            Equals.Click += OnClick_Equals;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void OnClick_FlipPolarity(object? sender, EventArgs e)
        {
            if (numbers.Count == operations.Count)
                return;
            if (numbers[operations.Count] == 0)
                return;

            const int flipPolarity = -1;
            numbers[operations.Count] *= flipPolarity;
            InputResult.Text = Evaluate();
            OutputResult.Text = string.Empty;
        }

        private void OnClick_Dot(object? sender, EventArgs e)
        {
            if (hasDot)
            {
                return;
            }
            dot = true;
            hasDot = true;
            InputResult.Text = Evaluate();
            OutputResult.Text = string.Empty;
        }

        private void OnClick_Operation(object? sender, EventArgs e, Operation operation)
        {
            if (numbers.Count == operations.Count)
            {
                operations[operations.Count - 1] = operation;
                InputResult.Text = Evaluate();
                return;
            }
            operations.Add(operation);
            dot = false;
            hasDot = false;
            InputResult.Text = Evaluate();
            OutputResult.Text = string.Empty;
        }

        private void OnClick_Equals(object? sender, EventArgs e)
        {
            InputResult.Text = Evaluate();
            OutputResult.Text = Calculate();
        }

        public string Evaluate()
        {
            if (numbers.Count <= 0)
                return string.Empty;

            string result = string.Empty;
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                result += $"{numbers[i]}";
                if (i >= operations.Count)
                {
                    continue;
                }
                result += $" {operations[i].GetValue()} ";
            }
            result += $"{numbers[numbers.Count - 1]}";
            if (numbers.Count == operations.Count)
            {
                result += $"{operations[operations.Count - 1].GetValue()}";
            }
            return result;
        }

        public string Calculate(List<double> numbers, List<Operation> operations)
        {
            if (numbers.Count - 1 != operations.Count)
                return string.Empty;
            for (int i = 0; i < operations.Count; i++)
            {
                switch (operations[i])
                {
                    case Operation.Multiplication:
                        numbers[i] *= numbers[i + 1];
                        numbers.RemoveAt(i + 1);
                        operations.RemoveAt(i);
                        i--;
                        break;
                    case Operation.Division:
                        if (numbers[i + 1] == 0)
                            return "Cannot divide by zero.";
                        numbers[i] /= numbers[i + 1];
                        numbers.RemoveAt(i + 1);
                        operations.RemoveAt(i);
                        i--;
                        break;
                    case Operation.Modulus:
                        if (numbers[i + 1] == 0)
                            return "Cannot perform modulus with zero.";
                        numbers[i] %= numbers[i + 1];
                        numbers.RemoveAt(i + 1);
                        operations.RemoveAt(i);
                        i--;
                        break;
                    default:
                        break;
                }
            }

            double result = numbers[0];

            for (int i = 0, j = 1; i < operations.Count; i++)
            {
                switch (operations[i])
                {
                    case Operation.Addition:
                        result += numbers[j];
                        j++;
                        break;
                    case Operation.Subtraction:
                        result -= numbers[j];
                        j++;
                        break;
                    default:
                        break;
                }
            }
            return $"{result}";
        }

        public string Calculate()
        {
            List<double> numbers = this.numbers.ToArray().ToList();
            List<Operation> operations = this.operations.ToArray().ToList();
            return Calculate(numbers, operations);
        }


        private void OnClick_Numpad(object? sender, EventArgs e, int number)
        {
            double value;
            string input = string.Empty;
            switch (numbers.Count == operations.Count)
            {
                case true:
                    if (dot)
                        input += '.';
                    input += $"{number}";
                    value = double.Parse(input);
                    numbers.Add(value);
                    break;

                case false:
                    input += $"{numbers[operations.Count]}";
                    if (dot)
                        input += '.';
                    input += $"{number}";
                    value = double.Parse(input);
                    numbers[operations.Count] = value;
                    break;
            }
            InputResult.Text = Evaluate();
            OutputResult.Text = string.Empty;
        }

        private void OnClick_AllClear(object? sender, EventArgs e)
        {
            if (sender != AllClear)
            {
                return;

            }

            dot = false;
            hasDot = false;
            numbers.Clear();
            operations.Clear();
            numbers.Add(operations.Count);
            InputResult.Text = $"{operations.Count}";
            OutputResult.Text = string.Empty;
        }
    }
}