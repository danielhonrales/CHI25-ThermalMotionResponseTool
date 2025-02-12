using System;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class UserResponseTool_Client : MonoBehaviour
{

    public static string pcIP = "172.16.136.202";  // Replace with the Raspberry Pi's IP address
    public static int port = 25568;             // Replace with the port number used on the Raspberry Pi

    public TcpClient client;
    public NetworkStream stream;
    private Thread receiveThread;
    private bool isRunning = false;

    public Image button;
    public TMP_InputField inputIP;

    public TrialController trialController;

    public bool startTrial = false;
    public float waitTime;
    public int trialNumber;
    public string pattern;

    // Start is called before the first frame update
    void Start()
    {
        inputIP.text = pcIP;
    }

    // Update is called once per frame
    void Update()
    {
        if (client != null && !client.Connected) {
            //Debug.Log("SignalSender is not connected");
            button.color = Colors.darkRed;
        }

        if (startTrial) {
            startTrial = false;
            StartCoroutine(trialController.TrialStart(waitTime, trialNumber, pattern));
        }
    }

    public void EstablishConnection() {
        try
        {
            // Create a TCP/IP socket
            client = new TcpClient();

            client.ConnectAsync(inputIP.text, port).Wait(1000);

            // Get a client stream for reading and writing
            stream = client.GetStream();

            isRunning = true;
            receiveThread = new Thread(new ThreadStart(ReadSignal));
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("SignalSender is connected");
            button.color = Colors.darkGreen;
        }
        catch (SocketException e)
        {
            SocketError(e);
        }
    }

    public void SendSignal(string signal)
    {
        int packetSize = 4096;

        signal = signal + "$";
        try
        {
            // Translate the signal string into bytes
            byte[] data = Encoding.ASCII.GetBytes(signal);

            // Send message count
            int packetCount = (int)Math.Ceiling((float)data.Length / packetSize);
            byte[] countData = Encoding.ASCII.GetBytes(packetCount.ToString());

            // Send messages
            for (int i = 0; i < packetCount; i++)
            {
                stream.Write(data[(i * packetSize)..Math.Min(((i + 1) * packetSize), data.Length)], 0, data.Length);
            }
        }
        catch (Exception e)
        {
            SocketError(e);
        }
    }

    public void ReadSignal() {
        try
        {
            byte[] buffer = new byte[4096];
            while (isRunning)
            {
                if (stream.CanRead)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log("Received: " + receivedMessage);
                        
                        if (receivedMessage.StartsWith("$trialstart")) {
                            string[] messageParams = receivedMessage.Split(",");
                            waitTime = float.Parse(messageParams[1]);
                            trialNumber = int.Parse(messageParams[2]);
                            pattern = messageParams[3];
                            startTrial = true;
                        }

                        if (receivedMessage.StartsWith("$trialend")) {
                            trialController.EndExperiment();
                        }

                        if (receivedMessage.StartsWith("$clear")) {
                            trialController.StartNewExperiment();
                        }
                        // Inform trial controller to start, pass trial time
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    private void SocketError(Exception e) {
        Debug.Log("Socket Exception: " + e);
        button.color = Colors.darkRed;
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();
    }
}
