using Microsoft.EntityFrameworkCore;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class UnitService
    {

        private readonly AppDbContext _dbContext;

        public UnitService (AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UnitResponseDTO>> GetAllUnits()
        {
            var allUnits = await _dbContext.Units.ToListAsync();

            return allUnits.Select(u => new UnitResponseDTO
            {
                UnitId = u.UnitId,
                PropertyId = u.PropertyId,
                UnitNumber = u.UnitNumber,
                RentAmount = u.RentAmount,
                Status = u.Status,
            }).ToList();
        }

        public async Task<UnitResponseDTO> GetUnitById (Guid id)
        {
            return await _dbContext.Units
                .Where(u => u.UnitId == id)
                .Select(u => new UnitResponseDTO
                {
                    UnitId = u.UnitId,
                    PropertyId = u.PropertyId,
                    UnitNumber = u.UnitNumber,
                    RentAmount = u.RentAmount,
                    Status = u.Status,
                }).FirstOrDefaultAsync();
        }

        public async Task <IEnumerable<UnitResponseDTO>> GetUnitsByPropertyID (Guid propertyId)
        {
            var propertyExists = await _dbContext.Properties
                .AnyAsync(p => p.PropertyId == propertyId);

            if(!propertyExists)
            {
                throw new KeyNotFoundException($"Property with ID {propertyId} was not found");
            }

            var units = await _dbContext.Units
                .Where(u => u.PropertyId == propertyId)
                .Select(u => new UnitResponseDTO
                {
                    UnitId = u.UnitId,
                    PropertyId = u.PropertyId,
                    UnitName = u.UnitName,
                    Bedrooms = u.Bedrooms,
                    Bathrooms = u.Bathrooms,
                    Parking = u.Parking,
                    Description = u.Description,
                    RentAmount = u.RentAmount,
                    Deposit = u.Deposit,
                    Status = u.Status,
                }).ToListAsync();

            return units;
        }

        public async Task<UnitResponseDTO> CreateUnit (CreateUnitDTO urDTO)
        {
            var property = await _dbContext.Properties.FindAsync(urDTO.PropertyId);

            if (property == null)
                throw new Exception("Property not found");

            var unit = new Unit
            {
                PropertyId = urDTO.PropertyId,
                UnitName = urDTO.UnitName,
                UnitNumber = urDTO.UnitNumber,
                Parking = urDTO.Parking,
                Bedrooms = urDTO.Bedrooms,
                Bathrooms = urDTO.Bathrooms,
                RentAmount = urDTO.RentAmount,
                Description = urDTO.Description,
                Deposit = urDTO.Deposit,
                Status = urDTO.Status,
            };

            _dbContext.Units.Add(unit);
            await _dbContext.SaveChangesAsync();

            return new UnitResponseDTO
            {
                UnitId = unit.UnitId,
                PropertyId = unit.PropertyId,
                UnitNumber = unit.UnitNumber,
                UnitName = unit.UnitName,
                Parking = unit.Parking,
                Bedrooms = unit.Bedrooms,
                Bathrooms = unit.Bathrooms,
                RentAmount = unit.RentAmount,
                Deposit = unit.Deposit,
                Description= unit.Description,
                Status = unit.Status
            };

        }

        public async Task<bool> UpdateUnit (Guid id, UpdateUnitDTO uuDTO)
        {
            var unit = await _dbContext.Units.FindAsync(id);

            if (unit == null)
                return false;

            unit.UnitNumber = uuDTO.UnitNumber;
            unit.Parking = uuDTO.Parking;
            unit.Bathrooms = uuDTO.Bathrooms;
            unit.Bedrooms = uuDTO.Bedrooms;
            unit.RentAmount = uuDTO.RentAmount;
            unit.Deposit = uuDTO.Deposit;
            unit.Description = uuDTO.Description;
            unit.Status = uuDTO.Status;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUnit (Guid id)
        {
            var unit = await _dbContext.Units.FindAsync(id);

            if (unit == null)
                return false;

            _dbContext.Units.Remove(unit);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
