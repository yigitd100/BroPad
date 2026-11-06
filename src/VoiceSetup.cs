using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace BroPad {
 public static class AudioRouting {
  public static bool VirtualCableName(string name){return name.IndexOf("VB-Audio Virtual Cable",StringComparison.OrdinalIgnoreCase)>=0;}
  public static bool StereoCableName(string name){return VirtualCableName(name)&&name.Replace(" ","").IndexOf("16ch",StringComparison.OrdinalIgnoreCase)<0;}
  public static bool HasVoicePair(){using(var en=new MMDeviceEnumerator()){
   bool render=false,capture=false;
   foreach(var d in en.EnumerateAudioEndPoints(DataFlow.Render,DeviceState.Active))using(d)render|=StereoCableName(d.FriendlyName);
   foreach(var d in en.EnumerateAudioEndPoints(DataFlow.Capture,DeviceState.Active))using(d)capture|=StereoCableName(d.FriendlyName);
   return render&&capture;
  }}
 }
 public static class DriverPackage {
  public const string ArchiveHash="bc44e8974191a5a2427d6c979e2dab08dbb09925f3f435dfa2f0ba3e45b6eb90";
  public const string InstallerHash="734c35dfa6d98f48782a451633ceb471166ec70d60482fd89a1123d0ee3c4f41";
  public static string Hash(string file){using(var sha=SHA256.Create())using(var stream=File.OpenRead(file))return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLowerInvariant();}
  public static string SafePath(string root,string entry){string basePath=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;string result=Path.GetFullPath(Path.Combine(basePath,entry));if(Path.IsPathRooted(entry)||entry.Contains(":")||!result.StartsWith(basePath,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Invalid archive path.");return result;}
  public static void Extract(string archive,string root){Directory.CreateDirectory(root);using(var zip=ZipFile.OpenRead(archive))foreach(var entry in zip.Entries){string target=SafePath(root,entry.FullName);if(string.IsNullOrEmpty(entry.Name))Directory.CreateDirectory(target);else{Directory.CreateDirectory(Path.GetDirectoryName(target));entry.ExtractToFile(target,true);}}}
  public static string Prepare(){
   string archive=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"dependencies","VBCABLE_Driver_Pack46.zip");
   if(!File.Exists(archive)||Hash(archive)!=ArchiveHash)throw new InvalidDataException(L.T("Paket doğrulanamadı. BroPad'i yeniden indir."));
   string stage=Path.Combine(Store.Root,"setup",Guid.NewGuid().ToString("N"));Extract(archive,stage);
   string installer=Path.Combine(stage,"VBCABLE_Setup_x64.exe");
   if(Hash(installer)!=InstallerHash||!Authenticode.IsTrusted(installer))throw new InvalidDataException(L.T("Sürücünün dijital imzası doğrulanamadı."));
   return installer;
  }
  public static int Install(){
   // An existing working cable belongs to the user and is never reinstalled.
   if(AudioRouting.HasVoicePair())return 0;
   string installer=Prepare();
   var start=new ProcessStartInfo(installer,"-i -h"){UseShellExecute=true,Verb="runas",WorkingDirectory=Path.GetDirectoryName(installer),WindowStyle=ProcessWindowStyle.Normal};
   using(var process=Process.Start(start)){process.WaitForExit();return process.ExitCode;}
  }
 }
 public static class Authenticode {
  [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)] struct FileInfo {public uint Size;[MarshalAs(UnmanagedType.LPWStr)]public string Path;public IntPtr File,Subject;}
  [StructLayout(LayoutKind.Sequential)] struct TrustData {public uint Size;public IntPtr Callback,Sip;public uint UI,Revocation,Choice;public IntPtr File;public uint StateAction;public IntPtr State,Url;public uint Flags,Context;}
  [DllImport("wintrust.dll",ExactSpelling=true,CharSet=CharSet.Unicode)] static extern int WinVerifyTrust(IntPtr window,[In] ref Guid action,[In] ref TrustData data);
  public static bool IsTrusted(string path){var file=new FileInfo{Size=(uint)Marshal.SizeOf(typeof(FileInfo)),Path=path};IntPtr memory=Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FileInfo)));try{Marshal.StructureToPtr(file,memory,false);var data=new TrustData{Size=(uint)Marshal.SizeOf(typeof(TrustData)),UI=2,Choice=1,File=memory,Flags=0x1000};var action=new Guid("00AAC56B-CD44-11d0-8CC2-00C04FC295EE");return WinVerifyTrust(new IntPtr(-1),ref action,ref data)==0;}finally{Marshal.DestroyStructure(memory,typeof(FileInfo));Marshal.FreeHGlobal(memory);}}
 }
 public class VoiceSetupForm : DpiForm {
  Label description;Button install,copy;bool installing;
  public VoiceSetupForm(){Text=L.T("Sesli sohbet kurulumu");ClientSize=new Size(540,410);BackColor=Main.Bg;ForeColor=Color.White;StartPosition=FormStartPosition.CenterParent;FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;
   description=new Label{Location=new Point(24,22),Size=new Size(490,198),Text=AudioRouting.HasVoicePair()?L.T("Sesli sohbet hazır.\n\nDiscord veya oyunun mikrofon ayarında CABLE Output seç. BroPad'de Sesli sohbet / oyunlar modunu kullan.\n\nKendi sesin de iletilsin istiyorsan fiziksel mikrofonunu seçip Mikrofonumu da karıştır seçeneğini aç."):L.T("Tek seferlik hazırlık\n\nGerekli imzalı VB-CABLE sürücüsü BroPad'in içinde. Ayrı bir indirme gerekmez.\n\nEtkinleştir'e bastığında Windows yönetici onayı ister. Kurulumdan sonra Windows'u yeniden başlatman gerekebilir. Yeniden başlatma otomatik yapılmaz.")};Controls.Add(description);
   install=Main.Button(L.T("Etkinleştir"),24,232,240);install.Enabled=!AudioRouting.HasVoicePair();install.Click+=async delegate{installing=true;install.Enabled=false;description.Text=L.T("Windows onayını tamamla. Sesli sohbet hazırlanıyor…");try{int code=await Task.Run((Func<int>)DriverPackage.Install);for(int i=0;i<15&&!AudioRouting.HasVoicePair();i++)await Task.Delay(1000);if(IsDisposed)return;description.Text=AudioRouting.HasVoicePair()?L.T("Hazır! Discord / oyunda mikrofon olarak CABLE Output seç."):L.T("Kurulum tamamlandı fakat aygıt henüz görünmüyor. Windows'u yeniden başlatıp BroPad'i tekrar aç.")+"\n"+L.T("Kurulum kodu: ")+code;copy.Enabled=AudioRouting.HasVoicePair();}catch(Win32Exception e){description.Text=e.NativeErrorCode==1223?L.T("Kurulum iptal edildi. Yerel ses çalma kullanılabilir."):L.T("Kurulum açılamadı: ")+e.Message;}catch(Exception e){description.Text=L.T("Kurulum açılamadı: ")+e.Message;}finally{installing=false;if(!IsDisposed)install.Enabled=!AudioRouting.HasVoicePair();}};Controls.Add(install);
   copy=Main.Button(L.T("Mikrofon adını kopyala"),280,232,234);copy.Enabled=AudioRouting.HasVoicePair();copy.Click+=delegate{Clipboard.SetText("CABLE Output");};Controls.Add(copy);
   var attribution=new Label{Text="VB-CABLE by VB-Audio · Donationware",Location=new Point(24,294),Size=new Size(490,25),ForeColor=Main.Muted};Controls.Add(attribution);
   var source=new LinkLabel{Text=L.T("Resmi site ve bağış / lisans"),Location=new Point(24,324),Size=new Size(490,28),LinkColor=Color.FromArgb(169,148,255)};source.LinkClicked+=delegate{Process.Start(new ProcessStartInfo("https://vb-audio.com/Cable/"){UseShellExecute=true});};Controls.Add(source);
   var terms=new Label{Text=L.T("Paket değiştirilmeden sunulur. VB-CABLE kendi lisansına tabidir."),Location=new Point(24,363),Size=new Size(490,36),ForeColor=Main.Muted};Controls.Add(terms);
   FormClosing+=delegate(object sender,FormClosingEventArgs e){if(installing)e.Cancel=true;};L.Apply(this);ResumeLayout(true);
  }
 }
}
