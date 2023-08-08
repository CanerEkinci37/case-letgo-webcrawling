# Letgo Web Crawling
## Aşamalar
### Sitenin Analizi
Letgo sitesinde ilk aşamada cookiesler karşımıza çıkmakta ve bunu kapatıp işimize devam etmekteyiz. Sitenin anasayfasında en son satılık ürünler çıkmakta ve bunların hepsi aynı class'a bağlı. Ürünün detay sayfasına girilip nitelikleri bakılarak Product aslı class oluşturdum. 
### Seleniumda Uygulanması
#### Driver konfigürasyonu
Driver olarak ChromeDriver kullanılmakta ve konfigürasyonlar olarak konsoldaki çıktıları engellemek için log-level belirledim, ip bana karşı proxy yöntemini yapmak istedim fakat başaramadım bu sebeple user agent kullanarak geliştirdim. Chrome headless modda çalışamamakta. Letgo sayfası izin vermemekte ve bu C#'da screenshot.png olarak kaydı alınmıştır. Sonrasında driver'i letgo.com'a yönlendirilmesi işlemi yapılmıştır.
#### Daha Fazla Yükle İşlemi
Sayfada daha fazla yükle butonu bulunmakta bu butona ise xpath ile (iyi bir yöntem olmasa da) webelementi oluşturulup sayfada denenmesi yapılıp başarılı olmuştur.
#### Ürünlerin Verilerinin Çekilmesi
Ürünlerin bilgilerinin çekilmesi aşamasında ilk olarak anasayfada toplam kaç ürün var bunun hesabını yaptım. Sonrasında eleman sayısı kadar for döngüsü dönecek şekilde liste elemanlarını çektim indekse göre de liste elemanlarını indeksleyip altındaki <a> etiketini alıp detay sayfasına yönlendirilerek data-aud-id class'ı adı altında ürünün başlığını, sahibini ve fiyatını çektim. Bu bilgileri bir nesneye atanıp sonrasında ise ürünler nesne listesinine ekleniyor. Bu aşamada tabii ki kırık link olabilir. Bu sebeple bir ürünün fiyatı olmak zorunda yani eğer detay sayfasında ürün fiyatı yoksa bunu kırık link olarak değerlendirilip anasayfaya dönülüp diğer ürünle devam etmesini sağladım. Bazı ürünlerin sahibi de yazmıyor bunun içinse Placeholder Kullanıcı diye varsayılan değer atadım ve anasayfaya geri dönüp diğer ürünle devam edildi.
#### Ürünlerin Çıktısı
Konsol için bahsettiğim ürünlerin bilgilerinin çekildiği for döngüsünde yazdırılmakta. Txt dosyası oluşturulup ürünler nesne listesinin iterate edilip ürün bilgilerinin satır satır yazdırdım. 
Konsolda fiyat ortalaması için LINQ sorgusu ile ortalamasını aldım.

Caner Ekinci
