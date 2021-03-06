﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NCore;
using NCore.UI;
using NCore.USB;
using NCore.USB.Models;
using NFirmware;
using NFirmwareEditor.Core;
using NFirmwareEditor.Managers;

namespace NFirmwareEditor.Windows
{
	internal partial class FirmwareUpdaterWindow : EditorDialogWindow
	{
		private readonly Firmware m_firmware;
		private readonly FirmwareLoader m_loader;
		private readonly BackgroundWorker m_worker = new BackgroundWorker { WorkerReportsProgress = true };

		private SimpleDataflash m_dataflash;
		private HidDeviceInfo m_deviceInfo = HidDeviceInfo.UnknownDevice;
		private string m_connectedDeviceProductId;
		private string m_hardwareVersion;
		private string m_firmwareVersion;

		public FirmwareUpdaterWindow(Firmware firmware, FirmwareLoader loader)
		{
			InitializeComponent();
			Icon = NFEPaths.ApplicationIcon;
			InitializeControls();

			m_firmware = firmware;
			m_loader = loader;
			m_worker.DoWork += BackgroundWorker_DoWork;
			m_worker.ProgressChanged += BackgroundWorker_ProgressChanged;

			Load += (s, e) =>
			{
				// When form shown as dialog we dont need to handle click event to close it, 
				// otherwise we need to close application mannualy.
				if (!Modal) CancelButton.Click += (s1, e1) => Application.Exit();
			};

			Closing += (s, e) =>
			{
				if (!CancelButton.Enabled)
				{
					e.Cancel = true;
					return;
				}
				HidConnector.Instance.DeviceConnected -= DeviceConnected;
			};
		}

		private void DeviceConnected(bool isConnected)
		{
			if (isConnected)
			{
				System.Diagnostics.Trace.WriteLine("Connected " + DateTime.Now);

				try
				{
					m_dataflash = HidConnector.Instance.ReadDataflash();
				}
				catch
				{
					return;
				}

				m_connectedDeviceProductId = m_dataflash.ProductId;
				m_deviceInfo = HidDeviceInfo.Get(m_connectedDeviceProductId);
				m_hardwareVersion = (m_dataflash.HardwareVersion / 100f).ToString("0.00", CultureInfo.InvariantCulture);
				m_firmwareVersion = (m_dataflash.FirmwareVersion / 100f).ToString("0.00", CultureInfo.InvariantCulture);

				UpdateUI(() =>
				{
					DeviceNameTextBox.Text = string.Format("[{0}] {1}", m_dataflash.ProductId, m_deviceInfo.Name);
					HardwareVersionTextBox.Text = m_hardwareVersion;
					FirmwareVersionTextBox.Text = m_firmwareVersion;
					BootModeTextBox.Text = m_dataflash.LoadFromLdrom ? "LDROM" : "APROM";
					UpdateStatusLabel.Text = @"Device is ready.";
					SetUpdaterButtonsState(true);
				});
			}
			else
			{
				System.Diagnostics.Trace.WriteLine("Disconnected " + DateTime.Now);
				m_connectedDeviceProductId = null;
				m_dataflash = null;

				UpdateUI(() =>
				{
					DeviceNameTextBox.Clear();
					HardwareVersionTextBox.Clear();
					FirmwareVersionTextBox.Clear();
					BootModeTextBox.Clear();
					UpdateStatusLabel.Text = @"Waiting for device...";
					SetUpdaterButtonsState(false);
				});
			}
		}

		private void InitializeControls()
		{
			DeviceNameTextBox.BackColor = Color.White;
			HardwareVersionTextBox.BackColor = Color.White;
			FirmwareVersionTextBox.BackColor = Color.White;
			BootModeTextBox.BackColor = Color.White;

			DeviceNameTextBox.ReadOnly = true;
			HardwareVersionTextBox.ReadOnly = true;
			FirmwareVersionTextBox.ReadOnly = true;
			BootModeTextBox.ReadOnly = true;

			HidConnector.Instance.DeviceConnected += DeviceConnected;
			HidConnector.Instance.StartUSBConnectionMonitoring();
			Closing += (s, e) => HidConnector.Instance.StopUSBConnectionMonitoring();

			LogoButton.Click += LogoButton_Click;
			UpdateButton.Click += UpdateButton_Click;
			UpdateFromFileButton.Click += UpdateFromFileButton_Click;

			ResetDataflashButton.Click += ResetDataflashButton_Click;
			ReadDataflashButton.Click += ReadDataflashButton_Click;
			WriteDataflashButton.Click += WriteDataflashButton_Click;

			ChangeHWButton.Click += ChangeHWButton_Click;
			ChangeBootModeButton.Click += ChangeBootModeButton_Click;
		}

