using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaAPIs.cdDTOs;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class PropertyService
    {
        private readonly AppDbContext _dbContext;

        public PropertyService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PropertyResponseDTO>> GetAllProperties()
        {
            var allProperties = await _dbContext.Properties.ToListAsync();

            return allProperties.Select(p => new PropertyResponseDTO
            {
                PropertyId = p.PropertyId,
                Nickname = p.Nickname,
                Description = p.Description,
                StreetNumber = p.StreetNumber,
                StreetName = p.StreetName,
                Suburb = p.Suburb,
                City = p.City,
                Province = p.Province,
                PropertyType = p.PropertyType,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Deposit = p.Deposit,
                RentAmount = p.RentAmount,
                Status = p.Status

            }).ToList();
        }

        public async Task<PropertyResponseDTO> GetPropertyById (Guid id)
        {
            var property = await _dbContext.Properties
                .Include(p => p.Units)
                .Include(p => p.Amenities)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return null;

            var advertisements = await _dbContext.Advertisements
                                        .Where(a => a.PropertyId == id)
                                        .ToListAsync();

            var leases = await _dbContext.Leases
                                .Where(l => l.PropertyID == id)
                                .ToListAsync();

            var signedLeases = leases.Where(l =>
                                !string.IsNullOrWhiteSpace(l.Status) &&
                                (
                                    l.Status.ToLower() == "signed" ||
                                    l.Status.ToLower() == "signedbytenant" ||
                                    l.Status.ToLower() == "active" ||
                                    l.Status.ToLower() == "completed"
                                )
                                ).ToList();

            var signedLeaseDTOs = new List<SignedLeaseSummaryDTO>();

            foreach(var lease in signedLeases)
            {
                var tenant = await _dbContext.Tenants
                                    .FirstOrDefaultAsync(t => t.TenantId == lease.TenantID);

                TenantSummaryDTO tenantDTO = null;

                if(tenant != null)
                {
                    tenantDTO = new TenantSummaryDTO
                    {
                        TenantId = tenant.TenantId,
                        Name = $"{tenant.FirstName} {tenant.Surname}".Trim(),
                        Phone = tenant.Cellphone
                    };
                }

                signedLeaseDTOs.Add(new SignedLeaseSummaryDTO
                {
                    LeaseId = lease.LeaseId,
                    TenantId = lease.TenantID,
                    PropertyId = lease.PropertyID,
                    UnitId = lease.UnitId,
                    StartDate = lease.StartDate,
                    EndDate = lease.EndDate,
                    MonthlyRent = lease.MonthlyRent,
                    DepositAmount = lease.DepositAmount,
                    PaymentFrequency = lease.PaymentFrequency,
                    LeaseTerm = lease.LeaseTerm,
                    Status = lease.Status,
                    TenantSignedAt = lease.TenantSignedAt,
                    ReturnedToLandlordAt = lease.ReturnedToLandlordAt,
                    Tenant = tenantDTO
                });
            }

            return new PropertyResponseDTO
            {
                PropertyId = property.PropertyId,
                landlordID = property.LandlordId,
                Nickname = property.Nickname,
                StreetNumber = property.StreetNumber,
                StreetName = property.StreetName,
                Suburb = property.Suburb,
                City = property.City,
                Province = property.Province,
                PropertyType = property.PropertyType,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Parking = property.Parking,
                RentAmount = property.RentAmount,
                Deposit = property.Deposit,
                Description = property.Description,
                Status = property.Status,

                isMultiUnit = property.Units != null && property.Units.Count > 0,
                IsAdvertised = advertisements.Any(),

                Advertisements = advertisements.Select(a => new AdvertisementSummaryDTO
                {
                    AdvertisementId = a.AdvertisementId,
                    PropertyId = a.PropertyId,
                    UnitId = a.UnitId,
                    Status = GetAdvertisementStatus(a),
                    IsActive = IsAdvertisementActive(a)
                }).ToList(),

                HasSignedLease = signedLeaseDTOs.Any(),
                SignedLeases = signedLeaseDTOs,

                Units = property.Units.Select(unit => new UnitResponseDTO
                {
                    UnitId = unit.UnitId,
                    PropertyId = unit.PropertyId,
                    UnitName = unit.UnitName,
                    Bedrooms = unit.Bedrooms,
                    Bathrooms = unit.Bathrooms,
                    Parking = unit.Parking,
                    RentAmount = unit.RentAmount,
                    Deposit = unit.Deposit,
                    Description = unit.Description,
                    Status = unit.Status
                }).ToList(),

                Amenities = property.Amenities.Select(a => new AmenityDTO
                {
                    Id = a.Id,
                    Name = a.AmenityName
                }).ToList(),
            };
        }

        private string GetAdvertisementStatus(dynamic advertisement)
        {
            try
            {
                var statusProperty = advertisement.GetType().GetProperty("Status");

                if(statusProperty != null)
                {
                    return statusProperty.GetValue(advertisement)?.ToString();
                }
            }
            catch
            {

            }

            return "Advertised";
        }

        private bool IsAdvertisementActive(dynamic advertisement)
        {
            try
            {
                var isActiveProperty = advertisement.GetType().GetProperty("IsActive");

                if(isActiveProperty != null)
                {
                    var value = isActiveProperty.GetValue(advertisement);

                    if (value != null)
                        return Convert.ToBoolean(value);
                }

                var statusProperty = advertisement.GetType().GetProperty("Status");

                if(statusProperty != null)
                {
                    var status = statusProperty.GetValue(advertisement)?.ToString().ToLower();

                    return status == "active" ||
                           status == "advertised" ||
                           status == "available" ||
                           status == "listed";
                }
            }
            catch
            {

            }

            return true;
        }

        public async Task<PropertyResponseDTO> CreateProperty(
    string userId,
    CreatePropertyDTO createDTO
)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
            {
                throw new Exception("Landlord profile not found.");
            }

            var amenityNames = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase
            )
    {
        { "pool", "Pool" },
        { "garden", "Garden" },
        { "geyser", "Geyser" },
        { "stove", "Stove" },
        { "builtins", "Built-in Cupboards" },
        { "furnished", "Furnished" },
        { "unfurnished", "Unfurnished" },
        { "parking", "Parking" },
        { "petFriendly", "Pet Friendly" },
        { "security", "24/7 Security" }
    };

            var selectedNames = (createDTO.Amenities ?? new List<string>())
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Select(key =>
                {
                    var cleanKey = key.Trim();

                    return amenityNames.TryGetValue(
                        cleanKey,
                        out var displayName
                    )
                        ? displayName
                        : cleanKey;
                })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingAmenities = await _dbContext.Set<Amenity>()
                .Where(a => selectedNames.Contains(a.AmenityName))
                .ToListAsync();

            var existingNames = existingAmenities
                .Select(a => a.AmenityName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newAmenities = selectedNames
                .Where(name => !existingNames.Contains(name))
                .Select(name => new Amenity
                {
                    Id = Guid.NewGuid(),
                    AmenityName = name
                })
                .ToList();

            var propertyAmenities = existingAmenities
                .Concat(newAmenities)
                .ToList();

            var newProperty = new Property
            {
                PropertyId = Guid.NewGuid(),

                LandlordId = landlord.LandlordID,

                Nickname = createDTO.Nickname,
                Description = createDTO.Description,

                StreetName = createDTO.StreetName,
                StreetNumber = createDTO.StreetNumber,

                City = createDTO.City,
                Suburb = createDTO.Suburb,
                Province = createDTO.Province,

                PropertyType = createDTO.PropertyType,

                Bedrooms = createDTO.Bedrooms,
                Bathrooms = createDTO.Bathrooms,
                Parking = createDTO.Parking,

                RentAmount = createDTO.RentAmount,
                Deposit = createDTO.Deposit,

                Status = createDTO.Status,
                CreatedDate = DateTime.UtcNow,

                Amenities = propertyAmenities
            };

            _dbContext.Properties.Add(newProperty);

            await _dbContext.SaveChangesAsync();

            return new PropertyResponseDTO
            {
                PropertyId = newProperty.PropertyId,

                Nickname = newProperty.Nickname,
                Description = newProperty.Description,

                StreetNumber = newProperty.StreetNumber,
                StreetName = newProperty.StreetName,

                Suburb = newProperty.Suburb,
                City = newProperty.City,
                Province = newProperty.Province,

                PropertyType = newProperty.PropertyType,

                Bedrooms = newProperty.Bedrooms,
                Bathrooms = newProperty.Bathrooms,
                Parking = newProperty.Parking,

                Deposit = newProperty.Deposit,
                RentAmount = newProperty.RentAmount,

                Status = newProperty.Status,

                Amenities = newProperty.Amenities
                    .Select(a => new AmenityDTO
                    {
                        Id = a.Id,
                        Name = a.AmenityName
                    })
                    .ToList()
            };
        }

        public async Task<IEnumerable<PropertyResponseDTO>> GetMyProperties(string userId)
        {
            var landlord = await _dbContext.Landlords.FirstOrDefaultAsync(l => l.UserId == userId);

            if(landlord == null)
            {
                throw new Exception("Landlord profile not found");
            }

            var properties = await _dbContext.Properties
                .Include(p => p.Amenities)
                .Where(p => p.LandlordId == landlord.LandlordID)
                .ToListAsync();

            return properties.Select(p => new PropertyResponseDTO
            {
                PropertyId = p.PropertyId,
                landlordID = p.LandlordId,
                Nickname = p.Nickname,
                Description = p.Description,
                StreetName = p.StreetName,
                StreetNumber = p.StreetNumber,
                City = p.City,
                Suburb = p.Suburb,
                Province = p.Province,
                PropertyType = p.PropertyType,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                RentAmount = p.RentAmount,
                Deposit = p.Deposit,
                Status = p.Status,

            });
        }

        public async Task<bool> UpdateProperty (Guid propertyId, UpdatePropertyDTO updateDTO)
        {
            var property = await _dbContext.Properties.FindAsync(propertyId);

            if (property == null)
                return false;

            property.Nickname = updateDTO.Nickname;
            property.Description = updateDTO.Description;
            property.StreetName = updateDTO.StreetName;
            property.StreetNumber = updateDTO.StreetNumber;
            property.City = updateDTO.City;
            property.Suburb = updateDTO.Suburb;
            property.Province = updateDTO.Province;
            property.PropertyType = updateDTO.PropertyType;
            property.Bedrooms = updateDTO.Bedrooms;
            property.Bathrooms = updateDTO.Bathrooms;
            property.RentAmount = updateDTO.RentAmount;
            property.Deposit = updateDTO.Deposit;
            property.Status = updateDTO.Status;

            await _dbContext.SaveChangesAsync();

            return true;           
        }

        public async Task<bool> DeleteProperty (Guid propertyId)
        {
            var property = await _dbContext.Properties.FindAsync(propertyId);

            if (property == null)
                return false;

            _dbContext.Properties.Remove(property);
            
            await _dbContext.SaveChangesAsync();

            return true;

        }

    }
}
