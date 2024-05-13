namespace EM.Domain.Utilitarios
{
	public class Validacoes
	{
		public static bool CPFValidacao(string cpf)
		{
			cpf = cpf.Trim().Replace(".", "").Replace("-", "");

			if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
				return false;

			int[] multiplicador1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
			int[] multiplicador2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

			string tempCpf = cpf[..9];
			int soma = tempCpf.Zip(multiplicador1, (x, y) => int.Parse(x.ToString()) * y).Sum();
			int resto = soma % 11;
			int digito1 = resto < 2 ? 0 : 11 - resto;

			tempCpf += digito1;
			soma = tempCpf.Zip(multiplicador2, (x, y) => int.Parse(x.ToString()) * y).Sum();
			resto = soma % 11;
			int digito2 = resto < 2 ? 0 : 11 - resto;

			return cpf.EndsWith(digito1.ToString() + digito2.ToString());
		}
	}
}