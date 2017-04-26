using Lidgren.Network;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using MumbleLink_CSharp;
using MumbleLink_CSharp_GW2;
using System.Text;
using AutoIt;

namespace LittleBuddy {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private BackgroundWorker worker = new BackgroundWorker();
        private NetPeer peer;
        private bool isServer = false;
        private float distanceThreshold = 3.0f;
        private float turnThreshold = 0.9f;

        private enum MessageType { ePosition };
        private enum KeyHeldDown { eNone, eLeft, eRight, eForward };

        GW2Link link;
        KeyHeldDown prevKey = KeyHeldDown.eNone;

        public MainWindow () {
            //AutoItX.Sleep(10);
            InitializeComponent();
            link = new GW2Link();
        }

        private void btnClient_Click (object sender, RoutedEventArgs e) {

            btnClient.IsEnabled = false;
            btnServer.IsEnabled = false;

            var config = new NetPeerConfiguration("application name");
            var client = new NetClient(config);
            client.Start();
            client.Connect(host: "192.168.1.34", port: 12345);
            peer = client;
            
            worker.DoWork += DoBackgroundWork;
            worker.RunWorkerAsync();
        }

        private void btnServer_Click (object sender, RoutedEventArgs e) {
            btnClient.IsEnabled = false;
            btnServer.IsEnabled = false;

            var config = new NetPeerConfiguration("application name") { Port = 12345 };
            var server = new NetServer(config);
            server.Start();
            peer = server;

            worker.DoWork += DoBackgroundWork;
            worker.RunWorkerAsync();

            isServer = true;
        }
        
        private void LogText(string text) {
            Dispatcher.BeginInvoke((Action)(() => LogTextUIThread(text)));
        }

        private void LogTextUIThread(string text) {
			//txtLog.AppendText(text + "\n");
			//txtLog.ScrollToEnd();
			txtLog.Text = text;
        }

        private void DoBackgroundWork (object sender, DoWorkEventArgs e) {
            while (true) {
                Thread.Sleep(10);

                NetIncomingMessage message;
                while ((message = peer.ReadMessage()) != null) {
                    switch (message.MessageType) {
                        case NetIncomingMessageType.Data:
                            HandleIncomingMessage(message);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            LogText(message.ReadString());
                            break;
                        default:
                            break;
                    }
                }

                if(isServer) {

                    SendLocation();
                }
                else {
                    // get mumble position
                }
            }

            //LogText("Message loop complete.");
        }

        private void button_Click (object sender, RoutedEventArgs e) {

            if (peer == null) {
                MessageBox.Show("Please choose a connection type");
                return;
            }

            if(peer.Connections==null || peer.Connections.Count == 0) {
                MessageBox.Show("You're not connected to anyone.");
                return;
            }

            SendLocation();
        }

        private void SendLocation() {
            if (peer == null || peer.Connections == null || peer.Connections.Count == 0)
                return;

            NetOutgoingMessage sendMsg = peer.CreateMessage();
            
            MumbleLinkedMemory data = link.Read();

            sendMsg.Write((byte)MessageType.ePosition);
            sendMsg.Write(data.FAvatarPosition[0]);
            sendMsg.Write(data.FAvatarPosition[1]);
            sendMsg.Write(data.FAvatarPosition[2]);

            peer.SendMessage(sendMsg, peer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
        
        private void HandleIncomingMessage(NetIncomingMessage message) {
            //LogText(message);

            if (isServer)
                return;

            if (!GameHasFocus())
                return;

            MessageType messageType = (MessageType)message.ReadByte();
            var serverPos = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
            
            MumbleLinkedMemory data = link.Read();
            var clientPos = new Vector3(data.FAvatarPosition);
            var clientFront = new Vector3(data.FAvatarFront);
			var clientRight = Vector3.Up().Cross(clientFront);

            var vecToTarget = Vec.Normalize(serverPos - clientPos);
            
            float dot = Vec.DotProduct(clientFront, vecToTarget);
			float right = Vec.DotProduct(clientRight, vecToTarget);
			float distance = Vec.Distance(clientPos, serverPos);
            
            LogText(string.Format("Received position info: {0}, {1}, {2}", serverPos.x, serverPos.y, serverPos.z));

			if(distance < distanceThreshold) {
			}
            else if (dot < turnThreshold && right < 0.0f) {
                LogText("  Turn left    " + dot + "    " + distance);
                PressKey(KeyHeldDown.eLeft);
            }
            else if(dot < turnThreshold && right > 0.0f) {
                LogText("  Turn right    " + dot + "    " + distance);
                PressKey(KeyHeldDown.eRight);
            } else if(distance > distanceThreshold) {
                LogText("  run forward    " + dot + "    " + distance);
                PressKey(KeyHeldDown.eForward);
            }
            else {
				//PressKey(KeyHeldDown.eNone);
				LogText( "  dont need to move.    " + dot + "    " + distance);
            }
        }

        private void PressKey(KeyHeldDown key) {

            if (key == prevKey)
                return;

            if(prevKey == KeyHeldDown.eForward)
                AutoItX.Send("{w up}", 0);
            else if (prevKey == KeyHeldDown.eLeft)
                AutoItX.Send("{a up}", 0);
            else if (prevKey == KeyHeldDown.eRight)
                AutoItX.Send("{d up}", 0);
            
            prevKey = key;

            if (key == KeyHeldDown.eForward)
                AutoItX.Send("{w down}", 0);
            else if (key == KeyHeldDown.eLeft)
                AutoItX.Send("{a down}", 0);
            else if (key == KeyHeldDown.eRight)
                AutoItX.Send("{d down}", 0);
        }

        private bool GameHasFocus() {
            IntPtr hwnd = Win32.GetForegroundWindow();
            if (hwnd == null || hwnd == IntPtr.Zero)
                return false;

            var sb = new StringBuilder(256);
            Win32.GetClassName(hwnd, sb, sb.Capacity);

            return sb.ToString() == "ArenaNet_Dx_Window_Class";
        }

        private void Window_Closed (object sender, EventArgs e) {
            link.Dispose();

            if (GameHasFocus())
                PressKey(KeyHeldDown.eNone);
        }

		private void checkAlwaysOnTop_Checked(object sender, RoutedEventArgs e)
		{
			 //= checkAlwaysOnTop.IsChecked.Value;
		}
	}
}
