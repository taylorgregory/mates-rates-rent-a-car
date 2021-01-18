using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MRRCManagement;

namespace MRRCManagement
{
    public class Fleet 
    {
        // This class manages the rentals of vehicles. It also contains the search methods
        // used for the search component of the GUI
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
        private int numSeats;
        private bool GPS;
        private bool sunRoof;
        private double dailyRate;
        private string colour;
        public List<Vehicle> vehicles = new List<Vehicle>();
        public Dictionary<string, int> rentals = new Dictionary<string, int>();
        private string rego;//This is to add rego to the rentals dictionary
        private int customerID;//This is for the rental dictionary

        // Data file locations
        public string fleetFile = @"..\..\..\Data\fleet.csv";
        public string rentalFile = @"..\..\..\Data\rentals.csv";




        //Loads or creates the fleet and rental files
        public Fleet()
        {
            if (File.Exists(fleetFile))
            {
                // loads the rental and fleet files if the 
                // fleet file exists
                LoadFromFile();
            }
            else
            {
                // create empty fleet and rental files if 
                // the fleet file doesn't exist
                File.Create(fleetFile);
                File.Create(rentalFile);
            }

        }

        // Checks the vehicles to see if a given rego
        // already exists. Returns true if the rego does
        // not exist, false if it does
        public bool RegoDoesNotExist(string rego)
        {
        	// cycle through all of the vehicles
            for (int i = 0; i < vehicles.Count(); i++)
            {
            	//check if rego exists in the current vehicle
                if (vehicles[i].VehicleRego == rego)
                {
                    return false;
                }
            }
            return true;
        }

        // Adds a vehicle if it does not already exists
        // returns true if vehicle does not exist
        public bool AddVehicle(Vehicle vehicle)
        {
            // cycle through all of the vehicles
            for (int i = 0; i < vehicles.Count(); i++)
            {
                // if the vehicle exists, return false
                if (vehicles[i] == vehicle)
                {
                    return false;
                }
            }
            // add vehicle and return true if it does not exist
            vehicles.Add(vehicle);
            return true;
        }

        // Removes a vehicle it if is not currently rented
        public bool RemoveVehicle(Vehicle vehicle)
        {
            // get the rego of the input vehicle
            string rego = vehicle.WhatRego(vehicle);

            // return false if vehicle is rented
            if (IsRented(rego) == true)
            {
                return false;
            }
            else
            {
                // if the vehicles exists, remove it and return true
                for (int i = 0; i < vehicles.Count(); i++)
                {
                    if (vehicles[i] == vehicle)
                    {
                        vehicles.Remove(vehicle);
                        return true; 
                    }
                }
                // if the vehicle is not rented and does not exist, return false
                return false;
            }
            
        }

        // removes a vehicle if it is not rented given the vehicle rego
        public bool RemoveVehicle(string vehicleRego)
        {
            //return false if the vehicle is rented
            if (IsRented(vehicleRego) == true)
            {
                return false;
            }
            else
            {
                // if the vehicle is availabe and exists, remove it and return true
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (vehicles[i].ContainsRego(vehicleRego))
                    {
                        vehicles.Remove(vehicles[i]);
                        return true;
                    }
                }
                // if the vehicle is not available, return false
                return false;
            }
        }

        // returns the list of vehicles in the fleet
        public List<Vehicle> GetFleet()
        {
            return vehicles;
        }

        // returns a list of vehicles that are either being rented (true input)
        // or are available to rent (false input) 
        public List<Vehicle> GetFleet(bool rented)
        {
            List<Vehicle> getFleet = new List<Vehicle>();

            // get rented vehicles if input is true
            if (rented == true)
            {
                // loops through each rental in the rentals dictionary
                foreach (KeyValuePair<string, int> item in rentals)
                {
                    for (int i = 0; i < vehicles.Count; i++)
                    {
                        // compares the rego in the dictionary to the rego of all
                        // the vehicles. If they match, add to the list
                        if (item.Key == vehicles[i].WhatRego(vehicles[i]))
                        {
                            getFleet.Add(vehicles[i]);
                        }
                    }
                }
                return getFleet;
            }

            // return not rented vehicles
            else
            {
                // loops through each rental in the rentals dictionary
                foreach (KeyValuePair<string, int> item in rentals)
                {
                    for (int i = 0; i < vehicles.Count; i++)
                    {
                        // compares the rego in the dictionary to the rego of all
                        // the vehicles. If they do not match, add to the list
                        if (item.Key != vehicles[i].WhatRego(vehicles[i]))
                        {
                            getFleet.Add(vehicles[i]);
                        }
                    }
                }
                return getFleet;
            }
        }


