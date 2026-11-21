using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.ML;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Interfaces;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class TenantApplicationService : ITenantApplicationService
    {
        private readonly AppDbContext _dbContext;

        public TenantApplicationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TenantApplicationPreviewdto> GetApplicationPreview (Guid advertisementId ,string userId)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.UserId == userId);

            if(tenant == null)
            {
                throw new InvalidOperationException("Tenant profile was not found");
            }

            var advertisement = await _dbContext.Advertisements
                                    .Include(a => a.Property)
                                    .Include(a => a.Unit)
                                    .FirstOrDefaultAsync(a => a.AdvertisementId == advertisementId);

            if(advertisement == null)
            {
                throw new InvalidOperationException("Advertisement was not found");
            }

            if(!string.Equals(advertisement.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This property is no longer available");
            }

            var existingApplication = await _dbContext.TenantApplications
                                            .AnyAsync(a => a.TenantId == tenant.TenantId &&
                                            a.AdvertisementId == advertisementId &&
                                            (
                                                a.Status == "Pending" ||
                                                a.Status == "Approved"
                                            ));

            if(existingApplication)
            {
                throw new InvalidOperationException("You already have an active application for this property");
            }

            var addressParts = new[]
            {
                advertisement.Property?.StreetNumber,
                advertisement.Property?.StreetName,
                advertisement.Property?.Suburb,
                advertisement.Property?.City,
                advertisement.Property?.Province

            }
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

            var address = string.Join(",", addressParts);

            return new TenantApplicationPreviewdto
            {
                AdvertismentId = advertisement.AdvertisementId,
                PropertyId = advertisement.PropertyId,
                UnitId = advertisement.UnitId,
                PropertyTitle = advertisement.Title,
                PropertyAddress = address,
                UnitName = advertisement.Unit?.UnitName,
                RentAmount = advertisement.RentAmount,
                TenantId = tenant.TenantId,
                FirstName = tenant.FirstName,
                Surname = tenant.Surname,
                Cellphone = tenant.Cellphone,
                MonthlyIncome = tenant.MonthlyIncome,
                MonthlyExpenses = tenant.MonthlyExpenses,
                AppScore = tenant.AppScore,
                AcceptanceScore = tenant.AppScore,

            };
        }

        public async Task <TenantApplicationResponseDTO> CreateApplication (CreateTenantApplicationdto ctadto, string userId)
        {
            if(ctadto == null)
            {
                throw new ArgumentException(nameof(ctadto));
            }

            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.UserId == userId);

            if(tenant == null)
            {
                throw new InvalidOperationException("Tenant profile was not found");
            }

            var advertisement = await _dbContext.Advertisements
                                        .Include(a => a.Property)
                                        .Include(a => a.Unit)
                                        .FirstOrDefaultAsync(a =>
                                            a.AdvertisementId == ctadto.AdvertisementId);

            if(advertisement == null)
            {
                throw new InvalidOperationException("Advertisment was not found");
            }

            if(!string.Equals(advertisement.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This property is no longer available");
            }

            var duplicate = await _dbContext.TenantApplications
                                        .AnyAsync(a => a.TenantId == tenant.TenantId &&
                                        a.AdvertisementId == ctadto.AdvertisementId &&
                                        (
                                            a.Status == "Pending" ||
                                            a.Status == "Approved"
                                        ));

            if(duplicate)
            {
                throw new InvalidOperationException("You already have an active applicaton for this property");
            }

            var application = new TenantApplication
            {
                TenantApplicationId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                AdvertisementId = advertisement.AdvertisementId,
                PropertyId = advertisement.PropertyId,
                UnitId = advertisement.UnitId,
                Status = "Pending",
                FirstName = tenant.FirstName,
                Surname = tenant.Surname,
                Cellphone = tenant.Cellphone,
                MonthlyIncome = tenant.MonthlyIncome,
                MonthlyExpenses = tenant.MonthlyExpenses,
                AppScore = tenant.AppScore,
                AcceptanceScore = tenant.AppScore,
                TenantMessage = ctadto.TenantMessage,
                SubmittedDate = DateTime.UtcNow,
            };

            _dbContext.TenantApplications.Add(application);

            await _dbContext.SaveChangesAsync();

            var property = await _dbContext.Properties
                            .FirstOrDefaultAsync(p => p.PropertyId == advertisement.PropertyId);

            if (property == null)
            {
                throw new Exception("Property not found.");
            }

            Unit? unit = null;

            if (application.UnitId.HasValue)
            {
                unit = await _dbContext.Units
                    .FirstOrDefaultAsync(u => u.UnitId == application.UnitId.Value);
            }

            return MapApplication(application, advertisement, property, unit);
        }

        public async Task<TenantApplicationResponseDTO?> GetApplication(Guid applicationId, string userId)
        {
            // 1. Find the logged-in landlord
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
            {
                throw new InvalidOperationException(
                    "Logged-in landlord could not be found."
                );
            }

            // 2. Find the application ONLY by its TenantApplicationId
            var application = await _dbContext.TenantApplications
                .FirstOrDefaultAsync(a =>
                    a.TenantApplicationId == applicationId);

            if (application == null)
            {
                return null;
            }

            // 3. Load the advertisement
            var advertisement = await _dbContext.Advertisements
                .FirstOrDefaultAsync(a =>
                    a.AdvertisementId == application.AdvertisementId);

            if (advertisement == null)
            {
                return null;
            }

            // 4. Load the property
            var property = await _dbContext.Properties
                .FirstOrDefaultAsync(p =>
                    p.PropertyId == advertisement.PropertyId);

            if (property == null)
            {
                return null;
            }

            // 5. SECURITY CHECK:
            // Make sure this property belongs to the logged-in landlord
            if (property.LandlordId != landlord.LandlordID)
            {
                return null;
            }

            // 6. Load unit if the advertisement has one
            Unit? unit = null;

            if (advertisement.UnitId.HasValue)
            {
                unit = await _dbContext.Units
                    .FirstOrDefaultAsync(u =>
                        u.UnitId == advertisement.UnitId.Value);
            }

            // 7. Map to response
            return MapApplication(
                application,
                advertisement,
                property,
                unit
            );
        }

        public async Task<List<LandlordApplicationPropertyDTO>> GetReceivedApplications(
    string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidOperationException(
                    "User identity was not found."
                );
            }

            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
            {
                throw new InvalidOperationException(
                    "Landlord profile was not found."
                );
            }

            Console.WriteLine(
                $"LANDLORD ID: {landlord.LandlordID}"
            );

            var advertisements = await _dbContext.Advertisements
                .Include(a => a.Property)
                .Include(a => a.Unit)
                .Where(a =>
                    a.Property != null &&
                    a.Property.LandlordId == landlord.LandlordID
                )
                .ToListAsync();

            Console.WriteLine(
                $"LANDLORD ADVERTISEMENTS FOUND: {advertisements.Count}"
            );


            foreach (var advertisement in advertisements)
            {
                Console.WriteLine(
                    $"ADVERTISEMENT: " +
                    $"ID={advertisement.AdvertisementId}, " +
                    $"PROPERTY={advertisement.PropertyId}, " +
                    $"UNIT={advertisement.UnitId}, " +
                    $"TITLE={advertisement.Title}"
                );
            }

            var advertisementIds = advertisements
                .Select(a => a.AdvertisementId)
                .ToList();

            var applications = await _dbContext.TenantApplications
                .Where(a => advertisementIds.Contains(a.AdvertisementId))
                .ToListAsync();

            Console.WriteLine(
                $"APPLICATIONS FOR LANDLORD ADVERTISEMENTS: " +
                $"{applications.Count}"
            );


            foreach (var application in applications)
            {
                Console.WriteLine(
                    $"APPLICATION: " +
                    $"ID={application.TenantApplicationId}, " +
                    $"ADVERTISEMENT={application.AdvertisementId}, " +
                    $"PROPERTY={application.PropertyId}, " +
                    $"UNIT={application.UnitId}, " +
                    $"TENANT={application.TenantId}, " +
                    $"STATUS={application.Status}"
                );
            }

            var result =
                new List<LandlordApplicationPropertyDTO>();


            foreach (var advertisement in advertisements)
            {
                var property = advertisement.Property;

                if (property == null)
                {
                    continue;
                }

                var propertyApplications = applications
                    .Where(a =>
                        a.AdvertisementId ==
                        advertisement.AdvertisementId
                    )
                    .ToList();


                Console.WriteLine(
                    $"ADVERTISEMENT {advertisement.AdvertisementId} " +
                    $"HAS {propertyApplications.Count} APPLICATION(S)"
                );

                var addressParts = new[]
                {
            property.StreetNumber,
            property.StreetName,
            property.Suburb,
            property.City,
            property.Province
        }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

                var address = string.Join(", ", addressParts);

                var applicationdtos =
                    new List<TenantApplicationResponseDTO>();

                foreach (var application in propertyApplications)
                {
                    Unit? unit = null;

                    if (application.UnitId.HasValue)
                    {
                        unit = await _dbContext.Units
                            .FirstOrDefaultAsync(
                                u => u.UnitId ==
                                     application.UnitId.Value
                            );
                    }


                    applicationdtos.Add(
                        new TenantApplicationResponseDTO
                        {
                            TenantApplicationId =
                                application.TenantApplicationId,

                            TenantId =
                                application.TenantId,

                            AdvertisementId =
                                application.AdvertisementId,

                            PropertyId =
                                application.PropertyId,

                            UnitId =
                                application.UnitId,

                            PropertyTitle =
                                advertisement.Title,

                            PropertyAddress =
                                address,

                            UnitName =
                                unit?.UnitName,

                            RentAmount =
                                advertisement.RentAmount,

                            FirstName =
                                application.FirstName,

                            Surname =
                                application.Surname,

                            Cellphone =
                                application.Cellphone,

                            MonthlyIncome =
                                application.MonthlyIncome,

                            MonthlyExpenses =
                                application.MonthlyExpenses,

                            AppScore =
                                application.AppScore,

                            AcceptanceScore =
                                application.AcceptanceScore,

                            PredictionLabel =
                                application.PredictionLabel,

                            Status =
                                application.Status,

                            SubmittedDate =
                                application.SubmittedDate,

                            UpdatedDate =
                                application.UpdatedDate
                        }
                    );
                }

                result.Add(
                    new LandlordApplicationPropertyDTO
                    {
                        AdvertisementId =
                            advertisement.AdvertisementId,

                        PropertyId =
                            advertisement.PropertyId,

                        UnitId =
                            advertisement.UnitId,

                        PropertyTitle =
                            advertisement.Title,

                        PropertyAddress =
                            address,

                        UnitName =
                            advertisement.Unit?.UnitName,

                        RentAmount =
                            advertisement.RentAmount,

                        Status =
                            advertisement.Status,

                        ImageUrl =
                            null,

                        ApplicationCount =
                            applicationdtos.Count,

                        PendingApplicationCount =
                            applicationdtos.Count(
                                a =>
                                    string.Equals(
                                        a.Status,
                                        "Pending",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                            ),

                        Applications =
                            applicationdtos
                    }
                );


                Console.WriteLine(
                    $"PROPERTY RESULT: " +
                    $"Advertisement={advertisement.AdvertisementId}, " +
                    $"Property={advertisement.PropertyId}, " +
                    $"Applications={applicationdtos.Count}"
                );
            }


            Console.WriteLine(
                $"FINAL PROPERTY RESULTS: {result.Count}"
            );


            foreach (var item in result)
            {
                Console.WriteLine(
                    $"FINAL PROPERTY: " +
                    $"Advertisement={item.AdvertisementId}, " +
                    $"Property={item.PropertyId}, " +
                    $"Applications={item.ApplicationCount}"
                );
            }


            return result;
        }

        public async Task<TenantApplicationResponseDTO> UpdateApplicationStatus(Guid applicationId, string userId, string status)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidOperationException(
                    "User identity was not found."
                );
            }

            if (!string.Equals(status, "Accepted", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Application status must be Accepted or Rejected."
                );
            }

            // Find landlord
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
            {
                throw new InvalidOperationException(
                    "Landlord profile was not found."
                );
            }

            // Find application
            var application = await _dbContext.TenantApplications
                .FirstOrDefaultAsync(
                    a => a.TenantApplicationId == applicationId
                );

            if (application == null)
            {
                throw new InvalidOperationException(
                    "Application was not found."
                );
            }

            // Find the advertisement/property
            var advertisement = await _dbContext.Advertisements
                .Include(a => a.Property)
                .Include(a => a.Unit)
                .FirstOrDefaultAsync(
                    a => a.AdvertisementId == application.AdvertisementId
                );

            if (advertisement == null)
            {
                throw new InvalidOperationException(
                    "Advertisement was not found."
                );
            }

            if (advertisement.Property == null)
            {
                throw new InvalidOperationException(
                    "Property associated with the application was not found."
                );
            }

            // SECURITY CHECK:
            // Make sure this property belongs to the logged-in landlord.
            if (advertisement.Property.LandlordId != landlord.LandlordID)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorised to manage this application."
                );
            }

            // Only pending applications can be accepted/rejected.
            if (!string.Equals(
                    application.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"This application has already been {application.Status?.ToLower()}."
                );
            }

            // Update status
            application.Status =
                string.Equals(
                    status,
                    "Accepted",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Accepted"
                    : "Rejected";

            application.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            // Return the updated application
            return new TenantApplicationResponseDTO
            {
                TenantApplicationId =
                    application.TenantApplicationId,

                TenantId =
                    application.TenantId,

                AdvertisementId =
                    application.AdvertisementId,

                PropertyId =
                    application.PropertyId,

                UnitId =
                    application.UnitId,

                PropertyTitle =
                    advertisement.Title,

                PropertyAddress =
                    $"{advertisement.Property?.StreetNumber} " +
                    $"{advertisement.Property?.StreetName}, " +
                    $"{advertisement.Property?.Suburb}, " +
                    $"{advertisement.Property?.City}, " +
                    $"{advertisement.Property?.Province}",

                UnitName =
                    advertisement.Unit?.UnitName,

                RentAmount =
                    advertisement.RentAmount,

                FirstName =
                    application.FirstName,

                Surname =
                    application.Surname,

                Cellphone =
                    application.Cellphone,

                MonthlyIncome =
                    application.MonthlyIncome,

                MonthlyExpenses =
                    application.MonthlyExpenses,

                AppScore =
                    application.AppScore,

                AcceptanceScore =
                    application.AcceptanceScore,

                PredictionLabel =
                    application.PredictionLabel,

                Status =
                    application.Status,

                TenantMessage =
                    application.TenantMessage,

                SubmittedDate =
                    application.SubmittedDate,

                UpdatedDate =
                    application.UpdatedDate
            };
        }

        public async Task<List<TenantApplicationResponseDTO>> GetMyApplications (string userId)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.UserId == userId);

            if(tenant == null)
            {
                throw new InvalidOperationException("Tenant profile was not found");
            }

            var applications = await _dbContext.TenantApplications
                                    .Where(a => a.TenantId == tenant.TenantId)
                                    .Include(a => a.Advertisement)
                                        .ThenInclude(ad => ad.Property)
                                    .Include(a => a.Advertisement)
                                        .ThenInclude(ad => ad.Unit)
                                    .OrderByDescending(a => a.SubmittedDate)
                                    .ToListAsync();

            return applications.Select(a => new TenantApplicationResponseDTO
            {
                TenantApplicationId = a.TenantApplicationId,
                TenantId = a.TenantId,
                AdvertisementId = a.AdvertisementId,
                PropertyId = a.Advertisement.PropertyId,
                UnitId = a.Advertisement.UnitId,
                PropertyTitle = a.Advertisement.Property.Nickname,
                PropertyAddress = string.Join(",", new[]
                {
                    a.Advertisement.Property.StreetNumber,
                    a.Advertisement.Property.StreetName,
                    a.Advertisement.Property.Suburb,
                    a.Advertisement.Property.City,
                    a.Advertisement.Property.Province
                }.Where(x => !string.IsNullOrWhiteSpace(x))),
                UnitName = a.Advertisement.Unit != null ? a.Advertisement.Unit.UnitName : null,
                RentAmount = a.Advertisement.Unit != null ? a.Advertisement.Unit.RentAmount : a.Advertisement.Property.RentAmount,
                FirstName = a.FirstName,
                Surname = a.Surname,
                Email = a.Email,
                Cellphone = a.Cellphone,
                MonthlyIncome = a.MonthlyIncome,
                MonthlyExpenses = a.MonthlyExpenses,
                AppScore = a.AppScore,
                AcceptanceScore = a.AcceptanceScore,
                PredictionLabel = a.PredictionLabel,
                Status = a.Status,
                TenantMessage = a.TenantMessage,
                SubmittedDate = a.SubmittedDate,
                UpdatedDate = a.UpdatedDate,
            }).ToList();

        }


        private string GetPredictionLabel(int score)
        {
            if(score >= 75)
            {
                return "High";
            }
            
            if (score >= 50)
            {
                return "Moderate";
            }

            return "Low";
        }

        private TenantApplicationResponseDTO MapApplication(TenantApplication application, Advertisement advertisement, Property property, Unit? unit)
        {
            var addressParts = new[]
            {
                property.StreetNumber,
                property.StreetName,
                property.Suburb,
                property.City,
                property.Province
            }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

            var address = string.Join(", ", addressParts);

            return new TenantApplicationResponseDTO
            {
               
                TenantApplicationId = application.TenantApplicationId,
                TenantId = application.TenantId,

                AdvertisementId = advertisement.AdvertisementId,
                PropertyId = property.PropertyId,
                UnitId = unit?.UnitId,

                PropertyTitle = advertisement.Title,

                PropertyAddress = address,

                UnitName = unit?.UnitName,

                RentAmount = advertisement.RentAmount,

                
                FirstName = application.FirstName,
                Surname = application.Surname,
                Cellphone = application.Cellphone,

                
                MonthlyIncome = application.MonthlyIncome,
                MonthlyExpenses = application.MonthlyExpenses,

                
                AppScore = application.AppScore,
                AcceptanceScore = application.AcceptanceScore,
                PredictionLabel = application.PredictionLabel,

                
                Status = application.Status,

                TenantMessage = application.TenantMessage,

                SubmittedDate = application.SubmittedDate,

                UpdatedDate = application.UpdatedDate
            };
        }
    }
}
