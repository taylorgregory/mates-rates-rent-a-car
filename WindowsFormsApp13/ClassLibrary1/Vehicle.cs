using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRRCManagement
{
    public class Vehicle
    {
        // This class manages the construction of the "Vehicle". It also enables a "Vehicle" to be
        // represented in various was for saving and searching capabilites (i.e. converting to string)
        //
        // Author's: Oliver Pye (n9703977) and Taylor Gregory (n9719768)
        // Queensland University of Technology
        // CAB201 Major Project
        // May 2018


        // defining the variables used in this class
        private string vehicleRego;
        private string make;
        private string model;
        private int year;
        public enum VehicleClass { Economy, Family, Luxury, Commercial };
        public VehicleClass vehicleClass;
        private int numSeats;
        public enum TransmissionType { Automatic, Manual };
        public TransmissionType transmission;
        public enum FuelType { Petrol, Diesel, Hybrid };
        public FuelType fuel;
        private bool GPS;
        private bool sunRoof;
        private double dailyRate;
        private string colour;


        //This method returns true if the rego exists
        public bool ContainsRego(string rego)
        {
            return vehicleRego.Contains(rego);
        }
        
        // This method returns the rego of a vehicle
        public string WhatRego(Vehicle vehicle)
        {
            // get the attributes for the input vehicle
            List<string> attributes = new List<string>();
            attributes = vehicle.GetAttributeList();

            //store the first attribute as the rego
            string rego = attributes[0];
            return rego;
        }


        // Constructor that provides values for all parameters of the vehicle
        public Vehicle(string vehicleRego, VehicleClass vehicleClass, string make, string model, int year,
            int numSeats, TransmissionType transmission, FuelType fuel, bool GPS, bool sunRoof,
            double dailyRate, string colour)
        {
            this.vehicleRego = vehicleRego;
            this.vehicleClass = vehicleClass;
            this.make = make;
            this.model = model;
            this.year = year;
            this.numSeats = numSeats;
            this.transmission = transmission;
            this.fuel = fuel;
            this.GPS = GPS;
            this.sunRoof = sunRoof;
            this.dailyRate = dailyRate;
            this.colour = colour;
        }

        // Constructor provides only the mandatory parameters of the vehicle
        // Others are set based on the defaults of each class
        public Vehicle(string vehicleRego, VehicleClass vehicleClass, string make, string model, int year)
        {
            //Default attributes
            this.vehicleRego = vehicleRego;
            this.make = make;
            this.model = model;
            this.year = year;
            numSeats = 4;
            transmission = TransmissionType.Automatic;
            fuel = FuelType.Petrol;
            GPS = false;
            sunRoof = false;
            colour = "black";
            dailyRate = 50;

            if (vehicleClass == VehicleClass.Economy)
            {
                transmission = TransmissionType.Automatic;
                dailyRate = 50;
            }
            else if (vehicleClass == VehicleClass.Family)
            {
                //Can have either manual or automatic for family class. 
                //Automatic will be the default for ease of the driver
                transmission = TransmissionType.Automatic;
                dailyRate = 80;
            }
            else if (vehicleClass == VehicleClass.Luxury)
            {
                sunRoof = true;
                GPS = true;
                dailyRate = 120;
            }
            else if (vehicleClass == VehicleClass.Commercial)
            {
                fuel = FuelType.Diesel;
                dailyRate = 130;
            }
        }



        //Below are the getters and setters for the variables used in Vehicle
        public string VehicleRego
        {
            get { return vehicleRego; }
            set { vehicleRego = value; }
        }
        public string Make
        {
            get { return make; }
            set { make = value; }
        }
        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        public VehicleClass VehicleClass_Property
        {
            get { return vehicleClass; }
            set { vehicleClass = value; }
        }

        public int NumSeats
        {
            get { return numSeats; }
            set { numSeats = value; }
        }

        public TransmissionType Transmission_Property
        {
            get { return transmission; }
            set { transmission = value; }
        }

        public FuelType Fuel_Property
        {
            get { return fuel; }
            set { fuel = value; }
        }

        public bool GPS_Property
        {
            get { return GPS; }
            set { GPS = value; }
        }

        public bool SunRoof
        {
            get { return sunRoof; }
            set { sunRoof = value; }
        }

        public double DailyRate
        {
            get { return dailyRate; }
            set { dailyRate = value; }
        }

        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        //Converts a vehicle into a string, with each attribute separated by a "," so that the returned
        //string is in CSV format
        public string ToCSVString()
        {
            return vehicleRego + "," + make + "," + model + "," + year.ToString() + "," + vehicleClass.ToString() +
                "," + numSeats + "," + transmission + "," + fuel + "," + GPS + "," + sunRoof + "," + colour + "," +
                dailyRate;
        }

        //Coverts a vehicle into a string which each attribute separated by a space
        public override string ToString()
        {
            return vehicleRego + " " + make + " " + model + " " + year.ToString() + " " + vehicleClass.ToString() +
                " " + numSeats.ToString() + " " + transmission.ToString() + " " + fuel.ToString() + " " +
                GPS.ToString() + " " + sunRoof.ToString() + " " + colour + " " + dailyRate.ToString();
        }

        //Retrieves all of the attriubtes of a vehicle and store them is a list of strings
        public List<string> GetAttributeList()
        {
            List<string> attributes = new List<string>();
            //Add each attribute to the list and convert to string if needed
            attributes.Add(vehicleRego);
            attributes.Add(make);
            attributes.Add(model);
            attributes.Add(year.ToString());
            attributes.Add(vehicleClass.ToString());
            //make number of seats readable i.e. if 4, the result is 4-Seater
            attributes.Add(numSeats + "-Seater");
            attributes.Add(transmission.ToString());
            attributes.Add(fuel.ToString());
            //if there is a GPS or sunroof, add "GPS" or "sunroof" rather than
            //true or false
            if (GPS == true)
            {
                attributes.Add("GPS");
            }
            if (sunRoof == true)
            {
                attributes.Add("sunroof");
            }
            attributes.Add(colour);

            return attributes;
        }
    }
}