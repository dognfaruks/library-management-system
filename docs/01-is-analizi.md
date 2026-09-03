# Sprint 1 – İş Analizi

## 1. Kullanıcı (USER) Rolü Neler Yapabilir?
- Kayıt olma / giriş yapma (register / login)
- Kitapları listeleme, detay görüntüleme
- Kitaplarda arama, filtreleme, sıralama, sayfalama yapma
- Kitap ödünç alma talebi oluşturma (`POST /borrowings`)
- Ödünç aldığı kitabı iade etme (`PATCH /borrowings/:id/return`)
- Stokta olmayan bir kitap için rezervasyon oluşturma (`POST /reservations`)
- Kendi rezervasyonlarını görüntüleme (`GET /reservations`)
- Okuduğu bir kitaba yorum/puan bırakma (`POST /reviews`)
- Kitaplara ait yorumları görüntüleme (`GET /reviews`)

## 2. Yönetici (ADMIN) Rolü Neler Yapabilir?
Kullanıcının yapabildiği her şeye ek olarak:
- Author (yazar) CRUD işlemleri
- Category (kategori) CRUD işlemleri
- Publisher (yayınevi) CRUD işlemleri
- Book (kitap) CRUD işlemleri (yazar/kategori ilişkilendirme dahil)
- Tüm ödünç kayıtlarını görüntüleme (`GET /borrowings`)

## 3. İş Kuralları
1. Şifreler asla düz metin saklanmaz; bcrypt ile hash'lenir.
2. Girişte başarılı doğrulama sonrası JWT Access Token üretilir.
3. Korunan endpointler role göre yetkilendirilir (USER, ADMIN).
4. Bir kitabın stok adedi negatif olamaz.
5. Ödünç alma işleminde stok kontrolü yapılır; stok yoksa ödünç verilemez.
6. Ödünç oluşturma + stok azaltma aynı transaction içinde çalışır.
7. Eşzamanlı ödünç isteklerinde stok kontrolü atomik SQL komutu ile korunur (`UPDATE ... WHERE Stock > 0`); iki kullanıcı aynı anda son kopyayı alamaz.
8. Aynı kullanıcı, aynı kitap için aktif bir rezervasyonu varken tekrar rezervasyon oluşturamaz.
9. Rezervasyonlar sıralarına göre (`QueueOrder`) işleme alınır.
10. Review puanı yalnızca 1–5 aralığında olabilir.
11. Bir kullanıcı bir kitaba yalnızca bir kez yorum yapabilir.
12. Login, Register, Borrowing, Reservation işlemleri loglanır.

## 4. Hata Senaryoları
| Senaryo | Beklenen Davranış |
|---|---|
| Var olan email ile register | 409 Conflict |
| Yanlış şifre/email ile login | 401 Unauthorized |
| Token olmadan korumalı endpoint'e erişim | 401 Unauthorized |
| USER rolüyle ADMIN endpoint'ine erişim | 403 Forbidden |
| Stokta olmayan kitaba borrowing isteği | 400 Bad Request |
| Var olmayan kitap/kullanıcı ID'si | 404 Not Found |
| Zaten iade edilmiş bir borrowing'i tekrar iade etme | 400 Bad Request |
| Aynı kitaba tekrar aktif rezervasyon oluşturma | 409 Conflict |
| 1-5 aralığı dışında review puanı | 400 Bad Request |
| Aynı kitaba ikinci kez review yapma | 409 Conflict |