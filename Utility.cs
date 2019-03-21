using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using xLaunch;

internal class Utility
{
  static HttpClient client = new HttpClient();

  public async static Task<List<LaunchFile>> getServerFiles()
  {
    client.BaseAddress = new Uri(Settings.BaseUrl);

    List<LaunchFile> serverFiles = new List<LaunchFile>();
    HttpResponseMessage response = await client.GetAsync(Settings.LockFile);

    string data = await response.Content.ReadAsStringAsync();
    string[] launchFiles = data.Split(';'); // ofc we can use json, but too much

    foreach (string launchFile in launchFiles)
    {
      LaunchFile file = new LaunchFile();

      string[] launchFileData = launchFile.Split(':');
      string filename = launchFileData[0];
      string hash = launchFileData[1];

      file.filename = filename;
      file.hash = hash;

      serverFiles.Add(file);
    }

    return serverFiles;
  }

  public static List<LaunchFile> getLocalFiles()
  {
    List<LaunchFile> localFiles = new List<LaunchFile>();
    string[] files = Utility.getFilesInDirectory(Settings.BaseDir);

    foreach (string filename in files)
    {
      LaunchFile launchFile = new LaunchFile();

      launchFile.filename = filename;
      launchFile.hash = Utility.getFileHash(filename);

      localFiles.Add(launchFile);
    }

    return localFiles;
  }

  public static string getFileHash(string filename)
  {
    MD5 md5 = MD5.Create();

    using (FileStream stream = File.OpenRead(filename))
    {
      byte[] hash = md5.ComputeHash(stream);

      return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
  }

  public static string[] getFilesInDirectory(string directory)
  {
    return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
  }
}