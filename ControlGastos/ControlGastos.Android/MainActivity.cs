using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.OneSignal;
using Plugin.LocalNotifications;
using Android.Gms.Ads;

namespace ControlGastos.Droid
{
    [Activity(Label = "Gestor de Gastos", Icon = "@drawable/logotipoGG", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            OneSignal.Current.StartInit("d18e950f-8242-437d-beb8-28fc657cf0a4").EndInit();
            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.logotipoGGnotification;
            MobileAds.Initialize(ApplicationContext, "ca-app-pub-4740837376040145~7975705945");

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

