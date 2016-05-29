using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SaleOfDetails.Domain.Models;
using SaleOfDetails.Web.Models.Mapping;

namespace SaleOfDetails.Web.Models
{
    /// <summary>
    /// Продажа
    /// </summary>
    public class SaleViewModel : IHaveCustomMappings
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int SaleId { get; set; }

        /// <summary>
        /// Продукт
        /// </summary>
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductCost { get; set; }

        /// <summary>
        /// Количество товаров
        /// </summary>
        private int _numberOfProducts;
        public int NumberOfProducts
        {
            get
            {
                return _numberOfProducts == 0 ? 1 : _numberOfProducts;
            }
            set
            {
                _numberOfProducts = value;
            }
        }

        /// <summary>
        /// Общая стоимость 
        /// </summary>
        // public decimal TotalCost { get; set; }

        public decimal TotalCostView { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public int ClientId { get; set; }

        public string ClientFullName { get; set; }

        public IEnumerable<ClientViewModel> Clients { get; set; } 

        /// <summary>
        /// Сотрудник / продавец
        /// </summary>
        public int EmployeeId { get; set; }

        public string EmployeeFullName { get; set; }

        public IEnumerable<EmployeeViewModel> Employees { get; set; }

        /// <summary>
        /// Дата продажи
        /// </summary>
        public DateTime? SaleDate { get; set; }


        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Sale, SaleViewModel>("Sale")
                .ForMember(m => m.ProductName, opt => opt.MapFrom(s => s.Product.ProductName))
                .ForMember(m => m.ProductCost, opt => opt.MapFrom(s => s.Product.Cost))
                .ForMember(m => m.EmployeeFullName, opt => opt.MapFrom(s => s.Employee.Person.FullName))
                .ForMember(m => m.ClientFullName, opt => opt.MapFrom(s => s.Client.Person.FullName))
                .ForMember(m => m.TotalCostView, opt => opt.MapFrom(s => s.NumberOfProducts*s.Product.Cost));

            configuration.CreateMap<SaleViewModel, Sale>("Sale");
        }
    }

}
