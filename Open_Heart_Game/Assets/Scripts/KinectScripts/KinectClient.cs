using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;


public class KinectClient : MonoBehaviour {
	
	

	NetworkStream s;
	Socket soc;
	BinaryReader br;
	BinaryWriter bw;

	// Use in your scripts to determine if a kinect is connected to your computer, 
	// if this is false then lx, ly lz, rx, ry, rz will never change from zero, so you must use alternate input for debugging
	// purposes
	public static bool KINECT_CONNECTED = false; 

	// Use to check if the perosn is currently being tracked by the Kinect. These can change frame by frame,
	// and can be used to give messages to the user if they move out of the sensor range.
	public static bool SKELETON_TRACKED = false;

	/// <summary>
	/// The sensor elevation angle, in degrees. This value is constrained between MinElevationAngle and MaxElevationAngle.
	/// </summary>
	public int KinectElevationAngle = 10;
	/*
	 *  The tilt is relative to gravity rather than relative to the sensor base. An elevation angle of zero indicates 
	 * that the Kinect is pointing perpendicular to gravity.
	 * The angle is subject to the physical limitations of the sensor. 
	 * If the sensor base is resting on a tilted surface, the middle of the sensor's tilt range will 
	 * not correspond to an elevation angle of zero, and the sensor may not be physically capable of 
	 * reaching the outer limits of the range allowed by the API.
	 * If the sensor is moved so that the base is at a different angle relative to gravity, or if the 
	 * sensor is tilted manually, the angle reported by the API will change, even if the elevation angle 
	 * has not been changed programmatically. 
	*/
	private int MinElevationAngle;
	private int MaxElevationAngle;

	// indices for the Joint array "Joints", DON'T CHANGE THE ORDER!! Use this struct for your own code when using Joints array
	// see KinectSkeleton script
	public struct JointType {  
		public const int HipCenter=0;
		public const int Spine=1;
		public const int ShoulderCenter=2;
		public const int Head=3;
		public const int ShoulderLeft=4;
		public const int ElbowLeft=5;
		public const int WristLeft=6;
		public const int HandLeft=7;
		public const int ShoulderRight=8;
		public const int ElbowRight=9;
		public const int WristRight=10;
		public const int HandRight=11;
		public const int HipLeft=12;
		public const int KneeLeft=13;
		public const int AnkleLeft=14;
		public const int FootLeft=15;
		public const int HipRight=16;
		public const int KneeRight=17;
		public const int AnkleRight=18;
		public const int FootRight=19;
	}

	private enum JointEnum {  // indices for the Joint array "Joints", DON'T CHANGE THE ORDER!!
		HipCenter=0,
		Spine=1,
		ShoulderCenter=2,
		Head=3,
		ShoulderLeft=4,
		ElbowLeft=5,
		WristLeft=6,
		HandLeft=7,
		ShoulderRight=8,
		ElbowRight=9,
		WristRight=10,
		HandRight=11,
		HipLeft=12,
		KneeLeft=13,
		AnkleLeft=14,
		FootLeft=15,
		HipRight=16,
		KneeRight=17,
		AnkleRight=18,
		FootRight=19
	}

