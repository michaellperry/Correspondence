using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace UpdateControls.Correspondence.FileStream
{
    public class Table<TRecord>
    {
        private string _fileName;
        private List<TRecord> _records;
        private Func<BinaryReader, TRecord> _read;
        private Action<BinaryWriter, TRecord> _write;

        public Table(string fileName, Func<BinaryReader, TRecord> read, Action<BinaryWriter, TRecord> write)
        {
            _fileName = fileName;
            _read = read;
            _write = write;
        }

        public async Task LoadAsync()
        {
            if (_records != null)
                return;

            var correspondenceFolder = await ApplicationData.Current.LocalFolder
                .CreateFolderAsync("Correspondence", CreationCollisionOption.OpenIfExists);
            var tableFile = await correspondenceFolder
                .CreateFileAsync(_fileName, CreationCollisionOption.OpenIfExists);

            List<TRecord> records = new List<TRecord>();
            using (var stream = await tableFile.OpenStreamForReadAsync())
            {
                await Task.Run(delegate
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        while (reader.PeekChar() != -1)
                        {
                            TRecord record = _read(reader);
                            records.Add(record);
                        }
                    }
                });
            }
            _records = records;
        }

        public async Task SaveAsync(TRecord record)
        {
            var correspondenceFolder = await ApplicationData.Current.LocalFolder
                .GetFolderAsync("Correspondence");
            var tableFile = await correspondenceFolder
                .GetFileAsync(_fileName);

            using (var stream = await tableFile.OpenStreamForWriteAsync())
            {
                await Task.Run(delegate
                {
                    stream.Seek(0, SeekOrigin.End);
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        _write(writer, record);
                    }
                });
            }
        }

        public IEnumerable<TRecord> Records
        {
            get { return _records; }
        }
    }
}