        // checks whether the vehicle with the provided rego is currently
        // being rented
        public bool IsRented(string vehicleRego)
        {
            // loop through each rental in the dicitonary
            foreach (KeyValuePair<string, int> item in rentals)
            {
                // compare the rego of the rental to the input and
                // return true if they are equal (match)
                if (item.Key == vehicleRego)
                {
                    return true;
                }
            }
            // return false if none of the rentals match the input rego
            return false;
        }

        // checks if the customer with the input ID is currently renting a vehicle.
        public bool IsRenting(int customerID)
        {
            // loop through each rental
            foreach (KeyValuePair<string, int> item in rentals)
            {
                // comapre the customer id of the rentals with the input
                // and if they match return true
                if (item.Value == customerID)
                {
                    return true;                    
                }
            }
            // return false if there are no matches
            return false;
        }

        // returns the customer ID of the customer who is renting the vehicle
        // with the given input rego
        public int RentedBy(string vehicleRego)
        {
            // check that the vehcile is being rented by someone
            if (IsRented(vehicleRego) == true)// this if statement propably not needed
            {
                //comare the rental regos with the input rego
                foreach (KeyValuePair<string, int> item in rentals)
                {
                    // if there is a match, return the cusomter ID
                    if (item.Key == vehicleRego)
                    {
                        return item.Value;
                    }
                }
            }
            // If there is no match return -1
            return -1;
        }

        // rents the vehicle with the given rego to the customer with the given id
        public bool RentCar(string vehicleRego, int customerID)
        {
            //The try/catch is used to check if the car already rented
            try
            {
                //if not already rented, add the detials of the cusomter and vehicle
                //to the rentals dictionary and return true
                rentals.Add(vehicleRego, customerID);
                return true;
            }
            //If an error is thrown that the input is already stored in the dictionary,
            // return false. 
            catch (ArgumentException)
            {
                return false;
            }
        }


        //returns the car with the given rego. Does not have validation
        public int ReturnCar(string vehicleRego)
        {
            rentals.Remove(vehicleRego);
            return customerID;
        }


        // returns the vehicle with the given registration
        public Vehicle GetVehicle(string registration)
        {
            //cycle through all of the vehicles
            foreach (Vehicle vehicle in vehicles)
            {
                // return the vehicle with the same rego
                if (vehicle.VehicleRego == registration)
                {
                    return vehicle;
                }
            }
            return null;
        }

        //Saves the data by converting the data to CSV strings and then writing it to the CSV files.
        public void SaveToFile()
        {
            FileStream fs_fleet = new FileStream(fleetFile, FileMode.Create, FileAccess.Write);
            StreamWriter fileOut_fleet = new StreamWriter(fs_fleet);

            string heading_fleet = "Rego,Make,Model,Year,Vehicle Class,Number of Seats,Transmission,Fuel,GPS,Sun Roof,Colour,Daily Rate";
            fileOut_fleet.WriteLine(heading_fleet);

            if (GetFleet() != null)
            {
                foreach (Vehicle vehicle in GetFleet())
                {
                    string body_fleet = vehicle.ToCSVString();
                    fileOut_fleet.WriteLine(body_fleet);
                }
            }
            fileOut_fleet.Close();
            fs_fleet.Close();

            FileStream fs_rental = new FileStream(rentalFile, FileMode.Create, FileAccess.Write);
            StreamWriter fileOut_rental = new StreamWriter(fs_rental);

            string heading_rental = "Vehicle,Customer";
            fileOut_rental.WriteLine(heading_rental);

            if (GetFleet(true) != null)
            {
                foreach (Vehicle vehicle in GetFleet(true))
                {
                    string body_rental = vehicle.VehicleRego + ',' + RentedBy(vehicle.VehicleRego);
                    fileOut_rental.WriteLine(body_rental);
                }
            }
            fileOut_rental.Close();
            fs_rental.Close();
        }


