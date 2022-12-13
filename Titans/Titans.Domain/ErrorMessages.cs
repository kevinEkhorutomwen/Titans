namespace Titans.Domain
{
    public static class ErrorMessages
    {
        public static string CanNotBeEmpty(string property) => $"{property} muss angegeben werden.";
        public static string UserNotFound(string username) => $"Kein Nutzer namens {username} gefunden";
        public const string WrongPassword = "Passwort ist falsch.";
        public const string UserAlreadyExist = "Der Benutzername ist bereits vergeben.";
        public const string PasswordMustBeIdentical = "Die Passwörter müssen übereinstimmen.";
        public const string CreationDateCantBefAfterExpiredDate = "Das Erstellungsdatum kann nicht nach dem Ablaufdatum sein.";
        public const string TokenInvalid = "Der Token ist ungültig.";
        public const string TokenExpired = "Der Token ist abgelaufen.";
    }
}
