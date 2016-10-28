//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Threading;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class MainWindow : Window
    {


        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor sensor;


        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        // writes the data to the socket
        private NetworkStream ns;
        private BinaryWriter bw;
        private BinaryReader br;
        private MemoryStream ms;
        

 

        private TcpClient client;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            
            InitializeComponent();
        }


        private void OpenSocket()
        {
            String host = "localhost";
            Int32 port = 26000;
            bool socketOpened = false;
            do
            {
                try
                {
                    client = new TcpClient(host, port);
                    socketOpened = true;

                }
                catch (Exception)
                {
                    Thread.Sleep(1);
                }
            } while (!socketOpened);
            

            // to write bytes to the socket
            ns = client.GetStream();
            ms = new MemoryStream();
            bw = new BinaryWriter(ms);
            br = new BinaryReader(ns);
            

            
        }

        private void CloseSocket()
        {
            if (bw != null)
            {
                bw.Close();
            }
            if (br != null)
            {
                br.Close();
            }
            if (ms != null)
            {
                ms.Close();
            }
            if (ns != null)
            {
                ns.Close();
            }
            
            client.Close();
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            /*
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length != 0) // no skeletons detected by kinect
            {
                foreach (Skeleton skel in skeletons)
                {
                   

                    if (skel.TrackingState == SkeletonTrackingState.Tracked) // found skeleton that is being tracked
                    {


                        bw.Write(1); // tell client that skeleton data is available
                        
                        foreach (JointType joint in Enum.GetValues(typeof(JointType)))
                        {
                            if (skel.Joints[joint].TrackingState == JointTrackingState.Tracked) // joint tracked
                            {
                                bw.Write(skel.Joints[joint].Position.X);
                                bw.Write(skel.Joints[joint].Position.Y);
                                bw.Write(skel.Joints[joint].Position.Z);
                            }
                            else // write something arbitrary so unity knows the joint is not tracked
                            {
                                bw.Write(-1000.0F);
                                bw.Write(-1000.0F);
                                bw.Write(-1000.0F);
                            }
                        }

                        byte[] array = ms.ToArray(); //send memory stream to byte array
                        ms.SetLength(0); // set memory stream length to zero
                        WriteToSocket(array); // write to socket
                        return;                    
                    }
                }               
            }
            bw.Write(0); // tell client no data available, have to do this write even when no skeleton data to detect socket closure
            byte[] array2 = ms.ToArray(); //send memory stream to byte array
            ms.SetLength(0); // set memory stream length to zero
            WriteToSocket(array2); // write to socket
             */

        }


        private void WriteToSocket(byte [] array)
        {

            try
            {
                ns.Write(array, 0, array.Length);
                ns.Flush();
                Thread.Sleep(1);
            }
            catch (System.IO.IOException)
            {
                this.Close(); // On exception (due to Socket closure) exit application
            }
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Execute initialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            OpenSocket();
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {


                TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
                {
                    smoothingParam.Smoothing = 0.7f;
                    smoothingParam.Correction = 0.3f;
                    smoothingParam.Prediction = 1.0f;
                    smoothingParam.JitterRadius = 1.0f;
                    smoothingParam.MaxDeviationRadius = 1.0f;
                };

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable(smoothingParam);

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                try
                {
                    // Start the sensor!
                    this.sensor.Start();
                   
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }


            }

            if (null == this.sensor) // no Kinect connected, or no kinect ready
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;

                // say kinect is not ready
                bw.Write(-1); // arbitrary value
                byte[] array = ms.ToArray(); //send memory stream to byte array
                ms.SetLength(0); // set memory stream length to zero
                WriteToSocket(array); // write to socket
                
                this.Close();
              
            }
            // say kinect is ready
            bw.Write(1); // arbitrary value
            bw.Write(this.sensor.MinElevationAngle); // send over the min elevation angle
            bw.Write(this.sensor.MaxElevationAngle); // send over the max elevation angle
            byte[] array2 = ms.ToArray(); //send memory stream to byte array
            ms.SetLength(0); // set memory stream length to zero
            WriteToSocket(array2); // write to socket



            this.sensor.ElevationAngle = br.ReadInt32(); // get angle set by the client




            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {
                //recognitionSpans = new List<Span> { forwardSpan, backSpan, rightSpan, leftSpan };

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                /****************************************************************
                * 
                * Use this code to create grammar programmatically rather than from
                * a grammar file.
                * 
                * var directions = new Choices();
                * directions.Add(new SemanticResultValue("forward", "FORWARD"));
                * directions.Add(new SemanticResultValue("forwards", "FORWARD"));
                * directions.Add(new SemanticResultValue("straight", "FORWARD"));
                * directions.Add(new SemanticResultValue("backward", "BACKWARD"));
                * directions.Add(new SemanticResultValue("backwards", "BACKWARD"));
                * directions.Add(new SemanticResultValue("back", "BACKWARD"));
                * directions.Add(new SemanticResultValue("turn left", "LEFT"));
                * directions.Add(new SemanticResultValue("turn right", "RIGHT"));
                *
                * var gb = new GrammarBuilder { Culture = ri.Culture };
                * gb.Append(directions);
                *
                * var g = new Grammar(gb);
                * 
                ****************************************************************/

                // Create a grammar from grammar definition XML file.
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);


                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }


        }









        /// <summary>
        /// Execute uninitialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            CloseSocket();
            if (null != this.sensor)
            {

                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;




            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                byte[] array;
                switch (e.Result.Semantics.Value.ToString())
                {
                        
                    case "PUMPON":
                        bw.Write(1);
                        array = ms.ToArray(); //send memory stream to byte array
                        ms.SetLength(0); // set memory stream length to zero
                        WriteToSocket(array); // write to socket                        
                        break;
                    case "PUMPOFF":
                        bw.Write(2);
                        array = ms.ToArray(); //send memory stream to byte array
                        ms.SetLength(0); // set memory stream length to zero
                        WriteToSocket(array); // write to socket                        
                        break;
                    case "CLAMPLINES":
                        bw.Write(3);
                        array = ms.ToArray(); //send memory stream to byte array
                        ms.SetLength(0); // set memory stream length to zero
                        WriteToSocket(array); // write to socket                        
                        break;
                    case "TREND":
                        bw.Write(4);
                        array = ms.ToArray(); //send memory stream to byte array
                        ms.SetLength(0); // set memory stream length to zero
                        WriteToSocket(array); // write to socket                        
                        break;
                    case "BUMP":
                        bw.Write(5);
                        array = ms.ToArray(); //send memory stream to byte array
                        ms.SetLength(0); // set memory stream length to zero
                        WriteToSocket(array); // write to socket                        
                        break;
                    case "COOL":
                        bw.Write(6);
                        array = ms.ToArray(); //send memory stream to byte array
                        ms.SetLength(0); // set memory stream length to zero
                        WriteToSocket(array); // write to socket                        
                        break;
                        /*
                        sw.WriteLine("grabl");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;
                        
                    case "ROTL":
                        sw.WriteLine("rotl");
                        sw.Flush();
                        break;

                    case "SCAL":
                        sw.WriteLine("scal");
                        sw.Flush();
                        break;

                    case "RELEASELEFT":
                        sw.WriteLine("rel");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "GRABR":
                        sw.WriteLine("grabr");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "ROTR":
                        sw.WriteLine("rotr");
                        sw.Flush();
                        break;

                    case "SCAR":
                        sw.WriteLine("scar");
                        sw.Flush();
                        break;

                    case "RELEASERIGHT":
                        sw.WriteLine("rer");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "BACKWARD":
                        sw.WriteLine("backward");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "LEFT":
                        sw.WriteLine("left");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "RIGHT":
                        sw.WriteLine("right");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "NEWCUBE":
                        sw.WriteLine("nc");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "NEWSPHERE":
                        sw.WriteLine("ns");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "DELETEL":
                        sw.WriteLine("dl");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "DELETER":
                        sw.WriteLine("dr");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "HELPON":
                        sw.WriteLine("hon");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;

                    case "HELPOFF":
                        sw.WriteLine("hoff");
                        sw.Flush();
                        Thread.Sleep(1);
                        break;*/
                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {

        }
        
    }
}