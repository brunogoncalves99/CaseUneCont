namespace UneContApp.Service
{
    public class ValidaCNPJ
    {

        #region Validação CNPJ
        /// <summary>
        /// Valida um CNPJ utilizando o algoritmo oficial de validação dos dígitos verificadores
        /// </summary>
        public static bool ValicaoCNPJ(string cnpj)
        {
            if(string.IsNullOrEmpty(cnpj))
                return false;

            if(cnpj.Length != 14)
                return false;

            if (cnpj.Distinct().Count() == 1)
                return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;

            for (int i = 0; i < 12; i++)
            {
                soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];
            }

            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cnpj[12].ToString()) != digitoVerificador1)
                return false;

            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;

            for (int i = 0; i < 13; i++)
            {
                soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cnpj[13].ToString()) == digitoVerificador2;
        }
        #endregion

    }
}