		private void UpdateFirmware(Func<byte[]> firmwareFunc)
		{
			var skipValidation = ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Alt);
			try
			{
				var firmware = firmwareFunc();
				if (!skipValidation && firmware.FindByteArray(Encoding.UTF8.GetBytes(m_connectedDeviceProductId)) == -1)
				{
					InfoBox.Show("Opened firmware file is not suitable for the connected device.");
					return;
				}
				m_worker.RunWorkerAsync(new AsyncProcessWrapper(worker => UpdateFirmwareAsyncWorker(worker, firmware)));
			}
			catch (Exception ex)
			{
				InfoBox.Show("An error occured during firmware update.\n" + ex.Message);
			}
		}

		private void UpdateFirmwareAsyncWorker(BackgroundWorker worker, byte[] firmware)
		{
			var isSuccess = false;
			try
			{
				UpdateUI(() => UpdateStatusLabel.Text = @"Reading dataflash...");
				var dataflash = HidConnector.Instance.ReadDataflash(worker);
				if (dataflash.LoadFromLdrom == false && dataflash.FirmwareVersion > 0)
				{
					Trace.Info("Switching boot mode...");
					dataflash.LoadFromLdrom = true;

					UpdateUI(() => UpdateStatusLabel.Text = @"Writing dataflash...");
					Trace.Info("Writing dataflash...");
					HidConnector.Instance.WriteDataflash(dataflash, worker);
					Trace.Info("Writing dataflash... Done. Waiting 500 msec.");
					Thread.Sleep(100);

					UpdateUI(() => UpdateStatusLabel.Text = @"Restarting device...");
					Trace.Info("Restarting device...");
					HidConnector.Instance.RestartDevice();
					Thread.Sleep(200);
					Trace.Info("Restarting device... Done.");

					Trace.Info("Waiting for device after reset...");
					UpdateUI(() => UpdateStatusLabel.Text = @"Waiting for device after reset...");

					var deviceFoundResult = SpinWait.SpinUntil(() =>
					{
						Thread.Sleep(1000);
						var isConnected = HidConnector.Instance.IsDeviceConnected;
						if (!isConnected)
						{
							Trace.Info("Device is not connected. Next attempt in 1 sec.");
							return false;
						}
						try
						{
							var df = HidConnector.Instance.ReadDataflash();
							Trace.Info(df.LoadFromLdrom
								? "Device found in the LDROM boot mode."
								: "Device found in the APROM boot mode. Waiting for the LDROM boot mode.");
							return df.LoadFromLdrom;
						}
						catch (Exception ex)
						{
							Trace.Info("Device found, but an error occured during attemp to read dataflash...\n" + ex);
							return false;
						}
					}, TimeSpan.FromSeconds(15));

					if (deviceFoundResult) Trace.Info("Waiting for device after reset... Done.");
					if (!deviceFoundResult)
					{
						Trace.Info("Waiting for device after reset... Failed.");
						InfoBox.Show("Device is not connected. Update process interrupted.");
						return;
					}
				}

				UpdateUI(() => UpdateStatusLabel.Text = @"Uploading firmware...");

				var writeFirmwareResult = SpinWait.SpinUntil(() =>
				{
					try
					{
						Trace.Info("Uploading firmware...");
						HidConnector.Instance.WriteFirmware(firmware, worker);
						Trace.Info("Uploading firmware... Done.");
						return true;
					}
					catch (Exception ex)
					{
						Trace.Info(ex, "Uploading firmware... Failed. Next attempt in 1 sec.");
						Thread.Sleep(1000);
						return false;
					}
				}, TimeSpan.FromSeconds(15));

				if (!writeFirmwareResult)
				{
					InfoBox.Show("Firmware update failed!");
					return;
				}

				isSuccess = true;
			}
			catch (Exception ex)
			{
				Trace.Warn(ex);
				InfoBox.Show("An exception occured during firmware update.\n" + ex.Message);
			}
			finally
			{
				UpdateUI(() =>
				{
					UpdateStatusLabel.Text = string.Empty;
					worker.ReportProgress(0);
				});

				if (isSuccess)
				{
					InfoBox.Show("Firmware successfully updated.");
				}
			}
		}