        // Loads the data from csv files at given locations. Separates the data by ",". 
        // Reads in each line as a vehicle and assigns each attribute. 
        public void LoadFromFile()
        {
            string csvVehicle;
            StreamReader inFile;

            inFile = new StreamReader(fleetFile);
            while ((csvVehicle = inFile.ReadLine()) != null)
            {
                string[] listVehicle = csvVehicle.Split(',');

                if (listVehicle[0] != "Rego") // Skips the Headers
                {
                    vehicleRego = listVehicle[0];
                    make = listVehicle[1];
                    model = listVehicle[2];
                    year = int.Parse(listVehicle[3]);
                    Vehicle.VehicleClass vehicleClass = (Vehicle.VehicleClass)Enum.Parse(typeof(Vehicle.VehicleClass), listVehicle[4]);
                    numSeats = int.Parse(listVehicle[5]);
                    Vehicle.TransmissionType transmission = (Vehicle.TransmissionType)Enum.Parse(typeof(Vehicle.TransmissionType), listVehicle[6]);
                    Vehicle.FuelType fuel = (Vehicle.FuelType)Enum.Parse(typeof(Vehicle.FuelType), listVehicle[7]);
                    if (listVehicle[8] == "False")
                    {
                        GPS = false;
                    }
                    else
                    {
                        GPS = true;
                    }
                    if (listVehicle[9] == "False")
                    {
                        sunRoof = false;
                    }
                    else
                    {
                        sunRoof = true;
                    }
                    colour = listVehicle[10];
                    dailyRate = double.Parse(listVehicle[11]);

                    Vehicle newVehicle = new Vehicle(vehicleRego, vehicleClass, make, model, year, numSeats, transmission, fuel, GPS, sunRoof, dailyRate, colour);
                    vehicles.Add(newVehicle);
                }
            }
            inFile.Close();

            // loads the rental data into the dictionary
            string csvRental;
            StreamReader inFileRental;

            inFileRental = new StreamReader(rentalFile);
            while ((csvRental = inFileRental.ReadLine()) != null)
            {
                string[] listRental = csvRental.Split(',');

                if (listRental[0] != "Vehicle") // Skips the Headers
                {
                    rego = listRental[0];
                    customerID = int.Parse(listRental[1]);

                    // makes sure no vehicle is added twice
                    try
                    {
                        rentals.Add(rego, customerID);
                    }

                    catch (ArgumentException)
                    {
                        Console.WriteLine("Vehicle Already exists");
                    }
                }

            }
            Console.WriteLine(rentals);

            inFileRental.Close();

        }


        //---------------------------------------------------------------------------------------//
        //                                     SEARCH METHODS                                    //
        //---------------------------------------------------------------------------------------//


        //This returns all available cars in the fleet
        public List<Vehicle> Query()
        {
            return GetFleet(false);
        }


        //This is a method to search for vehicles within a certain price range
        public List<Vehicle> Query(double min, double max)
        {

            //get the vechicles available for rental
            List<Vehicle> availableVehicles = new List<Vehicle>();
            availableVehicles = GetFleet(false);

            List<Vehicle> queryPriceResults = new List<Vehicle>();

            // add the available vehicles that have a daily rate that is between the input values
            // to the output list
            for (int i = 0; i < availableVehicles.Count; i++)
            {
                if (availableVehicles[i].DailyRate > min && availableVehicles[i].DailyRate < max)
                {
                    queryPriceResults.Add(availableVehicles[i]);
                }
            }
            // return the available vehicles within the price range
            return queryPriceResults;
        }



        //This method searches for a vehicle within a price range and can accept 
        // one attribute or many OR type attributes
        public List<Vehicle> Query(string query, double min, double max)
        {

            //get the vechicles available for rental within price range
            List<Vehicle> availableVehicles = new List<Vehicle>();
            availableVehicles = Query(min, max);

            // List to store search results
            List<Vehicle> queryResult = new List<Vehicle>();

            // split attributes by OR so each can be searched individually
            string[] queryOR = query.Split(new string[] { " OR " }, StringSplitOptions.None);


            // search the attributes for each available vehicle. If there is a match
            // to the query input, then add it to the list of results
            foreach (Vehicle vehicle in availableVehicles)
            {
                foreach (string option in queryOR)
                {
                    List<string> vehicleAttributes = vehicle.GetAttributeList();
                    foreach (string attribute in vehicleAttributes)
                    {
                        if (attribute.ToLower() == option.ToLower())
                        {

                            queryResult.Add(vehicle);
                        }
                    }
                }
            }

            // Removes double-ups in the results list
            List<Vehicle> queryResultDistinct = queryResult.Distinct().ToList();

            return queryResultDistinct;

        }



        // Searches for vehicles with attributes of the given input. Can accpet OR statements.
        // This is if no price range is specified
        public List<Vehicle> Query(string query)
        {
            //get the vechicles available for rental
            List<Vehicle> availableVehicles = new List<Vehicle>();
            availableVehicles = GetFleet(false);


            // List to store search results
            List<Vehicle> queryResult = new List<Vehicle>();

            // split attributes by OR so each can be searched individually
            string[] queryOR = query.Split(new string[] { " OR " }, StringSplitOptions.None);

            // search the attributes for each available vehicle. If there is a match
            // to the query input, then add it to the list of results
            foreach (Vehicle vehicle in availableVehicles)
            {
                foreach (string option in queryOR)
                {
                    List<string> vehicleAttributes = vehicle.GetAttributeList();
                    foreach (string attribute in vehicleAttributes)
                    {
                        if (attribute.ToLower() == option.ToLower())
                        {

                            queryResult.Add(vehicle);
                        }
                    }
                }
            }

            // Removes double-ups in the results list
            List<Vehicle> queryResultDistinct = queryResult.Distinct().ToList();

            return queryResultDistinct;
        }
    }
}