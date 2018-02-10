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
using ControlGastos.Config;

namespace ControlGastos.Droid
{
    [Activity(Label = "Gestor de Gastos", Icon = "@drawable/logotipoGG", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        configuracion config;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            OneSignal.Current.StartInit(config.OneSignalAndroid).EndInit();
            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.logotipoGGnotification;
            MobileAds.Initialize(ApplicationContext, config.MobileAds);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