	/// <summary>
	/// The joints position, use the JointType enum to get the correct index of the joint you want.
	/// </summary>
	public static Vector3[] Joints = new Vector3[20];

	
	
	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject); // Don't destroy the kinect manager until we quit the game

		try //open up the socket
		{
			IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
			TcpListener listener = new TcpListener(ipAddress, 26000);
			listener.Start();

			// Start mini Kinect Server
			System.Diagnostics.Process.Start(Path.Combine (Application.dataPath, @"..\SpeechBasics-WPF\bin\Debug\SpeechBasics-WPF.exe"));
			soc = listener.AcceptSocket(); // look for connection from mini Kinect Server
			s = new NetworkStream(soc); 
			//sr = new StreamReader(s); // set up reading end of the socket.
			br = new BinaryReader(s);// set up binary reader, reads data from the kinect program
			bw = new BinaryWriter(s); // set up binary writer, used for initialization purposes only

			// check if the kinect is operational
			int check = br.ReadInt32();
			if(check == -1){
				KINECT_CONNECTED = false;
				Debug.Log("NO AVAILABLE KINECT DETECTED!");
			} else if(check == 1){ // kinect connected
				KINECT_CONNECTED = true;
				MinElevationAngle = br.ReadInt32(); // read min/max angles
				MaxElevationAngle = br.ReadInt32();
				Mathf.Clamp (KinectElevationAngle, MinElevationAngle, MaxElevationAngle);
				bw.Write(KinectElevationAngle); // tell kinect program the angle we want the kinect at
				Debug.Log("KINECT CONNECTED");
			}



		}catch (Exception e)
		{
			Debug.Log(e.Message);
		}
	}


	
	//read in information from the socket, once per frame
	void Update(){

		if (KINECT_CONNECTED) {
			if(!s.DataAvailable){ // no data available, socket may have closed on server side, or data isn't ready
				SKELETON_TRACKED = false;
				return;
			}


			try{
                
                Int32 numberRead = br.ReadInt32();
                Debug.Log(numberRead);


                switch (numberRead)
                {
                    case 1:
                        Pump.isOn = true;
                        break;
                    case 2:
                        Pump.isOn = false;
                        break;
                    case 4:
                        Table.trendelenburg = true;
                        break;

                }



				// check if the kinect has an available skeleton frame for us
				// before every skeleton frame, the kinect program will send either 0 or 1
				// 0 means no data ready, meaning no skeleton is being tracked, 1 means ready.
                /*
				if(br.ReadInt32() == 0){
					return; // no data ready, so just return
				}
                */
                

				// attempts to read exact number of bytes from stream.
				// if fails, then can check length of array returned. 
				// if exception thrown, then need to handle it
                /*
				byte[] data = ReadExactly(4); // read 20 * 3 = 60 floats, 60 * 4 bytes = 240 bytes

				if(data.Length != 4){ // end of stream reached, or error
					SKELETON_TRACKED = false;
					KINECT_CONNECTED = false;
					CloseSocket();
					Debug.Log("Error reading from server");
					return;
				}else{
					SKELETON_TRACKED = true;
				}

                
                */

                /*
				int byteNumber = 0;
				foreach (JointEnum joint in Enum.GetValues(typeof(JointEnum))) // get all the joint info
				{
					float tempX = BitConverter.ToSingle(data, byteNumber); // convert data from byte[]
					byteNumber+=4; // advance 4 bytes
					float tempY = BitConverter.ToSingle(data, byteNumber);
					byteNumber+=4;
					float tempZ = BitConverter.ToSingle(data, byteNumber);
					byteNumber+=4;


					if(tempX < -500.0f){ // the joint isn't being tracked, arbitrary number sent over from the server
						continue;
					}

					Joints[(int)joint].x = tempX;
					Joints[(int)joint].y = tempY;
					Joints[(int)joint].z = tempZ;
				}
                 */
			}catch(EndOfStreamException){
				//exception caught
				SKELETON_TRACKED = false;
				KINECT_CONNECTED = false;
				CloseSocket();
				Debug.Log("Error reading from server");
			}

		}
	}

	/// <summary>
	/// Reads exactly bytes. If fails to read bytes bytes, returns a byte array whose length is less than bytes specified
	/// </summary>
	byte[] ReadExactly(int bytes)
	{
		byte[] buffer = new byte[bytes];
		int total = 0;
		int read = 0;
		
		do
		{
			read = br.Read(buffer, total, bytes - total);
			total += read;
		}
		while (read > 0 && total < bytes);


		
		// Create new array if we didn't read all the bytes, and return that array
		if (total < bytes) {
			byte[] buf2 = new byte[total];
			Array.Copy(buffer,buf2,total);
			return buf2;
		}
		
		return buffer;
	}

	void CloseSocket(){
		if (br != null) {
			br.Close ();
		}
		if (bw != null) {
			bw.Close ();
		}
		if (s != null) {
			s.Close ();
		}
		if (soc != null) {
			soc.Close ();
		}
	}

	void OnApplicationQuit()
	{
		CloseSocket ();
	}
}
