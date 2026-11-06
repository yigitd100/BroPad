using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.CoreAudioApi;

namespace BroPad {
 public static class L {
  public static string Language="en";
  public static string Normalize(string value){return value=="tr"?"tr":"en";}
  static readonly Dictionary<string,string> English=new Dictionary<string,string> {
   {"Sesli sohbet kurulumu","Voice chat setup"},
   {"Tek seferlik hazırlık\n\nGerekli imzalı VB-CABLE sürücüsü BroPad'in içinde. Ayrı bir indirme gerekmez.\n\nEtkinleştir'e bastığında Windows yönetici onayı ister. Kurulumdan sonra Windows'u yeniden başlatman gerekebilir. Yeniden başlatma otomatik yapılmaz.","One-time setup\n\nThe required signed VB-CABLE driver is included with BroPad. No separate download is needed.\n\nClick Enable to approve the Windows administrator prompt. You may need to restart Windows afterwards. BroPad will never restart it automatically."},
   {"Sesli sohbet hazır.\n\nDiscord veya oyunun mikrofon ayarında CABLE Output seç. BroPad'de Sesli sohbet / oyunlar modunu kullan.\n\nKendi sesin de iletilsin istiyorsan fiziksel mikrofonunu seçip Mikrofonumu da karıştır seçeneğini aç.","Voice chat is ready.\n\nChoose CABLE Output as the microphone in Discord or your game. Use Voice chat / games mode in BroPad.\n\nTo include your voice, choose your physical microphone and enable Mix in my microphone."},
   {"Etkinleştir","Enable"},
   {"Windows onayını tamamla. Sesli sohbet hazırlanıyor…","Approve the Windows prompt. Preparing voice chat…"},
   {"Hazır! Discord / oyunda mikrofon olarak CABLE Output seç.","Ready! Choose CABLE Output as the microphone in Discord / your game."},
   {"Kurulum tamamlandı fakat aygıt henüz görünmüyor. Windows'u yeniden başlatıp BroPad'i tekrar aç.","Setup finished, but the device is not available yet. Restart Windows and reopen BroPad."},
   {"Kurulum kodu: ","Setup exit code: "},
   {"Kurulum iptal edildi. Yerel ses çalma kullanılabilir.","Setup was cancelled. Local playback is still available."},
   {"Kurulum açılamadı: ","Could not run setup: "},
   {"Mikrofon adını kopyala","Copy microphone name"},
   {"Resmi site ve bağış / lisans","Official website and donation / license"},
   {"Paket değiştirilmeden sunulur. VB-CABLE kendi lisansına tabidir.","Package distributed unchanged. VB-CABLE has its own license."},
   {"Paket doğrulanamadı. BroPad'i yeniden indir.","Package verification failed. Download BroPad again."},
   {"Sürücünün dijital imzası doğrulanamadı.","The driver digital signature could not be verified."},
   {"Sesleri şurada çal","Play sounds through"},
   {"Cihazımda dinle","On my device"},
   {"Sesli sohbet / oyunlar","Voice chat / games"},
   {"Sesli sohbeti etkinleştir","Enable voice chat"},
   {"Yerel ses hazır → ","Local playback ready → "},
   {"Sesli sohbet için bir kez kurulum yap. Sesler şimdilik cihazında çalıyor.","Enable voice chat once. Sounds are playing on your device for now."},
   {"Sesli sohbet hazır · Discord / oyunda mikrofonu CABLE Output seç.","Voice chat ready · Choose CABLE Output as your microphone in Discord / your game."},
   {"Ses ayarları","Audio settings"},
   {"Yeniden bağlan","Reconnect audio"},
   {"Yerel modda mikrofon karıştırılmaz.","Microphone mixing is only available in voice chat mode."},
   {"Kütüphaneden '{0}' kaldırılsın mı?","Remove '{0}' from your library?"},
   {"Tuş ata","Assign shortcut"},
   {"Yalnızca mono veya stereo ses dosyaları destekleniyor.","Only mono or stereo audio files are supported."},
   {"Önce çıkışı başlat.","Start the output first."},
   {"Sesi düzenle","Edit sound"},
   {"Ses adı","Sound name"},
   {"Tuş yakala","Capture shortcut"},
   {"Şimdi kısayola bas…","Press your shortcut now…"},
   {"Atamayı kaldır","Clear shortcut"},
   {"Bu sesin seviyesi","Sound volume"},
   {"Kaydet","Save"},
   {"F1–F24 veya Ctrl / Alt ile bir tuş.","F1–F24, or a key with Ctrl / Alt."},
   {"Ctrl / Alt kullan veya F tuşu seç.","Use Ctrl / Alt, or choose an F key."},
   {"BroPad · Ses paneli","BroPad · Soundboard"},
   {"Kütüphane okunamadı. Yedek dosyanı koru: ","Could not load the library. Keep your backup: "},
   {"SESİN, TEK TUŞ UZAĞINDA.","YOUR SOUND. ONE KEY AWAY."},
   {"＋  Ses ekle","＋  Add sounds"},
   {"SES YÖNLENDİRME","AUDIO ROUTING"},
   {"Gönderilecek çıkış","Output device"},
   {"Fiziksel mikrofon","Physical microphone"},
   {"Mikrofonumu da karıştır","Mix in my microphone"},
   {"Sesleri kulaklığımda da duy","Hear sounds in my headphones"},
   {"Çıkışı başlat","Start output"},
   {"Aygıtları yenile","Refresh devices"},
   {"Mikrofona nasıl gönderirim?","How to use as a microphone?"},
   {"Ses yönlendirme","Audio routing"},
   {"Kütüphanen","Your library"},
   {"Ses ara","Search sounds"},
   {"■  Tüm sesleri durdur","■  Stop all sounds"},
   {"ANA SES","VOLUME"},
   {"Çıkış kapalı · Bir aygıt seç ve başlat.","Output off · Choose a device and start."},
   {"Karta tıkla: çal  ·  Sağ tıkla: tuş ata / düzenle","Click a pad to play  ·  Right-click to edit / assign a shortcut"},
   {"Aygıtlar okunamadı: ","Could not list devices: "},
   {"Kaydedilemedi: ","Could not save: "},
   {"Çıkış kapalı · Ayarları uygulamak için başlat.","Output off · Start to apply your settings."},
   {"Çıkış aygıtı bulunamadı.","No output device found."},
   {"Çıkışı yeniden başlat","Restart output"},
   {"Hazır → ","Ready → "},
   {"Çıkış açılamadı: ","Could not start output: "},
   {"Mikrofon bulunamadı.","No microphone found."},
   {"Fiziksel mikrofon seç; CABLE Output geri besleme oluşturur.","Choose a physical microphone; CABLE Output would create feedback."},
   {"Mikrofon + sesler seçili çıkışa gönderiliyor.","Microphone + sounds are routed to the selected output."},
   {"Mikrofon: ","Microphone: "},
   {"Ses dosyalarını ekle","Add audio files"},
   {"Ses dosyaları|*.wav;*.mp3;*.aiff;*.aif;*.wma;*.m4a;*.aac|Tüm dosyalar|*.*","Audio files|*.wav;*.mp3;*.aiff;*.aif;*.wma;*.m4a;*.aac|All files|*.*"},
   {"Mono / stereo dosya seç.","Choose a mono / stereo file."},
   {" ses eklendi."," sound(s) added."},
   {"Eklenemeyen dosyalar","Files that could not be added"},
   {" ses  /  sınırsız kısayol"," sounds  /  unlimited shortcuts"},
   {"Düzenle / tuş ata","Edit / assign shortcut"},
   {"Kütüphaneden kaldır","Remove from library"},
   {"' kütüphaneden kaldırılsın mı?","' from your library?"},
   {"Sesi kaldır","Remove sound"},
   {"İlk sesini ekle.\n\nMP3, WAV veya M4A dosyalarını buraya sürükle.\nHer ses için sağ tıkla ve bir kısayol ata.","Add your first sound.\n\nDrop MP3, WAV or M4A files here.\nRight-click any pad to assign a shortcut."},
   {"Bu aramada ses bulunamadı.","No sounds match your search."},
   {"Bu kısayol zaten kullanılıyor. Değişiklikler kaydedilmedi.","This shortcut is already in use. Changes were not saved."},
   {"Çalındı: ","Played: "},
   {"Çalınamadı: ","Could not play: "},
   {"Tüm sesler durduruldu · Mikrofon açıksa iletim sürer.","All sounds stopped · Your microphone stays on if enabled."},
   {"Kısayol kullanımda: ","Shortcut already in use: "},
   {" · Durdurma kısayolu kullanılamıyor."," · Stop shortcut is unavailable."},
   {"BroPad zaten açık. Görev çubuğundan açabilirsin.","BroPad is already running. Open it from the taskbar."},
   {"Dil","Language"},
   {"1. VB-CABLE sanal ses sürücüsünü resmi sitesinden kur. Gerekirse Windows'u yeniden başlat.\n\n2. Gönderilecek çıkış: CABLE Input seç ve Çıkışı başlat'a bas.\n\n3. Discord / oyun mikrofonu: CABLE Output seç.\n\n4. Kendi sesin de gitsin istiyorsan fiziksel mikrofonunu seç ve Mikrofonumu da karıştır'ı aç.\n\n5. Kendin de duymak için kulaklık seçip dinlemeyi aç. CABLE Output'u fiziksel mikrofon olarak seçme; geri besleme yapar.\n\nSes efektleri için Discord'un gürültü bastırmasını kapatman gerekebilir.\n\nResmi site: https://vb-audio.com/Cable/","1. Install the VB-CABLE virtual audio driver from the official website. Restart Windows if needed.\n\n2. In BroPad, select CABLE Input as your output and click Start output.\n\n3. In Discord / your game, choose CABLE Output as the microphone.\n\n4. To include your voice, choose your physical microphone and enable Mix in my microphone.\n\n5. To hear the sounds yourself, choose your headphones and enable monitoring. Do not select CABLE Output as your physical microphone; this causes feedback.\n\nYou may need to turn off noise suppression in Discord for sound effects.\n\nOfficial website: https://vb-audio.com/Cable/"}
  };
  public static string T(string turkish){string english;return Language=="tr" || !English.TryGetValue(turkish,out english)?turkish:english;}
  public static void Apply(Control root){
   if(!(root is TextBox) && !(root is ComboBox)){
    string key=root.Tag as string;
    if(key==null){if(English.ContainsKey(root.Text))key=root.Text;else foreach(var pair in English)if(pair.Value==root.Text){key=pair.Key;break;}if(key!=null)root.Tag=key;}
    if(key!=null && English.ContainsKey(key))root.Text=T(key);
   }
   foreach(Control child in root.Controls)Apply(child);
  }
 }

