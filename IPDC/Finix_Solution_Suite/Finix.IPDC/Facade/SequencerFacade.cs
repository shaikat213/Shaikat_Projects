using Finix.IPDC.Infrastructure.Models;
using System;
using System.Linq;

namespace Finix.IPDC.Facade
{
    public class SequencerFacade : BaseFacade
    {
        public string GetUpdatedCIFPersonalNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }
                    
                var cif = (Convert.ToDecimal(sequence.CIFNumber.Trim()) + 1).ToString("00000000");
                sequence.CIFNumber = cif;
                GenService.Save(sequence);
                return "CIFP-" + cif;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetUpdatedCIFOrganizationalNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var cif = (Convert.ToDecimal(sequence.CIFNumber.Trim()) + 1).ToString("00000000");
                sequence.CIFNumber = cif;
                GenService.Save(sequence);
                return "CIFO-" + cif;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetUpdatedApplicationNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var appNo = (Convert.ToDecimal(sequence.ApplicationNumber.Trim()) + 1).ToString("00000000");
                sequence.ApplicationNumber = appNo;
                GenService.Save(sequence);
                return "APN-" + appNo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetUpdatedAccGroupNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var grpNo = (Convert.ToDecimal(sequence.AccGroupId.Trim()) + 1).ToString("00000000");
                sequence.AccGroupId = grpNo;
                GenService.Save(sequence);
                return "CIFG-" + grpNo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetUpdatedOfferLetterNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var offerNo = (Convert.ToDecimal(sequence.OfferLetterNo.Trim()) + 1).ToString("00000000");
                sequence.OfferLetterNo = offerNo;
                GenService.Save(sequence);
                return "OfLtr-" + offerNo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetUpdatedDCLNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var dclNo = (Convert.ToDecimal(sequence.DCLNo.Trim()) + 1).ToString("00000000");
                sequence.DCLNo = dclNo;
                GenService.Save(sequence);
                return "Dcl-" + dclNo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GetUpdatedDMNo
        public string GetUpdatedDMNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var dmnNo = (Convert.ToDecimal(sequence.DMNo.Trim()) + 1).ToString("00000000");
                sequence.DMNo = dmnNo;
                GenService.Save(sequence);
                return "DMN-" + dmnNo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IndexInitializer()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if(sequence == null)
                {
                    sequence = new IndexSequencer();
                    sequence.AccGroupId = "00000000";
                    sequence.ApplicationNumber = "00000000";
                    sequence.CIFNumber = "00000000";
                    sequence.CIFNumber = "00000000";
                    sequence.CreditMemoNo = "00000000";
                    sequence.OfferLetterNo = "00000000";
                    sequence.DCLNo = "00000000";
                    sequence.DMNo = "00000000";
                    sequence.CreateDate = DateTime.Now;
                    GenService.Save(sequence);
                    GenService.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public string GetUpdatedCallNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var call = (Convert.ToDecimal(sequence.CallNo.Trim()) + 1).ToString("00000000");
                sequence.CallNo = call;
                GenService.Save(sequence);
                return "Call-" + call;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUpdatedCreditMemoNo()
        {
            try
            {
                var sequence = GenService.GetAll<IndexSequencer>().First();
                if (sequence == null)
                {
                    IndexInitializer();
                    sequence = GenService.GetAll<IndexSequencer>().First();
                }

                var crm = (Convert.ToDecimal(sequence.CreditMemoNo.Trim()) + 1).ToString("00000000");
                sequence.CreditMemoNo = crm;
                GenService.Save(sequence);
                return "CRM-" + crm;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
