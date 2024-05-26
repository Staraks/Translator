using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    /// <summary>
    /// Класс для описания токена
    /// </summary>
    class Token
    {
        /// <summary>
        /// Свойство описывает тип токена
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// Свойтсво описывает значение токена
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Конструктор присваевает свойству Type принимаемый аргумент type
        /// </summary>
        /// <param name="type"></param>
        public Token(TokenType type)
        {
            Type = type;
        }

        /// <summary>
        /// Возвращает строку в формате "{Значение токена} - {Тип токена}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} : {1}", Value, Type);
        }
    }
}