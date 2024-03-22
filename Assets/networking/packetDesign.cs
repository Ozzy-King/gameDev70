using System.Collections.Generic;

public class basePacket
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
        if (typeData.Length < 2) { return null; }
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

                    if (!float.TryParse(allData["rotx"], out packet.rotx)) { return null; }
                    if (!float.TryParse(allData["roty"], out packet.roty)) { return null; }
                    if (!float.TryParse(allData["rotz"], out packet.rotz)) { return null; }

                    if (!float.TryParse(allData["rothx"], out packet.rothx)) { return null; }
                    if (!float.TryParse(allData["rothy"], out packet.rothy)) { return null; }
                    if (!float.TryParse(allData["rothz"], out packet.rothz)) { return null; }

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
            if (!newDataDic.ContainsKey(keyVal[0]))
            {
                newDataDic.Add(keyVal[0], keyVal[1]);
            }
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
                    movePacket movePacketInstance = this as movePacket;
                    int temp = (int)this.packetType;
                    dataString += temp.ToString() + ",";//add packet type
                    dataString += "\"id\":" + movePacketInstance.id.ToString() + ",";//add packet type

                    dataString += "\"posx\":" + movePacketInstance.posx.ToString() + ",";//add packet type
                    dataString += "\"posy\":" + movePacketInstance.posy.ToString() + ",";//add packet type
                    dataString += "\"posz\":" + movePacketInstance.posz.ToString() + ",";//add packet type

                    dataString += "\"rotx\":" + movePacketInstance.rotx.ToString() + ",";//add packet type
                    dataString += "\"roty\":" + movePacketInstance.roty.ToString() + ",";//add packet type
                    dataString += "\"rotz\":" + movePacketInstance.rotz.ToString() + ",";//add packet type

                    dataString += "\"rothx\":" + movePacketInstance.rothx.ToString() + ",";//add packet type
                    dataString += "\"rothy\":" + movePacketInstance.rothy.ToString() + ",";//add packet type
                    dataString += "\"rothz\":" + movePacketInstance.rothz.ToString() + ",";//add packet type


                    dataString += "\"runningBool\":" + movePacketInstance.runningBool.ToString() + ",";//add packet type
                    dataString += "\"moveingBool\":" + movePacketInstance.moveingBool.ToString() + ",";//add packet type
                    dataString += "\"reverseBool\":" + movePacketInstance.reverseBool.ToString() + ",";//add packet type


                    return dataString;
                }
            case _packetType.connectPacket:
                {
                    connectPacket connectPacketInstance = this as connectPacket;

                    int temp = (int)connectPacketInstance.packetType;
                    dataString += temp.ToString() + ",";//add packet type
                    dataString += "\"id\":" + connectPacketInstance.id.ToString() + ",";

                    return dataString;
                }

            case _packetType.unknown:
            default:
                return "";

        }
    }

    public basePacket clone() {
        basePacket packet;
        movePacket thisPacketInstance;
        switch (this.packetType)
        {
            case _packetType.movePacket:
                packet = new movePacket();
                {
                    thisPacketInstance = this as movePacket;
                    movePacket sendPacketInstance = packet as movePacket;

                    sendPacketInstance.id = thisPacketInstance.id;
                    sendPacketInstance.packetType = thisPacketInstance.packetType;
                    sendPacketInstance.posx = thisPacketInstance.posx;
                    sendPacketInstance.posy = thisPacketInstance.posy;
                    sendPacketInstance.posz = thisPacketInstance.posz;

                    sendPacketInstance.rotx = thisPacketInstance.rotx;
                    sendPacketInstance.roty = thisPacketInstance.roty;
                    sendPacketInstance.rotz = thisPacketInstance.rotz;

                    sendPacketInstance.rothx = thisPacketInstance.rothx;
                    sendPacketInstance.rothy = thisPacketInstance.rothy;
                    sendPacketInstance.rothz = thisPacketInstance.rothz;

                    sendPacketInstance.runningBool = thisPacketInstance.runningBool;
                    sendPacketInstance.reverseBool = thisPacketInstance.reverseBool;
                    sendPacketInstance.moveingBool = thisPacketInstance.moveingBool;
                }
                break;

            case _packetType.connectPacket:
                packet = new connectPacket();
                {
                    thisPacketInstance = this as movePacket;
                    connectPacket sendPacketInstance = packet as connectPacket;

                    sendPacketInstance.id = thisPacketInstance.id;
                    sendPacketInstance.packetType = thisPacketInstance.packetType;
                }
                break;
            case _packetType.unknown:
            default:
                return null;

        }
        return packet;

    } 

}

public class movePacket : basePacket
{

    public float posx = 0;
    public float posy = 0;
    public float posz = 0;

    public float rotx = 0;
    public float roty = 0;
    public float rotz = 0;

    public float rothx = 0;
    public float rothy = 0;
    public float rothz = 0;

    public bool runningBool = false;
    public bool moveingBool = false;
    public bool reverseBool = false;
    public movePacket() {
        this.packetType = _packetType.movePacket;
    }
}
public class connectPacket : basePacket {

    public connectPacket() {
        this.packetType = _packetType.connectPacket;
    }
}