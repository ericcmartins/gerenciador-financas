using System;
using System.Linq;

namespace gerenciador.financas.Domain.Utils
{
    public class CpfValidator
    {
        private readonly string _cpf;

        public CpfValidator(string cpf)
        {
            _cpf = cpf;
        }

        public bool Valido()
        {
            if (string.IsNullOrWhiteSpace(_cpf))
                return false;

            string cpf = new string(_cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = cpf.Take(9).Select((c, i) => (c - '0') * multiplicadores1[i]).Sum();
            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = cpf.Take(10).Select((c, i) => (c - '0') * multiplicadores2[i]).Sum();
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
        }

        public static bool ValidaCpf(string cpf)
        {
            return new CpfValidator(cpf).Valido();
        }
    }
}