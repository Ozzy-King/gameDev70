using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
#nullable enable

//packets--
//move packet
//  packet Type = 1
//  player ID
//  player pos:
//      posx
//      posy
//      posz
//  player animtion states:
//      runningBool    
//      moveingBool
//      reverseBool
//
//connect packet (just sets the id to a number and send back to the player)
//  packet Type = 2
//  player ID   <-- changed by server sent back to client
//


public class clientSide : MonoBehaviour
{
    public GameObject playerModel;
    Queue<basePacket> packetsToProcess = new Queue<basePacket>(); 
    public Dictionary<uint, GameObject> players = new Dictionary<uint, GameObject>();
    public Dictionary<uint, GameObject> enemys = new Dictionary<uint, GameObject>();
    Socket connection;

    public string HostIP;
    public int port;

    void clientRecvPrint(basePacket packet) {
        print("CLIENT_RECV>> " + packet.packetToString());
    }
    void clientSendPrint(basePacket packet) {
        print("CLIENT_SEND>> " + packet.packetToString());
    }


    void ListenThread()
    {
        while (true) {
            string data = "";
            byte[] bytes = new byte[1024];
            int bytesRec = connection.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            basePacket? parsedPacket = basePacket.parseData(data);
            if (parsedPacket != null) {
                clientRecvPrint(parsedPacket);
                packetsToProcess.Enqueue(parsedPacket);
            }
        }
    }

    public int connect()
    {
        IPAddress Host = IPAddress.Parse(HostIP); //local host ip

        IPEndPoint localEndPoint = new IPEndPoint(Host, port);

        // Create a Socket that will use Tcp protocol
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //this uses ipv4 : InterNetwork

        //sender.Blocking = false;
        //conenct to server
        try
        {
            sender.Connect(localEndPoint);
        }
        catch(SocketException e) {
            print(e);
            Destroy(this.gameObject);
            return 1;
        }
            this.connection = sender;
        return 0;
    }

    public void start()
    {


        //setup listener thread (handles packet sfrom server)
        Thread myThread = new Thread(() => ListenThread());
        myThread.Start();

        //send inital connect packet to server for personal id
        basePacket initalConnect = new connectPacket();
        initalConnect.packetType = basePacket._packetType.connectPacket;
        initalConnect.id = 0;
        sendPacket(initalConnect);
        clientSendPrint(initalConnect);

        basePacket processingPacket = new movePacket();
        movePacket packet = processingPacket as movePacket;
        playercameraLook playController = GameObject.Find("player").GetComponent<playercameraLook>();
        packet.posx = playController.player.transform.position.x;
        packet.posy = playController.player.transform.position.y;
        packet.posz = playController.player.transform.position.z;

        packet.rotx = playController.player.transform.eulerAngles.x;
        packet.roty = playController.player.transform.eulerAngles.y;
        packet.rotz = playController.player.transform.eulerAngles.z;

        packet.rothx = playController.headJoint.transform.eulerAngles.x;
        packet.rothy = playController.headJoint.transform.eulerAngles.y;
        packet.rothz = playController.headJoint.transform.eulerAngles.z;
        sendPacket(processingPacket);
        clientSendPrint(processingPacket);


    }

    public void sendPacket(basePacket packet) {
        string stringOfPacket = packet.packetToString();
        byte[] msg = Encoding.ASCII.GetBytes(packet.packetToString());
        connection.Send(msg);
    }


    // Start is called before the first frame update
    void Start()
    {
        start();
    }

    void packetHandler() {
        //will either precces 5 or all packet in a single update. which ever is lowest
        basePacket processingPacket;
        for (int i = 0; i < 5 && packetsToProcess.Count > 0; i++)
        {
            processingPacket = packetsToProcess.Dequeue();
            if (processingPacket == null) { return; }
            switch (processingPacket.packetType)
            {
                //send back ping pong packet
                case basePacket._packetType.pingpongPacket:
                    { 
                        pingpongPacket pack = new pingpongPacket();
                        sendPacket(pack);
                    }
                    break;

                case basePacket._packetType.connectPacket:
                    {
                        players.Add(processingPacket.id, Instantiate(playerModel, new Vector3(0,0,0), Quaternion.identity));
                    }
                    break;
                case basePacket._packetType.disconnectPacket:
                    {
                        Destroy(players[processingPacket.id]);
                        players.Remove(processingPacket.id);
                    }
                    break;
                case basePacket._packetType.movePacket:
                    {
                        movePacket packet = processingPacket as movePacket;
                        otherPlayerController playController = players[packet.id].GetComponent<otherPlayerController>();
                        playController.PositionInfo = (movePacket)packet.clone();
                    }
                    break;

                //spawns new enemy
                case basePacket._packetType.enemySpawnPacket:
                    {
                        enemySpawnPacket packet = processingPacket as enemySpawnPacket;
                        GameObject en = Instantiate(GameObject.Find("GameController").GetComponent<gameControllerScript>().enemyPrefab, new Vector3(packet.spawnx, 0, packet.spawnz), Quaternion.identity);
                        print(packet.packetToString());
                        en.name = "enemy" + packet.id.ToString();
                        enemys.Add(packet.id, en);
                    }
                    break;
                //give the enmy ai a new target position
                case basePacket._packetType.enemyMovePacket:
                    {
                        enemyMovePacket packet = processingPacket as enemyMovePacket;
                        enemys[packet.id].GetComponent<gingeyMovement>().targetPos = new Vector3(packet.targetx, packet.targety, packet.targetz);
                    }
                    break;

                case basePacket._packetType.unknown:
                default:
                    break;
            }
        }
    }

    //handel packet in main thread
    void Update()
    {
        packetHandler();
    }

}
