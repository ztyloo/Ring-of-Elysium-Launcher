using System.Windows;

namespace RingOfElysiumLauncher.Data {

    public class LaunchParameters {


        public string Token { get; }        // Токен
		public string Uid { get; }          // ID пользователя
		public string Language { get; set; }		// Язык
		public string Server { get; }		// Сервер

		// Конструктор
		public LaunchParameters(string[] args) {
			foreach(string str in args) {
                if(IsParameter(str, "token"))
                    Token = GetParameterValue(str, "token");

                else if (IsParameter(str, "uid"))
                    Uid = GetParameterValue(str, "uid");

                else if(IsParameter(str, "language"))
                    Language = GetParameterValue(str, "language");

                else if(IsParameter(str, "server"))
                    Server = GetParameterValue(str, "server");
			}
        }

        public LaunchParameters(string token, string uid, string language, string server) {
            Token = token;
            Uid = uid;
            Language = language;
            Server = server;
        }

        bool IsParameter(string str, string name) {
            if(str.IndexOf($"-{name}=") != -1) return true;

            return false;
        }

		// Возвращает зачение параметра имеющего тип "-(parameter)=(value)"
		string GetParameterValue(string str, string name) {
			if(IsParameter(str, name)) {
				return str.Replace($"-{name}=", "");
			}

			return "";
		}

        // Проверка на пустоту параметров
        public bool IsEmpty() {
            if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Uid) || string.IsNullOrEmpty(Language) || string.IsNullOrEmpty(Server))
                return true;

            return false;
        }
	}
}
