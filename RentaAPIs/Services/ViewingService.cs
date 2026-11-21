using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using RentaAPIs.cdDTOs;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class ViewingService
    {
        private readonly AppDbContext _dbContext;

        public ViewingService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ViewingAvailability> CreateAvailability(CreateViewingAvailabilityDTO cvdto, Guid userId)
        {

            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId.ToString());

            if(landlord == null)
            {
                throw new KeyNotFoundException("Landlord account could not be found for this user.");
            }
            
            var property = await _dbContext.Properties
                .FirstOrDefaultAsync(p =>
                p.PropertyId == cvdto.PropertyId &&
                p.LandlordId == landlord.LandlordID);

            if(property == null)
            {
                throw new KeyNotFoundException("Property not found or you do not own this property");
            }

            if(cvdto.EndTime <= cvdto.StartTime)
            {
                throw new ArgumentException("End time must be after start time");
            }

            if(cvdto.SlotDurationMinutes <= 0)
            {
                throw new ArgumentException("Slot duration must be greater than zero");
            }

            var availability = new ViewingAvailability
            {
                ViewingAvailabilityId = Guid.NewGuid(),
                PropertyId = cvdto.PropertyId,
                Date = cvdto.Date.Date,
                StartTime = cvdto.StartTime,
                EndTime = cvdto.EndTime,
                SlotDurationMinutes = cvdto.SlotDurationMinutes,
                isActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.ViewingAvailabilities.Add(availability);

            await _dbContext.SaveChangesAsync();

            return availability;
        }

        public async Task<ViewingScheduleResponseDTO> GenerateViewingSchedule (CreateViewingScheduleDTO cvsdto, Guid userId)
        {
            var landlord = await _dbContext.Landlords
                                .FirstOrDefaultAsync(l => l.UserId == userId.ToString());

            if(landlord == null)
            {
                throw new KeyNotFoundException("Landlord account could not be found for this user");
            }

            var property = await _dbContext.Properties
                                .FirstOrDefaultAsync(p =>
                                    p.PropertyId == cvsdto.PropertyId &&
                                    p.LandlordId == landlord.LandlordID);

            if(property == null)
            {
                throw new KeyNotFoundException("Property not found or you do not own this property");
            }

            var startDate = cvsdto.StartDate;
            var endDate = cvsdto.EndDate;

            if(endDate <  startDate)
            {
                throw new ArgumentException("End date must be on or after the start date.");
            }

            if(cvsdto.DaysOfWeek == null || cvsdto.DaysOfWeek.Count == 0)
            {
                throw new ArgumentException("Please select at least one day of the week");
            }

            if(cvsdto.EndTime <= cvsdto.StartTime)
            {
                throw new ArgumentException("End time must be after the start time");
            }

            if(cvsdto.SlotDurationMinutes <= 0)
            {
                throw new ArgumentException("Slot duration must greater than zero");
            }

            var viewingPeriodMinutes = (cvsdto.EndTime - cvsdto.StartTime).TotalMinutes;

            if(cvsdto.SlotDurationMinutes > viewingPeriodMinutes)
            {
                throw new ArgumentException("Slot duration cannot be longer than the viewing period.");
            }

            var selectedDays = cvsdto.DaysOfWeek.Distinct().ToHashSet();

            var existingAvailability = await _dbContext
                                        .ViewingAvailabilities
                                        .Where(v =>
                                            v.PropertyId == cvsdto.PropertyId &&
                                            v.Date >= startDate &&
                                            v.Date <= endDate &&
                                            v.isActive)
                                        .ToListAsync();

            var newAvailability = new List<ViewingAvailability>();

            int skipped = 0;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if(!selectedDays.Contains(date.DayOfWeek))
                {
                    continue;
                }

                var alreadyExists = existingAvailability.Any(v =>
                                        v.Date.Date == date.Date &&
                                        v.StartTime == cvsdto.StartTime &&
                                        v.EndTime == cvsdto.EndTime &&
                                        v.SlotDurationMinutes == cvsdto.SlotDurationMinutes);

                if (alreadyExists)
                {
                    skipped++;
                    continue;
                }

                var availability = new ViewingAvailability
                {
                    ViewingAvailabilityId = Guid.NewGuid(),
                    PropertyId = cvsdto.PropertyId,
                    Date = date,
                    StartTime = cvsdto.StartTime,
                    EndTime = cvsdto.EndTime,
                    SlotDurationMinutes = cvsdto.SlotDurationMinutes,
                    isActive = true,
                    CreatedDate = DateTime.UtcNow,
                };

                newAvailability.Add(availability);
            }

            if(newAvailability.Count > 0)
            {
                await _dbContext.ViewingAvailabilities.AddRangeAsync(newAvailability);

                await _dbContext.SaveChangesAsync();
            }

            return new ViewingScheduleResponseDTO
            {
                Success = true,
                Message = newAvailability.Count > 0 ? "Viewing schedule generated successfully." : "No new viewing availability was created. The selected dates already exist",
                PropertyId = cvsdto.PropertyId,
                StartDate = startDate,
                EndDate = endDate,
                SlotsCreated = newAvailability.Count,
                SlotsSkipped = skipped,
                CreatedAvailabilityIds = newAvailability.Select(a => a.ViewingAvailabilityId).ToList()
            };

        }

        public async Task<List<object>> GetAvailableViewings(Guid propertyId)
        {
            var propertyExists = await _dbContext.Properties
                .AnyAsync(p => p.PropertyId == propertyId);

            if(!propertyExists)
            {
                throw new KeyNotFoundException("Property Not Found");
            }

            var availability = await _dbContext
                .ViewingAvailabilities
                .Where(v =>
                        v.PropertyId == propertyId &&
                        v.isActive)
                .OrderBy(v => v.Date)
                .ThenBy(v => v.StartTime)
                .ToListAsync();

            var result = new List<object>();

            foreach(var available in availability)
            {
                var bookings = await _dbContext
                    .ViewingAvailabilities
                    .Where(v =>
                            v.PropertyId == propertyId &&
                            v.isActive)
                    .OrderBy(v => v.Date)
                    .ThenBy(v => v.StartTime)
                    .ToListAsync();

                result.Add(new
                {
                    availabilityId = available.ViewingAvailabilityId,
                    date = available.Date,
                    startTime = available.StartTime,
                    endTime = available.EndTime,
                    slotDurationMinutes = available.SlotDurationMinutes,
                    bookedSlots = bookings
                });
            }

            return result;
        }

        public async Task<ViewingBooking> BookViewing(CreateViewingBookingDTO cvdto, Guid tenantId)
        {
            var availability = await _dbContext
                .ViewingAvailabilities
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v =>
                    v.ViewingAvailabilityId == cvdto.ViewingAvailabilityId &&
                    v.isActive);


            if(availability == null)
            {
                throw new KeyNotFoundException("Viewing availability not found");
            }

            var requestedStart = cvdto.StartTime;

            var requestedEnd = requestedStart.Add(TimeSpan.FromMinutes(availability.SlotDurationMinutes));

            if(requestedStart < availability.StartTime || requestedEnd > availability.EndTime)
            {
                throw new ArgumentException("The selected time is outside the available viewing period");
            }

            var minutesFromStart = (requestedStart - availability.StartTime).TotalMinutes;

            if(minutesFromStart % availability.SlotDurationMinutes != 0)
            {
                throw new ArgumentException("Please select a valid viewing time slot");
            }

            var alreadyBooked = await _dbContext
                .ViewingBookings
                .AnyAsync(b =>
                    b.ViewingAvailabilityId ==
                    cvdto.ViewingAvailabilityId &&

                    b.StartTime == requestedStart &&
                    b.Status != "Cancelled");

            if(alreadyBooked)
            {
                throw new InvalidOperationException("This viewing slot has already been booked.");
            }

            var booking = new ViewingBooking
            {
                ViewingBookingId = Guid.NewGuid(),
                ViewingAvailabilityId = availability.ViewingAvailabilityId,
                PropertyId = availability.PropertyId,
                TenantId = tenantId,
                ViewingDate = availability.Date,
                StartTime = requestedStart,
                EndTime = requestedEnd,
                Status = "Confirmed",
                CreatedDate = DateTime.UtcNow,
            };

            _dbContext.ViewingBookings.Add(booking);

            await _dbContext.SaveChangesAsync();

            return booking;
        }

        public async Task<List<ViewingBookingResponseDTO>> GetTenantBookings(
    Guid tenantId)
        {
            var bookings =
                await _dbContext.ViewingBookings
                    .AsNoTracking()
                    .Where(b =>
                        b.TenantId == tenantId
                    )
                    .OrderBy(b =>
                        b.ViewingDate
                    )
                    .ThenBy(b =>
                        b.StartTime
                    )
                    .Select(b =>
                        new ViewingBookingResponseDTO
                        {
                            ViewingBookingId =
                                b.ViewingBookingId,

                            ViewingAvailabilityId =
                                b.ViewingAvailabilityId,

                            PropertyId =
                                b.PropertyId,

                            TenantId =
                                b.TenantId,

                            ViewingDate =
                                b.ViewingDate,

                            StartTime =
                                b.StartTime,

                            EndTime =
                                b.EndTime,

                            Status =
                                b.Status,

                            CreatedDate =
                                b.CreatedDate,
                        })
                    .ToListAsync();

            return bookings;
        }

        public async Task CancelViewing(Guid bookingId, Guid tenantId)
        {
            var booking = await _dbContext
                .ViewingBookings
                .FirstOrDefaultAsync(b => b.ViewingBookingId == bookingId &&
                                          b.TenantId == tenantId);

            if(booking == null)
            {
                throw new KeyNotFoundException("Viewing booking not found");
            }

            if(booking.Status == "Cancelled")
            {
                throw new InvalidOperationException("The viewing is already cancelled");
            }

            booking.Status = "Cancelled";

            await _dbContext.SaveChangesAsync();
        }
    }
}
