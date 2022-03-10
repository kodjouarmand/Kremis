namespace Kremis.Utility.Helpers
{
    public static class ConstantHelper
    {
        public const string DEFAULT_ERROR_PAGE = "/User/Home/Error";
        public const string TEMP_DATA_ERROR = "Error";
        public const string TEMP_DATA_SUCCESS = "Success";

        public const string DEFAULT_LAND_TITLE_DOCUMENT_FOLDER_NAME = "documents/tites fonciers";
        public const string DEFAULT_PARCEL_DOCUMENT_FOLDER_NAME = "documents/lots";
        public const string DEFAULT_CUSTOMER_DOCUMENT_FOLDER_NAME = "documents/clients";
        public const string DEFAULT_DOCUMENT_EXTENSION = ".pdf";

        public const string ROLE_NAME_SIMPLE_USER = "Simple user";
        public const string ROLE_NAME_ADMIN = "Administrator";
        public const string ROLE_NAME_SUPER_ADMIN = "Super administrator";

        public const string SESSION_KEY_CURRENT_USER = "CurrentUser";

        public const string INIT_DATA_PATH = @"data\init";

        public const string STATUS_AVAILABLE = "Disponible";
        public const string STATUS_NOT_AVAILABLE = "Indisponible";
        public const string STATUS_RESERVED = "Reservé";
        public const string STATUS_UNPAID = "Impayé";
        public const string STATUS_PARTIALLY_PAID = "Avancé";
        public const string STATUS_PAID= "Payé";

        public const string COMMISSION_NONE = "Aucune";
        public const string COMMISSION_PERCENTAGE = "Pourcentage";
        public const string COMMISSION_FIXED = "Fixe";

        public const string PAYMENT_MODE_CASH = "Cash";

        public const string DEFAULT_CURRENCY = "F CFA";

        public const int DEFAULT_PAGE_SIZE = 10;
    }
}
