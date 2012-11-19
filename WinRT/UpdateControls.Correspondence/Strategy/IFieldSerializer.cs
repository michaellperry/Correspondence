using System.IO;

namespace UpdateControls.Correspondence.Strategy
{
    public interface IFieldSerializer
    {
        object ReadData(BinaryReader input);
        void WriteData(BinaryWriter output, object value);
    }
}
