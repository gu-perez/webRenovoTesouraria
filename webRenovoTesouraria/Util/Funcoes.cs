using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.Util
{
    public class Funcoes
    {
        public static string ValidarLogin(string valor)
        {
            string retorno = "";
            if (String.IsNullOrEmpty(valor))
                retorno = "Os dois nomes precisam estar preenchidos";
            else
            {
                string[] nome_array = valor.Split(' ');

                if (valor.Length < 5) retorno = "Preencher os dois nomes completos;";
                if (nome_array.Count() < 2) retorno = "Preencher com Nome Completo";
                if (ValidarNumeros(valor)) retorno = "Preencha os campos com os nomes completos de forma correta.";
            }

            return retorno;
        }

        public static bool ValidarNumeros(string valor)
        {
            System.Text.RegularExpressions.Regex oValidador = new System.Text.RegularExpressions.Regex("[0-9]");
            return oValidador.IsMatch(valor) || oValidador.IsMatch(valor);
        }

        public static string RetornaDia()
        {
            DateTime dt_hoje = DateTime.Now;
            string retorno = "";
            switch (dt_hoje.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    retorno = "Domingo";
                    break;
                case DayOfWeek.Monday:
                    retorno = "Segunda-feira";
                    break;
                case DayOfWeek.Tuesday:
                    retorno = "Terça-feira";
                    break;
                case DayOfWeek.Wednesday:
                    retorno = "Quarta-feira";
                    break;
                case DayOfWeek.Thursday:
                    retorno = "Quinta-feira";
                    break;
                case DayOfWeek.Friday:
                    retorno = "Sexta-feira";
                    break;
                case DayOfWeek.Saturday:
                    retorno = "Sábado";
                    break;
                default:
                    break;
            }

            return retorno;
        }

        public static string RetornaPeriodo()
        {
            DateTime dt_hoje = DateTime.Now;
            if (dt_hoje.Hour > 6 && dt_hoje.Hour < 12) return "Manhã";
            if (dt_hoje.Hour > 12 && dt_hoje.Hour < 18) return "Tarde";
            if (dt_hoje.Hour > 18 && dt_hoje.Hour <= 24) return "Noite";
            return "Madrugada";
        }

    }
}
