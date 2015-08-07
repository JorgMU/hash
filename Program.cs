using System;
using System.IO;
using System.Security.Cryptography;

namespace hash
{
  class Program
  {

    static void Main(string[] args)
    {
      if (args.Length < 1)
      {
        Console.Error.WriteLine("The file name or pattern to be prossed pust be passed on the commandline.");
        return;
      }

      string exe = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

      string algo = exe.ToUpper();
      if (algo.Contains("SHA1")) algo = "SHA1";
      else if (algo.Contains("SHA256")) algo = "SHA256";
      else if (algo.Contains("MD5")) algo = "MD5";
      else
      {
        Console.WriteLine("Rename {0} so that is contains SHA1, SHA256, or MD5 to select algorithm.", exe);
        return;
      }

      string dir = Path.GetDirectoryName(args[0]);
      string fn = Path.GetFileName(args[0]);

      if (dir == "") dir = Environment.CurrentDirectory;
      if (fn == "") fn = ".";
      if (fn == "..") { dir = Path.Combine(dir, ".."); fn = "."; }

      string[] files = null;

      files = Directory.GetFiles(dir, fn);

      if (files.Length == 0)
      {
        Console.WriteLine("{0}: No files match: \"{1}\"", algo, args[0]);
        return;
      }
      else
      {
        foreach (string file in files)
          if (Path.GetDirectoryName(file) != Environment.CurrentDirectory)
            Console.WriteLine("{0}: {1} \"{2}\"", algo, ComputeHash(algo, file), file);
          else
            Console.WriteLine("{0}: {1} \"{2}\"", algo, ComputeHash(algo, file), Path.GetFileName(file));
      }
    }

    private static string ComputeHash(string Algorithm, string FilePath)
    {
      switch (Algorithm)
      {
        case "SHA1": return GetSHA1Hash(FilePath);
        case "SHA256": return GetSHA256Hash(FilePath);
        case "MD5": return GetMD5Hash(FilePath);
        default: throw new SystemException("Invalid algroithm: " + Algorithm);
      }
    }


    private static string GetSHA1Hash(string FilePath)
    {
      string hash = "";
      byte[] hashBytes;
      FileStream fs = null;
      SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();

      try
      {
        fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        hashBytes = hasher.ComputeHash(fs);
        hash = BitConverter.ToString(hashBytes);
      }
      catch (SystemException se) { Error(se); }
      finally { if (fs != null) fs.Close(); }

      return hash.Replace("-", "");
    }

    private static string GetSHA256Hash(string FilePath)
    {
      string hash = "";
      byte[] hashBytes;
      FileStream fs = null;
      SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();

      try
      {
        fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        hashBytes = hasher.ComputeHash(fs);
        hash = BitConverter.ToString(hashBytes);
      }
      catch (SystemException se) { Error(se); }
      finally { if (fs != null) fs.Close(); }

      return hash.Replace("-", "");
    }

    private static string GetMD5Hash(string FilePath)
    {
      string hash = "";
      byte[] hashBytes;
      FileStream fs = null;
      MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();

      try
      {
        fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        hashBytes = hasher.ComputeHash(fs);
        hash = BitConverter.ToString(hashBytes);
      }
      catch (SystemException se) { Error(se); }
      finally { if (fs != null) fs.Close(); }

      return hash.Replace("-", "");
    }

    private static void Error(SystemException se)
    {
      Console.Error.WriteLine(se.Message);
      if (se.InnerException != null) Console.Error.WriteLine(se.InnerException.Message);
    }
  }
}
