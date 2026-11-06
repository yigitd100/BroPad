# BroPad

**Favori seslerin, tek tuş uzağında.**

[Windows x64 için indir](https://github.com/yigitd100/BroPad/releases/download/v1.2.0-preview.1/BroPad.exe) · [Sürümler](https://github.com/yigitd100/BroPad/releases) · [English](README.md)

Tek dosya indir: **BroPad.exe**. Aç, sesini ekle, çal. Ayrı uygulama kurulumu veya
VB-CABLE indirmesi gerekmez.

- **On my device / Cihazımda dinle:** uygulama açılınca yerel ses çıkışı otomatik
  hazırlanır. Yeni kütüphanede üç kısa örnek ses bulunur.
- **Voice chat / games / Sesli sohbet / oyunlar:** Discord ve oyunlara ses gönder.
  İlk kullanımda **Enable voice chat / Sesli sohbeti etkinleştir** düğmesine bas.
  Gerekli imzalı sürücü uygulama paketinin içindedir. Windows bir kez yönetici
  onayı ister; yeniden başlatma gerekebilir. BroPad otomatik yeniden başlatmaz.
- Discord/oyun ayarlarında mikrofonu **CABLE Output** seç. Kendi sesin de gitsin
  istiyorsan fiziksel mikrofonunu seçip karıştırma seçeneğini aç.
- Kurulumu atladığında veya iptal ettiğinde yerel ses çalma kullanılabilir.
- Karta sağ tıklayarak tuş ata. **Ctrl + Shift + Space** bütün efektleri durdurur.
- Sol alttaki **Language → Türkçe** ile dil değiştir. Tercihin kaydedilir.

Windows 10 22H2 / Windows 11 x64 ve .NET Framework 4.8 hedeflenir. BroPad çalışma
dosyalarını kullanıcı klasörüne açar; ses kütüphanesi güncellemelerde korunur.

**VB-CABLE, VB-Audio tarafından geliştirilen bir donationware yazılımdır; bağış ve
lisans katkıları desteklenir.** Orijinal paket değiştirilmeden sunulur.
Kaynak: https://www.vb-cable.com/ · https://vb-audio.com/Cable/
Bağış/lisans: https://shop.vb-audio.com/
Dağıtım koşulları: https://vb-audio.com/Services/licensing.htm

Bu sürüm tek dosya paketini, ilk açılış/yerel çalışma davranışını ve dil/DPI
kontrollerini içerir. Sıfırdan sürücü kurulumu ve yeniden başlatma akışının temiz
bir Windows sanal makinesinde doğrulanması gerekir. BroPad EXE dosyası henüz
imzalı değildir; paket içindeki üçüncü taraf sürücü imzalıdır.

Kaynak kodunu derlemek için `./build.ps1`, test için `./tests/run.ps1` çalıştır.
Detaylar ve lisanslar için [İngilizce belgeye](README.md) bak.
