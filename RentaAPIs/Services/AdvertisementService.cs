using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using RentaAPIs.cdDTOs;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class AdvertisementService
    {
        private readonly AppDbContext _dbContext;

        public AdvertisementService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AdvertisementResponseDTO> CreateAdvertisement(string userId, CreateAdvertisementDTO cadto)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                throw new Exception("Landlord profile not found");


            var property = await _dbContext.Properties
                .Include(p => p.Amenities)
                .FirstOrDefaultAsync(p =>
                    p.PropertyId == cadto.PropertyId &&
                    p.LandlordId == landlord.LandlordID);

            if (property == null)
                throw new Exception(
                    "Property not found or does not belong to this landlord"
                );

            Unit? unit = null;

            if (cadto.UnitId.HasValue)
            {
                unit = await _dbContext.Units
                    .FirstOrDefaultAsync(u =>
                        u.UnitId == cadto.UnitId.Value &&
                        u.PropertyId == cadto.PropertyId);

                if (unit == null)
                {
                    throw new Exception(
                        "Unit not found or does not belong to this property"
                    );
                }

                if (unit.Status == "Occupied")
                {
                    throw new Exception(
                        "An occupied unit cannot be advertised"
                    );
                }

                unit.Status = "Advertised";
            }

            var advertisement = new Advertisement
            {
                AdvertisementId = Guid.NewGuid(),

                PropertyId = cadto.PropertyId,

                UnitId = cadto.UnitId,

                Title = cadto.Title,

                Description = cadto.Description,

                RentAmount = cadto.RentAmount,

                Deposit = cadto.Deposit,

                Status = "Active",

                CreatedDate = DateTime.UtcNow,

                ExpiryDate = cadto.ExpiryDate
            };

            _dbContext.Advertisements.Add(advertisement);

            property.Status = "Advertised";

            await _dbContext.SaveChangesAsync();

            return new AdvertisementResponseDTO
            {
                AdvertisementId = advertisement.AdvertisementId,
                PropertyId = advertisement.PropertyId,
                UnitId = advertisement.UnitId,
                Title = advertisement.Title,
                Description = advertisement.Description,
                RentAmount = advertisement.RentAmount,
                Deposit = advertisement.Deposit,
                Status = advertisement.Status,
                CreatedDate = advertisement.CreatedDate,
                ExpiryDate = advertisement.ExpiryDate,
                PropertyName = property.Nickname,
                Amenities = property.Amenities.Select(a => a.AmenityName).ToList(),
                UnitName = unit?.UnitName,
                Address =
                    $"{property.StreetNumber} " +
                    $"{property.StreetName}, " +
                    $"{property.Suburb}, " +
                    $"{property.City}"
            };
        }


        public async Task<List<AdvertisementResponseDTO>> GetAllAdvertisements()
        {
            var advertisements = await _dbContext.Advertisements
                .Include(a => a.Property)
                .Include(a => a.Unit)
                .Where(a =>
    a.Status == "Active" &&
    !_dbContext.Leases.Any(l =>
        l.PropertyID == a.PropertyId &&
        l.UnitId == a.UnitId &&
        l.IsSignedByTenant &&
        (l.Status == "SignedByTenant" ||
         l.Status == "Signed" ||
         l.Status == "Active")))
                .Select(a => new AdvertisementResponseDTO
                {
                    AdvertisementId = a.AdvertisementId,
                    PropertyId = a.PropertyId,
                    Title = a.Title,
                    PropertyName = a.Property != null ? a.Property.Nickname : null,
                    StreetName = a.Property != null ? a.Property.StreetName : null,
                    StreetNumber = a.Property != null ? a.Property.StreetNumber : null,
                    City = a.Property != null ? a.Property.City : null,
                    Province = a.Property != null ? a.Property.Province : null,
                    UnitId = a.UnitId,
                    UnitName = a.Unit != null ? a.Unit.UnitName : null,
                    Bedrooms = a.Unit != null ? a.Unit.Bedrooms : a.Property.Bedrooms,
                    Bathrooms = a.Unit != null ? a.Unit.Bathrooms : a.Property.Bathrooms,
                    RentAmount = a.Unit != null ? a.Unit.RentAmount : a.Property.RentAmount,
                    Status = a.Status,
                }).ToListAsync();

            return advertisements;
        }

        public async Task<IEnumerable<AdvertisementResponseDTO>> GetMyAdvertisements(string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                throw new Exception("Landlord profile not found");

            var advertisements = await _dbContext.Advertisements
                .Include(a => a.Property)
                .Include(a => a.Unit)
                .Where(a => a.Property.LandlordId == landlord.LandlordID)
                .ToListAsync();

            return advertisements.Select(a => new AdvertisementResponseDTO
            {
                AdvertisementId = a.AdvertisementId,
                PropertyId = a.PropertyId,
                UnitId = a.UnitId,
                Title = a.Title,
                Description = a.Description,
                RentAmount = a.RentAmount,
                Status = a.Status,
                CreatedDate = a.CreatedDate,
                ExpiryDate = a.ExpiryDate,
                PropertyName = a.Property.Nickname,
                UnitName = a.Unit != null ? a.Unit.UnitName : null,
                Address = $"{a.Property.StreetNumber}" + $"{a.Property.StreetName}" + $"{a.Property.Suburb}" + $"{a.Property.City}"
            });
        }

        public async Task<AdvertisementResponseDTO> GetAdvertisement(Guid advertisementId)
        {
            var advertisement = await _dbContext.Advertisements
                .Include(a => a.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(a => a.Property)
                    .ThenInclude(p => p.Amenities)
                .Include(a => a.Unit)
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.AdvertisementId == advertisementId
                );

            if (advertisement == null)
            {
                throw new Exception("Advertisement not found");
            }

            var property = advertisement.Property;
            var unit = advertisement.Unit;
            var landlord = property?.Landlord;

            return new AdvertisementResponseDTO
            {
                AdvertisementId = advertisement.AdvertisementId,

                PropertyId = advertisement.PropertyId,

                UnitId = advertisement.UnitId,

                Title = advertisement.Title,

                Description = property?.Description,

                RentAmount = advertisement.RentAmount,

                Deposit = advertisement.Deposit,

                Status = advertisement.Status,

                CreatedDate = advertisement.CreatedDate,

                ExpiryDate = advertisement.ExpiryDate,

                PropertyName = property?.Nickname,

                UnitName = unit?.UnitName,

                Bedrooms = unit != null
                    ? unit.Bedrooms
                    : property?.Bedrooms,

                Bathrooms = unit != null
                    ? unit.Bathrooms
                    : property?.Bathrooms,

                StreetName = property?.StreetName,

                StreetNumber = property?.StreetNumber,

                City = property?.City,

                Suburb = property?.Suburb,

                Province = property?.Province,

                PropertyType = property?.PropertyType,

                Address = property != null
                    ? $"{property.StreetNumber} " +
                      $"{property.StreetName}, " +
                      $"{property.Suburb}, " +
                      $"{property.City}, " +
                      $"{property.Province}"
                    : null,

                LandlordId = property?.LandlordId,

                LandlordName = landlord != null
                    ? $"{landlord.FirstName} {landlord.Surname}"
                    : null,

                LandlordEmail = landlord?.Email,

                LandlordPhone = landlord?.Cellphone,

                // Load amenities associated with this property.
                Amenities = property?.Amenities
                    .Select(a => a.AmenityName)
                    .ToList() ?? new List<string>()
            };
        }

        public async Task<bool> DeleteAdvertisement(Guid advertisementId, string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                throw new Exception("Landlord profile not found");

            var advertisement = await _dbContext.Advertisements
                .Include(a => a.Property)
                .FirstOrDefaultAsync(a =>
                        a.AdvertisementId == advertisementId &&
                        a.Property.LandlordId == landlord.LandlordID);

            if (advertisement == null)
                return false;

            _dbContext.Advertisements.Remove(advertisement);

            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}
