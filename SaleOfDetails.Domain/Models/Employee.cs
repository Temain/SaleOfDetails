using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SaleOfDetails.Domain.Models
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    [Table("Employee", Schema = "dbo")]
    public class Employee
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Физическое лицо
        /// </summary>
        public int PersonId { get; set; }
        public Person Person { get; set; }

        /// <summary>
        /// Дата приема на работу
        /// </summary>
        public DateTime? EmployeeDateStart { get; set; }

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

        public ICollection<Sale> Sales { get; set; }
    }

}
