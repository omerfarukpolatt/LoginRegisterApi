# Login Register Api

## Proje Hakkında
Bu proje, .NET 8 ile geliştirilmiş JWT tabanlı bir kullanıcı kayıt ve giriş (login/register) API'sidir. Kullanıcılar güvenli şekilde kayıt olabilir ve giriş yaptıklarında JWT token alarak korumalı endpointlere erişebilirler.

## Özellikler
- Kullanıcı kaydı (register)
- Kullanıcı girişi (login)
- JWT ile kimlik doğrulama
- Swagger/OpenAPI dokümantasyonu
- Şifreler BCrypt ile hashlenir

## Kurulum
1. **Projeyi klonlayın:**
   ```bash
   git clone <repo-url>
   ```
2. **Gerekli NuGet paketlerini yükleyin:**
   - Microsoft.AspNetCore.Authentication.JwtBearer
   - BCrypt.Net-Next
   - Swashbuckle.AspNetCore
   (Visual Studio veya `dotnet restore` komutu ile otomatik yüklenir)
3. **Veritabanı bağlantınızı ayarlayın:**
   - `appsettings.json` dosyasındaki `DefaultConnection` kısmını kendi SQL bağlantı bilginizle güncelleyin.
   - Kullanıcılar için bir `Users` tablosu oluşturun:
     ```sql
     CREATE TABLE Users (
       Id INT PRIMARY KEY IDENTITY,
       Username NVARCHAR(100) NOT NULL,
       Email NVARCHAR(100) NOT NULL,
       PasswordHash NVARCHAR(200) NOT NULL
     );
     ```
4. **Projeyi başlatın:**
   ```bash
   dotnet run
   ```

## Kullanım
- Proje çalıştığında Swagger arayüzüne `https://localhost:<port>/swagger` adresinden ulaşabilirsiniz.
- API isteklerini Postman veya Swagger üzerinden test edebilirsiniz.

## API Endpointleri
### 1. Kayıt Ol (Register)
- **POST** `/auth/register`
- **Body:**
  ```json
  {
    "username": "username",
    "email": "mail@example.com",
    "password": "password"
  }
  ```
- **Başarılı yanıt:**
  ```json
  { "message": "User registered successfully." }
  ```

### 2. Giriş Yap (Login)
- **POST** `/auth/login`
- **Body:**
  ```json
  {
    "username": "username",
    "password": "password"
  }
  ```
- **Başarılı yanıt:**
  ```json
  { "token": "<JWT Token>" }
  ```

## Yapılandırma (appsettings.json)
```json
{
  "Jwt": {
    "Key": "***",
    "Issuer": "LoginRegisterApi"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=LoginRegisterDb;Trusted_Connection=True;"
  }
}
```
