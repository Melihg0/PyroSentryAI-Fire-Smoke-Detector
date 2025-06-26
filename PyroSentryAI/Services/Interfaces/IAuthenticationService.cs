using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(string username, string password);
        
        /*Asenkron bir metodun temel amacı, bir işin bitmesini beklerken programı 
         kilitlememektir ve bu işin sonunda bir değer dönecekse, bu "gelecekte dönecek olan değer" Task<T> ile temsil edilir. 
        */
    }
}
