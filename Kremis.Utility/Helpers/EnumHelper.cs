using Kremis.Utility.Enum;
using Kremis.Utility.Helpers;

namespace Kremis.Utility.Helpers
{
    public static class EnumHelper
    {
        public static string ToString(StatusEnum value)
        {
            return value switch
            {
                StatusEnum.Available => ConstantHelper.STATUS_AVAILABLE,
                StatusEnum.Unvailable => ConstantHelper.STATUS_NOT_AVAILABLE,
                StatusEnum.Reserved => ConstantHelper.STATUS_RESERVED,
                StatusEnum.Unpaid => ConstantHelper.STATUS_UNPAID,
                StatusEnum.PatiallyPaid => ConstantHelper.STATUS_PARTIALLY_PAID,
                StatusEnum.Paid => ConstantHelper.STATUS_PAID,
                _ => value.ToString(),
            };
        }

        public static string ToString(CommissionTypeEnum value)
        {
            return value switch
            {
                CommissionTypeEnum.None => ConstantHelper.COMMISSION_NONE,
                CommissionTypeEnum.Percentage => ConstantHelper.COMMISSION_PERCENTAGE,
                CommissionTypeEnum.Fixed => ConstantHelper.COMMISSION_FIXED,
                _ => value.ToString(),
            };
        }
    }
}
