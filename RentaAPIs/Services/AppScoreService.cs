using Microsoft.EntityFrameworkCore;

namespace RentaAPIs.Services
{
    public class AppScoreService
    {
        private readonly AppDbContext _dbContext;

        public AppScoreService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<int?> CalculateAppScore(Guid tenantId)
        {
            // ==========================================
            // 1. GET TENANT
            // ==========================================

            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.TenantId == tenantId);

            if (tenant == null)
            {
                return null;
            }

            // ==========================================
            // 2. GET VERIFIED MONTHLY RENT OBLIGATIONS
            // ==========================================

            // Exclude legacy mock-payment records that do not
            // have a RentalPeriod.
            //
            // This prevents unverified historical records from
            // incorrectly increasing the tenant's AppScore.

            var payments = await _dbContext.Payments
                .AsNoTracking()
                .Where(p =>
                    p.TenantID == tenantId &&
                    p.RentalPeriod.HasValue)
                .ToListAsync();

            // ==========================================
            // 3. GET COMPLETED LEASES
            // ==========================================

            int completedLeases = await _dbContext.Leases
                .CountAsync(l =>
                    l.TenantID == tenantId &&
                    l.Status.ToLower() == "completed");

            // ==========================================
            // 4. GET CURRENT SOUTH AFRICAN DATE
            // ==========================================

            var saTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "South Africa Standard Time"
                );

            DateTime today = TimeZoneInfo
                .ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    saTimeZone
                )
                .Date;

            // ==========================================
            // 5. CLASSIFY RENT PAYMENT PERFORMANCE
            // ==========================================

            int onTimePayments = 0;
            int latePayments = 0;
            int overduePayments = 0;

            foreach (var payment in payments)
            {
                bool fullyPaid =
                    payment.AmountPaid >= payment.Amount &&
                    payment.Amount > 0;

                bool hasOutstandingBalance =
                    payment.AmountPaid < payment.Amount;

                bool isOverdue =
                    hasOutstandingBalance &&
                    payment.PaymentDueDate.Date < today;

                if (fullyPaid)
                {
                    // Use the recorded late-payment flag.
                    //
                    // A payment made after the due date
                    // remains late even after it is settled.

                    if (payment.IsLate)
                    {
                        latePayments++;
                    }
                    else
                    {
                        onTimePayments++;
                    }
                }
                else if (isOverdue)
                {
                    // Includes overdue obligations that
                    // have been partially paid.

                    overduePayments++;
                }

                // Pending payments that are not yet due
                // do not affect the score.
            }

            // ==========================================
            // 6. CALCULATE TOTAL EVALUATED PAYMENTS
            // ==========================================

            int totalEvaluatedPayments =
                onTimePayments +
                latePayments +
                overduePayments;

            // ==========================================
            // 7. START WITH NEUTRAL SCORE
            // ==========================================

            double score = 50;

            // ==========================================
            // 8. CALCULATE PAYMENT PERFORMANCE
            // ==========================================

            if (totalEvaluatedPayments > 0)
            {
                double onTimeRate =
                    (double)onTimePayments /
                    totalEvaluatedPayments;

                double lateRate =
                    (double)latePayments /
                    totalEvaluatedPayments;

                double overdueRate =
                    (double)overduePayments /
                    totalEvaluatedPayments;

                // Reward on-time rent payments.
                score += onTimeRate * 30;

                // Penalise late rent payments.
                score -= lateRate * 15;

                // Penalise outstanding overdue rent.
                score -= overdueRate * 30;
            }

            // ==========================================
            // 9. REWARD COMPLETED LEASES
            // ==========================================

            int leaseBonus =
                Math.Min(completedLeases * 10, 20);

            score += leaseBonus;

            // ==========================================
            // 10. CLAMP SCORE BETWEEN 0 AND 100
            // ==========================================

            int finalScore = (int)Math.Round(
                Math.Clamp(score, 0, 100),
                MidpointRounding.AwayFromZero
            );

            // ==========================================
            // 11. SAVE APP SCORE
            // ==========================================

            tenant.AppScore = finalScore;

            await _dbContext.SaveChangesAsync();

            return finalScore;
        }

        public async Task RecalculateAllAppScores()
        {
            var tenantIds = await _dbContext.Tenants
                .AsNoTracking()
                .Select(t => t.TenantId)
                .ToListAsync();

            foreach (var tenantId in tenantIds)
            {
                var calculatedScore = await CalculateAppScore(tenantId);

                Console.WriteLine(
                    $"APPSCORE RECALCULATION: Tenant={tenantId}, Score={calculatedScore}"
                );
            }
        }
    }

}
