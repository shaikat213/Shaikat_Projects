using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Finix.IPDC.Infrastructure
{
    public enum Activity { Create = 1, Edit, Delete, Submit }
    public enum AgeRange
    {
        [Display(Name = "Not Specified")]
        NotSpecified = 0,
        [Display(Name = "Below 18")]
        Below18 = 1,
        [Display(Name = "18 - 30")]
        From18To30 = 2,
        [Display(Name = "31 - 45")]
        From31To45 = 3,
        [Display(Name = "46-55")]
        From46To55 = 4,
        [Display(Name = "56-65")]
        From56To65 = 5,
        [Display(Name = "Over 65")]
        Over65 = 6
    }
    public enum ApplicationSource
    {
        [Display(Name = "Sales Lead")]
        SalesLead = 1,
        [Display(Name = "Inbound Call")]
        InboundCall = 2
    }
    public enum ApprovalStatus { New = 1, Under_Review = 2, Approved = 3, Rejected = 4 }
    public enum ApplicationCustomerType { Individual = 1, Organizational = 2 }
    public enum AppType { Application = 1, DepositApplication = 2, LoanApplication = 3 }
    public enum ApplicationType { Single = 1, Joint = 2 }
    public enum ApplicantRole { [Display(Name = "Primary Applicant")]Primary = 1, [Display(Name = "Co Applicant")] Co = 2 }
    public enum ApplicationStatus { Accepted = 1, Deferred = 2, Rejected = 3 }
    public enum ApprovalAuthorityLevel { MCC = 1, EC, Board, HoRisk }
    public enum AssessmentParticulars
    {
        [Display(Name = "CIB Applicant")]
        CIB_Applicant = 1,
        [Display(Name = "CIB Co-Applicant")]
        CIB_Coapplicant,
        [Display(Name = "CIB Guarantor")]
        CIB_Guarantor,
        [Display(Name = "CPV Applicant")]
        CPV_Applicant,
        [Display(Name = "CPV Co-Applicant")]
        CPV_Coapplicant,
        [Display(Name = "CPV Guarantor")]
        CPV_Guarantor,
        [Display(Name = "Reference Check")]
        ReferenceCheck,
        [Display(Name = "Financial Strength Evaluation")]
        FinancialStrengthEvaluation,
        [Display(Name = "Lagal Verification")]
        LagalVerification,
        [Display(Name = "Technical Verification")]
        TechnicalVerification,
        [Display(Name = "Property Visit")]
        PropertyVisit

    }
    public enum ApplicationStage
    {
        [Display(Name = "Rejected by Operations")]
        RejectedByOperations = -6,
        [Display(Name = "Rejected by MCC")]
        RejectedByMCC = -5,
        [Display(Name = "Rejected by CRM")]
        RejectedByCRM = -4,
        [Display(Name = "Rejected by BM")]
        RejectedByBM = -3,
        [Display(Name = "Rejected by TL")]
        RejectedByTL = -2,
        [Display(Name = "Application Disposed")]
        ApplicationDisposed = -1,
        Drafted = 1,
        [Display(Name = "Sent to TL")]
        SentToTL = 2,
        [Display(Name = "Sent to BM")]
        SentToBM = 3,
        [Display(Name = "Sent to CRM")]
        SentToCRM = 4,
        [Display(Name = "Under Process at CRM")]
        UnderProcessAtCRM = 5,
        [Display(Name = "Sent To Operations")]
        SentToOperations = 6,
        [Display(Name = "Under Process at Operations")]
        UnderProcessAtOperations = 7,
        [Display(Name = "Fund Received")]
        FundReceived = 8,
        [Display(Name = "DCL under process")]
        DCLUnderProcess = 9,
        [Display(Name = "DCL Approved")]
        DCLApproved = 10,
        [Display(Name = "PO under process")]
        POUnderProcess = 11,
        [Display(Name = "PO Issued")]
        POIssued = 12,
        [Display(Name = "Approved by Operations")]
        ApprovedByOperations = 13,
        [Display(Name = "Ready for Disbursement")]
        ReadyForDeisbursement = 14,
        [Display(Name = "Partial Disbursement Complete")]
        PartialDisbursementComplete = 15,
        [Display(Name = "Disbursement Complete")]
        DisbursementComplete = 16,
        [Display(Name = "Account Opening Under Process")]
        AccountOpeningUnderProcess = 17,
        [Display(Name = "Account Opened")]
        AccountOpened = 18,
        [Display(Name = "Insturment Ready")]
        InstrumentReady = 19,
        [Display(Name = "Insturment Delivered to Client")]
        InsturmentDeliveredtoClient = 20,
        [Display(Name = "Insturment Sent To RM")]
        InsturmentSentToRM = 21,
        [Display(Name = "Insturment Sent To Branch")]
        InsturmentSentToBranch = 22,
        [Display(Name = "Insturment Kept in File")]
        InsturmentKeptinFile = 23,
        [Display(Name = "Pending Issue")]
        PendingIssue = 24,
        [Display(Name = "Instrument Under Lien")]
        InstrumentUnderLien = 25,
        [Display(Name = "Dcl Exception")]
        DclException = 26,
        [Display(Name = "Dcl Obtained")]
        DclObtained = 27
    }
    public enum BloodGroup { O_Positive = 1, O_Negative, A_Positive, A_Negative, AB_Positive, AB_Negetive, B_Positive, B_Negative }
    public enum BusinessShareType { Proprietorship = 1, Partnership = 2, Private_Limited = 3 }
    public enum BusinessSize { Cottage = 1, Small, Medium, Large }
    public enum BusinessType { Manufacturer = 1, Trade, Service }
    public enum BankDepositType { Fixed = 1, DPS = 2, Savings = 3, Current = 4 }
    //public enum CallCategory
    //{
    //    Solicited = 1,
    //    Referral
    //}
    public enum BankAccountType { Savings = 1, Current }
    public enum CallMode
    {
        Phone = 1,
        Email = 2,
        [Display(Name = "Physical Visit")]
        PhysicalVisit = 3,
        [Display(Name = "Personal Contact")]
        PersonalContact = 4
    }
    public enum CIF_Org_OwnersRole { Chairman = 1, MD = 2, ED = 3, Director = 4, Partner = 5, Shareholder = 6, Proprietor = 7, Other = 8 }
    public enum CIF_Org_SectorType { Public = 1, Private }
    public enum CompanyLegalStatus
    {
        [Display(Name = "Proprietorship")]
        Proprietorship = 1,
        [Display(Name = "Partnership")]
        Partnership = 2,
        [Display(Name = "Private Ltd")]
        Private_Ltd = 3,
        [Display(Name = "Public Ltd")]
        Public_Limited = 4,
        [Display(Name = "Joint Venture")]
        Joint_Venture = 5,
        [Display(Name = "NGO")]
        NGO = 6,
        [Display(Name = "Club")]
        Club = 7,
        [Display(Name = "Govt")]
        Govt = 8,
        [Display(Name = "Others")]
        Others = 9
    }
    public enum CustomerPriority { Normal = 1, Priority, Sensitive, [Display(Name = "Extremely High Value")] Extremely_High_Value }
    public enum CallCategory { Solicited = 1, Referral }
    public enum CallType
    {
        Self = 1,
        [Display(Name = "User Assigned")]
        User_Assigned = 2,
        [Display(Name = "Auto Assigned")]
        Auto_assign = 3
    }
    public enum CallFailReason
    {
        [Display(Name = "Customer not interested")]
        Customer_not_interested = 1,
        [Display(Name = "Customer out of reach")]
        Customer_out_of_reach = 2,
        [Display(Name = "Insufficient Income")]
        Insufficient_Income = 3,
        [Display(Name = "Unacceptability of Rate of Interest")]
        Unacceptability_of_Rate_of_Interest = 4,
        [Display(Name = "Not comfortable for RM")]
        Not_comfortable_for_RM = 5
    }
    public enum CallStatus { Unfinished = 0, Unsuccessful = 1, Successful = 2 }
    public enum CIBClassificationStatus { [Display(Name = "NIL")] NIL = 0, UC, SMA, SS, DF, BL, WO, SO }
    public enum CustomerSensitivity { PEP = 1, Behaviour_sensitive = 2, Profile_Sensitive = 3, Negative_Experience = 4, SR_Sensitive = 5, Islamic_Banking = 6, Price_Rate_Sensitive = 7, None = 8, Flexible = 9 }
    //public enum ConstructionTypeEnum
    //{
    //    [Display(Name = "RCC Construction")]
    //    RCC_Construction = 1,
    //    [Display(Name = "Steel Structure")]
    //    Steel_Structure = 2,
    //    [Display(Name = "Prefabricated Building")]
    //    Prefabricated_Building = 3,
    //    [Display(Name = "Semi Pacca")]
    //    Semi_Pacca = 4,
    //    Others = 5
    //}
    public enum ContactAddress { PermanentAddress = 1, OfficeAddress = 2, ResidenceAddress = 3 }
    public enum DepositType { Fixed = 1, Recurring = 2 }
    public enum DeveloperType { Developer_Proprietor = 1, Developer_Company, Individual_Builder, Group_Construction }
    public enum DeveloperCategory { Cat_1 = 1, Cat_2, Cat_3, Others }
    public enum DeveloperEnlistmentStatus
    {
        New = 1,
        [Display(Name = "Under Assessment")]
        Under_Assessment = 2,
        Approved = 3,
        Disapporved = 4
    }
    public enum DepositClass { Fixed_Deposit = 1, Recurring_Deposit = 2 }
    public enum DocCollectionStage { Application = 1, Operation = 2 }
    public enum DepositAccRenewalOpts { [Display(Name = "Renew Principal Only")]Renew_Principal_Only = 1, [Display(Name = "Renew Principal with Interest")] Renew_Principal_with_Interest = 2, [Display(Name = "As per Instruction at Maturity")] As_per_Instruction_at_Maturity = 3 }
    public enum DepositWithdrawalMode { Cheque = 1, EFT = 2 }
    public enum DeveloperProjectStatus
    {
        Upcoming = 1,
        Ongoing,
        [Display(Name = "Handed Over")]
        HandedOver
    }
    public enum DisbursementMode { Cheque = 1, EFT = 2 }
    public enum DisbursementTextType
    {
        [Display(Name = "Security")]
        Security = 1,
        [Display(Name = "Disburse To")]
        DisburseTo,
        Exception,
        [Display(Name = "Doc Status")]
        DocStatus,
        Recomendation,
        Enclosure
    }
    public enum DisbursementTo { Borrower = 1, [Display(Name = "Third Party")] Third_Party = 2 }
    public enum DocumentType { Application = 1, Approval = 2, Business = 3, Certificate = 4, Charge = 5, Company = 6, Contract = 7, Employment = 8, Financial = 9, Identity = 10, Land = 11, Miscellaneous = 12, Photo = 13, Security = 14, Undertaking = 15, Registration = 16 }
    public enum DocumentStatus { NotObtained = 1, Obtained = 2, Deferred = 3, Waived = 4, Not_Applicable = 5 }
    public enum EducationLevel { SSC = 1, HSC = 2, Graduate = 3, PostGraduate = 4, Others = 5 }
    public enum EmployeeType { Permanent = 1, Contractual = 2 }
    public enum EmploymentStatus { Permanent = 1, Contractual = 2, Temporary = 3 }
    public enum FindingStatus { Not_Found = 1, Found_Ok = 2, Mismatch = 3 }
    public enum FollowupType { Call = 1, Visit = 2 }
    public enum Gender { Male = 1, Female = 2 }
    public enum HomeOwnership { Rented = 1, Own = 2, [Display(Name = "Family Owned")] Family = 3, [Display(Name = "Employer Owned")] Employer = 4, Others = 5 }
    public enum IncomeRange
    {
        [Display(Name = "Not Specified")]
        NotSpecified = 0,
        [Display(Name = "20,000 - 50,000")]
        From20000To50000 = 1,
        [Display(Name = "50,001 - 75,000")]
        From50001To75000,
        [Display(Name = "75,001 - 100,000")]
        From75001To100000,
        [Display(Name = "100,001 -  150,000")]
        From100001To150000,
        [Display(Name = "150,001 - 200,000")]
        From150001To200000,
        [Display(Name = "Above 200,000")]
        Above200000
    }
    public enum InvestmentType { Shares = 1, Shanchay_Patra = 2, Life_Insurance = 3 }
    public enum InstrumentDispatchStatus
    {
        [Display(Name = "Received by client")]
        Received_by_client = 1,
        [Display(Name = "Received by RM")]
        Received_by_RM = 2,
        [Display(Name = "Sent to Branch")]
        Sent_to_Branch = 3,
        [Display(Name = "Kept in file")]
        Kept_in_file = 4,
        [Display(Name = "Under Lien")]
        Under_Lien = 5,
        Other = 6
    }
    public enum LeadType { General = 1, Referral = 2 }
    public enum LoanType { Personal = 1, Home = 2, Auto = 3, Credit_Card = 4, Business = 5, Office = 6, Others = 7 }
    public enum LiabilityType { Borrower = 1, Co_Borrower = 2, Guarantor = 3 }
    public enum LeadPriority
    {
        Normal = 1,
        Urgent,
        [Display(Name = "Extremely Urgent")]
        Extremely_Urgent
    }
    public enum LeadStatus { Drafted = 0, Submitted = 1, Unsuccessful = 2, FollowUp = 3, Prospective = 4 }
    public enum LeadStatusEntry { Drafted = 0, Submitted = 1 }
    //public enum LegalStatus { Proprietorship = 1, Partnership = 2, Private_Limited = 3, Public_Limited = 4 }
    public enum LoanChequeDeliveryOptions
    {
        [Display(Name = "To Client")]
        To_Client = 1,
        [Display(Name = "To Vendor")]
        To_Vendor = 2,
        [Display(Name = "To RM")]
        To_RM = 3,
        [Display(Name = "To Branch")]
        To_Branch = 4,
        [Display(Name = "Direct Deposit")]
        Direct_Deposit = 5
    }
    public enum LandedPropertyLoanType
    {
        [Display(Name = "Flat Purchase")]
        Flat_Purchase = 1,
        [Display(Name = "Construction")]
        Construction = 2,
        [Display(Name = "Extension")]
        Extension = 3,
        [Display(Name = "Renovation")]
        Renovation = 4
        //[Display(Name = "Plot Purchase")]
        //Plot_Purchase = 5,
        //[Display(Name = "Home Equity")]
        //Home_Equity = 6,
        //[Display(Name = "Take Over")]
        //Take_Over = 7
    }
    public enum LandedPropertySellertype { Individual = 1, Developer = 2 }
    public enum LandedPropertyValuationType
    {
        [Display(Name = "Flat Purchase")]
        Flat_Purchase = 1,
        [Display(Name = "Self Construction")]
        Self_Construction = 2
    }
    public enum LandType { Freehold = 1, Leasehold = 2, Other = 3 }

    public enum LoanPrimarySecurityType
    {
        [Display(Name = "Vehicle Primary Security")]
        VehiclePrimarySecurity = 1,
        [Display(Name = "Consumer Goods Primary Security")]
        ConsumerGoodsPrimarySecurity = 2,
        [Display(Name = "FDR Primary Security")]
        FDRPrimarySecurity = 3,
        [Display(Name = "Landed Property Primary Security")]
        LPPrimarySecurity = 4,
        [Display(Name = "Not Applicable")]
        NotApplicable = 5
    }
    public enum LocationFindibility
    {
        [Display(Name = "Easy to find")]
        EasyToFind = 1,
        [Display(Name = "Difficult to find")]
        DifficultToFind = 2,
        [Display(Name = "Out of range")]
        OutOfRange = 3,
        NA = 4
    }
    public enum LivingWith { Alone = 1, Family = 2, Other = 3 }
    public enum MaritalStatus { Married = 1, UnMarried = 2, Other }
    public enum MemoType { DCL = 1, Disbursment_Memo = 2, Credit_Memo = 3, Predisbursment_Memo = 4 }
    public enum ModeOfDeposit { Cash = 1, Cheque = 2, Transfer = 3 }
    public enum ModeOfOperations { Singly = 1, Jointly = 2, [Display(Name = "Either or Survivor")] EitherOrSurvivor = 3, Others = 4 }
    public enum OccupationType
    {
        [Display(Name = "Government Service")]
        Govt = 1,
        [Display(Name = "Private Service")]
        Private = 2,
        [Display(Name = "Business")]
        Business = 3,
        [Display(Name = "Housewife")]
        HouseWife = 4,
        [Display(Name = "Retired")]
        Retired = 5,
        [Display(Name = "Others")]
        Other = 6,
        [Display(Name = "Self Employed")]
        SelfEmployed = 7,
        [Display(Name = "Land Owner")]
        LandOwner = 8
    }
    public enum OrganizationType { A = 1, B = 2, C = 3 }
    public enum OfficeUnitType { Wing = 1, Dept, Section }
    public enum OverallVerificationStatus
    {
        [Display(Name = "Under Process")]
        UnderProcess = 1,
        Ok,
        [Display(Name = "Not OK")]
        NotOK
    }
    public enum Priority { A_Catagory = 1, B_Catagory = 2, C_Catagory = 3 }
    public enum PropertyType { Residential = 1, Commercial = 2, ResidentialAndCommerial = 3, Others = 4 }
    //public enum Nationalism { Bangladesh = 1 }
    public enum ProductType { Deposit = 1, Loan = 2 }
    public enum ProjectApprovalAuthority { RAJUK = 1, UP = 2, Pourashobha = 3, [Display(Name = "Cantonment Board")]
        Cantonment_Board = 4
    }
    public enum ProjectStatus
    {
        [Display(Name = "Foundation Work")]
        Foundation_Work = 1,
        [Display(Name = "Slab Custed")]
        Slab_Custed = 2,
        [Display(Name = "Brick Work")]
        Brick_Work = 3,
        [Display(Name = "Plaster Work")]
        Plaster_Work = 4,
        [Display(Name = "Sanitary & Electric")]
        Sanitary_And_Electric = 5,
        [Display(Name = "Tiles Work")]
        Tiles_Work = 6,
        [Display(Name = "Finishing Work")]
        Finishing_Work = 7,
        [Display(Name = "Ready")]
        Ready = 8
    }
    public enum ProposalProduct
    {
        Vehicle = 1,
        FDR = 2,
        [Display(Name = "Consumer Goods")]
        ConsumerGoods = 3,
        [Display(Name = "Landed Property")]
        LandedProperty = 4,
        //RLS = 5,
        Others = 6

    }
    public enum ProposalTextTypes
    {
        [Display(Name = "Asset Backup")]
        AssetBackup = 1,
        Strength,
        Weakness,
        [Display(Name = "Mode of Disbursement")]
        ModeOfDisbursement,
        [Display(Name = "Final Disbursement Conditions")]
        FinalDisbursementConditions,
        Exceptions
    }
    public enum ProposalSecurityDetailType
    {
        [Display(Name = "Collateral Security")]
        ColSecurity = 1,
        [Display(Name = "Other Security")]
        OtherSecurity
    }
    public enum FlatStatus
    {

        [Display(Name = "Slab Custed")]
        Slab_Custed = 1,
        [Display(Name = "Brick Work")]
        Brick_Work = 2,
        [Display(Name = "Plaster Work")]
        Plaster_Work = 3,
        [Display(Name = "Sanitary & Electric")]
        Sanitary_And_Electric = 4,
        [Display(Name = "Tiles Work")]
        Tiles_Work = 5,
        [Display(Name = "Finishing Work")]
        Finishing_Work = 6
    }
    public enum PropertyBounds { Road = 1, Building = 2, Open_Plot = 3, Tin_Shade = 4, Semi_Pakka = 5, House, Market, Marsh, Others }

    public enum ProposalFacilityType
    {
        [Display(Name = "Home Loan")]
        Home_Loan = 1,
        [Display(Name = "Auto Loan")]
        Auto_Loan,
        [Display(Name = "Personal Loan")]
        Personal_Loan,
        [Display(Name = "RLS")]
        RLS,
        [Display(Name = "Fixed Deposit")]
        Fixed,
        [Display(Name = "Recurrent Deposit")]
        Recurring
    }
    public enum Religion { Islam = 1, Hindu = 2, Buddist, Christian, Other }
    public enum ResidenceStatus { Resident = 1, Non_Resident = 2 }
    public enum RoleInBankOrFL { Chairman = 1, MD = 2, Director = 3, Shareholder = 4 }
    public enum RelationshipWithIPDC
    {
        Chariman = 1,
        MD = 2,
        Director = 3,
        Shareholder = 4,
        [Display(Name = "Relative of Director")]
        RelativeOfDirector = 5
    }
    public enum RelationshipWithApplicant { Self = 1, Father, Mother, Brother, Sister, Spouse, Neighbour, Colleague, Sibling, Relative, Son, Daughter, Others }
    public enum RiskLevel { Low = 1, Medium = 2, High = 3, Extreme = 4 }
    public enum SanctionCheck { [Display(Name = "No Record Found")]No_Record_Found = 1, [Display(Name = "Record Match")] Record_Match = 2 }
    public enum SectorCodeType { NBFI_1 = 1, NBFI_2_3, NBDC }
    public enum TimeLine
    {
        Today = 1,
        Yesterday = 2,
        MTD = 3, // month till date
        LMTD = 4,// last month till date
        YTD = 5,// year till date
        LYTD = 6,// last year till date
        QTD = 7,// quarter till date
        LYQTD = 8 // last year quarter till date
    }
    public enum Criteria
    {
        Number = 1,
        Amount = 2
    }

    public enum Stages
    {
        Call = 1,
        Lead = 2,
        [Display(Name="Files Submitted")]
        Files_Submitted = 3,
        [Display(Name = "Files Approved")]
        Files_Approved = 4,
        [Display(Name = "Files Disbursed")]
        Files_Disbursed = 5
    }

    public enum ProductSelection
    {
        [Display(Name = "Home Loan")]
        Home_Loan = 1,
        [Display(Name = "Auto Loan")]
        Auto_Loan,
        [Display(Name = "Personal Loan")]
        Personal_Loan,
        [Display(Name = "Fixed Deposit")]
        Fixed,
        [Display(Name = "Recurrent Deposit")]
        Recurring
    }

    public enum TypeOfWaiverReq { Processing_Fee = 1, CPV = 2, Valuation = 3, Property_Visit = 4 }
    public enum UomType { Length = 1, Area = 2 }
    public enum UpozilaOrThana { Upozilla = 1, Thana = 2 }
    public enum VehicleStatus { Brand_New = 1, Recondition = 2, Used_Cars = 3 }
    public enum VendorType { Individual = 1, Showroom = 2 }
    public enum VendorProductType { Car = 1, Electronics = 2, Others = 3 }
    public enum VerificationStatus
    {
        [Display(Name = "Not Verified")]
        Not_Verified = 1,
        [Display(Name = "Ok")]
        Ok = 2,
        [Display(Name = "Not Ok")]
        Not_Ok = 3
    }
    public enum VerificationAs
    {
        Applicant = 1,
        [Display(Name = "Co Applicant")]
        Co_Applicant = 2,
        Guarantor = 3
    }
    public enum VerificationState { [Display(Name = "Not Required")]NotRequired = 0, Pending, Verified }
    public enum VehicleType { Sedan_Car = 1, SUV = 2, Microbus = 3, Jeep = 4, Bus = 5, Truck = 6, Minibus = 7, Motor_Bike = 8 }
    public enum WelcomeLetterStatus { [Display(Name = "Not Issued")]Not_Issued = 1, Hold = 2, Sent = 3 }
    public enum YearsCurrentResidence
    {
        [Display(Name = "< 1 Year")]
        LessThenOneYear = 1,
        [Display(Name = "1-3 Year")]
        OneThreeRange = 2,
        [Display(Name = "5 Year")]
        FiveYear = 3,
        [Display(Name = "5-10 Year")]
        FiveTenRange = 4,
        [Display(Name = "> 10 Year")]
        GreaterTen = 5
    }
    public enum MessagingOffice { InterOffice = 1, IntraOffice = 2 }

    public enum PrinterFiltering
    {
        [Display(Name = "Before PO")]
        PO = 1,
        [Display(Name = "Before Disbursment")]
        Disbursment = 2
    }
    //public enum Purpose { Flat_Purchase = 1, Construction = 2, Semi_Paka_Construc = 1, Construction = 2 }
    public enum Purpose
    {
        [Display(Name = "Flat Purchase")]
        FlatPurchase = 1,
        [Display(Name = "Construction")]
        Construction = 2,
        [Display(Name = "Semi Paka Construction")]
        SemiPakaConstruction = 3,
        [Display(Name = "Home Equity")]
        HomeEquity = 4,
        [Display(Name = "Commercial Home Loan")]
        CommercialHomeLoan = 5
    }
    public enum OfferTextType
    {
        [Display(Name = "Early Settlement")]
        EarlySettlement = 1,
        [Display(Name = "Partial Payment")]
        PartialPayment = 2,
        [Display(Name = "Mode Of Disbursment")]
        DisbursmentMode = 3,
        [Display(Name = "Disbursment Condition")]
        DisbursmentCondition = 4,
        [Display(Name = "Documentation")]
        Documentation = 5
    }
    public enum ProposalStatus
    {
        [Display(Name = "Obtained")]
        Obtained = 1,
        [Display(Name = "Not Obtained")]
        Not_Obtained = 2
    }

    public enum InstrumentDeliveryStatus
    {
        [Display(Name = "Delivered to Client")]
        Delivered_to_client = 1,
        [Display(Name = "Delivered to RM")]
        Delivered_to_RM,
        [Display(Name = "Sent to Branch")]
        Sent_to_Branch,
        [Display(Name = "Kept in file")]
        Kept_in_file,
        [Display(Name = "Pending issue")]
        Pending_issue
    }
    public enum DCLExceptionAction
    {
        [Display(Name = "Rectify")]
        Rectify = 1,
        [Display(Name = "Replace")]
        Replace = 2,
        [Display(Name = "Waiver")]
        Waiver = 3,
        [Display(Name = "Deferral")]
        Deferral = 4,
        [Display(Name = "Others")]
        Others = 5,
        [Display(Name = "Obtained")]
        Obtained = 6
    }
    public enum AuthorityLevel { MCC = 1, EC = 2, Board = 3 }
    public enum PersonType
    {
        Customer = 1, Vendor = 2, Po = 3
    }
    public enum OfferLetterType
    {
        CRM = 1, Operation = 2, Customer = 3, Self_Assaignment = 4
    }
    public enum OtherCostEnum
    {
        Financing_Plan = 1, Landed_Property_Valuation = 2
    }

    public enum Stage
    {
        Call =1 ,Lead =2, Application=3,Approval= 4 ,Rejection = 5,Disbursement = 6
    }
    public enum NotificationType
    {
        //1
        [Display(Name = "Lead Waiting For Assignment")]
        LeadWaitingForAssignment,

        //2
        [Display(Name = "Lead Assigned")]
        LeadAssigned,

        //3
        [Display(Name = "Call Is Assigned To RM By BM")]
        CallAssignedRMByBM,

        //4
        [Display(Name = "Call Is Auto Assigned By Employee")]
        CallAutoAssignedByEmployee,

        //5
        [Display(Name = "Call Is Referred By Employee")]
        CallReferedByEmployee,

        //6
        [Display(Name = "Application Is Forwarded To CRM")]
        ApplicationForwardedToCRM,

        //7
        [Display(Name = "Application Under Process In CRM")]
        ApplicationUnderProcessInCRM,

        //8
        [Display(Name = "Application Under Process In Operations")]
        ApplicationUnderProcessInOperations,
        //9
        [Display(Name = "Offer Letter Issued")]
        ApplicationOfferLetterIssued,
        //10
        [Display(Name = "Application Is Ready For Disbursement")]
        ApplicationReadyForDisbursement,
        //11
        [Display(Name = "Application Is Fully or Partialy Disbursed")]
        ApplicationFullyPartialyDisbursement,
        //12
        [Display(Name = "Credit Memo Approved")]
        ApplicationCreditMemoApproved,
        //13
        [Display(Name = "Application Is Waiting For Approval By TL")]
        ApplicationWaitingForApprovalByTL,
        //14
        [Display(Name = "Application Is Waiting For Approval By BM")]
        ApplicationWaitingForApprovalByBM,
        //15
        [Display(Name = "Message Received")]
        MessageSentFromCRMOrOpsToAnyUser,
        //16
        [Display(Name = "Message Reply Received")]
        MessageReplySentFromCRMOrOpsToRM,
        //17
        [Display(Name = "Application Is Available For Generating Offer Letter")]
        ApplicationAvailableForGeneratingOfferLetter,
        //18
        [Display(Name = "Application Received From BM For Deposite Application")]
        ApplicationReceivedFromBMForDA ,
        //19
        [Display(Name = "DCL Approved For Deposit Application")]
        DCLApprovedForDA,
        //20
        [Display(Name = "Call Convert To Lead")]
        CallConvertToLead,
        //21
        [Display(Name = "Call Declared As Unsuccessful")]
        CallDeclaredAsUnsuccessful,
        //22
        [Display(Name = "Lead Declared As Successful")]
        LeadDeclaredAsSuccessful,
        //23
        [Display(Name = "Lead Declared As UnSuccessful")]
        LeadDeclaredAsUnSuccessful,
        //24
        [Display(Name = "Application Rejection By TL")]
        ApplicationRejectionByTL,
        //24
        [Display(Name = "Application Rejection By BM")]
        ApplicationRejectionByBM,
        //25
        [Display(Name = "Application Rejection By Credit Analyst")]
        ApplicationRejectionByCA,
        //26
        [Display(Name = "Application Rejection By Credit Memo Disapproval")]
        ApplicationRejectionByCreditMemoDisapproval,
        //27
        [Display(Name = "Application Rejected By Operations And FacilityCloser")]
        ApplicationRejectionByOpsAndFacilityCloser,
        //28
        [Display(Name = "RM Missed Lead Followup")]
        RMMissedLeadFollowupBy1Day,
        //29
        [Display(Name = "Upcoming Lead Followup in 30 mins")]
        BeforeScheduled30MinsLeadFollowUp,
        //30
        [Display(Name = "Today Is Sceduled Day for Document Collection As Per Application")]
        OnDayScheduledDocumentCollectionAsPerApplication,
        //31
        [Display(Name = "Today Is Sceduled Day for Document Collection As Per DCL")]
        OnDayScheduledDocumentCollectionAsPerDCL,
        //32
        [Display(Name = "RM Misses Lead Followup By 2 Days")]
        RMMissesLeadFollowupBy2Days,
        //33
        [Display(Name = "Yesterday Is Sceduled Day for Document Collection As Per DCL")]
        Day1BeforeScheduledDocumentCollectionDateAsPerDCL,
        //34
        [Display(Name = "Yesterday Is Sceduled Day for Document Collection As Per Application")]
        Day1BeforeScheduledDocumentCollectionDateAsPerApplication,
        //35
        [Display(Name = "Document Collection Date Is Past Due As Per DCL")]
        DocumentCollectionDateIsPastDueAsPerDCL,
        //36
        [Display(Name = "Document Collection Date Is Overdue For 1 days As Per DCL")]
        DocumentCollectionDateIsOverdueFor1daysAsPerDCL,
        //37
        [Display(Name = "Document Collection Date Is Overdue For 2 days As Per DCL")]
        DocumentCollectionDateIsOverdueFor2daysAsPerDCL,
        //38
        [Display(Name = "Document Collection Date Is Overdue For 3 days As Per DCL")]
        DocumentCollectionDateIsOverdueFor3daysAsPerDCL,
        //39
        [Display(Name = "Application waiting for Approval by TL for more than 2 days")]
        ApplicationWaitingForApprovalByTLForMore2Days,
        //40
        [Display(Name = "Application waiting for Approval by TL for more than 3 days")]
        ApplicationWaitingForApprovalByTLForMore3Days,
        //41
        [Display(Name = "Application waiting for Approval by BM for more than 3 days")]
        ApplicationWaitingForApprovalByBMForMore3Days,
        //42
        [Display(Name = "Application received from BM and unassigned for a day")]
        ApplicationReceivedFromBMAndUnassignedFor1Days,
        //43
        [Display(Name = "Application Received From CRM And Unassigned For 1 Days")]
        ApplicationReceivedFromCRMAndUnassignedFor1Days,
        //45
        [Display(Name = "Application Received From BM And Unassigned by 1 Days For Deposite Application")]
        ApplicationReceivedFromBMAndUnassignedFor1DaysForDA,
        //46
        [Display(Name = "Application Status For CRM Remains Unchanged by 2 Days")]
        ApplicationStatusForCRMDepartmentRemainsUnchangedForMore2Days,
        //47
        [Display(Name = "Application Status For Operations Department Remains Unchanged by 2 Days")]
        ApplicationStatusForOpsDepartmentRemainsUnchangedForMore2Days

    }
    public enum NotificationStatusType
    {
        ServiceNew, ServiceDone, New = 2, Viwed = 3, Done = 4
    }
}