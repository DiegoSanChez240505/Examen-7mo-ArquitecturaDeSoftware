using System.ComponentModel.DataAnnotations;

namespace ExamenDiegoUnidad2.Validators
{
    public class PromedioValidoAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is decimal promedio)
            {
                return promedio >= 0 && promedio <= 10;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"El {name} debe estar entre 0 y 10";
        }
    }

    public class SemestreValidoAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is sbyte semestre)
            {
                return semestre >= 1 && semestre <= 12;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"El {name} debe estar entre 1 y 12";
        }
    }

    public class MatriculaFormatoAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string matricula)
            {
                // Formato: 2 letras seguidas de 6-8 números
                return System.Text.RegularExpressions.Regex.IsMatch(matricula, @"^[A-Za-z]{2}\d{6,8}$");
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"La {name} debe tener el formato: 2 letras seguidas de 6-8 números (ej: AB123456)";
        }
    }

    public class NombreCompletoAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string nombre)
            {
                // Al menos dos palabras, solo letras y espacios
                var palabras = nombre.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return palabras.Length >= 2 && 
                       palabras.All(p => System.Text.RegularExpressions.Regex.IsMatch(p, @"^[A-Za-zÁáÉéÍíÓóÚúÑñ]+$"));
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"El {name} debe contener al menos nombre y apellido, solo letras y espacios";
        }
    }
}