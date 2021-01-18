using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MRRCManagement
{
    public class CRM
    {
        // This class manages the collection of customers. The CRM class loads the customers from 
        // file on start-up. IT is based on a list of Customer objects and supports operations
        // on the Customer (i.e. add, modify, delete)
        //
        // Authors: Oliver Pye (n9703977) and Taylor Gregory (n9719768)
        // Queensland University of Technology
        // CAB201 Major Project
        // May 2018

        public List<Customer> customers = new List<Customer>();
        private string crmFile = @"..\..\..\Data\customer.csv";

        // If there is no CRM file, this constructor constructs an empty CRM with no customers. 
        // Otherwise it loads the customers from file
        public CRM()
        {
            if (File.Exists(crmFile))
            {
                LoadFromFile();
            }
            else
            {
                File.Create(crmFile);
            }
        }

        // Adds the provided customer to the customer list if the customer ID doesn't already exist in the CRM
        public bool AddCustomer(Customer customer)
        {
            for (int i = 0; i < customers.Count(); i++)
            {
                if (customers[i] == customer)
                {
                    return false;
                }
            }
            customers.Add(customer);
            return true;
        }

        // Removes the customer from the CRM if they are not currently renting a vehicle
        public bool RemoveCustomer(Customer customer, Fleet fleet)
        {
            if (fleet.IsRenting(customer.CustomerID) == false)
            {
                customers.Remove(customer);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Removes the customer from the CRM if they are not currently renting a vehicle
        public bool RemoveCustomer(int customerID, Fleet fleet)
        {
            if (fleet.IsRenting(customerID) == false)
            {
                customers.Remove(GetCustomer(customerID));
                return true;
            }
            else
            {
                return false;
            }
        }

        // Tests to see if the Customer already exists in the CRM (based on customer ID)
        public bool CustIDDoesNotExist(int custID)
        {
            for (int i = 0; i < customers.Count(); i++)
            {
                if (customers[i].CustomerID == custID)
                {
                    return false;
                }
            }
            return true;
        }

        // Returns the list of current customers
        public List<Customer> GetCustomers()
        {
            return customers;
        }

        // Returns a customer based on Customer ID
        public Customer GetCustomer(int CID)
        {
            foreach (Customer customer in customers)
            {
                if (customer.CustomerID == CID)
                {
                    return customer;
                }
            }
            return null;
        }

        // Saves the current state of the CRM to file
        public void SaveToFile()
        {
            FileStream fs = new FileStream(crmFile, FileMode.Create, FileAccess.Write);
            StreamWriter fileOut = new StreamWriter(fs);


            string heading = "CustomerID, Title, FirstName, LastName, Gender, DOB";
            fileOut.WriteLine(heading);

            if (GetCustomers() != null)
            {
                foreach (Customer cust in GetCustomers())
                {
                    string body = cust.CustomerID.ToString() + ',' + cust.Title + ',' + cust.FirstNames + ',' + cust.LastName + ',' + cust.Gender_Property + ',' + cust.DateOfBirth;
                    fileOut.WriteLine(body);
                }
            }
            fileOut.Close();
            fs.Close();
        }

        // Loads the state of the CRM from file
        public void LoadFromFile() 
        {
            string inValue;
            StreamReader inFile;

            if (File.Exists(crmFile))
            {
                inFile = new StreamReader(crmFile);
                while ((inValue = inFile.ReadLine()) != null)
                {
                    string[] listInValue = inValue.Split(',');

                    if (listInValue[0] != "CustomerID")                 // to remove the headers
                    {
                        int customerID = int.Parse(listInValue[0]);
                        string title = listInValue[1];
                        string firstNames = listInValue[2];
                        string lastName = listInValue[3];
                        Customer.Gender gender = (Customer.Gender)Enum.Parse(typeof(Customer.Gender), listInValue[4]);
                        string dateOfBirth = listInValue[5];

                        Customer newCustomer = new Customer(customerID, title, firstNames, lastName, gender, dateOfBirth);
                        customers.Add(newCustomer);
                        Console.WriteLine(newCustomer);
                    }
                }
                inFile.Close();
            }
        }
        }
    }
