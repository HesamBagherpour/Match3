namespace HB.Match3.DataManagement
{
    public abstract class ServiceData
    {
        public abstract void Visit(IServiceDataVisitor visitor);

        public byte[] Serialize()
        {
            return Serialize(this);
        }

        private static byte[] Serialize(ServiceData msg)
        {
            return null;
            //return MessagePackSerializer.Serialize(msg);
        }

        public static ServiceData Deserialize(byte[] data)
        {
            return null;
            //return MessagePackSerializer.Deserialize<ServiceData>(data);
        }
    }
    
    public interface IServiceDataVisitor
    {
        //void Visit(DonationRequestCreatedServiceData data);
  
    }


}