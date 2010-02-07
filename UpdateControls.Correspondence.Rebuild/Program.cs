using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Rebuild
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Confirm the arguments.
                if (args.Length != 2)
                {
                    Usage();
                    return;
                }

                string sourceFilename = args[0];
                string destinationFilename = args[1];
                sourceFilename = GetAbsolutePath(sourceFilename);
                destinationFilename = GetAbsolutePath(destinationFilename);

                if (!File.Exists(sourceFilename))
                {
                    Console.WriteLine(string.Format("The source file {0} does not exist.", sourceFilename));
                    Console.WriteLine();
                    Usage();
                    return;
                }

                CreateCorrespondenceDatabase(destinationFilename);

                // Create the storage strategies.
                IStorageStrategy source = CreateStorageStrategy(sourceFilename);
                IStorageStrategy destination = CreateStorageStrategy(destinationFilename);
                int peerId = destination.SavePeer("SSCE", sourceFilename);

                CopyFacts(source, destination, peerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine();
                Usage();
            }
        }

        private static void CreateCorrespondenceDatabase(string databaseFilename)
        {
            if (!File.Exists(databaseFilename))
            {
                using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    typeof(Program), "Correspondence.sdf"))
                {
                    using (Stream fileStream = File.Create(databaseFilename))
                    {
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            int read = resourceStream.Read(buffer, 0, buffer.Length);
                            if (read <= 0)
                                break;
                            fileStream.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }

        private static IStorageStrategy CreateStorageStrategy(string filename)
        {
            return new SSCE.SSCEStorageStrategy(string.Format(@"DataSource=""{0}""", filename));
        }

        private static void CopyFacts(IStorageStrategy source, IStorageStrategy destination, int peerId)
        {
            foreach (IdentifiedFactMemento sourceFact in source.LoadAllFacts())
            {
                FactMemento destinationFact = new FactMemento(
                    new CorrespondenceFactType(
                        sourceFact.Memento.FactType.TypeName,
                        sourceFact.Memento.FactType.Version));
                destinationFact.Data = sourceFact.Memento.Data;
                destinationFact.AddPredecessors(sourceFact.Memento.Predecessors.Select
                    (p => new PredecessorMemento(
                        p.Role,
                        destination.GetFactIDFromShare(peerId, p.ID))));
                FactID destinationFactId;
                destination.Save(destinationFact, out destinationFactId);
                destination.SaveShare(peerId, sourceFact.Id, destinationFactId);
            }

            foreach (NamedFactMemento namedFact in source.LoadAllNamedFacts())
            {
                destination.SetID(namedFact.Name, destination.GetFactIDFromShare(peerId, namedFact.FactId));
            }
        }

        private static string GetAbsolutePath(string relativePath)
        {
            try
            {
                return Path.GetFullPath(relativePath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in path {0}: {1}", relativePath, ex.Message), ex);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  UpdateControls.Correspondence.Rebuild.exe <source> <destination>");
            Console.WriteLine();
            Console.WriteLine("  Read objects from <source> and write to <destination>.");
        }
    }
}
