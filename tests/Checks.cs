using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using BroPad;

class Checks {
 static void Check(bool condition,string message){if(!condition)throw new Exception(message);}
 static Control Find(Control root,string name){if(root.Name==name)return root;foreach(Control child in root.Controls){var found=Find(child,name);if(found!=null)return found;}return null;}
 static bool TextExists(Control root,string text){return root.Text==text||root.Controls.Cast<Control>().Any(c=>TextExists(c,text));}
 [STAThread] static int Main(){try{Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
  string root=Path.Combine(Path.GetTempPath(),"BroPad-Tests-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(Path.Combine(root,"sounds"));typeof(Store).GetField("Root").SetValue(null,root);
  File.WriteAllText(Store.Config,"{\"Clips\":[],\"Volume\":0}");
  Check(Store.Load().Language=="en","Old preferences must default to English.");
  using(var form=new BroPad.Main(delegate{return false;})){
   var mode=(ComboBox)Find(form,"PlaybackMode");Check(mode.SelectedIndex==0,"Default mode must play locally.");
   Check(TextExists(form,"＋  Add sounds"),"English UI is missing.");
   mode.SelectedIndex=1;
   var statusField=typeof(BroPad.Main).GetField("status",BindingFlags.NonPublic|BindingFlags.Instance);
   string message=((Label)statusField.GetValue(form)).Text;
   Check(message.Contains("playing on your device")||message.Contains("Could not start output"),"Missing driver must give local fallback or a real audio-device error.");
   Check(!((CheckBox)typeof(BroPad.Main).GetField("micOn",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form)).Enabled,"Physical microphone must not feed local playback.");
   Check(Find(form,"VoiceSetupButton").Text=="Enable voice chat","Missing driver needs an in-app setup action.");
   typeof(BroPad.Main).GetMethod("SeedSamples",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(form,null);
   var seeded=Store.Load();Check(seeded.Clips.Count==3&&seeded.Clips.All(c=>File.Exists(c.File)),"Bundled samples must be copied into the user's library.");
   var language=(ComboBox)Find(form,"LanguageSelector");language.SelectedIndex=1;Check(TextExists(form,"＋  Ses ekle")&&Store.Load().Language=="tr","Turkish live translation and persistence.");
  }
  L.Language="en";
  using(var form=new BroPad.Main(delegate{return false;})){
   Check(form.Text=="BroPad · Ses paneli","Saved language must restore.");((ComboBox)Find(form,"LanguageSelector")).SelectedIndex=0;
  }
  Console.WriteLine("PASS: English default, language persistence, driver-missing mode, local fallback, microphone isolation, setup action.");
  foreach(int dpi in new[]{96,120,144,192})using(var form=new BroPad.Main(delegate{return false;})){
   form.CreateControl();form.PerformAutoScale();float scale=dpi/(float)form.DeviceDpi;form.Scale(new SizeF(scale,scale));form.ClientSize=new Size(Math.Min(1920,960*dpi/96),Math.Min(1000,540*dpi/96));form.PerformLayout();
   var library=Find(form,"Library");var header=Find(form,"Header");var footer=Find(form,"Footer");var grid=library.Controls.OfType<FlowLayoutPanel>().Single();
   Check(grid.Top>=header.Bottom&&grid.Bottom<=footer.Top,"Layout overlaps at "+dpi+" DPI.");Check(footer.Bottom<=library.ClientSize.Height,"Footer is clipped.");
   foreach(Control control in header.Controls)Check(control.Left>=-1&&control.Right<=header.ClientSize.Width+2,"Header is clipped: "+control.Text);
   Check(((Panel)Find(form,"Sidebar")).AutoScroll,"Sidebar must scroll on a small screen.");
  }
  Console.WriteLine("PASS: compact layout geometry at simulated 100/125/150/200 percent scaling.");
  foreach(string bad in new[]{"../escape.exe","..\\escape.exe","C:\\escape.exe","/escape.exe","x:stream"}){bool rejected=false;try{DriverPackage.SafePath(root,bad);}catch{rejected=true;}Check(rejected,"Archive traversal was accepted: "+bad);}
  string setup=DriverPackage.Prepare();Check(DriverPackage.Hash(setup)==DriverPackage.InstallerHash&&Authenticode.IsTrusted(setup),"Pinned original installer must have a trusted signature.");
  Check(!Authenticode.IsTrusted(Store.Config),"An unsigned file was accepted.");
  Console.WriteLine("PASS: archive path isolation, driver SHA-256 and Authenticode, rejection of unsigned files. No driver was installed.");
  return 0;
 }catch(Exception error){Console.WriteLine(error);return 1;}}
}
