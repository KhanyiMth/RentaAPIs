using Microsoft.EntityFrameworkCore;
using RentaAPIs.cdDTOs;
using RentaAPIs.DTOs;
using RentaAPIs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace RentaAPIs.Services
{
    public class MaintenanceRequestService
    {

        private readonly AppDbContext _dbContext;
        private readonly RentaVisionService _visionService;
        private readonly IWebHostEnvironment _environment;
        private readonly MaintenanceRequestTriageService _triageService;

        public MaintenanceRequestService(AppDbContext dbContext, RentaVisionService visionService, IWebHostEnvironment environment, MaintenanceRequestTriageService triageService)
        {
            _dbContext = dbContext;
            _visionService = visionService;
            _environment = environment;
            _triageService = triageService;
        }

        public async Task<List<MaintenanceRequestResponseDTO>> GetAllMaintenanceRequests()
        {
            return await _dbContext.MaintenanceRequests.Select(r => new MaintenanceRequestResponseDTO
            {
                MaintenanceRequestId = r.MaintenanceReqId,
                TenantId = r.TenantID,
                TenantName = r.Tenant.FirstName + " " + r.Tenant.Surname,
                PropertyId = r.PropertyID,
                PropertyName = r.Property.Nickname,
                UnitId = r.UnitID,
                UnitNumber = r.Unit.UnitNumber,
                Title = r.Title,
                Description = r.Description,
                Category = r.Category,
                Priority = r.Priority,
                Status = r.Status,
                ReportedAt = r.ReportedAt,
                AIPredictedCategory = r.AIPredictedCategory,
                AIConfidence = r.AIConfidence,
                AIModelVersion = r.AIModelVersion,
                AICategoryConfirmed = r.AICategoryConfirmed,
                MaintenanceImagePath = r.MaintenanceImagePath,

            }).ToListAsync();
        }


        public async Task<string?> GetAuthorisedMaintenanceImagePath(
            Guid maintenanceRequestId,
            string userId)
        {
            var request = await _dbContext.MaintenanceRequests
                .AsNoTracking()
                .Where(m => m.MaintenanceReqId == maintenanceRequestId)
                .Select(m => new
                {
                    m.TenantID,
                    m.MaintenanceImagePath,
                    LandlordId = m.Property.LandlordId
                })
                .FirstOrDefaultAsync();

            if (request == null)
                return null;

            var tenant = await _dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserId == userId);

            bool isTenant =
                tenant != null &&
                tenant.TenantId == request.TenantID;

            var landlord = await _dbContext.Landlords
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.UserId == userId);

            bool isLandlord =
                landlord != null &&
                landlord.LandlordID == request.LandlordId;

            if (!isTenant && !isLandlord)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this image.");
            }

            if (string.IsNullOrWhiteSpace(request.MaintenanceImagePath))
                return null;

            // Only accept the server-generated image filename.
            var filename = Path.GetFileName(
                request.MaintenanceImagePath);

            if (!Guid.TryParse(
                Path.GetFileNameWithoutExtension(filename),
                out var fileId))
            {
                return null;
            }

            if (fileId != maintenanceRequestId ||
                !string.Equals(
                    Path.GetExtension(filename),
                    ".jpg",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var folder = Path.Combine(
                _environment.ContentRootPath,
                "PrivateUploads",
                "Maintenance");

            var fullPath = Path.Combine(folder, filename);

            if (!System.IO.File.Exists(fullPath))
                return null;

            return fullPath;
        }


        public async Task<MaintenanceRequestResponseDTO> GetMaintenanceRequestById (Guid mrId)
        {
            return await _dbContext.MaintenanceRequests
                .Include(r => r.Tenant)
                .Include(r => r.Property)
                .Include(r => r.Unit)
                .Where(r => r.MaintenanceReqId == mrId)
                .Select(r => new MaintenanceRequestResponseDTO
                {
                    MaintenanceRequestId = r.MaintenanceReqId,
                    TenantId = r.TenantID,
                    TenantName = r.Tenant != null ? r.Tenant.FirstName + " " + r.Tenant.Surname : null,
                    PropertyId = r.PropertyID,
                    PropertyName = r.Property != null ? r.Property.Nickname : null,
                    UnitId = r.UnitID,
                    UnitNumber = r.Unit != null ? r.Unit.UnitNumber : 0,
                    UnitName = r.Unit != null ? r.Unit.UnitName : null,
                    Title = r.Title,
                    Description = r.Description,
                    Category = r.Category,
                    Priority = r.Priority,
                    Status = r.Status,
                    ReportedAt = r.ReportedAt,
                    AIPredictedCategory = r.AIPredictedCategory,
                    AIConfidence = r.AIConfidence,
                    AIModelVersion = r.AIModelVersion,
                    AICategoryConfirmed = r.AICategoryConfirmed,
                    MaintenanceImagePath = r.MaintenanceImagePath,

                }).FirstOrDefaultAsync();
        }

        public async Task<List<MaintenanceRequestResponseDTO>> GetLandlordMaintenanceRequests(string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                throw new Exception("Landlord profile not found");

            var requests = await _dbContext.MaintenanceRequests
                .Where(m =>
                    m.Property != null &&
                    m.Property.LandlordId == landlord.LandlordID)
                .Include(m => m.Property)
                .Include(m => m.Tenant)
                .Include(m => m.Unit)
                .OrderByDescending(m => m.CreatedDate)
                .Select(m => new MaintenanceRequestResponseDTO
                {
                    MaintenanceRequestId = m.MaintenanceReqId,
                    Title = m.Title,
                    Description = m.Description,
                    Priority = m.Priority,
                    PropertyName = m.PropertyName,
                    PropertyId = m.PropertyID,
                    UnitId = m.UnitID,
                    Status = m.Status,
                    UnitName = m.Unit != null ? m.Unit.UnitName : null,
                    TenantId = m.TenantID,
                    AIPredictedCategory = m.AIPredictedCategory,
                    AIConfidence = m.AIConfidence,
                    AIModelVersion = m.AIModelVersion,
                    AICategoryConfirmed = m.AICategoryConfirmed,
                    MaintenanceImagePath = m.MaintenanceImagePath,

                    TenantName = m.Tenant != null ? m.Tenant.FirstName + "" + m.Tenant.Surname : "Unknown Tenant",

                }).ToListAsync();

            return requests;
        }


        public async Task<MaintenanceRequestResponseDTO> CreateMaintenanceRequest(CreateMaintenanceRequestDTO cmrequest, string userId)
        {
            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (tenant == null)
                throw new ArgumentException(
                    "The authenticated user is not registered as a tenant.");

            var property = await _dbContext.Properties
                .FirstOrDefaultAsync(
                    p => p.PropertyId == cmrequest.PropertyId);

            if (property == null)
                throw new ArgumentException(
                    "The selected property could not be found.");

            // Confirm the tenant has a lease for this property.
            // This check also prevents submissions against unrelated properties.
            var tenantLeases = await _dbContext.Leases
                .Where(l =>
                    l.TenantID == tenant.TenantId &&
                    l.PropertyID == cmrequest.PropertyId)
                .Select(l => new
                {
                    l.UnitId
                })
                .ToListAsync();

            if (tenantLeases.Count == 0)
                throw new UnauthorizedAccessException(
                    "You do not have a lease for this property.");

            Guid? validUnitId = null;

            if (cmrequest.UnitId.HasValue &&
                cmrequest.UnitId.Value != Guid.Empty)
            {
                bool unitExists = await _dbContext.Units
                    .AnyAsync(u =>
                        u.UnitId == cmrequest.UnitId.Value &&
                        u.PropertyId == cmrequest.PropertyId);

                if (!unitExists)
                    throw new ArgumentException(
                        "The selected unit does not belong to this property.");

                if (!tenantLeases.Any(l =>
                    l.UnitId == cmrequest.UnitId.Value))
                {
                    throw new UnauthorizedAccessException(
                        "You do not have a lease for the selected unit.");
                }

                validUnitId = cmrequest.UnitId.Value;
            }

            var category = cmrequest.Category?.Trim();

            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException(
                    "Please select a maintenance category.");
            }

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException(
                    "Please select a maintenance category.");

            if (string.IsNullOrWhiteSpace(cmrequest.Title) ||
                string.IsNullOrWhiteSpace(cmrequest.Description))
            {
                throw new ArgumentException(
                    "A title and description are required.");
            }

            var now = DateTime.UtcNow;
            var requestId = Guid.NewGuid();

            string? imagePath = null;
            string? absoluteImagePath = null;
            RentaVisionPredictionDTO? prediction = null;


            try
            {
                if (cmrequest.Image != null)
                {
                    var image = cmrequest.Image;

                    if (image.Length == 0 ||
                        image.Length > 10 * 1024 * 1024)
                    {
                        throw new ArgumentException(
                            "Image must be between 1 byte and 10 MB.");
                    }

                    // Validate actual image content, not just extension.
                    await using (var input = image.OpenReadStream())
                    {
                        var info = await Image.IdentifyAsync(input);

                        if (info == null ||
                            info.Width <= 0 ||
                            info.Height <= 0 ||
                            (long)info.Width * info.Height > 25_000_000)
                        {
                            throw new ArgumentException(
                                "The image is invalid or too large.");
                        }
                    }

                    // Run server-side CNN inference.
                    prediction = await _visionService.PredictAsync(image);

                    // Store uploads outside wwwroot.
                    var folder = Path.Combine(
                        _environment.ContentRootPath,
                        "PrivateUploads",
                        "Maintenance");

                    Directory.CreateDirectory(folder);

                    // Server-generated filename, independent of client filename.
                    var filename = $"{requestId:N}.jpg";
                    absoluteImagePath = Path.Combine(folder, filename);

                    // Decode and re-encode the image to avoid retaining
                    // untrusted file content or original metadata.
                    await using (var input = image.OpenReadStream())
                    using (var decoded =
                        await Image.LoadAsync<SixLabors.ImageSharp.PixelFormats.Rgb24>(
                            input))
                    {
                        await decoded.SaveAsJpegAsync(absoluteImagePath);
                    }

                    imagePath = $"Maintenance/{filename}";
                }

                var request = new MaintenanceRequest
                {
                    MaintenanceReqId = requestId,

                    TenantID = tenant.TenantId,
                    PropertyID = cmrequest.PropertyId,
                    UnitID = validUnitId,

                    Title = cmrequest.Title.Trim(),
                    PropertyName = property.Nickname,
                    Category = category,
                    Description = cmrequest.Description.Trim(),

                    Priority = "Medium",
                    Status = "Open",
                    ReportedAt = now,
                    CreatedDate = DateOnly.FromDateTime(now),
                    ResolvedAt = null,

                    AIPredictedCategory = prediction?.PredictedCategory,
                    AIConfidence = prediction?.Confidence,
                    AIModelVersion = prediction == null
                        ? null
                        : "MobileNetV3-Small-v1",

                    AICategoryConfirmed = prediction == null
                        ? null
                        : string.Equals(
                            category,
                            prediction.PredictedCategory,
                            StringComparison.OrdinalIgnoreCase),

                    MaintenanceImagePath = imagePath
                };

                string priority = _triageService.DeterminePriority(
                                    request.AIPredictedCategory,
                                    request.AIConfidence,
                                    request.Category,
                                    request.Title,
                                    request.Description
                );

                request.Priority = priority;

                _dbContext.MaintenanceRequests.Add(request);
                await _dbContext.SaveChangesAsync();

                return await GetMaintenanceRequestById(requestId)
                    ?? throw new InvalidOperationException(
                        "Maintenance request was saved but could not be retrieved.");
            }
            catch
            {
                // Clean up the file if database creation fails.
                if (absoluteImagePath != null &&
                    File.Exists(absoluteImagePath))
                {
                    File.Delete(absoluteImagePath);
                }

                throw;
            }
        }


        public async Task<List<MaintenanceRequestResponseDTO>> GetTenantMaintenanceRequests (string userId)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.UserId == userId);

            if (tenant == null)
                throw new Exception("Tenant profile not found");

            var requests = await _dbContext.MaintenanceRequests
                                .Where(m => m.TenantID == tenant.TenantId)
                                .Include(m => m.Property)
                                .Include(m => m.Unit)
                                .OrderByDescending(m => m.ReportedAt)
                                .Select(m => new MaintenanceRequestResponseDTO
                                {
                                    MaintenanceRequestId = m.MaintenanceReqId,
                                    TenantId = tenant.TenantId,
                                    TenantName = m.Tenant != null ? m.Tenant.FirstName + "" + m.Tenant.Surname : "Tenant",
                                    PropertyId = m.PropertyID,
                                    PropertyName = m.Property != null ? m.Property.Nickname : m.PropertyName,
                                    UnitId = m.UnitID,
                                    UnitNumber = m.Unit != null ? m.Unit.UnitNumber : 0,
                                    UnitName = m.Unit != null ? m.Unit.UnitName : null,
                                    Title = m.Title,
                                    Description = m.Description,
                                    Category = m.Category,
                                    Priority = m.Priority,
                                    Status = m.Status,
                                    ReportedAt = m.ReportedAt,
                                    AIPredictedCategory = m.AIPredictedCategory,
                                    AIConfidence = m.AIConfidence,
                                    AIModelVersion = m.AIModelVersion,
                                    AICategoryConfirmed = m.AICategoryConfirmed,
                                    MaintenanceImagePath = m.MaintenanceImagePath,

                                }).ToListAsync();

            return requests;
        }

        public async Task<bool> UpdateMaintenanceRequest(Guid mrId, UpdateMaintenanceRequestDTO umrequest, string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                throw new UnauthorizedAccessException(
                    "Only landlords can update maintenance requests.");

            var request = await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .FirstOrDefaultAsync(m => m.MaintenanceReqId == mrId);

            if (request == null)
                return false;

            if (request.Property == null ||
                request.Property.LandlordId != landlord.LandlordID)
            {
                throw new UnauthorizedAccessException(
                    "You cannot update another landlord's request.");
            }

            if (umrequest.Priority == null &&
                umrequest.Status == null)
            {
                throw new ArgumentException(
                    "Provide a priority or status to update.");
            }

            if (umrequest.Priority != null)
            {
                var priority = umrequest.Priority.Trim();

                if (!new[] { "Emergency", "Urgent", "Routine" }
                    .Contains(priority, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Invalid priority.");
                }

                request.Priority =
                    priority.Equals("Emergency", StringComparison.OrdinalIgnoreCase)
                        ? "Emergency"
                        : priority.Equals("Urgent", StringComparison.OrdinalIgnoreCase)
                            ? "Urgent"
                            : "Routine";
            }

            if (umrequest.Status != null)
            {
                var status = umrequest.Status.Trim();

                var validStatuses = new[]
                {
            "Open",
            "In Progress",
            "Resolved",
            "Closed"
        };

                if (!validStatuses.Contains(
                    status,
                    StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Invalid status.");
                }

                string canonicalStatus = validStatuses.First(
                    s => s.Equals(status, StringComparison.OrdinalIgnoreCase));

                bool wasResolved =
                    request.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase) ||
                    request.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
                    request.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase);

                request.Status = canonicalStatus;

                if (canonicalStatus == "Resolved" ||
                    canonicalStatus == "Closed")
                {
                    if (!wasResolved || request.ResolvedAt == null)
                        request.ResolvedAt = DateTime.UtcNow;
                }
                else
                {
                    request.ResolvedAt = null;
                }
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMaintenanceRequest(Guid mrId)
        {
            var request = await _dbContext.MaintenanceRequests.FindAsync(mrId);

            if (request == null)
                return false;

            _dbContext.MaintenanceRequests.Remove(request);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}

