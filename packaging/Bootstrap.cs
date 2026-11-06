using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

namespace BroPad.Launcher {
 public static class Payload {
  public static string SafePath(string root,string entry){string basePath=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;string path=Path.GetFullPath(Path.Combine(basePath,entry));if(Path.IsPathRooted(entry)||entry.Contains(":")||!path.StartsWith(basePath,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Invalid package path.");return path;}
  public static string Hash(Stream data){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(data)).Replace("-","").ToLowerInvariant();}
  public static MemoryStream Open(){using(var resource=typeof(Payload).Assembly.GetManifestResourceStream("BroPad.Payload")){if(resource==null)throw new InvalidDataException("BroPad package is missing.");var bytes=new MemoryStream();resource.CopyTo(bytes);bytes.Position=0;if(Hash(bytes)!=BuildInfo.PayloadHash){bytes.Dispose();throw new InvalidDataException("BroPad package verification failed.");}bytes.Position=0;return bytes;}}
  public static bool VerifyFiles(ZipArchive zip,string root){foreach(var entry in zip.Entries){string target=SafePath(root,entry.FullName);if(string.IsNullOrEmpty(entry.Name))continue;if(!File.Exists(target))return false;using(var expected=entry.Open())using(var actual=File.OpenRead(target))if(Hash(expected)!=Hash(actual))return false;}return true;}
  public static void ValidatePackage(ZipArchive zip){if(zip.GetEntry("BroPad.App.exe")==null||zip.GetEntry("BroPad.App.exe.config")==null)throw new InvalidDataException("Application files are missing.");foreach(var entry in zip.Entries)SafePath(Path.Combine(Path.GetTempPath(),"BroPad-Package-Check"),entry.FullName);var driver=zip.GetEntry("dependencies/VBCABLE_Driver_Pack46.zip");if(driver==null)throw new InvalidDataException("Voice chat setup is missing.");using(var source=driver.Open())if(Hash(source)!=BuildInfo.DriverHash)throw new InvalidDataException("Voice chat setup verification failed.");}
  public static string EnsureRuntime(ZipArchive zip,string dataRoot){string runtime=Path.Combine(dataRoot,"runtime",BuildInfo.PayloadHash.Substring(0,16));Directory.CreateDirectory(Path.GetDirectoryName(runtime));if(Directory.Exists(runtime)&&VerifyFiles(zip,runtime))return runtime;
   string stage=runtime+".stage-"+Guid.NewGuid().ToString("N");Directory.CreateDirectory(stage);
   foreach(var entry in zip.Entries){string target=SafePath(stage,entry.FullName);if(string.IsNullOrEmpty(entry.Name))Directory.CreateDirectory(target);else{Directory.CreateDirectory(Path.GetDirectoryName(target));entry.ExtractToFile(target);}}
   if(!VerifyFiles(zip,stage))throw new InvalidDataException("BroPad could not prepare its application files.");
   // Keep a damaged cache for inspection; never remove libraries or user sounds.
   if(Directory.Exists(runtime))Directory.Move(runtime,runtime+".old-"+Guid.NewGuid().ToString("N"));Directory.Move(stage,runtime);return runtime;
  }
 }
 static class Bootstrap {
  [STAThread] static int Main(string[] args){try{
   if(!Environment.Is64BitOperatingSystem)throw new PlatformNotSupportedException("BroPad requires 64-bit Windows 10 or 11.");
   using(var payload=Payload.Open())using(var zip=new ZipArchive(payload,ZipArchiveMode.Read)){
    Payload.ValidatePackage(zip);
    if(args.Length>0&&args[0]=="--verify-package"){if(args.Length>1)File.WriteAllText(args[1],"PASS: embedded package SHA-256, safe archive paths, application/config, bundled original driver SHA-256.\r\nVersion: "+BuildInfo.Version);return 0;}
    using(var mutex=new Mutex(false,"Local\\BroPad.Package")){if(!mutex.WaitOne(TimeSpan.FromSeconds(30)))throw new IOException("Another BroPad launch is still preparing its files. Try again in a moment.");string runtime;try{runtime=Payload.EnsureRuntime(zip,Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"BroPad"));}finally{mutex.ReleaseMutex();}
     Process.Start(new ProcessStartInfo(Path.Combine(runtime,"BroPad.App.exe")){UseShellExecute=false,WorkingDirectory=runtime});
    }
   }return 0;
  }catch(Exception e){if(args.Length>0&&args[0]=="--verify-package"){if(args.Length>1)File.WriteAllText(args[1],"FAIL: "+e.Message);return 1;}MessageBox.Show("BroPad could not start.\n\n"+e.Message,"BroPad",MessageBoxButtons.OK,MessageBoxIcon.Error);return 1;}}
 }
}
