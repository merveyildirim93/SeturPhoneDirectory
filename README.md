# Phone Directory Microservices Project

## Proje Hakkında

Bu proje, mikroservis mimarisiyle geliştirilmiş basit bir telefon rehberi uygulamasıdır. İki bağımsız servis ile çalışmaktadır:

* **ContactService:** Kişi ve iletişim bilgilerini yönetir.
* **ReportService:** Rehberdeki kişilerin bulundukları konuma göre istatistiksel raporları asenkron olarak oluşturur.

Proje **.NET 8** platformu kullanılarak geliştirilmiştir.

---

## Kullanılan Teknolojiler

* .NET 8
* Entity Framework Core
* PostgreSQL (ContactDb ve ReportDb olmak üzere 2 ayrı veritabanı)
* RabbitMQ (Mesaj Kuyruğu olarak)
* Docker (PostgreSQL ve RabbitMQ servisleri için)
* xUnit (Birim testleri için)
* Swagger (API dokümantasyonu ve testi için)

---

## Veritabanları ve Yapısı

Projede **2 farklı PostgreSQL veritabanı** kullanılmıştır:

| Veritabanı    | İçerik                                      | Açıklama                                    |
| ------------- | ------------------------------------------- | ------------------------------------------- |
| **ContactDb** | `Persons`, `ContactInformations` tabloları  | Kişi ve iletişim bilgilerini tutar.         |
| **ReportDb**  | `ReportRequests`, `ReportDetails` tabloları | Rapor talepleri ve rapor detaylarını tutar. |

Bu yapı, mikroservislerin bağımsızlığını ve ölçeklenebilirliğini artırmak için tasarlanmıştır.

---

## Servislerin Çalıştırılması için Gerekenler

### PostgreSQL ve RabbitMQ Kurulumu (Docker)

Projede PostgreSQL ve RabbitMQ servisleri Docker konteynerleri olarak çalıştırılmaktadır.

#### RabbitMQ

```bash
docker run -d -p 5672:5672 -p 15672:15672 --name seturrabbitmqcontainer rabbitmq:4.1.3-management
```

* `5672`: RabbitMQ mesaj kuyruğu portu
* `15672`: RabbitMQ yönetim paneli portu ([http://localhost:15672](http://localhost:15672))

#### PostgreSQL

Örnek docker komutu:

```bash
docker run -d --name seturpostgresql -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -e POSTGRES_DB=ContactDb -p 5432:5432 postgres:15-alpine
```

* Bu komut ContactDb için örnektir.
* `ReportDb` için benzer şekilde farklı bir container ya da aynı sunucu üzerinde farklı DB açılabilir.

---

## Proje Yapısı

* **PhoneDirectory.ContactService:** Kişi ve iletişim bilgileri servisi. ContactDb’ye bağlıdır.
* **PhoneDirectory.ReportService:** Raporlama servisi. ReportDb’ye bağlıdır ve RabbitMQ ile asenkron iletişim kurar.
* **Test Projeleri:** Her servis için ayrı birim test projeleri oluşturulmuştur.
* **Shared/Core:** Ortak kullanılan DTO, enum, entity yapıları burada yer almaktadır.

---

## API Kullanımı

API'ler Swagger üzerinden test edilebilir:
`http://localhost:{port}/swagger`

### Örnek Endpointler

* **ContactService:**

  * Kişi oluşturma, listeleme, güncelleme, silme
  * İletişim bilgisi ekleme, silme

* **ReportService:**

  * Rapor talebi oluşturma
  * Rapor listesini alma
  * Rapora ait detayları görüntüleme

---

## Proje Çalıştırma

1. Docker ile PostgreSQL ve RabbitMQ servislerini ayağa kaldırın.
2. ContactDb ve ReportDb için migration işlemlerini çalıştırın.
3. Uygulamaları çalıştırın.
4. Swagger UI üzerinden API'leri test edin.

---

## İletişim

Proje ile ilgili sorularınız için bana ulaşabilirsiniz.
Mail: merveyildirimm93@gmail.com
