/*
 *		AMULETUSB.C
 *		
 *		A simple class to monitor the Amulet USB dongle link & mic status in the
 *		background and report to any caller.
 *		
 *		Caller is responsible for monitoring devices being added/removed from the
 *		system and calling the Connect() method whenever that occurs.
 *		
 *		Copyright Amulet Devices 2010. All rights reserved. Permission is granted
 *		to re-use this code in any software intended to work with the Amulet Voice 
 *		Remote.
 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32.SafeHandles; 
using System.Runtime.InteropServices;

namespace AmuletJUSB
{
	public class AmuletUSB
	{
		const string	 DebugPrefix		= "[Amulet_USB] ";
		static bool		 USBDebug			= false;

		public bool		 Connected			= false;
		public bool		 RadioActive		= false;	// True when remote is awake
		public bool		 MicActive			= false;	// True when microphone is active (unmuted)

		public int		 PollTimeActive		= 50;		// Poll time when link is active   (50 ms)
		public int		 PollTimeInactive	= 200;		// Poll time when link is inactive (200 ms)

		DeviceManagement MyDeviceManagement = new DeviceManagement(); 
		Hid				 MyHid = new Hid(); 

		String			 myDevicePathName; 
		SafeFileHandle	 hidHandle; 

		//Signal strength registers
		const int				RSSI_ANT0_REG		= 0xf47b;
		const int				RSSI_ANT1_REG		= 0xf47c;
		const int				RSSI_CUR_REG		= 0xf47d;
		const int				TXPWR_REG			= 0xf43d;

		const int				STATUS_VERSION		= 0x01; 
		const int				STATUS_RX_BUSY		= 0x02; 
		const int				STATUS_TX_READY		= 0x04; 
		const int				STATUS_ERROR		= 0x08;	// not a real flag 
		const int				STATUS_ASSERT		= 0x40;

		// System status bits received from remote control
		const Int32 SSTATUS_ACCNORMAL			= 0x000001;
		const Int32 SSTATUS_ACCPUBLIC			= 0x000002;
		const Int32 SSTATUS_ACCSEMIPUB			= 0x000004;
		const Int32 SSTATUS_MUTEAUDIO			= 0x000008;
		const Int32 SSTATUS_MUTEMIC				= 0x000010;
		const Int32 SSTATUS_SILENCE				= 0x000020;
		const Int32 SSTATUS_SOFTOFF				= 0x000040;
		const Int32 SSTATUS_DATAREADY			= 0x000080;
		const Int32 SSTATUS_SEARCHING			= 0x000100;
		const Int32 SSTATUS_PAIRING				= 0x000200;
		const Int32 SSTATUS_PUBSEARCH			= 0x000400;
		const Int32 SSTATUS_POLL				= 0x000800;
		const Int32 SSTATUS_POLLON				= 0x001000;
		const Int32 SSTATUS_NEXTLISTENER		= 0x002000;
		const Int32 SSTATUS_VOICEON				= 0x004000;
		const Int32 SSTATUS_ACC_CLOSED			= 0x008000;
		const Int32 SSTATUS_POWER_ON			= 0x010000;
		const Int32 SSTATUS_ERROR				= 0x020000;
		const Int32 SSTATUS_VDC_OK				= 0x040000;
		const Int32 SSTATUS_BATTERY_LOW			= 0x080000;
		const Int32 SSTATUS_BATTERY_CHARGING 	= 0x100000;
		const Int32 SSTATUS_STANDBY				= 0x200000;
		const Int32 SSTATUS_ACTIVE				= 0x400000;
		const Int32 SSTATUS_RESYNC				= 0x800000;

		// We track the state of these bits and report them when USBDebugging is enabled, but do not otherwise use them
		const int				STATUS_MASK_UNUSED	= (STATUS_VERSION | STATUS_RX_BUSY | STATUS_ERROR | STATUS_ASSERT);

		const int				USB_VENDOR_AMULET  = 0x170D;
		const int				USB_PRODUCT_AV7101 = 0x0001;

		static Byte[]			StatusCmdBuf; 
		static Byte[]			PeekCmdBuf; 
		static Hid.OutFeatureReport StatusCmdReport; 

		static Byte[]			ReadBuf; 
		static Byte[]			LastReadBuf; 
		static Hid.InFeatureReport ReadReport;
		static Thread			MonitorThread;
		static bool				USBActive;

		// Used for raising events to callers
		public delegate void	USBChangedHandler(AmuletUSB sender, object dummy);
		static public event		USBChangedHandler USBChanged;

		public AmuletUSB()
		{
			// Create the USB monitoring thread
			USBActive = true;
			USBChanged = null;		// Reset the event handler chain for this new object

			if (MonitorThread == null)
			{
				MonitorThread				= new Thread(new ThreadStart(PeriodicAmuletCheck));
				MonitorThread.Name			= "AmuletRadioLinkCheck";
				MonitorThread.IsBackground	= true;
				MonitorThread.Start();
			}
		}

		~AmuletUSB()
		{
			USBActive = false;	// Stop thread executing
		}

        public void stopIt()
        {
            USBActive = false;
        }
		void PeriodicAmuletCheck()
		{
			while (true)
			{
				if (USBActive)
				{
					if (Connected)
					{
						PollAmuletState();
					}
				}
				// When the radio link is active, poll quicker to get faster
				// response to the mic coming on and off
				if (RadioActive)
					Thread.Sleep(PollTimeActive);		// Poll 20 times a second when remote is connected
				else
					Thread.Sleep(PollTimeInactive);		// Poll 5 times a second when remote is in standby
			}
		}

		/*
		 *		AmuletConnect()
		 *		
		 *		Create the initial connection to the Amulet USB device.
		 *		Returns true if successful, false if no receiver is connected.
		 */

		public bool AmuletConnect()
		{
			Connected = FindTheHid(USB_VENDOR_AMULET, USB_PRODUCT_AV7101);
			if (Connected)
			{
				createReports();
			}
			return (Connected);
		}

		public static void DebugLine(string msg)
		{
			Debug.WriteLine(DebugPrefix + msg.Replace("\n", "\n" + DebugPrefix));
		}

		///  <summary>
		///  Uses a series of API calls to locate a HID-class device
		///  by its Vendor ID and Product ID.
		///  </summary>
		/// 		 
		///  <returns>
		///   True if the device is detected, False if not detected.
		///  </returns>
		private Boolean FindTheHid(Int16 myVendorID, Int16 myProductID ) 
		{			  
			Boolean deviceFound = false; 
			String[] devicePathName = new String[ 1024 ];
			Guid hidGuid = Guid.Empty; 
			Int32 memberIndex = 0; 
			Boolean success = false;
			bool detected;
			
			try 
			{ 
				detected = false; 

				Hid.HidD_GetHidGuid( ref hidGuid );

				if (USBDebug)
					DebugLine("GUID for system HIDs: " + hidGuid.ToString() ); 
				
				//	Fill an array with the device path names of all attached HIDs.

				deviceFound = MyDeviceManagement.FindDeviceFromGuid( hidGuid, ref devicePathName ); 

				//	If there is at least one HID, attempt to read the Vendor ID and Product ID
				//	of each device until there is a match or all devices have been examined.
				
				if ( deviceFound ) 
				{					  
					memberIndex = 0; 
					
					do 
					{ 
						if (devicePathName[memberIndex] != null)
						{
							hidHandle = FileIO.CreateFile(devicePathName[memberIndex], 0, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);
			
							//DebugLine("	Returned handle: " + hidHandle.ToString() ); 
							if (!hidHandle.IsInvalid)  
							{							  
								//	The returned handle is valid, 
								//	so find out if this is the device we're looking for.
								
								//	Set the Size property of DeviceAttributes to the number of bytes in the structure.
								MyHid.DeviceAttributes.Size = Marshal.SizeOf( MyHid.DeviceAttributes ); 

								success = Hid.HidD_GetAttributes(hidHandle, ref MyHid.DeviceAttributes); 
								
								if ( success ) 
								{								 
									//DebugLine("	HIDD_ATTRIBUTES structure filled without error." ); 
									//DebugLine("	Structure size: " + MyHid.DeviceAttributes.Size );																								 
									if (USBDebug)
									{
										DebugLine("USB device: Vendor ID: 0x" + Convert.ToString(MyHid.DeviceAttributes.VendorID, 16) +								
												  "  Product ID: 0x" + Convert.ToString(MyHid.DeviceAttributes.ProductID, 16) +
												  "  Version Number: 0x" + Convert.ToString(MyHid.DeviceAttributes.VersionNumber, 16)); 
									}
									
									//	Find out if the device matches the one we're looking for.
									
									if ( ( MyHid.DeviceAttributes.VendorID == myVendorID ) && ( MyHid.DeviceAttributes.ProductID == myProductID ) ) 
									{ 
										//	Display the information in form's list box.
										if (USBDebug)
										{
											DebugLine("USB device matched: " +  								  
													  " Vendor ID: 0x" + Convert.ToString(MyHid.DeviceAttributes.VendorID, 16) +									
													  "	Product ID: 0x" + Convert.ToString(MyHid.DeviceAttributes.ProductID, 16));
										}

										MyHid.Capabilities = MyHid.GetDeviceCapabilities( hidHandle ); 
										if (USBDebug)
										{
											DebugLine("	Usage page: 0x" + Convert.ToString(MyHid.Capabilities.UsagePage, 16) +
													  "	Usage: 0x" + Convert.ToString(MyHid.Capabilities.Usage, 16));
										}
			
										if( (MyHid.Capabilities.UsagePage == -256 ) && 
											(MyHid.Capabilities.Usage == 0x01) &&
											(MyHid.Capabilities.FeatureReportByteLength > 0) )
										{
											string manufacturer = MyHid.GetManufacturerDetails(hidHandle);

											if (manufacturer.StartsWith("Amulet", StringComparison.CurrentCultureIgnoreCase))
											{
												detected = true;
												if (USBDebug)
													DebugLine("Found Amulet device - " + manufacturer);
											}
											else
											{
												if (USBDebug)
													DebugLine("Found non-Amulet device - " + manufacturer);
											}
										}
										//	Save the DevicePathName for OnDeviceChange().									
										myDevicePathName = devicePathName[ memberIndex ]; 
									}
								} 
								else 
								{ 
									//	There was a problem in retrieving the information.
									
									if (USBDebug)
										DebugLine("	Error in filling HIDD_ATTRIBUTES structure." ); 
									detected = false; 
								}							  
								if (!detected)
								{									  
									//	It's not a match, so close the handle.
									hidHandle.Close();									   
								}								  
							} 
						}
						//	Keep looking until we find the device or there are no devices left to examine.
						memberIndex = memberIndex + 1;						   
					} 
					while (!detected && memberIndex < devicePathName.Length);					   
				} 
				
				if ( detected ) 
				{					  
					//	The device was detected.
					//	Register to receive notifications if the device is removed or attached.
					
					//success = MyDeviceManagement.RegisterForDeviceNotifications( myDevicePathName, FrmMy.Handle, hidGuid, ref deviceNotificationHandle ); 
					
					//DebugLine( "RegisterForDeviceNotifications = " + success ); 
					
					//	Learn the capabilities of the device.
					
					MyHid.Capabilities = MyHid.GetDeviceCapabilities( hidHandle );
					//System.Threading.Thread.Sleep(1000);
				} 
				else 
				{ 
					//	The device wasn't detected.
					if (USBDebug)
					{
						DebugLine("USB device not found: VendorID 0x" + Convert.ToString(myVendorID, 16) +
								  " ProductID 0x" + Convert.ToString(myProductID, 16)); 
					}
				}				  
				return detected;				 
			} 
			catch ( Exception ex ) 
			{ 
				DebugLine("FindTheHid: Exception = " + ex.ToString()); 
				throw ; 
			} 
		}		

		// Returns a string of the form "Bitname=<on|off> " according to whether the
		// bit given in bitmask is set in 'value' or not.
		private static string AddBitValue(int value, string bitname, int bitmask)
		{
			if ((value & bitmask) == bitmask)
				return bitname + "=on ";
			else
				return bitname + "=off ";
		}

		private bool ExchangePeekReports() 
		{
			Boolean success = false; 
			bool res = false;
			
			try 
			{ 
				//	Write a report to the device
				success = SendCmd(PeekCmdBuf); 
				
				if ( !success ) 
				{ 
					if (USBDebug)
						DebugLine( "Status feature report failed." ); 
				} 

				//	Read a report from the device.
				do
				{
					ReadReport.Read(hidHandle, null, null, ref Connected, ref ReadBuf, ref success);

					if (USBDebug)
					{
						DisplayFeatureReport("Peek", ReadBuf, 0);
					}
					if (success && (ReadBuf[2] == 0x05))
					{
						if (USBDebug)
						{
							DisplayFeatureReport("Data", ReadBuf, 0);
						}
					}

					if( !success )
					{
						if (USBDebug)
							DebugLine( "Read feature report failed." ); 
					}
				}
				while (success && (ReadBuf[2]==0x05));
				
			} 
			catch ( Exception ex ) 
			{ 
				DebugLine( ex.ToString() ); 
				throw ; 
			}			  
			return res;
		}		  



  
		// Debug code - dump a feature report
		private void DisplayFeatureReport( String Desc, Byte[] Report, int Start )
		{
			String byteValue = Desc + " : ";
			Int32 count;
			
			for (count = Start; count <= Report[ 1 ] + 1; count++) 
			{
				//	Display bytes as 2-character Hex strings.
				byteValue += String.Format( "{0:X2} ", Report[ count ] ); 
			} 
			DebugLine( byteValue );
		}
		/*
		 *		createReports()
		 *		
		 *		Creates the report structure used to query Amulet's status via USB
		 */
		void createReports()
		{
			try 
			{
				StatusCmdReport = new Hid.OutFeatureReport(); 
		
				StatusCmdBuf = new Byte[ MyHid.Capabilities.FeatureReportByteLength ]; 
				
				/* status */
				StatusCmdBuf[ 0 ] = 0xFF; 
				StatusCmdBuf[ 1 ] = 0x05; 
				StatusCmdBuf[ 2 ] = 0; 
				StatusCmdBuf[ 3 ] = 0xDC; 
				StatusCmdBuf[ 4 ] = 0; 
				StatusCmdBuf[ 5 ] = 0; 
				StatusCmdBuf[ 6 ] = 0; 

				PeekCmdBuf = new Byte[ MyHid.Capabilities.FeatureReportByteLength ]; 
				
				/* status */
				PeekCmdBuf[ 0 ] = 0xFF; 
				PeekCmdBuf[ 1 ] = 0x05; 
				PeekCmdBuf[ 2 ] = 0; 
				PeekCmdBuf[ 3 ] = 0xB1; 
				PeekCmdBuf[ 4 ] = 0x3d; 
				PeekCmdBuf[ 5 ] = 0xf4; 
				PeekCmdBuf[ 6 ] = 0; 

				ReadReport = new Hid.InFeatureReport(); 

				ReadBuf = new Byte[MyHid.Capabilities.FeatureReportByteLength];
				LastReadBuf = new Byte[MyHid.Capabilities.FeatureReportByteLength];
				ReadBuf[0] = 0xFF;

			}
			catch ( Exception ex ) 
			{ 
				DebugLine( ex.ToString() ); 
				throw ; 
			}	
		}

		// Send command to dongle / remote
		private Boolean SendCmd(Byte[] CmdBuf)
		{
			try 
			{
				return StatusCmdReport.Write( CmdBuf, hidHandle ); 
			}
			catch ( Exception ex ) 
			{ 
				DebugLine( ex.ToString() ); 
				throw ; 
			}			  
		}
		/*
		 *		PollAmuletState()
		 *		
		 *		Checks the current Amulet state, raises events if changed
		 *		since the last time we polled
		 */
		public void PollAmuletState()
		{
			bool changed	= false;
			bool radio_on   = false;
			bool mic_on		= false;
			bool prev_connected = Connected;
			Int32 status;

			try
			{
				if (Connected)
				{
					if (ExchangeStatusReports(out status))
					{
						radio_on = (status & SSTATUS_ACTIVE)  != 0;
						mic_on   = (status & SSTATUS_MUTEMIC) == 0;
					}
					if (RadioActive != radio_on)
					{
						RadioActive = radio_on;
						changed = true;
					}
					if (MicActive != mic_on)
					{
						MicActive = mic_on;
						changed = true;
					}
					if (Connected != prev_connected)
						changed = true;

					if (changed)
					{
						// 	DebugLine("Connection state changed: connected=" + Connected + " Radio=" + RadioActive + " Mic=" + MicActive);
						USBChanged(this, null);
					}
				}
			}
			catch (Exception e)
			{
				DebugLine("AmuletUSBMonitorThread: Unexpected exception = " + e.ToString());
			}
		}

		/*
		 *		ExchangStatusReports()
		 *		
		 *		Send USB dongle status to remote over link, and get
		 *		a corresponding report of remote status in return
		 *		
		 *		Returns true if successful, false if failed for some reason
		 */
		private int last_status_unused = 0;
		private Int32 last_sys_status  = 0;
		private bool ExchangeStatusReports(out Int32 status) 
		{
			Boolean success = false; 

			status = 0;

			try 
			{ 
				//	Write a report to the device
				success = SendCmd(StatusCmdBuf); 
				
				if ( !success ) 
				{ 
					if (USBDebug)
						DebugLine( "ExchangeStatusReports: Status feature report failed" ); 
					return (false);
				} 

				//	Read a report from the device.
				do
				{
					bool changed = false;

					ReadReport.Read(hidHandle, null, null, ref Connected, ref ReadBuf, ref success);

					// if (changed && USBDebug)
					//	DisplayFeatureReport("Stat", ReadBuf, 0);

					if (success && ((ReadBuf[2] & STATUS_TX_READY) == STATUS_TX_READY))
					{
						// Check if buffers are equal
						for (int i = 0; i <= ReadBuf[1] + 1; i++)
						{
							if (LastReadBuf[i] != ReadBuf[i])
							{
								LastReadBuf[i] = ReadBuf[i];
								changed = true;
							}
						}
						if (changed && USBDebug)
							DisplayFeatureReport("Data", ReadBuf, 0);

						status = (ReadBuf[4] << 16) | (ReadBuf[5] << 8) | ReadBuf[6];
						if (USBDebug && last_sys_status != status)
						{
							last_sys_status = status;
							DebugLine("New status = 0x" + String.Format("{0:X6}", status));
						}
						return (true);
					}

					// Debug messages for Conor
					if (success && ((ReadBuf[2] & STATUS_MASK_UNUSED) != last_status_unused))
					{
						int status_changed = last_status_unused ^ ReadBuf[2];

						last_status_unused = (ReadBuf[2] & STATUS_MASK_UNUSED);
						if (USBDebug)
						{
							string changelist = "";

							if ((status_changed & STATUS_VERSION) != 0)	changelist += AddBitValue(last_status_unused, "Version", STATUS_VERSION);
							if ((status_changed & STATUS_RX_BUSY) != 0)	changelist += AddBitValue(last_status_unused, "RX_Busy", STATUS_RX_BUSY);
							if ((status_changed & STATUS_ERROR)   != 0) changelist += AddBitValue(last_status_unused, "Error",   STATUS_ERROR);
							if ((status_changed & STATUS_ASSERT)  != 0) changelist += AddBitValue(last_status_unused, "Assert",  STATUS_ASSERT);

							DebugLine("ExchangeStatusReports: Status change: " + changelist);
						}
					}

					if (!success)
					{
						if (USBDebug)
							DebugLine( "ExchangeStatusReports: Read feature report failed" ); 
					}
				}
				while (success && (ReadBuf[2] & STATUS_TX_READY) == STATUS_TX_READY);
			} 
			catch ( Exception ex ) 
			{ 
				DebugLine( ex.ToString() ); 
				throw ; 
			}			  
			return (false);
		}		  

	}
}
