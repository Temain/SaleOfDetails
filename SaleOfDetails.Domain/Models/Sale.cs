using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SaleOfDetails.Domain.Models
{
    /// <summary>
    /// Продажа
    /// </summary>
    [Table("Sale", Schema = "dbo")]
    public class Sale
    {
        public int SaleId { get; set; }

        /// <summary>
        /// Продукт
        /// </summary>
        public int ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Количество товаров
        /// </summary>
        public int NumberOfProducts { get; set; }

        /// <summary>
        /// Общая стоимость 
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public int ClientId { get; set; }
        public Client Client { get; set; }

        /// <summary>
        /// Сотрудник / продавец
        /// </summary>
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Дата продажи
        /// </summary>
        public DateTime? SaleDate { get; set; }

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
    }

}
