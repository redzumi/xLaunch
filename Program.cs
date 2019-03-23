using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xLaunch
{
  class Program
  {
    static async Task Main(string[] args)
    {
      List<LaunchFile> localFiles = Utility.getLocalFiles();
      List<LaunchFile> serverFiles = await Utility.getServerFiles();

      List<LaunchFile> extraFiles = CompareUtility.getExtraFiles(localFiles, serverFiles);
      List<LaunchFile> wrongFiles = CompareUtility.getWrongFiles(localFiles, serverFiles);
      List<LaunchFile> missingFiles = CompareUtility.getMissingFiles(localFiles, serverFiles);

      Console.WriteLine("=== extra files (delete) ===");
      foreach (LaunchFile extraFile in extraFiles)
      {
        Console.WriteLine(extraFile.filename + " : " + extraFile.hash);
        Utility.deleteFile(extraFile.filename);
      }

      Console.WriteLine("=== missing files (download) ===");
      foreach (LaunchFile missingFile in missingFiles)
      {
        Console.WriteLine(missingFile.filename + " : " + missingFile.hash);
        await Utility.downloadFile(missingFile);
      }

      Console.WriteLine("=== wrong files (replace) ===");
      foreach (LaunchFile wrongFile in wrongFiles)
      {
        Console.WriteLine(wrongFile.filename + " : " + wrongFile.hash);
        Utility.deleteFile(wrongFile.filename);
        await Utility.downloadFile(wrongFile);
      }

      if (args.Length > 0)
      {
        if (args[0] == "--generate-lock" || args[0] == "-g")
        {
          Console.WriteLine("=== lock file ===");
          string lockFile = Utility.generateLockFile();
          Console.WriteLine(lockFile);
        }
      }
    }
  }
}
