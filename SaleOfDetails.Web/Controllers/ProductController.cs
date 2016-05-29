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
    public class ProductController : BaseApiController
    {
        public ProductController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        // GET: api/Product
        public IEnumerable<ProductViewModel> GetProducts()
        {
            var products = UnitOfWork.Repository<Product>()
                .GetQ();

            var productViewModels = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(products);

            return productViewModels;
        }

        // GET: api/Product/?query=#{query}
        public IHttpActionResult GetProducts(string query)
        {
            var products = UnitOfWork.Repository<Product>()
                .GetQ();

            if (query != null)
            {
                products = products.Where(x => x.ProductName.StartsWith(query));
            }

            var productViewModels = products.Select(x => new
            {
                Id = x.ProductId,
                Name = x.ProductName,
                Cost = x.Cost
            });

            return Ok(productViewModels);
        }

        // GET: api/Product/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            var product = UnitOfWork.Repository<Product>()
                .Get(x => x.ProductId == id)
                .SingleOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = Mapper.Map<Product, ProductViewModel>(product);

            return Ok(productViewModel);
        }

        // PUT: api/Product/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = UnitOfWork.Repository<Product>()
                .Get(x => x.ProductId == viewModel.ProductId)
                .SingleOrDefault();
            if (product == null)
            {
                return BadRequest();
            }

            Mapper.Map<ProductViewModel, Product>(viewModel, product);
            product.UpdatedAt = DateTime.Now;

            UnitOfWork.Repository<Product>().Update(product);

            try
            {
                UnitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(viewModel.ProductId))
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

        // POST: api/Product
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = Mapper.Map<ProductViewModel, Product>(viewModel);
            product.CreatedAt = DateTime.Now;          

            UnitOfWork.Repository<Product>().Insert(product);
            UnitOfWork.Save();

            return Ok();
        }

        // DELETE: api/Product/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = UnitOfWork.Repository<Product>().GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            UnitOfWork.Repository<Product>().Delete(product);
            UnitOfWork.Save();

            return Ok(product);
        }

        private bool ProductExists(int id)
        {
            return UnitOfWork.Repository<Product>().GetQ().Count(e => e.ProductId == id) > 0;
        }
    }
}