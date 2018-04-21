using System.Windows;

namespace RingOfElysiumLauncher.Data {

    public class LaunchParameters {

		public string Token { get; }        // Токен
		public string Uid { get; }          // ID пользователя
		public string Language { get; }		// Язык
		public string Server { get; }		// Сервер

		// Конструктор
		public LaunchParameters(string[] args) {
			foreach(string str in args) {
				Token = GetParameterValue(str, "token");
				Uid = GetParameterValue(str, "uid");
				Language = GetParameterValue(str, "language");
				Server = GetParameterValue(str, "server");
			}
		}

		// Возвращает зачение параметра имеющего тип "-(parameter)=(value)"
		string GetParameterValue(string str, string name) {
			if(str.IndexOf($"-{name}=") != -1) {
				return str.Replace($"-{name}=", "");
			}

			return "";
		}
	}
}
