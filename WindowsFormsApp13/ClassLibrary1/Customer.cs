using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MRRCManagement
{
    public class Customer
    {
        // This class manages the construction of the "Customer". It also enables a "Customer" to be
        // represented in various ways for saving and loading files.
        //
        // Authors: Oliver Pye (n9703977) and Taylor Gregory (n9719768)
        // Queensland University of Technology
        // CAB201 Major Project
        // May 2018

        // Data members, data fields or characteristics
        private int customerID;
        private string title;
        private string firstNames;
        private string lastName;
        public enum Gender { Male, Female};
        public Gender gender;
        private string dateOfBirth;

        // Constructor to construct a customer with the provided attributes
        public Customer(int customerID, string title, string firstNames, string lastName, Gender gender, string dateOfBirth)
        {
            this.customerID = customerID;
            this.title = title;
            this.firstNames = firstNames;
            this.lastName = lastName;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
        }

        // Properties
        public int CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string FirstNames
        {
            get { return firstNames; }
            set { firstNames = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public Gender Gender_Property
        {
            get { return gender; }
            set { gender = value; }
        }

        public string DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }

        // Returns a CSV representation of the customer that is consistent with the provided data files
        public string ToCSVString()
        {
            return customerID.ToString() + "," + title + "," + firstNames + "," + lastName + "," + gender + "," + dateOfBirth;
        }

        // Returns a string representation of the attributes of the customer
        public override string ToString()
        {
            return customerID.ToString() + " " + title + " " + firstNames + " " + lastName + " " + gender + " " + dateOfBirth;
        }
    }
}
