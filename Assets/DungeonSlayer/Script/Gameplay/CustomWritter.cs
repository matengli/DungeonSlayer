using Mirror;
using UnityEngine;

namespace DungeonSlayer.Script.Gameplay
{
    public static class CustomReadWriteFunctions 
    {
        public static void WriteMyType(this NetworkWriter writer, BuffBase.AddBuffInfo value)
        {
            writer.Write<BuffBase.BuffTime>(value.buffTime);
            writer.WriteString(value.model.name);
            writer.WriteInt(value.addStack);
        }

        public static BuffBase.AddBuffInfo ReadMyType(this NetworkReader reader)
        {
            BuffBase.AddBuffInfo addBuffInfo = new BuffBase.AddBuffInfo();
            addBuffInfo.buffTime = reader.Read<BuffBase.BuffTime>();
            addBuffInfo.model = Resources.Load<BuffModelBase>($"BuffModel/{reader.ReadString()}");
            addBuffInfo.addStack = reader.ReadInt();
            return addBuffInfo;
        }
    }
    
    public static class CustomReadWriteFunctions2
    {
        public static void WriteMyType(this NetworkWriter writer, BuffBase.BuffTime value)
        {
            writer.WriteBool(value.isLoop);
            writer.WriteFloat(value.timeElapsed);
            writer.WriteFloat(value.existTime);
        }

        public static BuffBase.BuffTime ReadMyType(this NetworkReader reader)
        {
            BuffBase.BuffTime addBuffInfo = new BuffBase.BuffTime();
            addBuffInfo.isLoop = reader.ReadBool();
            addBuffInfo.timeElapsed = reader.ReadFloat();
            addBuffInfo.existTime = reader.ReadFloat();
            
            return addBuffInfo;
        }
    }
}