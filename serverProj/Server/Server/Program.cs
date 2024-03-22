// See https://aka.ms/new-console-template for more information

//over server over to unity
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

//2, "tets1":1, "test2":2
//first: packet type
//rest: data

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
class clientClass
{
    public movePacket PlayerPosData;
    public Socket connection;
    public uint id;

    public clientClass(Socket con, uint id)
    {
        this.connection = con;
        this.id = id;
        this.PlayerPosData = new movePacket();
    }
}

class server
{
    static void serverSendPrint(clientClass client, basePacket packet) {
        Console.WriteLine("SERVER_SENDING_TO_ID_"+ client.id.ToString()+">> " + packet.packetToString());
    }

    static uint players = 1;

    static Queue<basePacket> packetQueue = new Queue<basePacket>();
    static List<clientClass> clients = new List<clientClass>();

    //user per connection used to recive packets
    static void handlerThread(clientClass clientSoc)
    {
        Console.WriteLine("CONNECTED_CLINET_ID>> " + clientSoc.id.ToString());
        while (true)
        {
            string data = "";
            byte[] bytes = new byte[1024];
            int bytesRec = clientSoc.connection.Receive(bytes); //recvie packet from cleint
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

            basePacket? parsedPacket = basePacket.parseData(data); //parse packet to packet class
            Console.WriteLine("got packet: " + data);
            if (parsedPacket != null) //if not null
            {
                //handle packet
                packetProcessor(ref parsedPacket, ref clientSoc);

                packetQueue.Enqueue(parsedPacket);
            }
        }
    }

    //broad cast packets to all player except the one who sent it.
    static void broadcastHandler()
    {
        while (true)
        {
            if (packetQueue.Count > 0) //get packet if it can
            {
                //dequeue and convert to byte array
                basePacket packet = packetQueue.Dequeue();

                foreach (clientClass clientCon in clients)
                {
                    //if the client is the who who sent dont send
                    if (packet.id == clientCon.id) { continue; }
                    //selse send to the client
                    sendPacket(clientCon, packet);
                }
            }
        }
    }

    //starts server (create broadcast handler and cleint handlers for when they connect)
    static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); //local host ip
        int port = 34197;

        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        // Create a Socket that will use Tcp protocol
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //this uses ipv4 : InterNetwork

        Thread SendHandlerThread = new Thread(() => broadcastHandler());
        SendHandlerThread.Start();

        listener.Bind(localEndPoint);
        // Specify how many requests a Socket can listen before it gives Server busy response.
        // We will listen 10 requests at a time
        listener.Listen(10);

        while (true)
        {
            Socket clientSocket = listener.Accept();
            clientClass newCon = new clientClass(clientSocket, players);
            Thread myThread = new Thread(() => handlerThread(newCon));

            myThread.Start();

            players++;
            clients.Add(newCon);
        }

    }

    static void sendPacket(clientClass SendTo, basePacket packet) {
        byte[] msg = Encoding.ASCII.GetBytes(packet.packetToString());
        serverSendPrint(SendTo, packet);
        SendTo.connection.Send(msg);
    }

    static void packetProcessor(ref basePacket thePacket, ref clientClass clientSoc) {

        //switch to process packets
        switch (thePacket.packetType) {

            case basePacket._packetType.connectPacket:
                thePacket.id = clientSoc.id;//give the curretn packet the sending clients id

                {//connect other clinet to newly connected client
                    basePacket clientSync = new connectPacket();
                    clientSync.packetType = basePacket._packetType.connectPacket;
                    foreach (clientClass client in clients) {
                        if (client.id == clientSoc.id) { continue; }
                        clientSync.id = client.id;
                        sendPacket(clientSoc, clientSync); //send connect packets to new client
                        sendPacket(clientSoc, client.PlayerPosData);//send tother players poses to new client
                    }
                }

                break;
            case basePacket._packetType.movePacket:
                {
                    //updates player position data on server side
                    thePacket.id = clientSoc.id;//give the curretn packet the sending clients id
                    clientSoc.PlayerPosData = (movePacket)thePacket.clone();

                }
                
                break;

            case basePacket._packetType.unknown:
            default:
                break;
        }
    }
}