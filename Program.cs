using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xLaunch
{
  class Program
  {
    static async Task Main(string[] args)
    {
      List<LaunchFile> localFiles = Utility.getLocalFiles();
      foreach (LaunchFile localFile in localFiles)
      {
        Console.WriteLine(localFile.filename + " : " + localFile.hash);
      }

      List<LaunchFile> serverFiles = await Utility.getServerFiles();
      foreach (LaunchFile serverFile in serverFiles)
      {
        Console.WriteLine(serverFile.filename + " : " + serverFile.hash);
      }
    }
  }
}
