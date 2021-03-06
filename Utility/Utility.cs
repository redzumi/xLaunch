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
      string hash = launchFileData[1].Trim();

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
      launchFile.hash = Utility.getFileHash(filename).Trim();

      localFiles.Add(launchFile);
    }

    return localFiles;
  }

  public async static Task downloadFile(LaunchFile launchFile)
  {
    string fileUrl = Settings.BaseUrl + launchFile.filename;

    HttpClient client = new HttpClient();
    HttpResponseMessage response = await client.GetAsync(fileUrl);

    using (Stream stream = await response.Content.ReadAsStreamAsync())
    {
      FileInfo fileInfo = new FileInfo(launchFile.filename);
      fileInfo.Directory.Create();

      using (FileStream fileStream = fileInfo.OpenWrite())
      {
        await stream.CopyToAsync(fileStream);
      }
    }
  }

  public static void deleteFile(string filename)
  {
    File.Delete(filename);
  }

  public static string generateLockFile()
  {
    List<LaunchFile> localFiles = Utility.getLocalFiles();

    string lockFile = "";
    foreach (LaunchFile localFile in localFiles)
    {
      string fileLockString = localFile.filename + ":" + localFile.hash + ";";
      lockFile += fileLockString;
    }

    lockFile = lockFile.Remove(lockFile.Length - 1, 1);

    string lockFileDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
    File.WriteAllText(lockFileDir + Settings.LockFile, lockFile);

    return lockFile;
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

  public static void checkBaseDirExist() {
    Directory.CreateDirectory(Settings.BaseDir);
  }
}