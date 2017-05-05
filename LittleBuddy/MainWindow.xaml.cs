using Lidgren.Network;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using MumbleLink_CSharp;
using MumbleLink_CSharp_GW2;
using System.Text;
using AutoIt;
using System.Media;

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
		private float runTurnThreshold = 0.65f;
		private bool bKeepRunning = true;
        private bool mFollowEnabled = true;
		private int mPort = 12343;
        Vector3 mServerPos;

		private enum MessageType { ePosition, eToggleMovement };

        GW2Link link;

        public MainWindow () {
            InitializeComponent();
			ResourceExtractor.ExtractResourceToFile("LittleBuddy.AutoItX3.dll", "AutoItX3.dll");
			ResourceExtractor.ExtractResourceToFile("LittleBuddy.AutoItX3.Assembly.dll", "AutoItX3.Assembly.dll");
			link = new GW2Link();
        }

        private void btnClient_Click (object sender, RoutedEventArgs e) {

			if(peer != null)
			{
				Disconnect();
				btnClient.Content = "Follower (Client)";
				btnServer.IsEnabled = true;
				return;
			}

			btnClient.Content = "Disconnect";
            btnServer.IsEnabled = false;
            
            var config = new NetPeerConfiguration("littlebuddy");
            var client = new NetClient(config);
            client.Start();
            
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client.DiscoverLocalPeers(mPort);

            peer = client;
            
            worker.DoWork += DoBackgroundWork;
            worker.RunWorkerAsync();
            LogText("Started client.");
			SetGameFocus();
        }

        private void btnServer_Click (object sender, RoutedEventArgs e) {

			if (peer != null) {
				Disconnect();
				btnServer.Content = "Leader (Server)";
				btnClient.IsEnabled = true;
				return;
            }

            btnClient.IsEnabled = false;
			btnServer.Content = "Stop Server";
			
            var config = new NetPeerConfiguration("littlebuddy") { Port = mPort };
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            var server = new NetServer(config);
            server.Start();
            peer = server;

			worker.DoWork += DoBackgroundWork;
			worker.RunWorkerAsync();
            LogText("Started Server.");
			SetGameFocus();
			isServer = true;
        }

		private void Disconnect() {
			bKeepRunning = false;
			Thread.Sleep(100);
			peer.Shutdown("force disconnect");
			peer = null;
			LogText("Stopped Server.");
			StatusText("Disconnected.");
		}
        
        private void LogText(string text) {
            Dispatcher.BeginInvoke((Action)(() => LogTextUIThread(text)));
        }

        private void LogTextUIThread(string text) {
			txtLog.AppendText(text + "\n");
			txtLog.ScrollToEnd();
        }

        private void StatusText (string text) {
            Dispatcher.BeginInvoke((Action)(() => txtStatus.Text = text));
        }
        
        private void DoBackgroundWork (object sender, DoWorkEventArgs e) {
            while (bKeepRunning) {
                Thread.Sleep(10);
                
                NetIncomingMessage message;
                while ((message = peer.ReadMessage()) != null) {
                    switch (message.MessageType) {
                        case NetIncomingMessageType.Data:
                            HandleIncomingMessage(message);
                            break;
                        case NetIncomingMessageType.DiscoveryResponse: // client receives this message
                            LogText("Found server at " + message.SenderEndPoint);
                            peer.Connect(host: message.SenderEndPoint.Address.ToString(), port: mPort);
                            break;
                        case NetIncomingMessageType.DiscoveryRequest: // server receives this message
                            LogText("Client attempting to connect.");
                            NetOutgoingMessage response = peer.CreateMessage();
                            // Send the response to the sender of the request
                            peer.SendDiscoveryResponse(response, message.SenderEndPoint);
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

				if(Win32.IsKeyToggle((int)VirtualKeyStates.VK_SUBTRACT))
				{
					mFollowEnabled = !mFollowEnabled;
					if(mFollowEnabled)
						SystemSounds.Beep.Play();
					else
						SystemSounds.Hand.Play();
					LogText("Following is " + (mFollowEnabled ? "enabled" : "disabled"));

					if(isServer) {
						NetOutgoingMessage sendMsg = peer.CreateMessage();

						sendMsg.Write((byte)MessageType.eToggleMovement);
						sendMsg.Write(mFollowEnabled);

						peer.SendMessage(sendMsg, peer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
					}
				}

				if(isServer)
                    SendLocation();
                else
                    TurnToLeader();
            }
        }
		
        private void SendLocation() {
            if (peer == null || peer.Connections == null || peer.Connections.Count == 0)
                return;

            NetOutgoingMessage sendMsg = peer.CreateMessage();
			
			MumbleLinkedMemory mumble = link.Read();
			
			sendMsg.Write((byte)MessageType.ePosition);
			sendMsg.Write(mumble.FAvatarPosition[0]);
			sendMsg.Write(mumble.FAvatarPosition[1]);
			sendMsg.Write(mumble.FAvatarPosition[2]);
			

            peer.SendMessage(sendMsg, peer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
        
        private void HandleIncomingMessage(NetIncomingMessage message) {
            if (isServer)
                return;

			if (!GameHasFocus())
                return;

            MessageType messageType = (MessageType)message.ReadByte();

			if(messageType == MessageType.ePosition)
				mServerPos = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
			else if(messageType == MessageType.eToggleMovement) {
				mFollowEnabled = message.ReadBoolean();
				if(mFollowEnabled)
					SystemSounds.Beep.Play();
				else
					SystemSounds.Hand.Play();
				LogText("Following is " + (mFollowEnabled ? "enabled" : "disabled"));
			}

			TurnToLeader();
		}

		private void TurnToLeader() {

			if(mServerPos == null)
				return;

            MumbleLinkedMemory data = link.Read();
			var clientPos = new Vector3(data.FAvatarPosition);
			var clientFront = new Vector3(data.FAvatarFront);
			var clientRight = Vector3.Up().Cross(clientFront);

			clientPos.y = mServerPos.y;
			var vecToTarget = Vec.Normalize(mServerPos - clientPos);

			float dot = Vec.DotProduct(clientFront, vecToTarget);
			float right = Vec.DotProduct(clientRight, vecToTarget);
			float distance = Vec.Distance(clientPos, mServerPos);

			bool moveForward = false;
			bool turnLeft = false;
			bool turnRight = false;

			if(!mFollowEnabled)
			{
				StatusText("Following disabled. Press hotkey to enable.");
			}
			else
			{
				if(distance > distanceThreshold)
					moveForward = true;

				if(dot < turnThreshold && right < 0.0f) {
					turnLeft = true;
					if(dot < runTurnThreshold)
						moveForward = false;
				}
				else if(dot < turnThreshold && right > 0.0f) {
					turnRight = true;
					if(dot < runTurnThreshold)
						moveForward = false;
				}

				StatusText(string.Format("Holding keys: Forward={0} Left={1} Right={2}", moveForward, turnLeft, turnRight ));
			}
			            
            SetKeyState("w", 'W', moveForward);
            SetKeyState("a", 'A', turnLeft);
            SetKeyState("d", 'D', turnRight);
        }
		
        private void SetKeyState ( string keyName, int key, bool shouldBeDown) {
            if (Win32.IsKeyDown(key) == shouldBeDown)
                return;

            if(shouldBeDown)
                AutoItX.Send("{" + keyName + " down}", 0);
            else
                AutoItX.Send("{" + keyName + " up}", 0);
            
        }

        private bool GameHasFocus() {
            IntPtr hwnd = Win32.GetForegroundWindow();
            if (hwnd == null || hwnd == IntPtr.Zero)
                return false;

            var sb = new StringBuilder(256);
            Win32.GetClassName(hwnd, sb, sb.Capacity);

            return sb.ToString() == "ArenaNet_Dx_Window_Class";
        }

		private void SetGameFocus()
		{
			IntPtr hwnd = Win32.FindWindow("ArenaNet_Dx_Window_Class", null);
			if(hwnd == null || hwnd == IntPtr.Zero)
				return;

			Win32.SetForegroundWindow(hwnd);
		}

		private void Window_Closing (object sender, EventArgs e) {
            bKeepRunning = false;

            Thread.Sleep(100);

            link.Dispose();
            link = null;

            if (GameHasFocus()) {
				SetKeyState("w", 'W', false);
				SetKeyState("a", 'A', false);
				SetKeyState("d", 'D', false);
			}
        }
	}
}
