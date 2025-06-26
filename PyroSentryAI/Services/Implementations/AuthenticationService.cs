using Microsoft.EntityFrameworkCore;
using PyroSentryAI.Models;
using PyroSentryAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly PyroSentryAiDbContext _context; //(veritabanı bağlantısı)

        public AuthenticationService(PyroSentryAiDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            var user = await _context.TblUsers.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
            {
                return false;
            }

            // veritabanındaki 'user.PasswordHash' ile bu hash'ler karşılaştırılır. Şimdilik yapının çalışması için basit kontrol yapıyoruz.
            if (user.PasswordHash == password) // Burayı daha sonra yapıcaz.
            {
                return true;
            }

            return false; //şifre yanlış

        }
    }
}