		private void UpdateLogoAsyncWorker(BackgroundWorker worker, byte[] block1ImageBytes, byte[] block2ImageBytes)
		{
			try
			{
				UpdateUI(() => UpdateStatusLabel.Text = @"Reading dataflash...");
				var dataflash = HidConnector.Instance.ReadDataflash(worker);
				if (dataflash.LoadFromLdrom == false && dataflash.FirmwareVersion > 0)
				{
					dataflash.LoadFromLdrom = true;
					UpdateUI(() => UpdateStatusLabel.Text = @"Writing dataflash...");
					HidConnector.Instance.WriteDataflash(dataflash, worker);
					HidConnector.Instance.RestartDevice();
					UpdateUI(() => UpdateStatusLabel.Text = @"Waiting for device after reset...");
					Thread.Sleep(2000);
				}
				UpdateUI(() => UpdateStatusLabel.Text = @"Uploading logo...");
				HidConnector.Instance.WriteLogo(block1ImageBytes, block2ImageBytes, worker);

				UpdateUI(() =>
				{
					UpdateStatusLabel.Text = string.Empty;
					worker.ReportProgress(0);
				});

				Thread.Sleep(500);
			}
			catch (Exception ex)
			{
				InfoBox.Show("An exception occured during logo update.\n" + ex.Message);
			}
		}

		private void ReadDataflashAsyncWorker(BackgroundWorker worker, string fileName)
		{
			try
			{
				UpdateUI(() => UpdateStatusLabel.Text = @"Reading dataflash...");
				var dataflash = HidConnector.Instance.ReadDataflash(worker);
				File.WriteAllBytes(fileName, dataflash.Data);
				UpdateUI(() => UpdateStatusLabel.Text = @"Dataflash was successfully read and saved to the file.");
			}
			catch (Exception ex)
			{
				InfoBox.Show("An exception occured during dataflash reading.\n" + ex.Message);
			}
		}

		private void WriteDataflashAsyncWorker(BackgroundWorker worker, SimpleDataflash simpleDataflash)
		{
			try
			{
				UpdateUI(() => UpdateStatusLabel.Text = @"Writing dataflash...");
				HidConnector.Instance.WriteDataflash(simpleDataflash, worker);
				UpdateUI(() =>
				{
					UpdateStatusLabel.Text = @"Dataflash was successfully written.";
					worker.ReportProgress(0);
				});
			}
			catch (Exception ex)
			{
				InfoBox.Show("An exception occured during dataflash reading.\n" + ex.Message);
			}
		}

		private void SetUpdaterButtonsState(bool enabled)
		{
			LogoButton.Enabled = enabled && m_deviceInfo.CanUploadLogo;
			UpdateButton.Enabled = enabled && m_firmware != null;
			UpdateFromFileButton.Enabled = enabled;

			ResetDataflashButton.Enabled = enabled;
			ReadDataflashButton.Enabled = enabled;
			WriteDataflashButton.Enabled = enabled;

			ChangeHWButton.Enabled = enabled;
			ChangeBootModeButton.Enabled = enabled;
		}

		private void SetAllButtonsState(bool enabled)
		{
			SetUpdaterButtonsState(enabled);
			CancelButton.Enabled = enabled;
		}

		private string GetDataflashFileName()
		{
			return string.Format("{0} HW v{1} FW v{2}", m_deviceInfo.Name, m_hardwareVersion, m_firmwareVersion);
		}

		// ReSharper disable once InconsistentNaming
		private void UpdateUI(Action action)
		{
			try
			{
				Invoke(action);
			}
			catch (Exception)
			{
				// Ignore
			}
		}

		private void LogoButton_Click(object sender, EventArgs e)
		{
			if (!m_deviceInfo.CanUploadLogo)
			{
				InfoBox.Show("Logo uploading for this device is not supported.");
				return;
			}

			Bitmap logoBitmap;
			using (var imageConverterWindow = new ImageConverterWindow(true, m_deviceInfo.LogoWidth, m_deviceInfo.LogoHeight))
			{
				if (imageConverterWindow.ShowDialog() != DialogResult.OK) return;

				logoBitmap = imageConverterWindow.GetConvertedImage();
				if (logoBitmap == null) return;
			}

			using (logoBitmap)
			{
				var imageData = logoBitmap.CreateRawFromBitmap();

				var block1ImageMetadata = new FirmwareImage1Metadata { Width = m_deviceInfo.LogoWidth, Height = m_deviceInfo.LogoHeight };
				var block2ImageMetadata = new FirmwareImage2Metadata { Width = m_deviceInfo.LogoWidth, Height = m_deviceInfo.LogoHeight };

				var block1ImageBytes = block1ImageMetadata.Save(imageData);
				var block2ImageBytes = block2ImageMetadata.Save(imageData);

				m_worker.RunWorkerAsync(new AsyncProcessWrapper(worker => UpdateLogoAsyncWorker(worker, block1ImageBytes, block2ImageBytes)));
			}
		}

