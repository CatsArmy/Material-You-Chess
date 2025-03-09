//using Android.Content;
//using Android.Views;
//using Google.Android.Material.Dialog;
//using Google.Android.Material.MaterialSwitch;
//using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

//namespace Chess.App.Networked;

//public class NetworkDialog
//{
//    public AlertDialog Dialog { get; set; }
//    public MaterialAlertDialogBuilder Builder { get; set; }
//    public NetworkedChessActivity Activity { get; set; }

//    public TextView? SearchingText { get; set; }
//    public View? SearchingIndicator { get; set; }
//    public MaterialSwitch? IsHost { get; set; }

//    public NetworkDialog(NetworkedChessActivity activity)
//    {
//        this.Activity = activity;
//        this.Builder = new MaterialAlertDialogBuilder(activity);
//        this.Builder.SetView(Resource.Layout.select_device_dialog);
//        this.Dialog = this.Builder.Create();
//        this.Dialog.ShowEvent += this.OnShow;
//    }

//    private void IsHost_CheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e)
//    {
//        this.SearchingText!.Text = e.IsChecked switch
//        {
//            true => "Searching for players",
//            false => "Waiting for players"
//        };
//    }

//    public void Show(object? sender, EventArgs args) => this.Dialog?.Show();
//    public void Show() => this.Dialog?.Show();


//    public void OnShow(object? sender, EventArgs args)
//    {
//        this.SearchingText = this.Dialog?.FindViewById<TextView>(Resource.Id.bsSearching);
//        //this.SearchingIndicator = this.Dialog?.FindViewById(Resource.Id.mliSearching);
//        this.IsHost = this.Dialog?.FindViewById<MaterialSwitch>(Resource.Id.msIsHost);
//        this.IsHost!.CheckedChange += this.IsHost_CheckedChange;
//        this.IsHost!.CheckedChange += this.Activity.IsHost_CheckedChange;
//    }

//    public void OnConfirm(object? sender, DialogClickEventArgs args)
//    {
//        //this.OnConfirmation(this.UsernameInput!.Text!);
//    }

//    public void OnCancel(object? sender, DialogClickEventArgs args) { }
//}



