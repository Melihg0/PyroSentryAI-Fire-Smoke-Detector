using Microsoft.EntityFrameworkCore;
using PyroSentryAI.Models;
using PyroSentryAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
namespace PyroSentryAI.Services.Implementations
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDbContextFactory<PyroSentryAiDbContext> _contextFactory;


        public DatabaseService(IDbContextFactory<PyroSentryAiDbContext> contextFactory)
        {
            _contextFactory= contextFactory;
        }

        public async Task<TblSetting> GetSettingsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.TblSettings.FirstOrDefaultAsync();
        }


        public async Task UpdateSettingsAsync(TblSetting settings)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.TblSettings.Update(settings);
            await context.SaveChangesAsync();
        }



        public async Task<List<TblCamera>> GetAllVisibleCamerasAsync()
        {
            //arşiv özelliği geldiğinde  (c => c.IsArchived == false) şartı eklenecek.
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.TblCameras.OrderByDescending(c => c.CameraId).ToListAsync();
        }

        public async Task<List<TblCamera>> GetActiveCamerasAsync()
        {
            // (c => c.IsArchived == false) şartı da gelicek.
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.TblCameras
                                 .Where(c => c.IsActive == true)
                                 .OrderByDescending(c => c.CameraId)
                                 .ToListAsync();
        }


        public async Task<TblCamera> AddCameraAsync(TblCamera newCamera)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.TblCameras.Add(newCamera);
            await context.SaveChangesAsync();
            return newCamera;
        }


        public async Task UpdateCameraAsync(TblCamera cameraToUpdate)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.TblCameras.Update(cameraToUpdate);
            await context.SaveChangesAsync();
        }
        
    }
}
