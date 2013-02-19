using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

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
            lock (this)
            {
                if (_records != null)
                    return;
            }

            List<TRecord> records = new List<TRecord>();

            var stream = await OpenTableFileForReadAsync();
            if (stream != null)
            {
                using (stream)
                {
                    await Task.Run(delegate
                    {
                        try
                        {
                            long length = stream.Length;
                            using (BinaryReader reader = new BinaryReader(stream))
                            {
                                while (stream.Position < length)
                                {
                                    TRecord record = _read(reader);
                                    records.Add(record);
                                }
                            }
                        }
                        catch (EndOfStreamException x)
                        {
                            // Done reading.
                        }
                    });
                }
            }

            lock (this)
            {
                _records = records;
            }
        }

        public async Task AppendAsync(TRecord record)
        {
            using (var stream = await OpenTableFileForWriteAsync())
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

            lock (this)
            {
                // Perform a non-mutating operation to
                // protect others using the collection.
                _records = new List<TRecord>(_records);
                _records.Add(record);
            }
        }

        public async Task ReplaceAsync(TRecord oldRecord, TRecord newRecord)
        {
            if (oldRecord == null)
                await AppendAsync(newRecord);

            List<TRecord> records;
            lock (this)
            {
                // Perform a non-mutating operation to
                // protect others using the collection.
                records = new List<TRecord>(_records);
                records.Remove(oldRecord);
                records.Add(newRecord);
            }

            using (var stream = await OpenTableFileForWriteAsync())
            {
                await Task.Run(delegate
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        foreach (var record in records)
                            _write(writer, record);
                    }
                });
            }

            lock (this)
            {
                _records = records;
            }
        }

        public IEnumerable<TRecord> Records
        {
            get
            {
                lock (this)
                {
                    return _records;
                }
            }
        }

        private Task<Stream> OpenTableFileForReadAsync()
        {
            return RetryAsync(async delegate
            {
                try
                {
                    var correspondenceFolder = await ApplicationData.Current.LocalFolder
                        .GetFolderAsync("Correspondence");
                    if (correspondenceFolder == null)
                        return null;

                    var tableFile = await correspondenceFolder.GetFileAsync(_fileName);
                    if (tableFile == null)
                        return null;

                    var stream = await tableFile.OpenStreamForReadAsync();
                    return stream;
                }
                catch (FileNotFoundException x)
                {
                    return null;
                }
            });
        }

        private Task<Stream> OpenTableFileForWriteAsync()
        {
            return RetryAsync(async delegate
            {
                var correspondenceFolder = await ApplicationData.Current.LocalFolder
                    .CreateFolderAsync("Correspondence", CreationCollisionOption.OpenIfExists);
                if (correspondenceFolder == null)
                    return null;

                var tableFile = await correspondenceFolder
                    .CreateFileAsync(_fileName, CreationCollisionOption.OpenIfExists);
                if (tableFile == null)
                    return null;

                var stream = await tableFile.OpenStreamForWriteAsync();
                return stream;
            });
        }

        private async Task<T> RetryAsync<T>(Func<Task<T>> function)
        {
            while (true)
            {
                try
                {
                    return await function();
                }
                catch (IOException x)
                {
                    // TODO: Tell the application to log this.
                }
                await Task.Delay(100);
            }
        }
    }
}
