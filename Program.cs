using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XmlLinqSamples
{
    class Program
    {
        private const string XmlFileName = "PeopleData.xml";

        static void Main(string[] args)
        {
            var addressBook = GetAddressBookWithLinq();

            Console.WriteLine(addressBook.ToString());
            Console.ReadKey();
        }

        public static AddressBook GetAddressBookWithLinq()
        {
            var addressBook = new AddressBook();
            var root = XDocument.Load(@"../../" + XmlFileName).Root;

            var e = root.Elements("person");

            addressBook.Contacts = root.Elements("person").Select(p => new Person()
            {
                FirstName = GetXmlElementValue(p, "firstName"),
                LastName = GetXmlElementValue(p, "lastName"),
                BirthDate = Convert.ToDateTime(GetXmlElementValue(p, "birthDate")),
                Address = p.Descendants("address").Select(a => new Address()
                {
                    Street = GetXmlElementValue(a, "street"),
                    City = GetXmlElementValue(a, "city"),
                    State = GetXmlElementValue(a, "state"),
                    ZipCode = GetXmlElementValue(a, "zipCode")
                }).FirstOrDefault()
            }).ToList();

            return addressBook;
        }

        //Get an Xml Element's Value if it is not null or return null
        public static string GetXmlElementValue(XElement element, string elementName)
        {
            var xElement = element.Element(elementName);
            return xElement != null ? xElement.Value : null;
        }
    }

    public class AddressBook
    {
        public AddressBook()
        {
            Contacts = new List<Person>();
        }
        public List<Person> Contacts { get; set; }

        public override string ToString()
        {
            StringBuilder contactsBuilder = new StringBuilder();

            foreach (var contact in this.Contacts)
            {
                //Name
                contactsBuilder.Append("Name: " + contact.LastName + ", " + contact.FirstName);
                contactsBuilder.AppendLine();
                
                //Age
                contactsBuilder.Append("Age: " + contact.GetAge());
                contactsBuilder.AppendLine();

                //Address
                contactsBuilder.AppendLine("Address: ").AppendLine(contact.Address.Street)
                    .AppendLine(contact.Address.City + ", " + contact.Address.State + " " + contact.Address.ZipCode);

                //Seperate the rows if it's not the last item in the address book
                if (!contact.Equals(Contacts.Last()))
                {
                    contactsBuilder.AppendLine("================================");
                    contactsBuilder.AppendLine().AppendLine(); 
                }
            }
            return contactsBuilder.ToString();
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }

        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - this.BirthDate.Year;
            
            if (this.BirthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
