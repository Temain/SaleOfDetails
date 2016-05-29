using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using SaleOfDetails.Domain.Context;
using SaleOfDetails.Domain.DataAccess.Interfaces;
using SaleOfDetails.Domain.Models;
using SaleOfDetails.Web.Models;

namespace SaleOfDetails.Web.Controllers
{
    public class SaleController : BaseApiController
    {
        public SaleController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        // GET: api/Sale
        public IEnumerable<SaleViewModel> GetSales()
        {
            var sales = UnitOfWork.Repository<Sale>()
                .Get(includeProperties: "Product, Employee, Employee.Person, Client, Client.Person");

            var saleViewModels = Mapper.Map<IEnumerable<Sale>, IEnumerable<SaleViewModel>>(sales);

            return saleViewModels;
        }

        // GET: api/Sale/5
        [ResponseType(typeof(Sale))]
        public IHttpActionResult GetSale(int id)
        {
            var sale = UnitOfWork.Repository<Sale>()
                .Get(x => x.SaleId == id, 
                    includeProperties: "Product, Employee, Employee.Person, Client, Client.Person")
                .SingleOrDefault();
            if (sale == null && id != 0)
            {
                return NotFound();
            }

            var saleViewModel = new SaleViewModel();
            if (id != 0)
            {
                saleViewModel = Mapper.Map<Sale, SaleViewModel>(sale);
            }

            var clients = UnitOfWork.Repository<Client>().Get(includeProperties: "Person");
            saleViewModel.Clients = Mapper.Map<IEnumerable<Client>, IEnumerable<ClientViewModel>>(clients);

            var employees = UnitOfWork.Repository<Employee>().Get(includeProperties: "Person");
            saleViewModel.Employees = Mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);

            return Ok(saleViewModel);
        }

        // PUT: api/Sale/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSale(SaleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sale = UnitOfWork.Repository<Sale>()
                .Get(x => x.SaleId == viewModel.SaleId)
                .SingleOrDefault();
            if (sale == null)
            {
                return BadRequest();
            }

            Mapper.Map<SaleViewModel, Sale>(viewModel, sale);
            sale.UpdatedAt = DateTime.Now;

            UnitOfWork.Repository<Sale>().Update(sale);

            try
            {
                UnitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(viewModel.SaleId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Sale
        [ResponseType(typeof(Sale))]
        public IHttpActionResult PostSale(SaleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sale = Mapper.Map<SaleViewModel, Sale>(viewModel);
            sale.CreatedAt = DateTime.Now;          

            UnitOfWork.Repository<Sale>().Insert(sale);
            UnitOfWork.Save();

            return Ok();
        }

        // DELETE: api/Sale/5
        [ResponseType(typeof(Sale))]
        public IHttpActionResult DeleteSale(int id)
        {
            Sale sale = UnitOfWork.Repository<Sale>().GetById(id);
            if (sale == null)
            {
                return NotFound();
            }

            UnitOfWork.Repository<Sale>().Delete(sale);
            UnitOfWork.Save();

            return Ok(sale);
        }

        // GET: api/Sale/ChartData
        [HttpGet]
        [Route("api/Sale/ChartData/{year}")]
        public IHttpActionResult ChartData(int year)
        {
            var sales = UnitOfWork.Repository<Sale>()
                .GetQ(filter: x => x.SaleDate.HasValue && x.SaleDate.Value.Year == year,
                    includeProperties: "Product, Employee, Employee.Person, Client, Client.Person");
            var data = sales
                .GroupBy(g => g.SaleDate.Value.Month)
                .Select(x => new
                {
                    Month = x.Key,
                    Amount = x.Sum(s => s.NumberOfProducts*s.Product.Cost)
                });
                //.OrderBy(x => x.Month)
                //.Select(x => x.Amount);

            var months = Enumerable.Range(0, 11);
            var response = months.GroupJoin(data,
                m => m,
                d => d.Month,
                (m, g) => g
                    .Select(r => new KeyValuePair<int, decimal>(m, r.Amount))
                    .DefaultIfEmpty(new KeyValuePair<int, decimal>(m, 0))
                )
                .SelectMany(g => g)
                .Select(x => x.Value);

            return Ok(response);
        }

        private bool SaleExists(int id)
        {
            return UnitOfWork.Repository<Sale>().GetQ().Count(e => e.SaleId == id) > 0;
        }
    }
}