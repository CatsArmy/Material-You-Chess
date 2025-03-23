using Android.Content;
using Android.Views.Animations;
using AndroidX.AppCompat.Content.Res;
using Google.Android.Material.FloatingActionButton;

namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
    public static void Spin(this ExtendedFloatingActionButton fab)
    {
        if (fab.Extended)
            return;

        fab.Rotation = 0;

        fab.Animate()?.Rotation(360).WithLayer().SetDuration(1000).SetInterpolator(new AccelerateDecelerateInterpolator()).Start();
    }

    public static void Spin(this FloatingActionButton fab)
    {
        fab.Rotation = 0;

        fab.Animate()?.Rotation(360).WithLayer().SetDuration(1000).SetInterpolator(new AccelerateDecelerateInterpolator()).Start();
    }

    public static void OnError(this ExtendedFloatingActionButton fab, ContextWrapper context)
    {
        if (fab.Extended)
            return;

        // Default color format is AARRGGBB / ARGB
        var color = fab.BackgroundTintList!;
        fab.BackgroundTintList = AppCompatResources.GetColorStateList(context, Resource.Attribute.colorErrorContainer);
        fab.Rotation = 0;
        fab.Animate()?.Rotation(120).WithLayer().SetDuration(500).SetInterpolator(new AccelerateDecelerateInterpolator())
            .WithEndAction(new Java.Lang.Runnable(() =>
            {
                fab.Animate()?.Rotation(-120).WithLayer().SetDuration(500).SetInterpolator(new AccelerateDecelerateInterpolator())
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    fab.BackgroundTintList = color;
                }))
                .Start();
            }))
            .Start();
    }

    public static void OnError(this FloatingActionButton fab, ContextWrapper context)
    {
        // Default color format is AARRGGBB / ARGB
        var color = fab.BackgroundTintList!;
        fab.BackgroundTintList = AppCompatResources.GetColorStateList(context, Resource.Attribute.colorErrorContainer);
        fab.Rotation = 0;
        fab.Animate()?.Rotation(120).WithLayer().SetDuration(500).SetInterpolator(new AccelerateDecelerateInterpolator())
            .WithEndAction(new Java.Lang.Runnable(() =>
            {
                fab.Animate()?.Rotation(-120).WithLayer().SetDuration(500).SetInterpolator(new AccelerateDecelerateInterpolator())
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    fab.BackgroundTintList = color;
                }))
                .Start();
            }))
            .Start();
    }
}
