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
    public class ClientController : BaseApiController
    {
        public ClientController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        // GET: api/Client
        public IEnumerable<ClientViewModel> GetClients()
        {
            var clients = UnitOfWork.Repository<Client>()
                .Get(includeProperties: "Person");

            var clientViewModels = Mapper.Map<IEnumerable<Client>, IEnumerable<ClientViewModel>>(clients);

            return clientViewModels;
        }

        // GET: api/Client/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult GetClient(int id)
        {
            var client = UnitOfWork.Repository<Client>()
                .Get(x => x.ClientId == id, includeProperties: "Person")
                .SingleOrDefault();
            if (client == null)
            {
                return NotFound();
            }

            var clientViewModel = Mapper.Map<Client, ClientViewModel>(client);

            return Ok(clientViewModel);
        }

        // PUT: api/Client/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClient(ClientViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = UnitOfWork.Repository<Client>()
                .Get(x => x.ClientId == viewModel.ClientId)
                .SingleOrDefault();
            if (client == null)
            {
                return BadRequest();
            }

            Mapper.Map<ClientViewModel, Client>(viewModel, client);
            client.Person.UpdatedAt = DateTime.Now;
            client.UpdatedAt = DateTime.Now;

            UnitOfWork.Repository<Client>().Update(client);

            try
            {
                UnitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(viewModel.ClientId))
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

        // POST: api/Client
        [ResponseType(typeof(Client))]
        public IHttpActionResult PostClient(ClientViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = Mapper.Map<ClientViewModel, Client>(viewModel);
            client.Person.CreatedAt = DateTime.Now;
            client.CreatedAt = DateTime.Now;          

            UnitOfWork.Repository<Client>().Insert(client);
            UnitOfWork.Save();

            return Ok();
            // return CreatedAtRoute("DefaultApi", new { id = client.ClientId }, client);
        }

        // DELETE: api/Client/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult DeleteClient(int id)
        {
            Client client = UnitOfWork.Repository<Client>().GetById(id);
            if (client == null)
            {
                return NotFound();
            }

            UnitOfWork.Repository<Client>().Delete(client);
            UnitOfWork.Save();

            return Ok(client);
        }

        private bool ClientExists(int id)
        {
            return UnitOfWork.Repository<Client>().GetQ().Count(e => e.ClientId == id) > 0;
        }
    }
}