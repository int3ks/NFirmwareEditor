using System;
using Android.App;
using Android.Content;

using Android.Widget;
using Android.OS;
using NToolbox.Models;

using System.ComponentModel;

using Android.Hardware.Usb;
using NCore.USB;
using NCore;
using static Android.Widget.AdapterView;
using Android.Views;
using static Android.Views.View;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;

namespace NToolboxAndroid
{

    [IntentFilter(new string[] { UsbManager.ActionUsbDeviceAttached })]
    [Activity(Label = "NToolboxAndroid", MainLauncher = true, Icon = "@drawable/ntoolbox_android", Theme = "@android:style/Theme.Light.NoTitleBar")]
    public class MainActivity : Activity, IOnItemSelectedListener, IOnClickListener
    {
        private const string ACTION_USB_PERMISSION = "com.android.example.USB_PERMISSION";
        private ArcticFoxConfiguration m_deviceConfiguration;
        private readonly BackgroundWorker m_worker = new BackgroundWorker { WorkerReportsProgress = true };
        private readonly Func<BackgroundWorker, byte[]> m_deviceConfigurationProvider = worker => HidConnector.Instance.ReadConfiguration(worker);
        Button uploadButton;
        private HidUsbReceiver m_HidUsbReceiver;

        private TextView profileName;

        private Spinner spinnerPreheat;
        private ArrayAdapter<string> adapterSpinnerPreheat;
        private Spinner spinnerPreheatCurve;
        private ArrayAdapter<string> adapterSpinnerPreheatCurve;

        private Spinner spinnerMode;
        private ArrayAdapter<string> adapterSpinnerMode;

        private Spinner spinnerMaterial;
        private ArrayAdapter<string> adapterSpinnerMaterial;
        private View layoutPreheatCurve;
        private View layoutPreheatPower;
        private Button downloadButton;
        private EditText power;
        private EditText preheatPower;
        private EditText preheatTime;
        private EditText preheatDelay;
        private EditText resistance;
        private EditText temperatur;
        private TextView preheatPowerType;
        private View layoutTempControl;
        private TextView temperaturType;
        private CheckBox temperaturDominant;
        private CheckBox resistanceLocked;
        private TextView deviceName;

        protected override void OnCreate(Bundle bundle)
        {



            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            InitControls();


            Handler mHandler = new Handler(Looper.MainLooper);

            _InitializeUsb();





        }

        private void InitControls()
        {
            uploadButton = FindViewById<Button>(Resource.Id.upload);
            uploadButton.SetOnClickListener(this);
            downloadButton = FindViewById<Button>(Resource.Id.download);
            downloadButton.SetOnClickListener(this);

            profileName = FindViewById<TextView>(Resource.Id.profileName);
            profileName.FocusChange += FocusChange;

            deviceName = FindViewById<TextView>(Resource.Id.deviceName);

            power = FindViewById<EditText>(Resource.Id.power);
            power.FocusChange += FocusChange;

            preheatPower = FindViewById<EditText>(Resource.Id.preheatPower);
            preheatPowerType = FindViewById<TextView>(Resource.Id.preheatPowerType);
            preheatPower.FocusChange += FocusChange;

            preheatTime = FindViewById<EditText>(Resource.Id.preheatTime);
            preheatTime.FocusChange += FocusChange;

            preheatDelay = FindViewById<EditText>(Resource.Id.preheatDelay);
            preheatDelay.FocusChange += FocusChange;

            resistance = FindViewById<EditText>(Resource.Id.resistance);
            resistance.FocusChange += FocusChange;
            resistanceLocked = FindViewById<CheckBox>(Resource.Id.resistanceLocked);

            temperatur = FindViewById<EditText>(Resource.Id.temperatur);
            temperatur.FocusChange += FocusChange;

            temperaturType = FindViewById<TextView>(Resource.Id.temperaturType);
            temperaturType.SetOnClickListener(this);

            temperaturDominant = FindViewById<CheckBox>(Resource.Id.tempDominant);

            layoutTempControl = FindViewById<View>(Resource.Id.layoutTempControl);

            spinnerPreheat = FindViewById<Spinner>(Resource.Id.spinnerPreheat);
            adapterSpinnerPreheat = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem);
            adapterSpinnerPreheat.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerPreheat.Adapter = adapterSpinnerPreheat;
            adapterSpinnerPreheat.Add("Absolute W");
            adapterSpinnerPreheat.Add("Relative %");
            adapterSpinnerPreheat.Add("Curve");
            spinnerPreheat.OnItemSelectedListener = this;

            layoutPreheatCurve = FindViewById<View>(Resource.Id.layoutPreheatCurve);
            layoutPreheatPower = FindViewById<View>(Resource.Id.layoutPreheatPower);

