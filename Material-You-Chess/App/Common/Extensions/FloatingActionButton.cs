using Android.Content;
using Android.Views.Animations;
using AndroidX.AppCompat.Content.Res;
using Google.Android.Material.FloatingActionButton;

namespace Chess.App.Common.Extensions;

/// <summary> a static class containing extension a method </summary>
public static class FloatingActionButtonAnimations
{
    public static void Spin(this ExtendedFloatingActionButton? fab)
    {
        if (fab is null || fab.Extended) return;

        fab.Rotation = 0;

        fab.Animate()?.Rotation(360).WithLayer().SetDuration(1000).SetInterpolator(new AccelerateDecelerateInterpolator()).Start();
    }

    public static void Spin(this FloatingActionButton? fab)
    {
        if (fab is null) return;
        fab.Rotation = 0;

        fab.Animate()?.Rotation(360).WithLayer().SetDuration(1000).SetInterpolator(new AccelerateDecelerateInterpolator()).Start();
    }

    public static void OnError(this ExtendedFloatingActionButton? fab, ContextWrapper context)
    {
        if (fab is null || fab.Extended) return;

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

    public static void OnError(this FloatingActionButton? fab, ContextWrapper context)
    {
        if (fab is null) return;
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
