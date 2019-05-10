using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.Domain.Extensions;
using Chinook.Domain.ApiModels;

namespace Chinook.Domain.Supervisor
{
    public partial class ChinookSupervisor
    {
        public async Task<IEnumerable<CustomerApiModel>> GetAllCustomerAsync(
            CancellationToken ct = default)
        {
            var customers = await _customerRepository.GetAllAsync(ct);
            return customers.ConvertAll();
        }

        public async Task<CustomerApiModel> GetCustomerByIdAsync(int id,
            CancellationToken ct = default)
        {
            var customerApiModel = (await _customerRepository.GetByIdAsync(id, ct)).Convert;
            customerApiModel.Invoices = (await GetInvoiceByCustomerIdAsync(customerApiModel.CustomerId, ct)).ToList();
            customerApiModel.SupportRep =
                await GetEmployeeByIdAsync(customerApiModel.SupportRepId.GetValueOrDefault(), ct);
            customerApiModel.SupportRepName =
                $"{customerApiModel.SupportRep.LastName}, {customerApiModel.SupportRep.FirstName}";
            return customerApiModel;
        }

        public async Task<IEnumerable<CustomerApiModel>> GetCustomerBySupportRepIdAsync(int id,
            CancellationToken ct = default)
        {
            var customers = await _customerRepository.GetBySupportRepIdAsync(id, ct);
            return customers.ConvertAll();
        }

        public async Task<CustomerApiModel> AddCustomerAsync(CustomerApiModel newCustomerApiModel,
            CancellationToken ct = default)
        {
            /*var customer = new Customer
            {
                FirstName = newCustomerApiModel.FirstName,
                LastName = newCustomerApiModel.LastName,
                Company = newCustomerApiModel.Company,
                Address = newCustomerApiModel.Address,
                City = newCustomerApiModel.City,
                State = newCustomerApiModel.State,
                Country = newCustomerApiModel.Country,
                PostalCode = newCustomerApiModel.PostalCode,
                Phone = newCustomerApiModel.Phone,
                Fax = newCustomerApiModel.Fax,
                Email = newCustomerApiModel.Email,
                SupportRepId = newCustomerApiModel.SupportRepId
            };*/

            var customer = newCustomerApiModel.Convert;

            customer = await _customerRepository.AddAsync(customer, ct);
            newCustomerApiModel.CustomerId = customer.CustomerId;
            return newCustomerApiModel;
        }

        public async Task<bool> UpdateCustomerAsync(CustomerApiModel customerApiModel,
            CancellationToken ct = default)
        {
            var customer = await _customerRepository.GetByIdAsync(customerApiModel.CustomerId, ct);

            if (customer == null) return false;
            customer.FirstName = customerApiModel.FirstName;
            customer.LastName = customerApiModel.LastName;
            customer.Company = customerApiModel.Company;
            customer.Address = customerApiModel.Address;
            customer.City = customerApiModel.City;
            customer.State = customerApiModel.State;
            customer.Country = customerApiModel.Country;
            customer.PostalCode = customerApiModel.PostalCode;
            customer.Phone = customerApiModel.Phone;
            customer.Fax = customerApiModel.Fax;
            customer.Email = customerApiModel.Email;
            customer.SupportRepId = customerApiModel.SupportRepId;

            return await _customerRepository.UpdateAsync(customer, ct);
        }

        public Task<bool> DeleteCustomerAsync(int id, CancellationToken ct = default) 
            => _customerRepository.DeleteAsync(id, ct);
    }
}