            spinnerPreheatCurve = FindViewById<Spinner>(Resource.Id.spinnerPreheatCurve);
            adapterSpinnerPreheatCurve = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem);
            adapterSpinnerPreheatCurve.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerPreheatCurve.Adapter = adapterSpinnerPreheatCurve;
            for (int i = 0; i < 8; i++) { adapterSpinnerPreheatCurve.Add($"Curve {i + 1}"); }

            spinnerMode = FindViewById<Spinner>(Resource.Id.spinnerMode);
            adapterSpinnerMode = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem);
            adapterSpinnerMode.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMode.Adapter = adapterSpinnerMode;
            adapterSpinnerMode.Add("Power");
            adapterSpinnerMode.Add("Temp. Control");
            spinnerMode.OnItemSelectedListener = this;

            spinnerMaterial = FindViewById<Spinner>(Resource.Id.spinnerMaterial);
            adapterSpinnerMaterial = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem);
            adapterSpinnerMaterial.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMaterial.Adapter = adapterSpinnerMaterial;
            foreach (var material in System.Enum.GetNames(typeof(ArcticFoxConfiguration.Material)))
            {
                adapterSpinnerMaterial.Add(material);
            }




        }

        private void FocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus) return;
            var ed = sender as EditText;
            if (ed != null && ed.Tag != null)
            {
                if (ed.Tag.ToString().StartsWith("regex"))
                {

                    var matches = Regex.Replace(ed.Text, ed.Tag.ToString().Substring(5), "");
                    ed.Text = matches;

                }
                else
                {


                    //try
                    //{
                    //    var d = double.Parse(ed.Text);
                    //    ed.Text = string.Format(CultureInfo.InvariantCulture,"{0:" + ed.Tag + "}", d);
                    //}
                    //catch { }
                }
            }

            CheckData();



        }

        double Parse(EditText textbox) {
            try {
                var str = textbox.Text;
                return double.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);

            } catch { return 0; }
        }


        bool CheckData() {
            try
            {
                profileName.Text = profileName.Text.ToUpper();


                power_val = Parse(power);
                if (power_val > 400)
                {
                    power_val = 400; power.Text = power_val.ToString();
                }

                preheatPower_val = Parse(preheatPower);
                double preheatPower_val_max = 250;
                if (spinnerPreheat.SelectedItemPosition == 0) preheatPower_val_max = 400;
                if (preheatPower_val > preheatPower_val_max)
                {
                    { preheatPower_val = preheatPower_val_max; preheatPower.Text = preheatPower_val_max.ToString(); };
                }



                preheatTime_val = Parse(preheatTime);
                if (preheatTime_val > 2.0f) { preheatTime_val = 2.0f; preheatTime.Text = preheatTime_val.ToString(); }

                preheatDelay_val = Parse(preheatDelay);
                if (preheatDelay_val > 180) { preheatDelay_val = 180; preheatDelay.Text = preheatDelay_val.ToString(); };

                resistance_val = Parse(resistance);
                if (resistance_val > 5.0f)
                {
                    resistance_val = 5.0f; resistance.Text = resistance_val.ToString();
                }

                tempteratur_val = Parse(temperatur);
                double tempteratur_val_max = 600;
                if (temperaturType.Text.Equals("°C")) { tempteratur_val_max = 315; }

                if (tempteratur_val > tempteratur_val_max) { tempteratur_val = tempteratur_val_max; temperatur.Text = tempteratur_val.ToString(); }


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long);
                return false;
            }
            return true;
        }


        private void _InitializeUsb()
        {
            IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);
            filter.AddAction(UsbManager.ActionUsbDeviceAttached);
            filter.AddAction(UsbManager.ActionUsbDeviceDetached);
            filter.AddAction(UsbManager.ActionUsbAccessoryAttached);
            filter.AddAction(UsbManager.ActionUsbAccessoryDetached);
            filter.AddAction(UsbManager.ActionUsbDeviceDetached);

            m_HidUsbReceiver = new HidUsbReceiver();
            RegisterReceiver(m_HidUsbReceiver, filter);

            HidConnector.Instance.DeviceConnected += isConnected => DeviceConnected(isConnected, false);
            HidConnector.Instance.StartUSBConnectionMonitoring();
        }



        private void DeviceConnected(bool isConnected, bool v)
        {
            DownloadProfile();

        }



        private void DownloadProfile()
        {
            HidConnector hc = HidConnector.Instance;
            if (hc.IsDeviceConnected)
            {
                var readResult = ReadBinaryConfiguration(m_deviceConfigurationProvider, false);
                m_deviceConfiguration = readResult.Configuration;

                RunOnUiThread(() =>
                {
                    if (m_deviceConfiguration != null)

                    {
                        LoadBaseData();

                        var selectedProfile = m_deviceConfiguration.General.Profiles[m_deviceConfiguration.General.SelectedProfile];

                        profileName.Text = selectedProfile.Name;

                        power.Text = (selectedProfile.Power / 10f).ToString();

                        spinnerPreheat.SetSelection((int)selectedProfile.PreheatType);

                        if (selectedProfile.PreheatType == ArcticFoxConfiguration.PreheatType.Watts)
                        {
                            preheatPower.Text = (selectedProfile.PreheatPower / 10f).ToString();
                        }
                        else {
                            preheatPower.Text = selectedProfile.PreheatPower.ToString();
                        }

                        preheatTime.Text = (selectedProfile.PreheatTime / 100f).ToString();

                        preheatDelay.Text = selectedProfile.PreheatDelay.ToString();

                        spinnerMode.SetSelection(((int)selectedProfile.Flags.Material) == 0 ? 0 : 1);

                        var coil = (int)selectedProfile.Flags.Material;
                        spinnerMaterial.SetSelection(coil);

                        resistance.Text = (selectedProfile.Resistance / 1000f).ToString();

                        resistanceLocked.Checked = selectedProfile.Flags.IsResistanceLocked;

                        temperatur.Text = selectedProfile.Temperature.ToString();

                        temperaturType.Text = selectedProfile.Flags.IsCelcius ? "°C" : "°F";

                        temperaturDominant.Checked = selectedProfile.Flags.IsTemperatureDominant;


                    }
                });
            }
        }
        private void CreateTestData()
        {
            m_deviceConfiguration = new ArcticFoxConfiguration
            {
                General = new ArcticFoxConfiguration.GeneralConfiguration
                {
                    Profiles = new ArcticFoxConfiguration.Profile[8]
                },
                Advanced = new ArcticFoxConfiguration.AdvancedConfiguration(),

            };

            m_deviceConfiguration.General.Profiles[m_deviceConfiguration.General.SelectedProfile] = new ArcticFoxConfiguration.Profile();
            m_deviceConfiguration.General.Profiles[m_deviceConfiguration.General.SelectedProfile].Flags = new ArcticFoxConfiguration.ProfileFlags();
        }

        double power_val, preheatPower_val, preheatTime_val, preheatDelay_val, resistance_val, tempteratur_val;
        public string CheckSetData()
        {
            if (m_deviceConfiguration == null && Debugger.IsAttached)
            {
                CreateTestData();
            }


            if (!CheckData()) {
                return "Checkdata failed!";
            }

            var selectedProfile = m_deviceConfiguration.General.Profiles[m_deviceConfiguration.General.SelectedProfile];



            try
            {
                selectedProfile.Name = profileName.Text.ToUpper();

                selectedProfile.Power = (ushort)(power_val * 10f);

                selectedProfile.PreheatType = (ArcticFoxConfiguration.PreheatType)spinnerPreheat.SelectedItemPosition;
                if (selectedProfile.PreheatType == ArcticFoxConfiguration.PreheatType.Watts) {
                    selectedProfile.PreheatPower = (ushort)(preheatPower_val * 10f);
                }
                else {
                    selectedProfile.PreheatPower = (ushort)(preheatPower_val);
                }
                
                selectedProfile.PreheatTime = (byte)(preheatTime_val * 100f);
                selectedProfile.PreheatDelay = (byte)preheatDelay_val;

                if (spinnerMode.SelectedItemPosition == 0)
                {
                    //powermode
                    selectedProfile.Flags.Material = 0;

                }
                else
                {
                    //tempmode
                    selectedProfile.Flags.Material = (ArcticFoxConfiguration.Material)spinnerMaterial.SelectedItemPosition;

                    selectedProfile.Resistance = (ushort)(resistance_val * 1000f);
                    selectedProfile.Flags.IsResistanceLocked = resistanceLocked.Checked;

                    selectedProfile.Temperature = (ushort)tempteratur_val;

                    selectedProfile.Flags.IsCelcius = temperaturType.Text.Equals("°C");
                    selectedProfile.Flags.IsTemperatureDominant = temperaturDominant.Checked;
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return "";
        }



        private void LoadBaseData()
        {
            deviceName.Text = HidDeviceInfo.Get(m_deviceConfiguration.Info.ProductId).Name+":";

            adapterSpinnerPreheatCurve.Clear();
            foreach (var curve in m_deviceConfiguration.Advanced.PowerCurves)
            {
                adapterSpinnerPreheatCurve.Add(curve.Name);
            }

            adapterSpinnerMaterial.Clear();
            for (int i = 0; i < 13; i++)
            {
                if (i < 5)
                {
                    adapterSpinnerMaterial.Add(System.Enum.GetNames(typeof(ArcticFoxConfiguration.Material))[i]);
                }
                else
                {
                    adapterSpinnerMaterial.Add($"[TFR]{m_deviceConfiguration.Advanced.TFRTables[i - 5].Name}");
                }
            }
        }

        private ConfigurationReadResult ReadBinaryConfiguration(Func<BackgroundWorker, byte[]> configurationProvider, bool useWorker = true)
        {
            if (configurationProvider == null) throw new ArgumentNullException("configurationProvider");

            try
            {
                var data = configurationProvider(useWorker ? m_worker : null);
                if (data == null) return new ConfigurationReadResult(null, ReadResult.UnableToRead);

                var info = BinaryStructure.ReadBinary<ArcticFoxConfiguration.DeviceInfo>(data);
                if (info.SettingsVersion < ArcticFoxConfiguration.SupportedSettingsVersion || info.FirmwareBuild < ArcticFoxConfiguration.MinimumSupportedBuildNumber)
                {
                    return new ConfigurationReadResult(null, ReadResult.OutdatedFirmware);
                }
                if (info.SettingsVersion > ArcticFoxConfiguration.SupportedSettingsVersion)
                {
                    return new ConfigurationReadResult(null, ReadResult.OutdatedToolbox);
                }

                var configuration = BinaryStructure.ReadBinary<ArcticFoxConfiguration>(data);
                return new ConfigurationReadResult(configuration, ReadResult.Success);
            }
            catch (TimeoutException)
            {
                return new ConfigurationReadResult(null, ReadResult.UnableToRead);
            }
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            if (parent.Id == spinnerPreheat.Id)
            {

                if (position == 2)
                {
                    layoutPreheatCurve.Visibility = ViewStates.Visible;
                    layoutPreheatPower.Visibility = ViewStates.Gone;
                }
                else
                {
                    layoutPreheatCurve.Visibility = ViewStates.Gone;
                    layoutPreheatPower.Visibility = ViewStates.Visible;
                    if (position == 0)
                    {
                        preheatPowerType.Text = "W";
                    }
                    else
                    {
                        preheatPowerType.Text = "%";
                    }


                }
            }
            if (parent.Id == spinnerMode.Id)
            {

                if (position == 0)
                {
                    layoutTempControl.Visibility = ViewStates.Gone;
                }
                else
                {
                    layoutTempControl.Visibility = ViewStates.Visible;
                }
            }
        }

        public void OnNothingSelected(AdapterView parent)
        {
            // throw new NotImplementedException();
        }













        public void OnClick(View v)
        {
            if (v.Id == uploadButton.Id)
            {
                string invalidData;
                if ((invalidData = CheckSetData()) == "")
                {
                    HidConnector hc = HidConnector.Instance;
                    if (hc.IsDeviceConnected)
                    {
                        if (m_deviceConfiguration != null)
                        {


                            var builder = new AlertDialog.Builder(this);
                            builder.SetMessage("sure? the developer of this tool is not resonsible if anything goes wrong. update at your own risk ;)");
                            builder.SetPositiveButton("OK", (s, e) =>
                            {
                                var data = BinaryStructure.WriteBinary(m_deviceConfiguration);
                                try
                                {
                                    HidConnector.Instance.WriteConfiguration(data, m_worker);
                                    Toast.MakeText(this, "Succesfull.", ToastLength.Long).Show();
                                    DownloadProfile();
                                }
                                catch (TimeoutException)
                                {
                                    NCore.Trace.Warn("Unable to write configuration.");
                                    Toast.MakeText(this, "Unable to write configuration.", ToastLength.Long).Show();
                                }
                            });
                            builder.SetNegativeButton("Cancel", (s, e) => { Toast.MakeText(this, "canceled.", ToastLength.Long).Show(); });
                            builder.Create().Show();


                        }
                        else
                        {
                            NCore.Trace.Warn("No Data!");
                            Toast.MakeText(this, "No Data!", ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        NCore.Trace.Warn("Not Connected!");
                        Toast.MakeText(this, "Not Connected!", ToastLength.Long).Show();
                    }

                }
                else
                {
                    NCore.Trace.Warn("Invalid Data!" + invalidData);
                    Toast.MakeText(this, "Invalid Data!" + invalidData, ToastLength.Long).Show();
                }
            }
            else

            if (v.Id == downloadButton.Id)
            {
                DownloadProfile();
            }
            else if (v.Id == temperaturType.Id)
            {
                temperaturType.Text = temperaturType.Text.Equals("°F") ? "°C" : "°F";
            }
        }


        private class ConfigurationReadResult
        {
            public ConfigurationReadResult(ArcticFoxConfiguration configuration, ReadResult result)
            {
                Configuration = configuration;
                Result = result;
            }

            public ArcticFoxConfiguration Configuration { get; private set; }

            public ReadResult Result { get; private set; }
        }
        private enum ReadResult
        {
            Success,
            UnableToRead,
            OutdatedFirmware,
            OutdatedToolbox
        }

    }
}

