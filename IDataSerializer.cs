namespace MppcDec
{
    public interface IDataSerializer
    {
        bool TryDeserialize(DataStream ds);
        DataStream Serialize(DataStream ds);
    }
}
