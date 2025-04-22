Make sure you have the .NET 10 SDK Installed: SDK 10.0.100-preview.3
https://dotnet.microsoft.com/en-us/download/dotnet/10.0

If you get a signing apk error when building check if you have the following files:
Properties/Material-You-Chess.Build.props.user

if you don't then you should recreate it using the Properties/Material-You-Chess.Build.props.template
copy Properties/Material-You-Chess.Build.props.template
remove the copied file's .template from the full name of it
open the newly renamed file
make sure that the following property is filled with the password of the "catsarmy.keystore" file
<AndroidSigningPassword>{::{
THE PRIVATE PASSWORD USED TO SIGN OUR APK
THIS IS A CONFIDENTIAL FIELD THAT SHOULD NOT BE SHARED
}::}</AndroidSigningPassword>
if you dont know the password of catsarmy.keystore
then you should recreate it using this: 
"https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-9.0#create-a-keystore-file" tutorial

if you get errors related to firebase check if your missing your "Properties/google-services.json" file you get in your firebase console
if you aren't missing the file open the "Properties/Material-You-Chess.Build.props.user" file.
check if the following property is set to the correct path by default it should be:
<GoogleServicesJsonFile>Properties\google-services.json</GoogleServicesJsonFile>
