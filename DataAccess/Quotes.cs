using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database;
using Model;
using ModelWrapper;

namespace DataAccess
{
    public class Quotes
    {
        private readonly IDatabase _database;
        internal Quotes(IDatabase database)
        {
            _database = database ?? throw new ArgumentException("IDatabase object cannot be null.");
        }

        public async Task<ChangeTrackingCollection<T1>> GetQuotesAsync<T1>(string userId)
            where T1 : QuoteWrapper
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException($"User I.D. drawn from takeoff is invalid");

            var quotesData = await _database.GetRowsAsync("quote", new Dictionary<string, dynamic>
            {
                {"user_id", userId},
                {"is_deleted",  0}
            });

            var quotes = new List<T1>();
            foreach (var quote in quotesData)
            {
                quotes.Add(Activator.CreateInstance(typeof(T1), new Quote
                {
                    Id = quote["quote_id"],
                    FileNumber = quote["file_number"] is DBNull ? "" : quote["file_number"],
                    Date = Convert.ToDateTime(quote["date"]).Date,
                    Amount = (double?)(quote["amount"] is double ? quote["amount"] : null),
                    Cost = (double?)(quote["cost"] is double ? quote["cost"] : null),
                    Client = await GetClientByIdAsync((int)quote["client_id"]),
                    UserId = quote["user_id"]
                }) as T1);
            }
            return new ChangeTrackingCollection<T1>(quotes);
        }

        public async Task<QuoteWrapper> GetQuoteByFileNumberAsync(string fileNumber)
        {
            var quoteTask = await _database.GetRowsAsync("quote", new Dictionary<string, dynamic>
            {
                {"file_number", fileNumber}
            });

            if (quoteTask.Count == 0)
                return null;

            var quoteWrapper = new QuoteWrapper(new Quote
            {
                Id = quoteTask[0]["quote_id"],
                FileNumber = quoteTask[0]["file_number"],
                Date = Convert.ToDateTime(quoteTask[0]["date"]).Date,
                Amount = (double?)(quoteTask[0]["amount"] is double ? quoteTask[0]["amount"] : null),
                Cost = (double?)(quoteTask[0]["cost"] is double ? quoteTask[0]["cost"] : null),
                Client = await GetClientByIdAsync((int)quoteTask[0]["client_id"]),
                UserId = quoteTask[0]["user_id"]
            });

            return quoteWrapper;
        }

        private async Task<Contact> GetContactByClientIdAsync(int clientId)
        {
            var contactTask = await _database.GetRowsAsync("contact", new Dictionary<string, dynamic>
            {
                {"client_id", clientId}
            });

            if (contactTask.Count == 0)
                return null;

            var contact = new Contact()
            {
                Id = contactTask[0]["contact_id"],
                FirstName = contactTask[0]["first_name"] is DBNull ? "" : contactTask[0]["first_name"],
                LastName = contactTask[0]["last_name"] is DBNull ? "" : contactTask[0]["last_name"],
                Address = await GetAddressByContactIdAsync((int)contactTask[0]["contact_id"]),
                Phone = await GetPhoneByContactIdAsync((int)contactTask[0]["contact_id"])
            };

            return contact;
        }

        private async Task<Phone> GetPhoneByContactIdAsync(int contactId)
        {
            var phoneTask = await _database.GetRowsAsync("phone", new Dictionary<string, dynamic>
            {
                {"contact_id", contactId}
            });

            if (phoneTask.Count == 0)
                return null;

            var phone = new Phone
            {
                Id = phoneTask[0]["phone_id"],
                AfterHours = phoneTask[0]["after_hours"] is DBNull ? "" : phoneTask[0]["after_hours"],
                CellPhone = phoneTask[0]["cell"] is DBNull ? "" : phoneTask[0]["cell"],
                WorkPhone = phoneTask[0]["work"] is DBNull ? "" : phoneTask[0]["work"]
            };

            return phone;
        }

        private async Task<Address> GetAddressByContactIdAsync(int contactId)
        {
            var addressTask = await _database.GetRowsAsync("address", new Dictionary<string, dynamic>
            {
                {"contact_id", contactId}
            });

            if (addressTask.Count == 0)
                return null;

            var address = new Address
            {
                Id = addressTask[0]["address_id"],
                Number = addressTask[0]["number"] is DBNull ? "" : addressTask[0]["number"],
                Street = addressTask[0]["street"] is DBNull ? "" : addressTask[0]["street"],
                StreetType = addressTask[0]["street_type"] is DBNull ? "" : addressTask[0]["street_type"],
                PostCode = addressTask[0]["post_code"] is DBNull ? "" : addressTask[0]["post_code"],
                Town = addressTask[0]["town"] is DBNull ? "" : addressTask[0]["town"]
            };

            return address;
        }

        private async Task<Client> GetClientByIdAsync(int clientId)
        {
            var clientTask = await _database.GetRowsAsync("client", new Dictionary<string, dynamic>
            {
                {"client_id", clientId}
            });

            if (clientTask.Count == 0)
                return null;

            var clientWrapper = new Client
            {

                Id = clientId,
                Name = clientTask[0]["name"] is DBNull ? "" : clientTask[0]["name"],
                Discount = await GetDiscountByIdAsync((int)clientTask[0]["discount_id"]),
                Contact = await GetContactByClientIdAsync((int)clientTask[0]["client_id"])
            };

            return clientWrapper;
        }

        private async Task<Discount> GetDiscountByIdAsync(int discountId)
        {
            var discountTask = await _database.GetRowsAsync("discount", new Dictionary<string, dynamic>
            {
                {"discount_id", discountId}
            });

            if (discountTask.Count == 0)
                return null;

            var discount = new Discount
            {
                Id = discountId,
                Code = discountTask[0]["code"] is DBNull ? "" : discountTask[0]["code"]
            };

            return discount;
        }

        public async Task<ChangeTrackingCollection<T1>> SearchQuotesByKeywordAsync<T1>(string keyword) where T1 : QuoteWrapper
        {
            var quotesData = await _database.SearchRowsAsync(
                $"quote LEFT JOIN client ON client.client_id = quote.client_id LEFT JOIN discount ON discount.discount_id = client.discount_id " +
                "LEFT JOIN contact ON contact.client_id = client.client_id LEFT JOIN phone ON phone.contact_id = contact.contact_id " +
                "LEFT JOIN address ON address.contact_id = contact.contact_id ",
                new List<string> { "quote.quote_id", "quote.file_number", "quote.date", "quote.amount", "quote.cost", "quote.user_id", "quote.client_id" },
                new List<string> { "quote.file_number", "client.name", "contact.first_name", "contact.last_name", "address.street", "address.town" },
                $"{keyword}");
            var quotes = new List<T1>();
            foreach (var quote in quotesData)
            {
                quotes.Add(Activator.CreateInstance(typeof(T1), new Quote
                {
                    Id = quote["quote_id"],
                    FileNumber = quote["file_number"] is DBNull ? "" : quote["file_number"],
                    Date = Convert.ToDateTime(quote["date"]).Date,
                    Amount = (double?)(quote["amount"] is double ? quote["amount"] : null),
                    Cost = (double?)(quote["cost"] is double ? quote["cost"] : null),
                    Client = await GetClientByIdAsync((int)quote["client_id"]),
                    UserId = quote["user_id"]
                }) as T1);
            }
            return new ChangeTrackingCollection<T1>(quotes);
        }
    }
}