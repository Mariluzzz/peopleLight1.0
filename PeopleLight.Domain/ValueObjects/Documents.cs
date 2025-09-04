namespace PeopleLight.Domain.ValueObjects
{
    public class Documents
    {
        public string Value { get; private set; }

        public Documents(string value)
        {
            if (!IsValid(value))
                throw new ArgumentException("CPF ou CNPJ inválido");
            Value = value;
        }

        private bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && (value.Length == 11 || value.Length == 14)
                   && value.All(char.IsDigit);
        }
    }
}
