using PyroSentryAI.Models;
using PyroSentryAI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Services.Interfaces
{
    public interface ICameraViewModelFactory
    {
        ///<summary>
        /// Verilen kamera veritabanı modeline dayanarak tam olarak yapılandırılmış bir CameraViewModel oluşturur.
        /// <param name="camera">Veritabanından gelen kamera bilgisi.</param>
        /// <returns>Yeni bir CameraViewModel örneği.</returns>
        CameraViewModel Create(TblCamera camera, bool initializePlayer=true); 
        
        /*Bu factoryi kurmazsak new leyerek alırsak CameraViewModeli newlediğimiz her ViewModel CameraViewModelin servislerini bilmesi gerek sırf
        bunun için constructorunda her ViewModel istemek zorunda kalıyor ve CameraViewModel kullandıgımız her metotda bunları vermemiz gerekiyor.*/
    }
}
