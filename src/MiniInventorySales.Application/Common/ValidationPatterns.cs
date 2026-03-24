namespace MiniInventorySales.Application.Common
{
    public static class ValidationPatterns
    {
        public const string FullName = @"^[A-Za-z\s\.\-']+$";
        public const string Username = @"^(?!.*[_.-]{2})[a-zA-Z0-9][a-zA-Z0-9._-]{2,49}$";
        public const string StrongPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,100}$";
        public const string NigeriaPhoneNumber = @"^(\+234|234|0)[789][01]\d{8}$";
        public const string InternationalPhoneNumber = @"^\+?[1-9]\d{9,14}$";
    }
}
