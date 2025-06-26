using PyroSentryAI.Models;
using PyroSentryAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace PyroSentryAI.Services.Implementations
{
    class DatabaseService : IDatabaseService
    {
        private readonly PyroSentryAiDbContext _context;


        public DatabaseService(PyroSentryAiDbContext context)
        {
            _context = context;
        }

        public async Task<TblSetting> GetSettingsAsync()
        {
            return await _context.TblSettings.FirstOrDefaultAsync();
        }


        public async Task UpdateSettingsAsync(TblSetting settings)
        {
            var existingSettings = await _context.TblSettings.FirstOrDefaultAsync();
            if (existingSettings != null)
            {
                existingSettings.ConfidenceThreshold = settings.ConfidenceThreshold;
                existingSettings.AnalysisFps = settings.AnalysisFps;
                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<TblCamera>> GetAllVisibleCamerasAsync()
        {
            //arşiv özelliği geldiğinde  (c => c.IsArchived == false) şartı eklenecek.

            return await _context.TblCameras.OrderByDescending(c => c.CameraId).ToListAsync();
        }

        public  async Task<List<TblCamera>> GetActiveCamerasAsync()
        {
            // (c => c.IsArchived == false) şartı da gelicek.
            return await _context.TblCameras
                                 .Where(c => c.IsActive == true)
                                 .OrderByDescending(c => c.CameraId)
                                 .ToListAsync();
        }


        public async Task<TblCamera> AddCameraAsync(TblCamera newCamera)
        {
            _context.TblCameras.Add(newCamera);
            await _context.SaveChangesAsync();
            return newCamera;
        }


        public async Task UpdateCameraAsync(TblCamera cameraToUpdate)
        {
            _context.TblCameras.Update(cameraToUpdate);
            await _context.SaveChangesAsync();
        }

       
    }
}
