using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SaleOfDetails.Domain.Models
{
    /// <summary>
    /// Физическое лицо
    /// </summary>
    [Table("Person", Schema = "dbo")]
    public class Person
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        [StringLength(500)]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [StringLength(500)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [StringLength(500)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Дата создания записи
        /// </summary>
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Дата обновления записи
        /// </summary>
        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        [JsonIgnore]
        public DateTime? DeletedAt { get; set; }


        public ICollection<Employee> Employees { get; set; }

        public ICollection<Client> Clients { get; set; }

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }


        /// <summary>
        /// Метод возвращает фамилию имя отчество
        /// </summary>
        public string FullName
        {
            get
            {
                var fullName = LastName + " " + FirstName;
                if (!String.IsNullOrEmpty(MiddleName))
                {
                    fullName += " " + MiddleName;
                }
                return fullName;
            }
        }

        /// <summary>
        /// Инициалы и фамилия: Иванов И.И.
        /// </summary>
        public string ShortName
        {
            get
            {
                const string initialTerminator = ".";
                var shortName = LastName + " " + FirstName[0] + initialTerminator;
                if (!String.IsNullOrEmpty(MiddleName))
                {
                    shortName += MiddleName[0] + initialTerminator;
                }

                return shortName;
            }
        }

    }

}
