using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PyroSentryAI.Models;

namespace PyroSentryAI.Services.Interfaces
{
    interface IDatabaseService
    {
        //Ayarlar için Metotlar.
        Task<TblSetting> GetSettingsAsync();
        Task UpdateSettingsAsync(TblSetting settings);


        //Kamera için Metotlar.

        Task<List<TblCamera>> GetAllVisibleCamerasAsync();
        //Şimdilik Yukardaki fonksiyon yarım olarak kullanılacak arşiv özelliği geldiğinde değiştirilecek 
        //Şu an tüm kameraları getirir.

        Task<List<TblCamera>> GetActiveCamerasAsync();
        //Sadece aktif kameraları getirir.Home da kullanılacak.Arşiv özelliği geldiğinde arşivlenmişmişmi diyede kontrol edicek.
        Task<TblCamera> AddCameraAsync(TblCamera newCamera);

        Task UpdateCameraAsync(TblCamera cameraToUpdate);
        //BU da yarım bir fonksiyon .Şu an sadece cameranın aktif pasif olma durumunu günceller.İleride kamera bilgilerini düzenlemek için
        //veritabanına ihtiyaç olmuyacak.
    }
}
