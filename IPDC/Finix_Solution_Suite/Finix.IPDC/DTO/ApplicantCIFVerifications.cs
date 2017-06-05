using Finix.IPDC.Infrastructure;
using System;

namespace Finix.IPDC.DTO
{
    public class ApplicantCIFVerifications
    {
        public string VerificationType { get; set; }
        public string ApplicationNo { get; set; }
        public string LatestApplicationNo { get; set; }
        public string VerifierName { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string VerificationDateText { get; set; }
        public DateTime? LatestVerificationDate { get; set; }
        public string LatestVerificationDateText { get; set; }
        public int? Count { get; set; }
        public VerificationState? VerificationStatus { get; set; }
        public string VerificationStatusName { get; set; }
        public VerificationState? VerificationStatusForThisApplication { get; set; }
        public string VerificationStatusForThisApplicationName { get; set; }
        public long? LatestVerificationId { get; set; }
        public long? LatestForThisApplicationId { get; set; }
        public string EditUrl { get; set; }
    }
}
