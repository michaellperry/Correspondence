using System.IO;

namespace Correspondence.Strategy
{
    public interface IFieldSerializer
    {
        object ReadData(BinaryReader input);
        void WriteData(BinaryWriter output, object value);
    }
}
