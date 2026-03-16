namespace OrderHub.Infrastructure.Extensions;

/// <summary>
/// Extensões para conversão e validação de tipos
/// </summary>
public static class ConversionExtensions
{
    /// <summary>
    /// Tenta converter string para Guid de forma segura
    /// </summary>
    /// <param name="value">Valor a ser convertido</param>
    /// <param name="result">Guid convertido ou Guid.Empty se falhar</param>
    /// <returns>True se conversão bem-sucedida, false caso contrário</returns>
    public static bool TryConvertToGuid(this string? value, out Guid result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = Guid.Empty;
            return false;
        }

        return Guid.TryParse(value, out result);
    }

    /// <summary>
    /// Tenta converter string para decimal de forma segura
    /// </summary>
    /// <param name="value">Valor a ser convertido</param>
    /// <param name="result">Decimal convertido ou 0 se falhar</param>
    /// <returns>True se conversão bem-sucedida, false caso contrário</returns>
    public static bool TryConvertToDecimal(this string? value, out decimal result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0;
            return false;
        }

        return decimal.TryParse(value, out result);
    }

    /// <summary>
    /// Tenta converter string para int de forma segura
    /// </summary>
    /// <param name="value">Valor a ser convertido</param>
    /// <param name="result">Int convertido ou 0 se falhar</param>
    /// <returns>True se conversão bem-sucedida, false caso contrário</returns>
    public static bool TryConvertToInt(this string? value, out int result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0;
            return false;
        }

        return int.TryParse(value, out result);
    }
}

/// <summary>
/// Extensões para validação de valores
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Valida se string está vazia ou nula
    /// </summary>
    public static bool IsEmpty(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Valida se string não está vazia
    /// </summary>
    public static bool IsNotEmpty(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Valida se Guid é vazio
    /// </summary>
    public static bool IsEmpty(this Guid value)
    {
        return value == Guid.Empty;
    }

    /// <summary>
    /// Valida se Guid não é vazio
    /// </summary>
    public static bool IsNotEmpty(this Guid value)
    {
        return value != Guid.Empty;
    }

    /// <summary>
    /// Valida se decimal é maior que zero
    /// </summary>
    public static bool IsGreaterThanZero(this decimal value)
    {
        return value > 0;
    }

    /// <summary>
    /// Valida se decimal é maior ou igual a zero
    /// </summary>
    public static bool IsGreaterOrEqualZero(this decimal value)
    {
        return value >= 0;
    }
}

/// <summary>
/// Extensões para formatação de valores
/// </summary>
public static class FormattingExtensions
{
    /// <summary>
    /// Formata Guid como string legível (com hífens)
    /// </summary>
    public static string ToFormattedString(this Guid value)
    {
        return value.ToString("D");
    }

    /// <summary>
    /// Formata decimal como valor monetário em BRL
    /// </summary>
    public static string ToCurrencyString(this decimal value)
    {
        return value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    }

    /// <summary>
    /// Trunca string para um tamanho máximo com sufixo "..."
    /// </summary>
    public static string Truncate(this string? value, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - 3) + "...";
    }
}
