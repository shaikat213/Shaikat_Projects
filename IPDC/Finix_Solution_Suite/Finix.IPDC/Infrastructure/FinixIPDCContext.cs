using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Finix.IPDC.Util;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.Infrastructure
{
    public interface IFinixIPDCContext
    {
    }

    public partial class FinixIPDCContext : DbContext, IFinixIPDCContext
    {
        #region ctor
        static FinixIPDCContext()
        {
            Database.SetInitializer<FinixIPDCContext>(null);
        }

        public FinixIPDCContext()
            : base("Name=FinixIPDCContext")
        {
            Database.SetInitializer(new FinixAccountsContextInitializer());
        }
        #endregion

        #region Models
        public DbSet<Address> Address { get; set; }
        public DbSet<AppDocChecklist> AppDocChecklist { get; set; }
        public DbSet<Application> Application { get; set; }
        public DbSet<ApplicationLog> ApplicationLog { get; set; }
        public DbSet<ApplicationCIFs> ApplicationCIFs { get; set; }
        //public DbSet<ApprovalAuthorityGroup> ApprovalAuthorityGroup { get; set; }
        //public DbSet<ApprovalAuthorityGroupDetail> ApprovalAuthorityGroupDetail { get; set; }
        //public DbSet<ApprovalAuthoritySignatory> ApprovalAuthoritySignatory { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<BankAccountCpv> BankAccountCpv { get; set; }
        public DbSet<Call> Call { get; set; }
        public DbSet<CIB_Personal> CIB_Personal { get; set; }
        public DbSet<CIB_Organizational> CIB_Organizational { get; set; }
        public DbSet<CIF_AdditionalIncomeDeclared> CIF_AdditionalIncomeDeclared { get; set; }
        public DbSet<CIF_BusinessShares> CIF_BusinessShares { get; set; }
        public DbSet<CIF_Personal> CIF_Personal { get; set; }
        public DbSet<CIF_Reference> CIF_Reference { get; set; }
        public DbSet<CIF_IncomeStatement> CIF_IncomeStatement { get; set; }
        public DbSet<CIF_Investment> CIF_Investment { get; set; }
        public DbSet<CIF_Liability> CIF_Liability { get; set; }
        public DbSet<CIF_NetWorth> CIF_NetWorth { get; set; }
        public DbSet<CIF_NW_Property> CIF_NW_Property { get; set; }
        public DbSet<CIF_RPT_Codes> CIF_RPT_Codes { get; set; }
        public DbSet<CIF_SavingsInBank> CIF_SavingsInBank { get; set; }
        public DbSet<CIF_Organizational> CIF_Organizational { get; set; }
        public DbSet<CIF_Org_Owners> CIF_Org_Owners { get; set; }
        public DbSet<CostCenter> CostCenter { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<ConsumerGoodsPrimarySecurity> ConsumerGoodsPrimarySecurity { get; set; }
        public DbSet<ConsumerGoodsPrimarySecurityValuation> ConsumerGoodsPrimarySecurityValuation { get; set; }
        public DbSet<ContactPointVerification> ContactPointVerification { get; set; }
        public DbSet<ConstructionType> ConstructionTypes { get; set; }
        public DbSet<CreditCard> CreditCard { get; set; }
        public DbSet<DCL_Signatory> DCL_Signatories { get; set; }
        public DbSet<Disbursment_Signatory> Disbursment_Signatories { get; set; }
        public DbSet<DepositApplicationTracking> DepositApplicationTracking { get; set; }
        public DbSet<DepAppCash> DepAppCash { get; set; }
        public DbSet<DepAppChequeDeposit> DepAppChequeDeposit { get; set; }
        public DbSet<DepAppTransfer> DepAppTransfer { get; set; }
        public DbSet<DepositApplication> DepositApplication { get; set; }
        public DbSet<DepositNominee> DepositNominee { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<DesignationRoleMapping> DesignationRoleMappings { get; set; }
        public DbSet<DesignationProductMapping> DesignationProductMappings { get; set; }
        public DbSet<Developer> Developer { get; set; }
        public DbSet<DeveloperMember> DeveloperMember { get; set; }
        public DbSet<DeveloperDirector> DeveloperDirector { get; set; }
        public DbSet<DeveloperDocument> DeveloperDocument { get; set; }
        public DbSet<DisbursementMemo> DisbursementMemo { get; set; }
        public DbSet<DMText> DMText { get; set; }
        public DbSet<DMDetail> DMDetail { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DocumentCheckList> DocumentCheckList { get; set; }
        public DbSet<DocumentCheckListDetail> DocumentCheckListDetail { get; set; }
        public DbSet<DocumentCheckListException> DocumentCheckListException { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDesignationMapping> EmployeeDesignationMapping { get; set; }
        public DbSet<FactoryAddress> FactoryAddress { get; set; }
        public DbSet<FDRPrimarySecurity> FDRPrimarySecurity { get; set; }
        public DbSet<FDRPSDetail> FDRPSDetail { get; set; }
        public DbSet<FollowupTrack> FollowupTracks { get; set; }
        public DbSet<FundConfirmation> FundConfirmations { get; set; }
        public DbSet<FundConfirmationDetail> FundConfirmationDetails { get; set; }
        public DbSet<GPSLog> GPSLog { get; set; }
        public DbSet<Guarantor> Guarantor { get; set; }
        public DbSet<IndexSequencer> IndexSequencer { get; set; }
        public DbSet<IncomeVerification> IncomeVerification { get; set; }
        public DbSet<IncomeVerificationAdditionalIncomeAssessed> IncomeVerificationAdditionalIncomeAssessed { get; set; }
        public DbSet<IPDCMessaging> IPDCMessaging { get; set; }
        public DbSet<LandOwnerProperty> LandOwnerProperty { get; set; }
        public DbSet<LoanApplication> LoanApplication { get; set; }
        public DbSet<LoanAppWaiverReq> LoanAppWaiverReq { get; set; }
        public DbSet<LoanAppColSecurity> LoanAppColSecurity { get; set; }
        public DbSet<LoanOtherSecurities> LoanOtherSecurities { get; set; }
        public DbSet<LPPrimarySecurity> LPPrimarySecurity { get; set; }
        public DbSet<LPPrimarySecurityValuation> LPPrimarySecurityValuation { get; set; }
        public DbSet<Nationality> Nationality { get; set; }
        public DbSet<NIDVerification> NIDVerification { get; set; }
        public DbSet<NetWorthVerification> NetWorthVerification { get; set; }
        public DbSet<NWV_SavingsInBank> NWV_SavingsInBank { get; set; }
        public DbSet<NWV_Investment> NWV_Investment { get; set; }
        public DbSet<NWV_Property> NWV_Property { get; set; }
        public DbSet<NWV_BusinessShares> NWV_BusinessShares { get; set; }
        public DbSet<NWV_Liability> NWF_Liability { get; set; }
        public DbSet<Occupation> Occupation { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<OfficeDesignationArea> OfficeDesignationAreas { get; set; }
        public DbSet<OfficeDesignationSetting> OfficeDesignationSettings { get; set; }
        public DbSet<OfficeLayer> OfficeLayers { get; set; }
        public DbSet<OfferLetterText> OfferLetterTexts { get; set; }
        public DbSet<OfferLetter> OfferLetters { get; set; }
        public DbSet<OfferLetterAmendment> OfferLetterAmendment { get; set; }
        public DbSet<OfferLetterAmendmentTexts> OfferLetterAmendmentTexts { get; set; }
        //public DbSet<OfficePosition> OfficePositions { get; set; }
        //public DbSet<OfficeUnit> OfficeUnit { get; set; }
        //public DbSet<OfficeUnitSetting> OfficeUnitSettings { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        //public DbSet<OrganoGram> Organograms { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectLegalVerification> ProjectLegalVerification { get; set; }
        public DbSet<ProjectTechnicalVerification> ProjectTechnicalVerification { get; set; }
        public DbSet<ProjectPropertyOwner> ProjectPropertyOwner { get; set; }
        public DbSet<ProjectImages> ProjectImages { get; set; }
        public DbSet<Proposal> Proposal { get; set; }
        public DbSet<Proposal_ClientProfile> Proposal_ClientProfile { get; set; }
        public DbSet<Proposal_Income> Proposal_Income { get; set; }
        public DbSet<Proposal_NetWorth> Proposal_NetWorth { get; set; }
        public DbSet<Proposal_CIB> Proposal_CIB { get; set; }
        public DbSet<Proposal_Liability> Proposal_Liabilities { get; set; }
        public DbSet<Proposal_Signatory> Proposal_Signatory { get; set; }
        public DbSet<Proposal_Text> Proposal_Text { get; set; }
        public DbSet<Proposal_FDR> Proposal_FDR { get; set; }
        public DbSet<Proposal_OverallAssessment> Proposal_OverallAssessment { get; set; }
        public DbSet<Proposal_OtherCost> Proposal_OtherCost { get; set; }
        public DbSet<Proposal_Guarantor> Proposal_Guarantor { get; set; }
        public DbSet<Proposal_StressRate> Proposal_StressRate { get; set; }
        public DbSet<Proposal_SecurityDetail> Proposal_SecurityDetail { get; set; }
        public DbSet<ProposalAmendment> ProposalAmendment { get; set; }
        public DbSet<ProposalAmendment_ModeOfDisbursement> ProposalAmendment_ModeOfDisbursement { get; set; }
        public DbSet<ProposalAmendment_DisbursementCondition> ProposalAmendment_DisbursementCondition { get; set; }
        public DbSet<ProposalAmendment_Signatory> ProposalAmendment_Signatory { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<PODocument> PODocument { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<QuestionAssignment> QuestionAssignments { get; set; }
        public DbSet<ReferenceCpv> ReferenceCpv { get; set; }
        public DbSet<SalesLead> SalesLeads { get; set; }
        public DbSet<SalesLeadAssignment> SalesLeadAssignments { get; set; }
        public DbSet<SectorCode> SectorCode { get; set; }
        public DbSet<Thana> Thanas { get; set; }
        public DbSet<VehiclePrimarySecurity> VehiclePrimarySecurity { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<VehiclePrimarySecurityValuation> VehiclePrimarySecurityValuation { get; set; }
        public DbSet<VendorShowrooms> VendorShowrooms { get; set; }
        public DbSet<UOM> UOM { get; set; }
        public DbSet<Upazila> Upazilas { get; set; }
        public DbSet<ProductRates> ProductRates { get; set; }
        public DbSet<ProductSpecialRate> ProductSpecialRate { get; set; }
        public DbSet<DPSMaturitySchedule> DPSMaturitySchedule { get; set; }
        public DbSet<DocumentSetup> DocumentSetup { get; set; }
        public DbSet<ProductSecurity> ProductSecurity { get; set; }
        public DbSet<IPDCBankAccounts> IPDCBankAccounts { get; set; }
        public DbSet<LegalDocument> LegalDocuments { get; set; }
        public DbSet<LegalDocPropType> LegalDocPropTypes { get; set; }
        public DbSet<LegalDocumentStatus> LegalDocumentStatus { get; set; }
        public DbSet<LegalDocumentVerification> LegalDocumentVerifications { get; set; }
        public DbSet<ProposalCreditCard> ProposalCreditCards { get; set; }
        public DbSet<Signatories> Signatories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            FluentConfigurator.ConfigureGenericSettings(modelBuilder);
            FluentConfigurator.ConfigureMany2ManyMappings(modelBuilder);
            FluentConfigurator.ConfigureOne2OneMappings(modelBuilder);
        }
    }

    internal class FinixAccountsContextInitializer : CreateDatabaseIfNotExists<FinixIPDCContext> //CreateDatabaseIfNotExists<CardiacCareContext>
    {
        protected override void Seed(FinixIPDCContext context)
        {
            var seedDataPath = IPDCSystem.SeedDataPath;
            var dropDb = IPDCSystem.DropDB;
            if (string.IsNullOrWhiteSpace(seedDataPath))
                return;
            var folders = Directory.GetDirectories(seedDataPath).ToList().OrderBy(x => x);
            var msg = "";
            bool error = false;
            try
            {
                foreach (var folder in folders)
                {
                    msg += string.Format("processing for: {0}{1}", folder, Environment.NewLine);

                    var fileDir = Path.Combine(new[] { seedDataPath, folder });
                    var sqlFiles = Directory.GetFiles(fileDir, "*.sql").OrderBy(x => x).ToList();
                    foreach (var file in sqlFiles)
                    {
                        try
                        {
                            msg += string.Format("processing for: {0}{1}", file, Environment.NewLine);
                            context.Database.ExecuteSqlCommand(File.ReadAllText(file));
                            msg += string.Format("Done....{0}", Environment.NewLine);
                        }
                        catch (Exception ex)
                        {
                            error = true;
                            msg += string.Format("Failed!....{0}", Environment.NewLine);
                            msg += string.Format("{0}{1}", ex.Message, Environment.NewLine);
                        }
                    }
                }
                if (error)
                {
                    throw new Exception(msg);
                }
                base.Seed(context);
            }
            catch (Exception ex)
            {
                msg = "Error Occured while inserting seed data" + Environment.NewLine;
                if (dropDb)
                {
                    context.Database.Delete();
                    msg += ("Database is droped" + Environment.NewLine);
                }
                var errFile = seedDataPath + "\\seed_data_error.txt";
                msg += ex.Message;
                File.WriteAllText(errFile, msg);

            }
        }

    }
}