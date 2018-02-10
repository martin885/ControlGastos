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

<<<<<<< HEAD
            OneSignal.Current.StartInit(config.OneSignalAndroid).EndInit();
=======
            OneSignal.Current.StartInit("").EndInit();
>>>>>>> 44a7e649cc59644a9163cb22b3b130f6437c1021
            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.logotipoGGnotification;
            MobileAds.Initialize(ApplicationContext, config.MobileAds);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

