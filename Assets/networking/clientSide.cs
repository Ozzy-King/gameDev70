using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Text;
using UnityEditor.Sprites;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

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
    public uint playerId;
    Socket connection;

    void ListenThread()
    {
        while (true) {
            string data = "";
            byte[] bytes = new byte[1024];
            int bytesRec = connection.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            print("recived");
            basePacket? parsedPacket = basePacket.parseData(data);
            if (parsedPacket != null) {
                print("the id is:" + parsedPacket.id.ToString());
                packetHandler(parsedPacket);
            }
        }
    }


    void start()
    {
        IPAddress Host = IPAddress.Parse("127.0.0.1"); //local host ip
        int port = 34197;

        IPEndPoint localEndPoint = new IPEndPoint(Host, port);

        // Create a Socket that will use Tcp protocol
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //this uses ipv4 : InterNetwork

        //conenct to server
        sender.Connect(localEndPoint);
        this.connection = sender;

        //setup listener thread (handles packet sfrom server)
        Thread myThread = new Thread(() => ListenThread());
        myThread.Start();

        //send inital connect packet to server for personal id
        basePacket initalConnect = new connectPacket();
        initalConnect.packetType = basePacket._packetType.connectPacket;
        initalConnect.id = 0;
        sendPacket(initalConnect);
        print("sent: " + initalConnect.packetToString());
    }


    void sendPacket(basePacket packet) {
        string stringOfPacket = packet.packetToString();
        byte[] msg = Encoding.ASCII.GetBytes(packet.packetToString());
        connection.Send(msg);
    }


    // Start is called before the first frame update
    void Start()
    {
        start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void packetHandler( basePacket packet) {

        switch (packet.packetType) {

            case basePacket._packetType.connectPacket:
                {
                    this.playerId = packet.id;
                
                }
                break;

            case basePacket._packetType.unknown:
            default:
                break;
        }



    }

}

class basePacket
{
    public enum _packetType
    {
        unknown = 0,
        movePacket = 1, //implimented
        connectPacket = 2
    }

    public uint id = 0; //given by the sending client
    public _packetType packetType = 0; //given by the packet

    //convert packet data to meaningful class
    public static basePacket? parseData(string data)
    {
        string[] typeData = data.Split(",", 2); //split into packet type and data
        Dictionary<string, string> allData = stringArrToDic(typeData[1].Split(",")); //split up data

        //parse packet type and set in class
        int packetType = 0;
        if (!int.TryParse(typeData[0], out packetType))
        { //if failed to parse
            return null;
        }

        //populate data in packet type
        switch ((_packetType)packetType)
        {

            case _packetType.movePacket:
                {
                    movePacket packet = new movePacket();
                    packet.packetType = (_packetType)packetType;
                    //assign all values
                    if (!uint.TryParse(allData["id"], out packet.id)) { return null; }

                    if (!float.TryParse(allData["posx"], out packet.posx)) { return null; }
                    if (!float.TryParse(allData["posy"], out packet.posy)) { return null; }
                    if (!float.TryParse(allData["posz"], out packet.posz)) { return null; }

                    if (!bool.TryParse(allData["runningBool"], out packet.runningBool)) { return null; }
                    if (!bool.TryParse(allData["moveingBool"], out packet.moveingBool)) { return null; }
                    if (!bool.TryParse(allData["reverseBool"], out packet.reverseBool)) { return null; }

                    return packet;
                }
            case _packetType.connectPacket:
                {
                    connectPacket packet = new connectPacket();
                    
                    packet.packetType = (_packetType)packetType;
                    if (!uint.TryParse(allData["id"], out packet.id)) { return null; }

                    return packet;
                }
            case _packetType.unknown:
            default:
                return null;

        }
    }
    static Dictionary<string, string> stringArrToDic(string[] data)
    {
        Dictionary<string, string> newDataDic = new Dictionary<string, string>();
        foreach (string s in data)
        {
            string[] keyVal = s.Split(":");
            if (keyVal.Length < 2) { continue; }
            keyVal[0] = keyVal[0].Trim().Trim('\"');
            keyVal[1] = keyVal[1].Trim().Trim('\"');
            newDataDic.Add(keyVal[0], keyVal[1]);
        }
        return newDataDic;
    }

    //convert class to packet data
    public string packetToString()
    {
        string dataString = "";
        switch (this.packetType)
        {

            case _packetType.movePacket:
                {
                    int temp = (int)this.packetType;
                    dataString += temp.ToString() + ",";//add packet type
                    dataString += "\"id\":" + ((movePacket)this).id.ToString() + ",";//add packet type

                    dataString += "\"posx\":" + ((movePacket)this).posx.ToString() + ",";//add packet type
                    dataString += "\"posy\":" + ((movePacket)this).posx.ToString() + ",";//add packet type
                    dataString += "\"posz\":" + ((movePacket)this).posy.ToString() + ",";//add packet type

                    dataString += "\"runningBool\":" + ((movePacket)this).runningBool.ToString() + ",";//add packet type
                    dataString += "\"moveingBool\":" + ((movePacket)this).moveingBool.ToString() + ",";//add packet type
                    dataString += "\"reverseBool\":" + ((movePacket)this).reverseBool.ToString() + ",";//add packet type


                    return dataString;
                }
            case _packetType.connectPacket:
                {
                    connectPacket connectPacketInstance = this as connectPacket;

                    int temp = (int)this.packetType;
                    dataString += temp.ToString() + ",";//add packet type
                    dataString += "\"id\":" + this.id.ToString() + ",";

                    return dataString;
                }

            case _packetType.unknown:
            default:
                return "";

        }

        return dataString;
    }
}

class movePacket : basePacket
{

    public float posx = 0;
    public float posy = 0;
    public float posz = 0;

    public bool runningBool = false;
    public bool moveingBool = false;
    public bool reverseBool = false;
}
class connectPacket : basePacket { }