 [DataContract] public class Clip {
  [DataMember] public string Name;
  [DataMember] public string File;
  [DataMember] public int Key;
  [DataMember] public int Mods;
  [DataMember] public float Gain = 0.8f;
  public string Shortcut { get { return Key == 0 ? L.T("Tuş ata") : ((Mods&2)!=0?"Ctrl + ":"")+((Mods&1)!=0?"Alt + ":"")+((Mods&4)!=0?"Shift + ":"")+((Keys)Key).ToString(); } }
 }
 [DataContract] public class Settings {
  [DataMember] public List<Clip> Clips = new List<Clip>();
  [DataMember] public string Output, Mic;
  [DataMember] public string Language="en";
  [DataMember] public string Mode="local";
  [DataMember] public int Volume = 75;
 }
 public static class Store {
  public static readonly string Root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BroPad");
  public static string Config { get { return Path.Combine(Root,"library.json"); } }
  public static Settings Load() {
   Directory.CreateDirectory(Path.Combine(Root,"sounds"));
   string legacy=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"KankaPad");
   string source=System.IO.File.Exists(Config)?Config:Path.Combine(legacy,"library.json");
   if(!System.IO.File.Exists(source))return new Settings();
   Settings settings;
   using(var stream=System.IO.File.OpenRead(source))settings=(Settings)new DataContractJsonSerializer(typeof(Settings)).ReadObject(stream);
   settings.Language=L.Normalize(settings.Language);
   if(settings.Clips==null)settings.Clips=new List<Clip>();
   if(source!=Config){
    foreach(var clip in settings.Clips){
     if(!string.IsNullOrEmpty(clip.File) && Path.GetFullPath(clip.File).StartsWith(legacy+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(clip.File)){
      string copy=Path.Combine(Root,"sounds",Guid.NewGuid()+Path.GetExtension(clip.File));System.IO.File.Copy(clip.File,copy);clip.File=copy;
     }
    }
    Save(settings);
   }
   return settings;
  }
  public static void Save(Settings s) {
   string tmp=Config+".tmp";
   using(var f=System.IO.File.Create(tmp)) new DataContractJsonSerializer(typeof(Settings)).WriteObject(f,s);
   if(System.IO.File.Exists(Config)) System.IO.File.Replace(tmp,Config,Config+".bak"); else System.IO.File.Move(tmp,Config);
  }
 }
 public class Limit : ISampleProvider {
  ISampleProvider source; public Limit(ISampleProvider s){source=s;}
  public WaveFormat WaveFormat {get{return source.WaveFormat;}}
  public int Read(float[] b,int o,int c){int n=source.Read(b,o,c);for(int i=o;i<o+n;i++)b[i]=Math.Max(-1,Math.Min(1,b[i]));return n;}
 }
 public class AudioBus : IDisposable {
  public readonly MixingSampleProvider Mixer=new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000,2)){ReadFully=true};
  public VolumeSampleProvider Master; WasapiOut player;
  readonly object gate=new object(); readonly Dictionary<ISampleProvider,AudioFileReader> files=new Dictionary<ISampleProvider,AudioFileReader>();
  public AudioBus(MMDevice device,float gain){Master=new VolumeSampleProvider(new Limit(Mixer)){Volume=gain};player=new WasapiOut(device,AudioClientShareMode.Shared,true,60);player.Init(Master); Mixer.MixerInputEnded+=delegate(object s,SampleProviderEventArgs e){ lock(gate){AudioFileReader r;if(files.TryGetValue(e.SampleProvider,out r)){files.Remove(e.SampleProvider);r.Dispose();}} };player.Play();}
  public static ISampleProvider Normalize(ISampleProvider p){if(p.WaveFormat.Channels==1)p=new MonoToStereoSampleProvider(p);if(p.WaveFormat.Channels!=2)throw new InvalidOperationException(L.T("Yalnızca mono veya stereo ses dosyaları destekleniyor."));if(p.WaveFormat.SampleRate!=48000)p=new WdlResamplingSampleProvider(p,48000);return p;}
  public void Play(string file,float gain){var r=new AudioFileReader(file);try{var p=new VolumeSampleProvider(Normalize(r)){Volume=gain};lock(gate){files.Add(p,r);}try{Mixer.AddMixerInput(p);}catch{lock(gate){files.Remove(p);}throw;}}catch{r.Dispose();throw;}}
  public void StopClips(){ISampleProvider[] keys;lock(gate){keys=files.Keys.ToArray();}foreach(var k in keys){Mixer.RemoveMixerInput(k);lock(gate){AudioFileReader r;if(files.TryGetValue(k,out r)){files.Remove(k);r.Dispose();}}}}
  public void Dispose(){player.Stop();player.Dispose();StopClips();Mixer.RemoveAllMixerInputs();}
 }
 public class Engine : IDisposable {
  public AudioBus Route,Monitor; WasapiCapture capture; ISampleProvider micProvider; BufferedWaveProvider buffer;
  public void Configure(MMDevice output,MMDevice listen,float volume){Dispose();try{Route=new AudioBus(output,volume);if(listen!=null && listen.ID!=output.ID)Monitor=new AudioBus(listen,volume);}catch{Dispose();throw;}}
  public void Play(Clip c){if(Route==null)throw new InvalidOperationException(L.T("Önce çıkışı başlat."));Route.Play(c.File,c.Gain);if(Monitor!=null)Monitor.Play(c.File,c.Gain);}
  public void Volume(float value){if(Route!=null)Route.Master.Volume=value;if(Monitor!=null)Monitor.Master.Volume=value;}
  public void Stop(){if(Route!=null)Route.StopClips();if(Monitor!=null)Monitor.StopClips();}
  public void MicOff(){if(capture!=null){capture.StopRecording();capture.Dispose();capture=null;}if(micProvider!=null && Route!=null)Route.Mixer.RemoveMixerInput(micProvider);micProvider=null;buffer=null;}
  public void MicOn(MMDevice device){if(Route==null)throw new InvalidOperationException(L.T("Önce çıkışı başlat."));MicOff();try{capture=new WasapiCapture(device);buffer=new BufferedWaveProvider(capture.WaveFormat){DiscardOnBufferOverflow=true,BufferDuration=TimeSpan.FromMilliseconds(300),ReadFully=true};micProvider=new VolumeSampleProvider(AudioBus.Normalize(buffer.ToSampleProvider())){Volume=0.8f};Route.Mixer.AddMixerInput(micProvider);capture.DataAvailable+=delegate(object s,WaveInEventArgs e){buffer.AddSamples(e.Buffer,0,e.BytesRecorded);};capture.StartRecording();}catch{MicOff();throw;}}
  public void Dispose(){MicOff();if(Route!=null){Route.Dispose();Route=null;}if(Monitor!=null){Monitor.Dispose();Monitor=null;}}
 }
 public class Pad : Control {
  public Clip Clip; Color accent; public bool Active; bool hover;
  public Pad(Clip c,int index){Clip=c;accent=new[]{Color.FromArgb(161,139,255),Color.FromArgb(91,216,180),Color.FromArgb(255,184,107),Color.FromArgb(111,181,255)}[index%4];Size=new Size(222,154);Margin=new Padding(0,0,14,14);Cursor=Cursors.Hand;DoubleBuffered=true;AccessibleName=c.Name+" — "+c.Shortcut;AccessibleRole=AccessibleRole.PushButton;TabStop=true;MouseEnter+=delegate{hover=true;Invalidate();};MouseLeave+=delegate{hover=false;Invalidate();};KeyDown+=delegate(object s,KeyEventArgs e){if(e.KeyCode==Keys.Enter)OnClick(EventArgs.Empty);};}
  int Px(int value){return (int)Math.Round(value*DeviceDpi/96f);}
  Rectangle Rect(int x,int y,int w,int h){return new Rectangle(Px(x),Px(y),Px(w),Px(h));}
  public void ApplyDpi(int dpi){float f=dpi/96f;Size=new Size((int)Math.Round(222*f),(int)Math.Round(154*f));Margin=new Padding(0,0,(int)Math.Round(14*f),(int)Math.Round(14*f));Invalidate();}
  protected override void OnPaint(PaintEventArgs e){
   base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.Clear(Parent.BackColor);
   using(var path=Round(new Rectangle(Px(1),Px(1),Width-Px(3),Height-Px(3)),Px(16))){
    using(var b=new SolidBrush(Active?Color.FromArgb(48,43,70):hover?Color.FromArgb(39,42,55):Color.FromArgb(29,32,43)))g.FillPath(b,path);
    using(var pen=new Pen(Active?accent:Color.FromArgb(48,52,66),Math.Max(1,DeviceDpi/96f)))g.DrawPath(pen,path);
   }
   using(var b=new SolidBrush(accent))g.FillEllipse(b,Rect(18,18,34,34));
   using(var b=new SolidBrush(Color.FromArgb(24,25,34)))g.FillPolygon(b,new[]{new Point(Px(32),Px(27)),new Point(Px(32),Px(43)),new Point(Px(43),Px(35))});
   using(var nameFont=new Font("Segoe UI",11,FontStyle.Bold))
    TextRenderer.DrawText(g,Clip.Name,nameFont,new Rectangle(Px(18),Px(68),Width-Px(32),Px(28)),Color.FromArgb(237,239,246),TextFormatFlags.EndEllipsis|TextFormatFlags.SingleLine);
   using(var detailFont=new Font("Segoe UI",8))
    TextRenderer.DrawText(g,Path.GetExtension(Clip.File).TrimStart('.').ToUpper()+"  ·  "+(int)(Clip.Gain*100)+"%",detailFont,new Point(Px(19),Px(101)),Color.FromArgb(133,142,163));
   using(var keyFont=new Font("Segoe UI",8,FontStyle.Bold))
    TextRenderer.DrawText(g,Clip.Shortcut,keyFont,new Rectangle(Px(19),Px(127),Width-Px(32),Px(23)),accent,TextFormatFlags.EndEllipsis|TextFormatFlags.SingleLine);
  }
  public static GraphicsPath Round(Rectangle r,int d){var p=new GraphicsPath();p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
 }
 public static class Brand {
  public static Icon LoadIcon(){using(var stream=typeof(Brand).Assembly.GetManifestResourceStream("BroPad.Icon"))using(var icon=new Icon(stream)){return (Icon)icon.Clone();}}
  public static Image LoadLogo(){using(var stream=typeof(Brand).Assembly.GetManifestResourceStream("BroPad.Logo"))using(var image=Image.FromStream(stream)){return new Bitmap(image);}}
 }
 public class DpiForm : Form {
  public DpiForm(){SuspendLayout();Icon=Brand.LoadIcon();Font=new Font("Segoe UI",10);AutoScaleDimensions=new SizeF(96,96);AutoScaleMode=AutoScaleMode.Dpi;}
 }
 public class EditClip : DpiForm {
  public TextBox TitleBox; public int Key,Mods; Label shortcut; TrackBar gain; public float Gain {get{return gain.Value/100f;}}
  public EditClip(Clip c){Text=L.T("Sesi düzenle");ClientSize=new Size(420,290);FormBorderStyle=FormBorderStyle.FixedDialog;StartPosition=FormStartPosition.CenterParent;MaximizeBox=false;MinimizeBox=false;BackColor=Main.Bg;ForeColor=Color.White;Font=new Font("Segoe UI",10);KeyPreview=true;Key=c.Key;Mods=c.Mods;
   Controls.Add(new Label{Text=L.T("Ses adı"),Location=new Point(22,18),AutoSize=true});TitleBox=new TextBox{Text=c.Name,Location=new Point(22,45),Width=375};Controls.Add(TitleBox);
   shortcut=new Label{Text=c.Shortcut,Location=new Point(22,88),Width=375};Controls.Add(shortcut);
   var assign=Main.Button(L.T("Tuş yakala"),22,119,170);assign.Click+=delegate{shortcut.Text=L.T("Şimdi kısayola bas…");assign.Focus();capturing=true;};Controls.Add(assign);
   var clear=Main.Button(L.T("Atamayı kaldır"),205,119,190);clear.Click+=delegate{Key=0;Mods=0;shortcut.Text=L.T("Tuş ata");};Controls.Add(clear);
   Controls.Add(new Label{Text=L.T("Bu sesin seviyesi"),Location=new Point(22,166),AutoSize=true});gain=new TrackBar{Minimum=0,Maximum=100,Value=Math.Max(0,Math.Min(100,(int)(c.Gain*100))),TickStyle=TickStyle.None,Location=new Point(175,158),Width=220};Controls.Add(gain);
   var save=Main.Button(L.T("Kaydet"),260,231,135);save.Click+=delegate{if(string.IsNullOrWhiteSpace(TitleBox.Text))return;DialogResult=DialogResult.OK;};Controls.Add(save);
   Controls.Add(new Label{Text=L.T("F1–F24 veya Ctrl / Alt ile bir tuş."),Location=new Point(22,207),AutoSize=true,ForeColor=Main.Muted});
   KeyDown+=delegate(object s,KeyEventArgs e){if(!capturing)return;e.SuppressKeyPress=true;if(e.KeyCode==Keys.ControlKey||e.KeyCode==Keys.Menu||e.KeyCode==Keys.ShiftKey)return;if(!(e.Control||e.Alt||(e.KeyCode>=Keys.F1&&e.KeyCode<=Keys.F24))){shortcut.Text=L.T("Ctrl / Alt kullan veya F tuşu seç.");return;}Key=(int)e.KeyCode;Mods=(e.Control?2:0)|(e.Alt?1:0)|(e.Shift?4:0);shortcut.Text=new Clip{Key=Key,Mods=Mods}.Shortcut;capturing=false;};
  L.Apply(this);ResumeLayout(true);
  } bool capturing;
 }
 public class ThemedCheckBox : CheckBox {
  public ThemedCheckBox(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
  protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.Clear(BackColor);int box=(int)Math.Round(14*DeviceDpi/96f),left=(int)Math.Round(2*DeviceDpi/96f),x=(int)Math.Round(24*DeviceDpi/96f),top=(Height-box)/2;Color color=Enabled?ForeColor:Main.Muted;using(var pen=new Pen(color,Math.Max(1,DeviceDpi/96f)))g.DrawRectangle(pen,left,top,box,box);if(Checked)using(var pen=new Pen(color,Math.Max(2,2*DeviceDpi/96f)))g.DrawLines(pen,new[]{new Point(left+box/5,top+box/2),new Point(left+box*2/5,top+box*4/5),new Point(left+box*4/5,top+box/5)});TextRenderer.DrawText(g,Text,Font,new Rectangle(x,0,Width-x,Height),color,TextFormatFlags.VerticalCenter|TextFormatFlags.SingleLine|TextFormatFlags.EndEllipsis);}
 }
 public class DeviceItem {public MMDevice Device;public override string ToString(){return Device.FriendlyName;}}
 public class Main : DpiForm {
  public static Color Bg=Color.FromArgb(18,20,28),Muted=Color.FromArgb(143,152,174);
  Settings settings;Engine engine=new Engine();MMDeviceEnumerator enumerator=new MMDeviceEnumerator();List<MMDevice> devices=new List<MMDevice>();
  ComboBox output,mic,monitor,language,mode;Button voiceSetup;bool routeInVoice;readonly Func<bool> voiceAvailable;bool resourcesReleased;CheckBox micOn,listen;TrackBar volume;Label status,count;FlowLayoutPanel pads;TextBox search;Button connect;Dictionary<int,Clip> hotkeys=new Dictionary<int,Clip>();HashSet<Clip> failed=new HashSet<Clip>();bool ready,loading=true,stopRegistered;int flash;Timer timer;
  [DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr h,int id,uint mods,uint key);
  [DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr h,int id);
  public static Button Button(string text,int x,int y,int width){var b=new Button{Text=text,Location=new Point(x,y),Size=new Size(width,36),FlatStyle=FlatStyle.Flat,BackColor=Color.FromArgb(42,45,61),ForeColor=Color.White,Cursor=Cursors.Hand};b.FlatAppearance.BorderSize=0;return b;}
  Label Label(string text,int x,int y,int size,Color color){var l=new Label{Text=text,Location=new Point(x,y),AutoSize=true,Font=new Font("Segoe UI",size,size>=16?FontStyle.Bold:FontStyle.Regular),ForeColor=color};Controls.Add(l);return l;}
  public Main():this(AudioRouting.HasVoicePair){}
  public Main(Func<bool> voiceAvailability){voiceAvailable=voiceAvailability??AudioRouting.HasVoicePair;Text=L.T("BroPad · Ses paneli");ClientSize=new Size(1160,780);MinimumSize=new Size(960,540);AutoScroll=true;AutoScrollMinSize=new Size(960,0);StartPosition=FormStartPosition.CenterScreen;BackColor=Bg;ForeColor=Color.White;Font=new Font("Segoe UI",10);AutoScaleMode=AutoScaleMode.Dpi;
   bool firstRun=!System.IO.File.Exists(Store.Config)&&!System.IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"KankaPad","library.json"));
   try{settings=Store.Load();if(settings.Clips==null)settings.Clips=new List<Clip>();}catch(Exception e){MessageBox.Show(L.T("Kütüphane okunamadı. Yedek dosyanı koru: ")+Store.Config+".bak\n"+e.Message);settings=new Settings();}
   L.Language=L.Normalize(settings.Language);if(firstRun)SeedSamples();
   var logo=new PictureBox{Image=Brand.LoadLogo(),Location=new Point(28,24),Size=new Size(40,40),SizeMode=PictureBoxSizeMode.Zoom,AccessibleName="BroPad logo",TabStop=false};logo.Disposed+=delegate{logo.Image.Dispose();};Controls.Add(logo);
   Label("BROPAD",79,25,20,Color.White);Label(L.T("SESİN, TEK TUŞ UZAĞINDA."),31,76,9,Muted);
   var add=Button(L.T("＋  Ses ekle"),28,115,205);add.BackColor=Color.FromArgb(126,101,226);add.Click+=delegate{AddFiles();};Controls.Add(add);
   Label(L.T("SES YÖNLENDİRME"),28,185,9,Muted);Label(L.T("Sesleri şurada çal"),28,217,10,Color.White);output=Combo(28,244,240);output.Visible=false;
   mode=Combo(28,244,240);mode.Name="PlaybackMode";mode.AccessibleName=L.T("Sesleri şurada çal");mode.Items.AddRange(new object[]{L.T("Cihazımda dinle"),L.T("Sesli sohbet / oyunlar")});mode.SelectedIndex=settings.Mode=="voice"?1:0;
   Label(L.T("Fiziksel mikrofon"),28,296,10,Color.White);mic=Combo(28,323,240);
   micOn=new ThemedCheckBox{Text=L.T("Mikrofonumu da karıştır"),Location=new Point(28,366),Width=245,Height=30};Controls.Add(micOn);
   listen=new ThemedCheckBox{Text=L.T("Sesleri kulaklığımda da duy"),Location=new Point(28,412),Width=245,Height=30};Controls.Add(listen);monitor=Combo(28,448,240);
   connect=Button(L.T("Çıkışı başlat"),28,502,240);connect.BackColor=Color.FromArgb(67,91,91);connect.Click+=delegate{Connect();};Controls.Add(connect);
   voiceSetup=Button(L.T("Sesli sohbeti etkinleştir"),28,547,240);voiceSetup.Name="VoiceSetupButton";voiceSetup.BackColor=Color.FromArgb(87,68,149);voiceSetup.Click+=delegate{using(var dialog=new VoiceSetupForm())dialog.ShowDialog(this);LoadDevices();if(voiceAvailable()){loading=true;mode.SelectedIndex=1;loading=false;settings.Mode="voice";}Connect();Save();};Controls.Add(voiceSetup);
   var refresh=Button(L.T("Aygıtları yenile"),28,600,240);refresh.Click+=delegate{Disconnect();LoadDevices();Connect();};Controls.Add(refresh);
   var help=Button(L.T("Ses ayarları"),28,646,240);help.Click+=delegate{using(var dialog=new VoiceSetupForm())dialog.ShowDialog(this);};Controls.Add(help);
   Label(L.T("Kütüphanen"),311,27,24,Color.White);count=Label("",312,73,10,Muted);
   search=new TextBox{Location=new Point(311,114),Width=470,BackColor=Color.FromArgb(30,33,45),ForeColor=Color.White,BorderStyle=BorderStyle.FixedSingle};search.TextChanged+=delegate{Render();};Controls.Add(search);Label(L.T("Ses ara"),794,116,9,Muted);
   var stop=Button(L.T("■  Tüm sesleri durdur"),876,32,250);stop.BackColor=Color.FromArgb(87,47,63);stop.Click+=delegate{Stop();};Controls.Add(stop);Label("Ctrl + Shift + Space",929,76,9,Muted);
   pads=new FlowLayoutPanel{Location=new Point(311,166),Size=new Size(820,430),BackColor=Bg,AutoScroll=true,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right|AnchorStyles.Bottom};Controls.Add(pads);
   Label(L.T("ANA SES"),312,640,9,Muted).Anchor=AnchorStyles.Left|AnchorStyles.Bottom;volume=new TrackBar{Location=new Point(398,630),Width=420,Minimum=0,Maximum=100,Value=Math.Max(0,Math.Min(100,settings.Volume)),TickStyle=TickStyle.None,Anchor=AnchorStyles.Left|AnchorStyles.Bottom};Controls.Add(volume);var level=Label(volume.Value+"%",830,640,11,Color.White);level.Anchor=AnchorStyles.Left|AnchorStyles.Bottom;volume.ValueChanged+=delegate{settings.Volume=volume.Value;engine.Volume(volume.Value/100f);level.Text=volume.Value+"%";Save();};
   status=Label(L.T("Çıkış kapalı · Bir aygıt seç ve başlat."),311,694,10,Muted);status.MaximumSize=new Size(810,60);status.Anchor=AnchorStyles.Left|AnchorStyles.Bottom;
   Label(L.T("Karta tıkla: çal  ·  Sağ tıkla: tuş ata / düzenle"),311,748,9,Muted).Anchor=AnchorStyles.Left|AnchorStyles.Bottom;
   output.SelectedIndexChanged+=delegate{if(!loading){settings.Output=Selected(output)==null?null:Selected(output).ID;Disconnect();Save();}};
   mic.SelectedIndexChanged+=delegate{if(!loading){settings.Mic=Selected(mic)==null?null:Selected(mic).ID;if(micOn.Checked)SetMic();Save();}};
   micOn.CheckedChanged+=delegate{if(!loading)SetMic();};listen.CheckedChanged+=delegate{if(!loading && ready)Connect();};monitor.SelectedIndexChanged+=delegate{if(!loading && ready)Connect();};
   Label(L.T("Dil"),28,700,10,Muted);language=Combo(28,728,240);language.Name="LanguageSelector";language.AccessibleName="Language / Dil";language.Items.AddRange(new object[]{"English","Türkçe"});language.SelectedIndex=L.Language=="tr"?1:0;
   language.SelectedIndexChanged+=delegate{if(loading)return;settings.Language=language.SelectedIndex==1?"tr":"en";L.Language=settings.Language;ApplyLanguage();Save();};
   mode.SelectedIndexChanged+=delegate{if(!loading){settings.Mode=mode.SelectedIndex==1?"voice":"local";Connect();Save();}};
   LoadDevices();loading=false;BuildResponsiveLayout();L.Apply(this);ResumeLayout(true);Render();Shown+=delegate{FitWorkingArea();Connect();RegisterAll();};DpiChanged+=delegate{BeginInvoke((Action)delegate{FitWorkingArea();Render();});};FormClosing+=delegate{UnregisterAll();Save();};
   AllowDrop=true;DragEnter+=delegate(object s,DragEventArgs e){if(e.Data.GetDataPresent(DataFormats.FileDrop))e.Effect=DragDropEffects.Copy;};DragDrop+=delegate(object s,DragEventArgs e){Import((string[])e.Data.GetData(DataFormats.FileDrop));};
   timer=new Timer{Interval=180};timer.Tick+=delegate{if(flash>0 && --flash==0)foreach(Pad p in pads.Controls.OfType<Pad>()){p.Active=false;p.Invalidate();}};timer.Start();
  }
  protected override void Dispose(bool disposing){if(disposing&&!resourcesReleased){resourcesReleased=true;if(timer!=null){timer.Stop();timer.Dispose();}engine.Dispose();foreach(var device in devices)device.Dispose();enumerator.Dispose();}base.Dispose(disposing);}
  void SeedSamples(){
   var examples=new[]{new[]{"Bildirim.wav","Notification"},new[]{"Seviye-atla.wav","Level up"},new[]{"Kisa-bip.wav","Short beep"}};
   for(int i=0;i<examples.Length;i++){string original=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Ornek-sesler",examples[i][0]);if(!System.IO.File.Exists(original))continue;string copy=Path.Combine(Store.Root,"sounds",Guid.NewGuid()+".wav");System.IO.File.Copy(original,copy);settings.Clips.Add(new Clip{Name=examples[i][1],File=copy,Key=(int)Keys.D1+i,Mods=3});}Store.Save(settings);
  }
  void ApplyLanguage(){
   SuspendLayout();L.Apply(this);search.AccessibleName=L.T("Ses ara");
   int selected=mode.SelectedIndex;loading=true;mode.Items.Clear();mode.Items.AddRange(new object[]{L.T("Cihazımda dinle"),L.T("Sesli sohbet / oyunlar")});mode.SelectedIndex=selected;loading=false;
   connect.Tag=ready?"Yeniden bağlan":"Çıkışı başlat";connect.Text=L.T((string)connect.Tag);UpdatePlaybackStatus();Render();ResumeLayout(true);
  }
  void UpdatePlaybackStatus(){
   bool pair=voiceAvailable();voiceSetup.Tag=pair?"Sesli sohbet kurulumu":"Sesli sohbeti etkinleştir";voiceSetup.Text=L.T((string)voiceSetup.Tag);status.Tag=null;
   micOn.Enabled=routeInVoice;listen.Enabled=mode.SelectedIndex==1;
   if(ready)status.Text=routeInVoice?L.T("Sesli sohbet hazır · Discord / oyunda mikrofonu CABLE Output seç."):(mode.SelectedIndex==1?L.T("Sesli sohbet için bir kez kurulum yap. Sesler şimdilik cihazında çalıyor."):L.T("Yerel ses hazır → ")+(Selected(monitor)==null?"":Selected(monitor).FriendlyName));
  }
  void FitWorkingArea(){var area=Screen.FromControl(this).WorkingArea;float f=DeviceDpi/96f;AutoScrollMinSize=new Size((int)(960*f),0);MinimumSize=new Size(Math.Min((int)(960*f),area.Width),Math.Min((int)(540*f),area.Height));if(WindowState==FormWindowState.Normal){Size=new Size(Math.Min(Width,area.Width),Math.Min(Height,area.Height));Location=new Point(Math.Max(area.Left,Math.Min(Left,area.Right-Width)),Math.Max(area.Top,Math.Min(Top,area.Bottom-Height)));}}
  void BuildResponsiveLayout(){
   var original=Controls.Cast<Control>().ToArray();
   var sidebar=new Panel{Dock=DockStyle.Left,Width=304,AutoScroll=true,BackColor=Bg,Name="Sidebar"};
   var content=new Panel{Size=new Size(874,780),Dock=DockStyle.Fill,Padding=new Padding(25,0,28,0),BackColor=Bg,Name="Library"};
   var header=new Panel{Size=new Size(821,166),Dock=DockStyle.Top,BackColor=Bg,Name="Header"};
   var footer=new Panel{Size=new Size(821,150),Dock=DockStyle.Bottom,BackColor=Bg,Name="Footer"};
   Controls.Clear();Controls.Add(content);Controls.Add(sidebar);
   content.Controls.Add(pads);content.Controls.Add(footer);content.Controls.Add(header);
   pads.Dock=DockStyle.Fill;
   foreach(var c in original){
    if(c==pads)continue;
    if(c.Left<300){sidebar.Controls.Add(c);c.Anchor=AnchorStyles.Top|AnchorStyles.Left;}
    else if(c.Top<166){int x=c.Left-311;header.Controls.Add(c);c.Location=new Point(x,c.Top);c.Anchor=AnchorStyles.Top|AnchorStyles.Left;
     if(x>=560)c.Anchor=AnchorStyles.Top|AnchorStyles.Right;
     else if(c==search)c.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
     else if(c.Text==L.T("Ses ara"))c.Anchor=AnchorStyles.Top|AnchorStyles.Right;
    }else{int x=c.Left-311,y=c.Top-630;footer.Controls.Add(c);c.Location=new Point(x,y);c.Anchor=AnchorStyles.Top|AnchorStyles.Left;
     if(c==volume)c.Anchor|=AnchorStyles.Right;
     else if(x>=500)c.Anchor=AnchorStyles.Top|AnchorStyles.Right;
     if(c==status){c.AutoSize=false;c.MaximumSize=Size.Empty;c.Size=new Size(790,50);c.Anchor|=AnchorStyles.Right;}
    }
   }
   // Establish the original logical container sizes before anchors respond to resizing.
   sidebar.AutoScrollMinSize=new Size(0,774);
  }
  ComboBox Combo(int x,int y,int w){var c=new ComboBox{Location=new Point(x,y),Width=w,DropDownStyle=ComboBoxStyle.DropDownList,BackColor=Color.FromArgb(30,33,45),ForeColor=Color.White,DropDownWidth=520};Controls.Add(c);return c;}
  MMDevice Selected(ComboBox c){return c.SelectedItem==null?null:((DeviceItem)c.SelectedItem).Device;}
  void LoadDevices(){loading=true;try{output.Items.Clear();monitor.Items.Clear();mic.Items.Clear();foreach(var d in devices)d.Dispose();devices.Clear();foreach(var d in enumerator.EnumerateAudioEndPoints(DataFlow.Render,DeviceState.Active)){devices.Add(d);output.Items.Add(new DeviceItem{Device=d});if(!AudioRouting.VirtualCableName(d.FriendlyName))monitor.Items.Add(new DeviceItem{Device=d});}foreach(var d in enumerator.EnumerateAudioEndPoints(DataFlow.Capture,DeviceState.Active)){devices.Add(d);if(!AudioRouting.VirtualCableName(d.FriendlyName))mic.Items.Add(new DeviceItem{Device=d});}
    SelectDevice(output,settings.Output,true);SelectDevice(mic,settings.Mic,false);SelectDevice(monitor,null,false);
   }catch(Exception e){status.Text=L.T("Aygıtlar okunamadı: ")+e.Message;}finally{loading=false;}}
  void SelectDevice(ComboBox c,string id,bool cable){int found=-1;for(int i=0;i<c.Items.Count;i++){var d=((DeviceItem)c.Items[i]).Device;if(d.ID==id){found=i;break;}if(cable && IsCable(d))found=i;}if(found<0 && c.Items.Count>0){try{using(var def=enumerator.GetDefaultAudioEndpoint(c==mic?DataFlow.Capture:DataFlow.Render,Role.Multimedia)){for(int i=0;i<c.Items.Count;i++)if(((DeviceItem)c.Items[i]).Device.ID==def.ID)found=i;}}catch{}if(found<0)found=0;}c.SelectedIndex=found;}
  public static bool IsCable(MMDevice d){return d.FriendlyName.IndexOf("VB-Audio Virtual Cable",StringComparison.OrdinalIgnoreCase)>=0 && d.FriendlyName.Replace(" ","").IndexOf("16ch",StringComparison.OrdinalIgnoreCase)<0;}
  void Save(){try{Store.Save(settings);}catch(Exception e){status.Text=L.T("Kaydedilemedi: ")+e.Message;}}
  void Disconnect(){engine.Dispose();ready=false;connect.Tag="Çıkışı başlat";connect.Text=L.T("Çıkışı başlat");status.Text=L.T("Çıkış kapalı · Ayarları uygulamak için başlat.");}
  void Connect(){try{
   routeInVoice=mode.SelectedIndex==1&&voiceAvailable();
   var cable=output.Items.Cast<DeviceItem>().FirstOrDefault(x=>IsCable(x.Device));
   if(cable==null)routeInVoice=false;
   var device=routeInVoice?cable.Device:Selected(monitor);
   if(device==null)throw new Exception(L.T("Çıkış aygıtı bulunamadı."));
   engine.Configure(device,routeInVoice&&listen.Checked?Selected(monitor):null,volume.Value/100f);ready=true;connect.Tag="Yeniden bağlan";connect.Text=L.T("Yeniden bağlan");UpdatePlaybackStatus();if(routeInVoice&&micOn.Checked)SetMic();
  }catch(Exception e){Disconnect();routeInVoice=false;micOn.Enabled=false;status.Text=L.T("Çıkış açılamadı: ")+e.Message;}}
  void SetMic(){try{if(!routeInVoice){engine.MicOff();return;}if(!micOn.Checked){engine.MicOff();return;}if(!ready)throw new Exception(L.T("Önce çıkışı başlat."));var d=Selected(mic);if(d==null)throw new Exception(L.T("Mikrofon bulunamadı."));if(d.FriendlyName.IndexOf("CABLE Output",StringComparison.OrdinalIgnoreCase)>=0)throw new Exception(L.T("Fiziksel mikrofon seç; CABLE Output geri besleme oluşturur."));engine.MicOn(d);status.Text=L.T("Mikrofon + sesler seçili çıkışa gönderiliyor.");}catch(Exception e){micOn.Checked=false;status.Text=L.T("Mikrofon: ")+e.Message;}}
  void AddFiles(){using(var f=new OpenFileDialog{Title=L.T("Ses dosyalarını ekle"),Filter=L.T("Ses dosyaları|*.wav;*.mp3;*.aiff;*.aif;*.wma;*.m4a;*.aac|Tüm dosyalar|*.*"),Multiselect=true})if(f.ShowDialog(this)==DialogResult.OK)Import(f.FileNames);}
  void Import(string[] paths){int added=0;var errors=new List<string>();foreach(var path in paths){string dest=null;try{using(var r=new AudioFileReader(path)){if(r.WaveFormat.Channels>2)throw new Exception(L.T("Mono / stereo dosya seç."));}dest=Path.Combine(Store.Root,"sounds",Guid.NewGuid()+Path.GetExtension(path));System.IO.File.Copy(path,dest);settings.Clips.Add(new Clip{Name=Path.GetFileNameWithoutExtension(path),File=dest});added++;}catch(Exception e){if(dest!=null && System.IO.File.Exists(dest))System.IO.File.Delete(dest);errors.Add(Path.GetFileName(path)+": "+e.Message);}}Save();Render();RegisterAll();status.Text=added+L.T(" ses eklendi.");if(errors.Count>0)MessageBox.Show(this,string.Join("\n",errors),L.T("Eklenemeyen dosyalar"));}
  void Render(){pads.SuspendLayout();foreach(Control c in pads.Controls.Cast<Control>().ToArray())c.Dispose();pads.Controls.Clear();count.Text=settings.Clips.Count+L.T(" ses  /  sınırsız kısayol");int i=0;foreach(var c in settings.Clips.Where(c=>c.Name.IndexOf(search.Text,StringComparison.CurrentCultureIgnoreCase)>=0)){var clip=c;var p=new Pad(c,i++);p.Click+=delegate{Play(clip);};var menu=new ContextMenuStrip();menu.Items.Add(L.T("Düzenle / tuş ata"),null,delegate{Edit(clip);});menu.Items.Add(L.T("Kütüphaneden kaldır"),null,delegate{if(MessageBox.Show(this,string.Format(L.T("Kütüphaneden '{0}' kaldırılsın mı?"),clip.Name),L.T("Sesi kaldır"),MessageBoxButtons.YesNo)==DialogResult.Yes){engine.Stop();settings.Clips.Remove(clip);Save();RegisterAll();Render();}});p.ContextMenuStrip=menu;p.Disposed+=delegate{menu.Dispose();};pads.Controls.Add(p);p.ApplyDpi(pads.DeviceDpi);}if(pads.Controls.Count==0)pads.Controls.Add(new Label{Text=settings.Clips.Count==0?L.T("İlk sesini ekle.\n\nMP3, WAV veya M4A dosyalarını buraya sürükle.\nHer ses için sağ tıkla ve bir kısayol ata."):L.T("Bu aramada ses bulunamadı."),ForeColor=Muted,Size=new Size(Math.Max(100,pads.ClientSize.Width-(int)(24*DeviceDpi/96f)),(int)(190*DeviceDpi/96f)),Padding=new Padding((int)(12*DeviceDpi/96f),(int)(26*DeviceDpi/96f),0,0),Font=new Font("Segoe UI",13)});pads.ResumeLayout();}
  void Edit(Clip c){UnregisterAll();using(var d=new EditClip(c)){if(d.ShowDialog(this)==DialogResult.OK){if(d.Key!=0 && ((d.Key==(int)Keys.Space && d.Mods==6)||settings.Clips.Any(x=>x!=c && x.Key==d.Key && x.Mods==d.Mods)))MessageBox.Show(this,L.T("Bu kısayol zaten kullanılıyor. Değişiklikler kaydedilmedi."));else{c.Name=d.TitleBox.Text.Trim();c.Key=d.Key;c.Mods=d.Mods;c.Gain=d.Gain;Save();}}}RegisterAll();Render();}
  void Play(Clip c){try{engine.Play(c);status.Text=L.T("Çalındı: ")+c.Name;foreach(Pad p in pads.Controls.OfType<Pad>()){p.Active=p.Clip==c;p.Invalidate();}flash=5;}catch(Exception e){status.Text=L.T("Çalınamadı: ")+e.Message;}}
  void Stop(){engine.Stop();flash=1;status.Text=L.T("Tüm sesler durduruldu · Mikrofon açıksa iletim sürer.");}
  void UnregisterAll(){foreach(var k in hotkeys.Keys)UnregisterHotKey(Handle,k);hotkeys.Clear();if(stopRegistered)UnregisterHotKey(Handle,9000);stopRegistered=false;}
  void RegisterAll(){if(!IsHandleCreated)return;UnregisterAll();failed.Clear();stopRegistered=RegisterHotKey(Handle,9000,0x4006,(uint)Keys.Space);int i=1;foreach(var c in settings.Clips){if(c.Key!=0){if(RegisterHotKey(Handle,i,(uint)c.Mods|0x4000,(uint)c.Key))hotkeys[i]=c;else failed.Add(c);}i++;}if(failed.Count>0 || !stopRegistered)status.Text=L.T("Kısayol kullanımda: ")+string.Join(", ",failed.Select(c=>c.Name+" ("+c.Shortcut+")"))+(!stopRegistered?L.T(" · Durdurma kısayolu kullanılamıyor."):"");}
  protected override void WndProc(ref Message m){if(m.Msg==0x312){int id=m.WParam.ToInt32();Clip c;if(id==9000)Stop();else if(hotkeys.TryGetValue(id,out c))Play(c);}base.WndProc(ref m);}
 }
 static class Program {
  [DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr h,int id,uint mods,uint key);
  [DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr h,int id);
  [STAThread] static int Main(string[] args){if(args.Contains("--self-test"))return Test();if(args.Contains("--route-test"))return RouteTest();using(var mutex=new System.Threading.Mutex(false,"Local\\BroPad.Desktop")){if(!mutex.WaitOne(0,false)){MessageBox.Show(L.T("BroPad zaten açık. Görev çubuğundan açabilirsin."));return 0;}try{Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Application.Run(new Main());return 0;}finally{mutex.ReleaseMutex();}}}
  static int RouteTest(){string report=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"route-test-results.txt");try{using(var en=new MMDeviceEnumerator()){var outputs=en.EnumerateAudioEndPoints(DataFlow.Render,DeviceState.Active);var inputs=en.EnumerateAudioEndPoints(DataFlow.Capture,DeviceState.Active);var output=outputs.FirstOrDefault(BroPad.Main.IsCable);var input=inputs.FirstOrDefault(BroPad.Main.IsCable);if(output==null || input==null)throw new Exception("Sanal kablo aygıtları bulunamadı.");using(var data=new MemoryStream())using(var capture=new WasapiCapture(input))using(var bus=new AudioBus(output,0.5f)){capture.DataAvailable+=delegate(object s,WaveInEventArgs e){lock(data)data.Write(e.Buffer,0,e.BytesRecorded);};capture.StartRecording();System.Threading.Thread.Sleep(200);bus.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Ornek-sesler","Bildirim.wav"),0.8f);System.Threading.Thread.Sleep(900);capture.StopRecording();byte[] bytes;lock(data)bytes=data.ToArray();float peak=0;long samples=0;using(var stream=new RawSourceWaveStream(new MemoryStream(bytes),capture.WaveFormat)){var source=stream.ToSampleProvider();var buffer=new float[4096];int n;while((n=source.Read(buffer,0,buffer.Length))>0){samples+=n;for(int i=0;i<n;i++)peak=Math.Max(peak,Math.Abs(buffer[i]));}}if(peak<0.005f)throw new Exception("Sinyal doğrulanamadı. Peak="+peak);System.IO.File.WriteAllText(report,"PASS — sanal kablo uçtan uca sinyal testi\r\nGönderen: "+output.FriendlyName+"\r\nAlan: "+input.FriendlyName+"\r\nÖrnek: "+samples+"\r\nTepe: "+peak+"\r\nFiziksel mikrofon açılmadı. Discord / oyun alımı ayrıca denenmelidir.");}foreach(var d in outputs)d.Dispose();foreach(var d in inputs)d.Dispose();}return 0;}catch(Exception e){System.IO.File.WriteAllText(report,e.ToString());return 1;}}
  static int Test(){try{var results=new List<string>();foreach(var rate in new[]{44100,48000})foreach(var ch in new[]{1,2}){string file=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".wav");try{using(var w=new WaveFileWriter(file,new WaveFormat(rate,16,ch))){for(int i=0;i<rate/10;i++)for(int c=0;c<ch;c++)w.WriteSample((float)(Math.Sin(i*440*2*Math.PI/rate)*0.2));}using(var r=new AudioFileReader(file)){var p=AudioBus.Normalize(r);var data=new float[960];int total=0,n;float peak=0;while((n=p.Read(data,0,data.Length))>0){total+=n;for(int i=0;i<n;i++)peak=Math.Max(peak,Math.Abs(data[i]));}if(total<9000 || total>10000 || peak<0.1)throw new Exception("Resample failed");results.Add(rate+" Hz / "+ch+" ch: PASS ("+total+" samples)");}}finally{System.IO.File.Delete(file);}}
   var s=new Settings();s.Clips.Add(new Clip{Name="Türkçe çığlık",File="test.wav",Key=(int)Keys.F8,Mods=2});using(var ms=new MemoryStream()){var j=new DataContractJsonSerializer(typeof(Settings));j.WriteObject(ms,s);ms.Position=0;var read=(Settings)j.ReadObject(ms);if(read.Clips[0].Name!=s.Clips[0].Name || read.Clips[0].Shortcut!="Ctrl + F8")throw new Exception("JSON roundtrip failed");}results.Add("Türkçe kütüphane + kısayol: PASS");
   using(var form=new Form()){var handle=form.Handle;bool registered=RegisterHotKey(handle,101,0x4003,(uint)Keys.F11);if(!registered)throw new Exception("Test kısayolu başka uygulamada kullanımda.");try{if(RegisterHotKey(handle,102,0x4003,(uint)Keys.F11)){UnregisterHotKey(handle,102);throw new Exception("Kısayol çakışması algılanmadı.");}}finally{UnregisterHotKey(handle,101);}results.Add("Windows global kısayol kaydı / çakışma: PASS");}
   var tone=new SignalGenerator(48000,2){Gain=2,Frequency=440};var limited=new Limit(tone);var block=new float[960];limited.Read(block,0,block.Length);if(block.Any(x=>Math.Abs(x)>1))throw new Exception("Limiter failed");results.Add("Ses sınırlandırıcı: PASS");
   using(var e=new MMDeviceEnumerator()){foreach(var d in e.EnumerateAudioEndPoints(DataFlow.Render,DeviceState.Active)){using(d){using(var bus=new AudioBus(d,0)){bus.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Ornek-sesler","Bildirim.wav"),0.5f);System.Threading.Thread.Sleep(220);bus.StopClips();}results.Add("WASAPI çıkış açma / çalma / durdurma (sessiz): PASS — "+d.FriendlyName);}}}System.IO.File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"test-results.txt"),results);return 0;
  }catch(Exception ex){System.IO.File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"test-results.txt"),ex.ToString());return 1;}}
 }
}
