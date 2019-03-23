using System;
using System.Collections.Generic;
using xLaunch;

internal class CompareUtility
{
  public static List<LaunchFile> getWrongFiles(List<LaunchFile> localFiles, List<LaunchFile> serverFiles)
  {
    List<LaunchFile> wrongFiles = new List<LaunchFile>();

    foreach (LaunchFile localFile in localFiles)
    {
      LaunchFile serverSignature = serverFiles.Find(x => x.filename == localFile.filename);

      if (serverSignature != null && serverSignature.hash != localFile.hash)
      {
        Console.WriteLine("==== found wrong file ====");
        Console.WriteLine("filename: " + serverSignature.filename);
        Console.WriteLine("server hash: " + serverSignature.hash);
        Console.WriteLine("local hash: " + localFile.hash);
        Console.WriteLine("==== found wrong file ====");
        wrongFiles.Add(serverSignature);
      }
    }

    return wrongFiles;
  }

  public static List<LaunchFile> getMissingFiles(List<LaunchFile> localFiles, List<LaunchFile> serverFiles)
  {
    List<LaunchFile> missingFiles = new List<LaunchFile>();

    foreach (LaunchFile serverFile in serverFiles)
    {
      LaunchFile localSignature = localFiles.Find(x => x.filename == serverFile.filename);

      if (localSignature == null)
      {
        missingFiles.Add(serverFile);
      }
    }

    return missingFiles;
  }

  public static List<LaunchFile> getExtraFiles(List<LaunchFile> localFiles, List<LaunchFile> serverFiles)
  {
    List<LaunchFile> extraFiles = new List<LaunchFile>();

    foreach (LaunchFile localFile in localFiles)
    {
      LaunchFile serverSignature = serverFiles.Find(x => x.filename == localFile.filename);

      if (serverSignature == null)
      {
        extraFiles.Add(localFile);
      }
    }

    return extraFiles;
  }
}