		private void UpdateButton_Click(object sender, EventArgs e)
		{
			if (m_firmware == null) return;
			UpdateFirmware(() => m_firmware.GetBody());
		}

		private void UpdateFromFileButton_Click(object sender, EventArgs e)
		{
			string fileName;
			using (var op = new OpenFileDialog { Title = @"Select encrypted or decrypted firmware file ...", Filter = Consts.FirmwareFilter })
			{
				if (op.ShowDialog() != DialogResult.OK) return;
				fileName = op.FileName;
			}
			UpdateFirmware(() => m_loader.LoadFile(fileName));
		}

		private void ResetDataflashButton_Click(object sender, EventArgs e)
		{
			try
			{
				HidConnector.Instance.ResetDataflash();
				UpdateStatusLabel.Text = @"Dataflash has been reseted.";
			}
			catch (Exception ex)
			{
				InfoBox.Show("An error occured during dataflash reset.\n" + ex.Message);
			}
		}

		private void ReadDataflashButton_Click(object sender, EventArgs e)
		{
			string fileName;
			using (var sf = new SaveFileDialog { Title = @"Select dataflash file location ...", Filter = Consts.DataFlashFilter, FileName = GetDataflashFileName() })
			{
				if (sf.ShowDialog() != DialogResult.OK) return;
				fileName = sf.FileName;
			}

			try
			{
				File.WriteAllBytes(fileName, new byte[1]);
				m_worker.RunWorkerAsync(new AsyncProcessWrapper(worker => ReadDataflashAsyncWorker(worker, fileName)));
			}
			catch (Exception ex)
			{
				InfoBox.Show("An error occured during dataflash reading.\n" + ex.Message);
			}
		}

		private void WriteDataflashButton_Click(object sender, EventArgs e)
		{
			string fileName;
			using (var op = new OpenFileDialog { Title = @"Select dataflash file ...", Filter = Consts.DataFlashFilter })
			{
				if (op.ShowDialog() != DialogResult.OK) return;
				fileName = op.FileName;
			}

			try
			{
				var data = File.ReadAllBytes(fileName);
				if (data.Length != 2044)
				{
					InfoBox.Show("Seems that the dataflash file has the wrong format.");
					return;
				}

				var dataflash = new SimpleDataflash { Data = data };
				m_worker.RunWorkerAsync(new AsyncProcessWrapper(worker => WriteDataflashAsyncWorker(worker, dataflash)));
			}
			catch (Exception ex)
			{
				InfoBox.Show("An error occured during dataflash write.\n" + ex.Message);
			}
		}

		private void ChangeHWButton_Click(object sender, EventArgs e)
		{
			if (m_dataflash == null) return;

			using (var hwVersionDialog = new HardwareVersionWindow(m_dataflash.HardwareVersion))
			{
				if (hwVersionDialog.ShowDialog() != DialogResult.OK) return;
				m_dataflash.HardwareVersion = hwVersionDialog.GetNewHWVersion();
			}

			try
			{
				m_worker.RunWorkerAsync(new AsyncProcessWrapper(worker =>
				{
					WriteDataflashAsyncWorker(worker, m_dataflash);
					HidConnector.Instance.RestartDevice();
				}));
			}
			catch (Exception ex)
			{
				InfoBox.Show("An error occured during dataflash write.\n" + ex.Message);
			}
		}

		private void ChangeBootModeButton_Click(object sender, EventArgs e)
		{
			if (m_dataflash == null) return;

			m_dataflash.LoadFromLdrom = !m_dataflash.LoadFromLdrom;
			try
			{
				m_worker.RunWorkerAsync(new AsyncProcessWrapper(worker =>
				{
					WriteDataflashAsyncWorker(worker, m_dataflash);
					HidConnector.Instance.RestartDevice();
				}));
			}
			catch (Exception ex)
			{
				InfoBox.Show("An error occured during switching boot mode.\n" + ex.Message);
			}
		}

		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			UpdateProgressBar.Value = e.ProgressPercentage;
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = (BackgroundWorker)sender;
			var wrapper = (AsyncProcessWrapper)e.Argument;

			try
			{
				UpdateUI(() => SetAllButtonsState(false));
				HidConnector.Instance.StopUSBConnectionMonitoring();
				wrapper.Processor(worker);
			}
			finally
			{
				UpdateUI(() => SetAllButtonsState(true));
				HidConnector.Instance.StartUSBConnectionMonitoring();
			}
		}
	}
}
