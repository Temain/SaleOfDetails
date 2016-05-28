using System;
using System.ComponentModel.DataAnnotations;

namespace SaleOfDetails.Web.Models
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    public class EmployeeViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Физическое лицо
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
        /// Дата приема на работу
        /// </summary>
        public DateTime EmployeeDateStart { get; set; }
    }

}
