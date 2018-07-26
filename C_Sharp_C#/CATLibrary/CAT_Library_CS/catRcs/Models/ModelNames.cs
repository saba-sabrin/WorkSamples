using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace CatRcs.Models
{
    public static class ModelNames
    {
        public enum Models
        {
            [Description("GRM")]
            GRM,
            [Description("MGRM")]
            MGRM,
            [Description("PCM")]
            PCM,
            [Description("GPCM")]
            GPCM,
            [Description("RSM")]
            RSM,
            [Description("NRM")]
            NRM,
            [Description("NoModel")]
            NoModel
        }

        public static string EnumToString(this Models val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static Models StringToEnum(string enumVal)
        {
            Models enumResult = new Models();

            try
            {
                enumResult = (Models)Enum.Parse(typeof(Models), enumVal);
            }
            catch(ArgumentException ex)
            {
                // Not Handled
            }

            return enumResult;
        }

        public enum MWI_Type
        {
            MLWI = 1,
            MPWI = 2
        }

        // Abilitiy Estimator Method Options

        public enum EstimaatorMethods
        {
            [Description("ML")]
            ML,
            [Description("BM")]
            BM,
            [Description("EAP")]
            EAP,
            [Description("WL")]
            WL
        }

        public static string EnumToString(this EstimaatorMethods val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static EstimaatorMethods StringToEnumMethods(string enumVal)
        {
            EstimaatorMethods enumResult = new EstimaatorMethods();

            try
            {
                enumResult = (EstimaatorMethods)Enum.Parse(typeof(EstimaatorMethods), enumVal);
            }
            catch (ArgumentException ex)
            {
                // Not Handled
            }

            return enumResult;
        }

        public enum KLTypes
        {
            KL = 1,
            KLP = 2
        }

        public enum InfoType
        {
            Fisher = 1,
            Observed = 2
        }

        public enum StartSelectTypes
        {
            NA = 0,
            BOpt = 1,
            MFI = 2,
            Proportional = 3,
            Progressive = 4,
            ThOpt = 5
        }

        public enum RuleType
        {
            Length = 1,
            Precision = 2
        }

        public enum CriterionTypes
        {
            bOpt = 1,
            thOpt = 2,
            KL = 3, 
            KLP = 4,
            MFI = 5,
            MEI = 6,
            MLWI = 7,
            MPWI = 8,
            MEPV = 9,
            progressive = 10,
            proportional = 11,
            random = 12
        }

        public enum DistributionTypes
        {
            norm = 1,
            unif = 2,
            Jeffreys = 3
        }
    }
}
