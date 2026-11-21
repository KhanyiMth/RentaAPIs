using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{ 
    public class LandlordService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public LandlordService(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<LandlordResponseDTO> GetLandlordById (Guid id)
        {
            return await _dbContext.Landlords
                .Where(l => l.LandlordID == id)
                .Select(l => new LandlordResponseDTO
                {
                    LandlordID = l.LandlordID,
                    FirstName = l.FirstName,
                    Surname = l.Surname,
                    Cellphone = l.Cellphone,
                    IDNumber = l.IDNumber,

                }).FirstOrDefaultAsync();
        }


        public async Task<LandlordResponseDTO> GetMe(string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
            {
                throw new Exception("Landlord profile not found");
            }

            var identityUser = await _userManager.FindByIdAsync(landlord.UserId);

            return new LandlordResponseDTO
            {
                LandlordID = landlord.LandlordID,
                UserId = landlord.UserId,
                FirstName = landlord.FirstName,
                Surname = landlord.Surname,
                Email = identityUser?.Email ?? string.Empty,
                Cellphone = landlord.Cellphone,
                IDNumber = landlord.IDNumber
            };
        }


        public async Task CreateLandlord (RegisterLandlordDTO registerLandlord)
        {
            var user = new AppUser
            {
                UserName = registerLandlord.Email,
                Email = registerLandlord.Email,
                PhoneNumber = registerLandlord.Cellphone
            };

            var result = await _userManager.CreateAsync(user, registerLandlord.Password);

            if(!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                throw new Exception(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Landlord");

            if(!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

                throw new Exception(errors);
            }

            var newLandlord = new Landlord
            {
                UserId = user.Id,
                FirstName = registerLandlord.FirstName,
                Surname = registerLandlord.Surname,
                Cellphone = registerLandlord.Cellphone,
                IDNumber = registerLandlord.IDNumber,
            };

            _dbContext.Landlords.Add(newLandlord);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateLandlord (Guid id, UpdateLandlordDTO ulDTO)
        {
            var landlord = await _dbContext.Landlords.FindAsync(id);

            if (landlord == null)
                return false;

            landlord.FirstName = ulDTO.FirstName;
            landlord.Surname = ulDTO.Surname;
            landlord.Cellphone = ulDTO.Cellphone;

            await _dbContext.SaveChangesAsync();

            return true;
        }


    }
}
