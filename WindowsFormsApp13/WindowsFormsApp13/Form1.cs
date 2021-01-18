using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MRRCManagement;

namespace WindowsFormsApp13
{
    public partial class Form1 : Form
    {
        // This class manages the construction of the GUI and user interaction. The GUI allows users to modify 
        // customers, vehicles, rental data and make search queries.
        //
        // Authors: Oliver Pye (n9703977) and Taylor Gregory (n9719768)
        // Queensland University of Technology
        // CAB201 Major Project
        // May 2018

        /// ------------------------------------------------------------------------------------------------------------- ///
        ///                                                     Setup                                                     ///
        /// ------------------------------------------------------------------------------------------------------------- ///

        // Constuctors in for the GUI's operation
        CRM crm = new CRM();
        Fleet fleet = new Fleet();
        private Customer selectedCustomer = null;
        private Vehicle selectedVehicle = null;

        // The headings for the tables in the GUI
        private string[] vehicleColumns = new string[] { "Rego", "Make", "Model", "Year", "Vehicle Class", "Number of Seats", "Transmission", "Fuel", "GPS", "Sun Roof", "Daily Rate", "Colour"};
        private string[] customerColumns = new string[] { "ID", "Title", "First name", "Last name", "Gender", "DOB" };
        private string[] rentalColumns = new string[] { "Vehicle Rego", "Customer ID", "Daily Rate" };
        
        // To be used with selecting rows in the DataGridViews
        private const int ID_COL = 0;

        // Constructing the form
        public Form1() 
        {
            InitializeComponent();
            SetupComponents();
            InitialValues();
            this.FormClosing += Form1_FormClosing;
        }

        // Warning for the user before exiting the form and saving data to file
        private void Form1_FormClosing(object sender, CancelEventArgs e) // Prompting 
        {
            MessageBox.Show("You are about to exit. Customer, vehicle and customer data will be saved to file.");
            fleet.SaveToFile();
            crm.SaveToFile();
        }

        // Setting up the major components of the form (i.e. column headings of the DataGridViews and the data from the csv files)
        private void SetupComponents()
        {
            SetupVehicleGridViewColumns();
            SetupCustomerGridViewColumns();
            SetupRentalColumns();

            PopulateDataGridViewVehicles();
            PopulateDataGridViewCustomers();
            PopulateDataGridViewRentals();
        }

        // Hiding all warnings related to mandatory vehicle fields
        private void HideVehicleWarnings()  
        {
            mod_rego_exclaim.Hide(); 
            mod_make_exclaim.Hide();
            mod_model_exclaim.Hide();
            mod_class_exclaim.Hide();
            mod_year_exclaim.Hide();
        }

        // Clear all vehicle fields
        private void ClearVehicleFields() 
        {
            mod_rego_textbox.Text = "";
            mod_make_textbox.Text = "";
            mod_model_textbox.Text = "";
            mod_class_combobox.SelectedItem = null;
            mod_year_textbox.Text = "";
            mod_seats_updown.Value = 0;
            mod_trans_combobox.SelectedItem = null;
            mod_fuel_combobox.SelectedItem = null;
            mod_gps_combobox.SelectedItem = null;
            mod_sunroof_combobox.SelectedItem = null;
            mod_dailyrate_updown.Value = 0;
            mod_colour_textbox.Text = "";
        }

        // Hide all warnings related to customer input
        private void HideCustomerWarnings() 
        {
            mod_cust_id_exclaim.Hide();
            mod_cust_title_exclaim.Hide();
            mod_cust_fn_exclaim.Hide();
            mod_cust_ln_exclaim.Hide();
            mod_cust_gender_exclaim.Hide();
            mod_cust_dob_exclaim.Hide();
        }

        // Clearing all customer fields
        private void ClearCustomerFields() 
        {
            mod_title_textbox.Text = ""; 
            mod_fn_textbox.Text = "";
            mod_ln_textbox.Text = "";
            mod_dob_textbox.Text = "";
            gender_select.SelectedItem = null;
            mod_add_cust_groupbox.Text = "";
        }

        // Initial values and conditions upon startup
        private void InitialValues() 
        {
            // --- Initial values that are relevant to the fleet tab ---//
            mod_vehicle_groupbox.Enabled = false; // Groupbox is not enabled until either 'modify' or 'add' is selected
            ClearVehicleFields(); // Vehicle fields are blank while groupbox is not enabled

            for (int i = 0; i < Enum.GetNames(typeof(Vehicle.VehicleClass)).Length; i++) // Adding vehicle classes to combobox
            {
                mod_class_combobox.Items.Add(Enum.GetNames(typeof(Vehicle.VehicleClass))[i]);
            }

            for (int i = 0; i < Enum.GetNames(typeof(Vehicle.TransmissionType)).Length; i++) // Adding transmission types to combobox
            {
                mod_trans_combobox.Items.Add(Enum.GetNames(typeof(Vehicle.TransmissionType))[i]);
            }

            for (int i = 0; i < Enum.GetNames(typeof(Vehicle.FuelType)).Length; i++) // Adding fuel types to combobox
            {
                mod_fuel_combobox.Items.Add(Enum.GetNames(typeof(Vehicle.FuelType))[i]);
            }

            mod_sunroof_combobox.Items.Add("False"); // Adding bool values to sunroof combobox
            mod_sunroof_combobox.Items.Add("True");

            mod_gps_combobox.Items.Add("False"); // Adding bool values to gps combobox
            mod_gps_combobox.Items.Add("True");

            HideVehicleWarnings();

            // --- Initial values that are relevant to the customers tab --- //
            mod_add_cust_groupbox.Enabled = false; // Hiding all warnings 
            HideCustomerWarnings();
            ClearCustomerFields();

            for (int i = 0; i < Enum.GetNames(typeof(Customer.Gender)).Length; i++) // Adding fuel types to combobox
            {
                gender_select.Items.Add(Enum.GetNames(typeof(Customer.Gender))[i]);
            }


            // --- Initial values that are relevant to the rental search tab --- //
            foreach (Customer cust in crm.GetCustomers())
            {
                customer_combobox.Items.Add(cust.CustomerID.ToString() + " - " + cust.FirstNames + " " + cust.LastName);
            }

            cust_renting_exclaim.Show();
            days_renting_exclaim.Show();
            search_button.Enabled = false;
            results_groupbox.Enabled = false;

        }

        /// ------------------------------------------------------------------------------------------------------------- ///
        ///                                                   Fleet tab                                                   ///
        /// ------------------------------------------------------------------------------------------------------------- ///

        // Setting up the table headings based on vehicleColumns
        private void SetupVehicleGridViewColumns() 
        {
            dataGridViewVehicles.ColumnCount = vehicleColumns.Length;
            for (int i = 0; i < vehicleColumns.Length; i++)
            {
                dataGridViewVehicles.Columns[i].Name = vehicleColumns[i];
            }
            dataGridViewVehicles.AllowUserToAddRows = false;
        }

        // Setting up the table data based on the Fleet
        private void PopulateDataGridViewVehicles()
        {
            dataGridViewVehicles.Rows.Clear();

            for (int i = 0; i < fleet.GetFleet().Count; i++)
            {
                dataGridViewVehicles.Rows.Add(fleet.GetFleet()[i].VehicleRego, fleet.GetFleet()[i].Make, fleet.GetFleet()[i].Model, fleet.GetFleet()[i].Year, 
                    fleet.GetFleet()[i].VehicleClass_Property, fleet.GetFleet()[i].NumSeats, fleet.GetFleet()[i].Transmission_Property, fleet.GetFleet()[i].Fuel_Property, 
                    fleet.GetFleet()[i].GPS_Property, fleet.GetFleet()[i].SunRoof, fleet.GetFleet()[i].DailyRate, fleet.GetFleet()[i].Colour);
            }
        }

        // Enter 'modify vehicle mode'
        private void modify_vehicle_button_Click(object sender, EventArgs e) 
        { 
            mod_vehicle_groupbox.Enabled = true;
            mod_vehicle_groupbox.Text = "Modify Vehicle";
            mod_rego_textbox.Enabled = false;

            if (mod_vehicle_groupbox.Text == "Modify Vehicle")
            {
                int rowsCount = dataGridViewVehicles.SelectedRows.Count;
                if (rowsCount == 0 || rowsCount > 1)
                {
                    selectedVehicle = null;
                    remove_vehicle_button.Enabled = false;
                }
                else
                {
                    string selectedRego = dataGridViewVehicles.SelectedRows[0].Cells[ID_COL].Value.ToString(); // Input all vehicle information into textboxes etc.
    
                    selectedVehicle = fleet.GetVehicle(selectedRego);
                    remove_vehicle_button.Enabled = true;
                    mod_rego_textbox.Text = fleet.GetVehicle(selectedRego).VehicleRego.ToString();
                    mod_make_textbox.Text = fleet.GetVehicle(selectedRego).Make.ToString();
                    mod_model_textbox.Text = fleet.GetVehicle(selectedRego).Model.ToString();
                    mod_year_textbox.Text = fleet.GetVehicle(selectedRego).Year.ToString();
                    mod_colour_textbox.Text = fleet.GetVehicle(selectedRego).Colour.ToString();
                    mod_class_combobox.SelectedIndex = (int)fleet.GetVehicle(selectedRego).VehicleClass_Property;
                    mod_trans_combobox.SelectedIndex = (int)fleet.GetVehicle(selectedRego).Transmission_Property;
                    mod_fuel_combobox.SelectedIndex = (int)fleet.GetVehicle(selectedRego).Fuel_Property;
                    mod_seats_updown.Value = fleet.GetVehicle(selectedRego).NumSeats;
                    mod_dailyrate_updown.Value = (int)fleet.GetVehicle(selectedRego).DailyRate;
                    mod_gps_combobox.SelectedIndex = fleet.GetVehicle(selectedRego).GPS_Property ? 1 : 0;
                    mod_sunroof_combobox.SelectedIndex = fleet.GetVehicle(selectedRego).SunRoof ? 1 : 0 ;

                    if (mod_class_combobox.SelectedItem.ToString() == "Economy") // Economy vehicles can only have automatic transmission
                    {
                        mod_trans_combobox.SelectedIndex = 0;
                        mod_trans_combobox.Enabled = false;
                    }
                    else
                    {
                        mod_trans_combobox.SelectedItem = null;
                        mod_trans_combobox.Enabled = true;
                    }
                }
            }
            mod_rego_exclaim.Hide();
        }

        // Removing a vehicle
        private void remove_vehicle_button_Click(object sender, EventArgs e) 
        {
            if (fleet.IsRented(fleet.GetVehicle(dataGridViewVehicles.CurrentRow.Cells[0].Value.ToString()).VehicleRego)) // Checking if vehicle is already being rented
            {
                MessageBox.Show("Cannot remove vehicle as it is currently being renting"); 
            }
            else
            {
                if (MessageBox.Show("Are you sure you would like to remove this vehicle?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    fleet.RemoveVehicle(fleet.GetVehicle(dataGridViewVehicles.CurrentRow.Cells[0].Value.ToString()));
                    PopulateDataGridViewVehicles();
                }
            }
        }

        // Change values in textboxes based on selected vehicle (if in 'modify vehicle mode')
        private void dataGridViewVehicles_SelectionChanged(object sender, EventArgs e) 
        {
            if (mod_vehicle_groupbox.Text == "Modify Vehicle")
            {
                int rowsCount = dataGridViewVehicles.SelectedRows.Count;
                if (rowsCount == 0 || rowsCount > 1)
                {
                    selectedVehicle = null;
                    remove_vehicle_button.Enabled = false;
                }
                else
                {
                    string selectedRego = dataGridViewVehicles.SelectedRows[0].Cells[ID_COL].Value.ToString(); // Input all vehicle information into textboxes etc.
                    selectedVehicle = fleet.GetVehicle(selectedRego);
                    remove_vehicle_button.Enabled = true;
                    mod_rego_textbox.Text = fleet.GetVehicle(selectedRego).VehicleRego.ToString();
                    mod_make_textbox.Text = fleet.GetVehicle(selectedRego).Make.ToString();
                    mod_model_textbox.Text = fleet.GetVehicle(selectedRego).Model.ToString();
                    mod_year_textbox.Text = fleet.GetVehicle(selectedRego).Year.ToString();
                    mod_colour_textbox.Text = fleet.GetVehicle(selectedRego).Colour.ToString();
                    mod_class_combobox.SelectedIndex = (int)fleet.GetVehicle(selectedRego).VehicleClass_Property;
                    mod_trans_combobox.SelectedIndex = (int)fleet.GetVehicle(selectedRego).Transmission_Property;
                    mod_fuel_combobox.SelectedIndex = (int)fleet.GetVehicle(selectedRego).Fuel_Property;
                    mod_seats_updown.Value = fleet.GetVehicle(selectedRego).NumSeats;
                    mod_dailyrate_updown.Value = (int)fleet.GetVehicle(selectedRego).DailyRate;
                    mod_gps_combobox.SelectedIndex = fleet.GetVehicle(selectedRego).GPS_Property ? 1 : 0;
                    mod_sunroof_combobox.SelectedIndex = fleet.GetVehicle(selectedRego).SunRoof ? 1 : 0;

                    if (mod_class_combobox.SelectedItem.ToString() == "Economy") // Economy vehicles can only have automatic transmission
                    {
                        mod_trans_combobox.SelectedIndex = 0;
                        mod_trans_combobox.Enabled = false;
                    }
                    else
                    {
                        mod_trans_combobox.SelectedItem = null;
                        mod_trans_combobox.Enabled = true;
                    }
                }
                mod_rego_exclaim.Hide();
            }
        }
        // Enter 'add vehicle mode'
        private void add_vehicle_button_Click(object sender, EventArgs e)
        {
            mod_vehicle_groupbox.Enabled = true;
            mod_vehicle_groupbox.Text = "Add Vehicle";
            mod_rego_textbox.Enabled = true;
            ClearVehicleFields();

            mod_rego_exclaim.Show(); // Showing all warnings as all fields are blank
            mod_make_exclaim.Show();
            mod_model_exclaim.Show();
            mod_class_exclaim.Show();
            mod_year_exclaim.Show();
        }

        // Warning if rego textbox is not valid
        private void mod_rego_textbox_TextChanged(object sender, EventArgs e) 
        {
            string input = mod_rego_textbox.Text;
            string r = "^[0-9]{3}[A-Z]{3}$";

            if (Regex.IsMatch(input, r) && fleet.RegoDoesNotExist(input) && mod_rego_textbox.Enabled == true)
            {
                mod_rego_exclaim.Hide();
            }
            else
            {
                mod_rego_exclaim.Show();
            }
        }

        // Warning if make textbox is not valid
        private void mod_make_textbox_TextChanged(object sender, EventArgs e) 
        {
            if (mod_make_textbox.Text != "")
            {
                mod_make_exclaim.Hide();
            }
            else
            {
                mod_make_exclaim.Show();
            }
        }

        // Warning if model textbox is not valid
        private void mod_model_textbox_TextChanged(object sender, EventArgs e) 
        {
            if (mod_model_textbox.Text != "")
            {
                mod_model_exclaim.Hide();
            }
            else
            {
                mod_model_exclaim.Show();
            }
        }

        // Warning if class combobox is not valid
        private void mod_class_combobox_TextChanged(object sender, EventArgs e)
        {

            if (mod_class_combobox.SelectedItem != null)
            {
                mod_class_exclaim.Hide();
                if (mod_class_combobox.SelectedItem.ToString() == "Economy") // Economy vehicles can only have automatic transmission
                {
                    mod_trans_combobox.SelectedIndex = 0;
                    mod_trans_combobox.Enabled = false;
                }
                else
                {
                    mod_trans_combobox.SelectedItem = null;
                    mod_trans_combobox.Enabled = true;
                }
            }
            else
            {
                mod_class_exclaim.Show();
            }
        }

        // Warning if year textbox is not valid
        private void mod_year_textbox_TextChanged(object sender, EventArgs e) 
        {
            string input = mod_year_textbox.Text;
            string r = "^[0-9]{4}$";
            if (Regex.IsMatch(input, r) && mod_year_textbox.Enabled == true)
            {
                int ignore;
                bool successfully_parsed = int.TryParse(mod_year_textbox.Text, out ignore);
                if (successfully_parsed)
                {
                    if (int.Parse(mod_year_textbox.Text) >= 1800 && int.Parse(mod_year_textbox.Text) <= 2018)
                    {
                        mod_year_exclaim.Hide();
                    }
                }
            }
            else
            {
                mod_year_exclaim.Show();
            }
        }

        // Press submit button
        private void vehicle_submit_button_Click(object sender, EventArgs e)
        {
            string error_string = "Error:"; // Displaying error messages when data entry is poor
            bool errors = false;

            if (mod_vehicle_groupbox.Text == "Add Vehicle") // Error messages that are only relevant if the user is adding a vehicle
            {
                if (fleet.RegoDoesNotExist(mod_rego_textbox.Text) == false)
                {
                    error_string += "\nVehicle is already in the fleet.";
                    errors = true;
                }
                if (Regex.IsMatch(mod_rego_textbox.Text, "^[0-9]{3}[A-Z]{3}$") == false)
                {
                    error_string += "\nRego is invalid. Please input a rego with 3 numbers (0-9) followed by 3 upper case letters (A-Z).";
                    errors = true;
                }
            }
            if (mod_make_textbox.Text == "") // Empty make field
            {
                error_string += "\nMake is empty. Please input a valid string.";
                errors = true;
            }
            if (mod_model_textbox.Text == "") // Empty model field
            {
                error_string += "\nModel is empty. Please input a valid string.";
                errors = true;
            }
            if (mod_class_combobox.SelectedItem == null) // Empty vehicle class
            {
                error_string += "\nVehicle class has not been selected. Please make a selection.";
                errors = true;
            }
            if (Regex.IsMatch(mod_year_textbox.Text, "^[0-9]+$") == false) // Invalid or empty year
            {
                error_string += "\nYear is invalid. Please input a valid integer.";
                errors = true;
            }
            int ignore;
            bool successfully_parsed = int.TryParse(mod_year_textbox.Text, out ignore); // Displaying error messages if data entry is poor
            if (successfully_parsed)
            {
                if (int.Parse(mod_year_textbox.Text) < 1800 || int.Parse(mod_year_textbox.Text) > 2018) // Checking that the year is reasonable
                {
                    error_string += "\nYear is outside of valid year range. Please enter a value from 1800 - 2018.";
                    errors = true;
                }
            }
            if (errors == true)
            {
                MessageBox.Show(error_string); // Display error message that has been created above
            }
            if (errors == false) // If all data entered is valid
            {
                if (mod_vehicle_groupbox.Text == "Modify Vehicle") // If in 'modify vehicle mode'
                {
                    fleet.GetVehicle(mod_rego_textbox.Text).Make = mod_make_textbox.Text;
                    fleet.GetVehicle(mod_rego_textbox.Text).Model = mod_model_textbox.Text;
                    fleet.GetVehicle(mod_rego_textbox.Text).VehicleClass_Property = (Vehicle.VehicleClass)mod_class_combobox.SelectedIndex;
                    fleet.GetVehicle(mod_rego_textbox.Text).Year = int.Parse(mod_year_textbox.Text);
                    fleet.GetVehicle(mod_rego_textbox.Text).NumSeats = (int)mod_seats_updown.Value;
                    fleet.GetVehicle(mod_rego_textbox.Text).Transmission_Property = (Vehicle.TransmissionType)mod_trans_combobox.SelectedIndex;
                    fleet.GetVehicle(mod_rego_textbox.Text).Fuel_Property = (Vehicle.FuelType)mod_fuel_combobox.SelectedIndex;
                    fleet.GetVehicle(mod_rego_textbox.Text).GPS_Property = mod_gps_combobox.SelectedIndex != 0;
                    fleet.GetVehicle(mod_rego_textbox.Text).SunRoof = mod_sunroof_combobox.SelectedIndex != 0;
                    fleet.GetVehicle(mod_rego_textbox.Text).DailyRate = (int)mod_dailyrate_updown.Value;
                    fleet.GetVehicle(mod_rego_textbox.Text).Colour = mod_colour_textbox.Text;

                    PopulateDataGridViewVehicles();
                }
                if (mod_vehicle_groupbox.Text == "Add Vehicle") // If in 'modify vehicle mode'
                {
                    Vehicle newVehicle = new Vehicle(mod_rego_textbox.Text, (Vehicle.VehicleClass)mod_class_combobox.SelectedIndex, mod_make_textbox.Text, mod_model_textbox.Text, int.Parse(mod_year_textbox.Text));
                    fleet.AddVehicle(newVehicle); // Add new vehicle with default values based on class
                    if (mod_trans_combobox.SelectedItem != null) // Replacing any additional information if the vehicle is customised
                    {
                        fleet.GetVehicle(mod_rego_textbox.Text).Transmission_Property = (Vehicle.TransmissionType)mod_trans_combobox.SelectedIndex;
                    }
                    if (mod_fuel_combobox.SelectedItem != null)
                    {
                        fleet.GetVehicle(mod_rego_textbox.Text).Fuel_Property = (Vehicle.FuelType)mod_fuel_combobox.SelectedIndex;
                    }
                    if (mod_seats_updown.Value != 0)
                    {
                        fleet.GetVehicle(mod_rego_textbox.Text).NumSeats = (int)mod_seats_updown.Value;
                    }
                    if (mod_sunroof_combobox.SelectedItem != null)
                    {
                        fleet.GetVehicle(mod_rego_textbox.Text).SunRoof = mod_sunroof_combobox.SelectedIndex != 0;
                    }
                    if (mod_gps_combobox.SelectedItem != null)
                    {
                        fleet.GetVehicle(mod_rego_textbox.Text).GPS_Property = mod_gps_combobox.SelectedIndex != 0;
                    }
                    if (mod_colour_textbox.Text != "")
                    {
                        fleet.GetVehicle(mod_rego_textbox.Text).Colour = mod_colour_textbox.Text;
                    }
                    if (mod_dailyrate_updown.Value != 0)
                    {
                        fleet.GetVehicle(mod_rego_textbox.Text).DailyRate = (int)mod_dailyrate_updown.Value;
                    }

                    PopulateDataGridViewVehicles();

                    ClearVehicleFields();

                    mod_vehicle_groupbox.Text = ""; // Make sure that the groupbox is not enabled
                    mod_vehicle_groupbox.Enabled = false;

                    HideVehicleWarnings();
                }
            }
        }

        // Pressing the cancel button with make fields empty
        private void vehicle_cancel_button_Click(object sender, EventArgs e)
        {
            ClearVehicleFields();
            mod_vehicle_groupbox.Text = ""; // Make sure that the groupbox is not enabled
            mod_vehicle_groupbox.Enabled = false;
            HideVehicleWarnings(); 
        }

        /// ------------------------------------------------------------------------------------------------------------- ///
        ///                                                  Customer tab                                                 ///
        /// ------------------------------------------------------------------------------------------------------------- ///
        // Setting up the table headings based on customerColumns
        private void SetupCustomerGridViewColumns() 
        {
            dataGridViewCustomers.ColumnCount = customerColumns.Length;
            for (int i = 0; i < customerColumns.Length; i++)
            {
                dataGridViewCustomers.Columns[i].Name = customerColumns[i];
            }
            dataGridViewCustomers.AllowUserToAddRows = false; 
        }

        // Setting up the table data based on the CRM
        private void PopulateDataGridViewCustomers() 
        {
            dataGridViewCustomers.Rows.Clear();

            for (int i = 0; i < crm.GetCustomers().Count; i++)
            {
                dataGridViewCustomers.Rows.Add(crm.GetCustomers()[i].CustomerID, crm.GetCustomers()[i].Title, crm.GetCustomers()[i].FirstNames, crm.GetCustomers()[i].LastName,
                    crm.GetCustomers()[i].Gender_Property, crm.GetCustomers()[i].DateOfBirth);
            }
        }

        // Removing a customer
        private void remove_cust_button_Click(object sender, EventArgs e) 
        {
            if (crm.GetCustomer(dataGridViewCustomers.CurrentRow.Index) != null) // Check that there is a customer at the current row
            {
                if (fleet.IsRenting(crm.GetCustomer(dataGridViewCustomers.CurrentRow.Index).CustomerID) == true) // Check if the customer is already renting
                {
                    MessageBox.Show("Cannot remove customer as they are currently renting");
                }
                else
                {
                    if (MessageBox.Show("Are you sure you would like to remove this customer?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        crm.RemoveCustomer(crm.GetCustomer(dataGridViewCustomers.CurrentRow.Index), fleet); // Removing customer if they are not already renting
                        PopulateDataGridViewCustomers();

                        customer_combobox.Items.Clear();
                        for (int i = 0; i < fleet.GetFleet(true).Count; i++)
                        {
                            customer_combobox.Items.Add(
                                fleet.RentedBy(fleet.GetFleet(true)[i].VehicleRego) + " - " +
                                crm.GetCustomer(fleet.RentedBy(fleet.GetFleet(true)[i].VehicleRego)).FirstNames + " " +
                                crm.GetCustomer(fleet.RentedBy(fleet.GetFleet(true)[i].VehicleRego)).LastName);
                        }
                    }
                }
            }

            customer_combobox.Items.Clear(); // Replacing the values in the search combobox based on customer list change
            foreach (Customer cust in crm.GetCustomers())
            {
                customer_combobox.Items.Add(cust.CustomerID.ToString() + " - " + cust.FirstNames + " " + cust.LastName); 
            }
        }

        // Enter 'modify customer mode'
        private void modify_cust_button_Click(object sender, EventArgs e) 
        {
            mod_add_cust_groupbox.Enabled = true;
            mod_add_cust_groupbox.Text = "Modify Customer";
            mod_custid_textbox.Enabled = false;

            if (mod_add_cust_groupbox.Text == "Modify Customer")
            {
                int rowsCount = dataGridViewCustomers.SelectedRows.Count;
                if (rowsCount == 0 || rowsCount > 1)
                {
                    selectedCustomer = null;
                    remove_cust_button.Enabled = false;
                }
                else
                {
                    int selectedID = (Int32)(dataGridViewCustomers.SelectedRows[0].Cells[ID_COL].Value);
                    selectedCustomer = crm.GetCustomer(selectedID);
                    remove_cust_button.Enabled = true;
                    mod_custid_textbox.Text = crm.GetCustomer(selectedID).CustomerID.ToString(); // Fill text boxes etc. based on selected customer values
                    mod_title_textbox.Text = crm.GetCustomer(selectedID).Title.ToString();
                    mod_fn_textbox.Text = crm.GetCustomer(selectedID).FirstNames.ToString();
                    mod_ln_textbox.Text = crm.GetCustomer(selectedID).LastName.ToString();
                    gender_select.SelectedIndex = (int)crm.GetCustomer(selectedID).Gender_Property;
                    mod_dob_textbox.Text = crm.GetCustomer(selectedID).DateOfBirth.ToString();

                    customer_combobox.Items.Clear(); // Replacing the values in the search combobox based on customer list change
                    foreach (Customer cust in crm.GetCustomers())
                    {
                        customer_combobox.Items.Add(cust.CustomerID.ToString() + " - " + cust.FirstNames + " " + cust.LastName); 
                    }
                }
            }
            mod_cust_id_exclaim.Hide();
        }

        // Change values in textboxes based on selected customer (if in 'modify customer mode')
        private void dataGridViewCustomers_SelectionChanged(object sender, EventArgs e) 
        {
            if (mod_add_cust_groupbox.Text == "Modify Customer")
            {
                int rowsCount = dataGridViewCustomers.SelectedRows.Count;
                if (rowsCount == 0 || rowsCount > 1)
                {
                    selectedCustomer = null;
                    remove_cust_button.Enabled = false;
                }
                else
                {
                    int selectedID = (Int32)(dataGridViewCustomers.SelectedRows[0].Cells[ID_COL].Value);
                    selectedCustomer = crm.GetCustomer(selectedID);
                    remove_cust_button.Enabled = true;
                    mod_custid_textbox.Text = crm.GetCustomer(selectedID).CustomerID.ToString(); // Fill the text boxes etc. based on the selected customer
                    mod_title_textbox.Text = crm.GetCustomer(selectedID).Title.ToString();
                    mod_fn_textbox.Text = crm.GetCustomer(selectedID).FirstNames.ToString();
                    mod_ln_textbox.Text = crm.GetCustomer(selectedID).LastName.ToString();
                    gender_select.SelectedIndex = (int)crm.GetCustomer(selectedID).Gender_Property;
                    mod_dob_textbox.Text = crm.GetCustomer(selectedID).DateOfBirth.ToString();


                    customer_combobox.Items.Clear(); // Replacing the values in the search combobox based on customer list change
                    foreach (Customer cust in crm.GetCustomers())
                    {
                        customer_combobox.Items.Add(cust.CustomerID.ToString() + " - " + cust.FirstNames + " " + cust.LastName);
                    }
                }
            }
            mod_cust_id_exclaim.Hide();
        }

        // Enter 'add customer mode'
        private void add_cust_button_Click(object sender, EventArgs e) 
        {
            mod_add_cust_groupbox.Enabled = true; // Empty text boxes
            mod_custid_textbox.Enabled = true;
            ClearCustomerFields();

            mod_cust_id_exclaim.Show(); // Show all errors
            mod_cust_title_exclaim.Show();
            mod_cust_fn_exclaim.Show();
            mod_cust_ln_exclaim.Show();
            mod_cust_gender_exclaim.Show();
            mod_cust_dob_exclaim.Show();

            mod_add_cust_groupbox.Text = "Add Customer";
        }

        // Warning if custID textbox is not valid or repeated
        private void mod_custid_textbox_TextChanged(object sender, EventArgs e) 
        {
            string input = mod_custid_textbox.Text;
            string r = "^[0-9]+$";


            if (Regex.IsMatch(input, r) && crm.CustIDDoesNotExist(int.Parse(input)) && mod_custid_textbox.Enabled == true) // Warning if customer is repeated or invalid
            {
                mod_cust_id_exclaim.Hide();
            }
            else 
            {
                mod_cust_id_exclaim.Show();
            }
        }

        // Warning if title textbox is blank
        private void mod_title_textbox_TextChanged(object sender, EventArgs e) 
        {
            if (mod_title_textbox.Text != "")
            {
                mod_cust_title_exclaim.Hide();
            }
            else
            {
                mod_cust_title_exclaim.Show();
            }
        }

        // Warning if first name textbox is blank
        private void mod_fn_textbox_TextChanged(object sender, EventArgs e) 
        {
            if (mod_fn_textbox.Text != "")
            {
                mod_cust_fn_exclaim.Hide();
            }
            else
            {
                mod_cust_fn_exclaim.Show();
            }
        }

        // Warning if last name textbox is blank
        private void mod_ln_textbox_TextChanged(object sender, EventArgs e) 
        {
            if (mod_ln_textbox.Text != "")
            {
                mod_cust_ln_exclaim.Hide();
            }
            else
            {
                mod_cust_ln_exclaim.Show();
            }
        }

        // Warning if no gender is selected
        private void gender_select_SelectedIndexChanged(object sender, EventArgs e) 
        {
            if (gender_select.SelectedItem != null)
            {
                mod_cust_gender_exclaim.Hide();
            }
            else
            {
                mod_cust_gender_exclaim.Show();
            }
        }

        // Warning if date of birth textbox is valid
        private void mod_dob_textbox_TextChanged(object sender, EventArgs e) 
        {
            string input = mod_dob_textbox.Text;
            string r1 = "^[0-2]{0,1}[0-9]{1}/[0]{0,1}[0-9]{1}/[0-9]{4}$"; // Various ways that an acceptable date could be formatted
            string r2 = "^[3]{1}[0-1]{1}/[0]{0,1}[0-9]{1}/[0-9]{4}$";
            string r3 = "^[0-2]{0,1}[0-9]{1}/[1]{1}[0-2]{1}/[0-9]{4}$";
            string r4 = "^[3]{1}[0-1]{1}/[1]{1}[0-2]{1}/[0-9]{4}$";

            if (Regex.IsMatch(input, r1)|| Regex.IsMatch(input, r2)|| Regex.IsMatch(input, r3)|| Regex.IsMatch(input, r4))
            {
                mod_cust_dob_exclaim.Hide();
            }
            else if (!Regex.IsMatch(input, r1) && !Regex.IsMatch(input, r2) && !Regex.IsMatch(input, r3) && !Regex.IsMatch(input, r4) && mod_dob_textbox.Enabled == true)
            {
                mod_cust_dob_exclaim.Show();
            }
        }

        // Making changes to customers list when submit button is pressed
        private void mod_cust_submit_button_Click(object sender, EventArgs e)
        {
            string error_string = "Error:"; // Displaying error messages if data entry is poor
            bool errors = false;

            if (mod_add_cust_groupbox.Text == "Add Customer") // This is only valid if the user is adding a customer
            {
                int ignore;
                bool successfully_parsed = int.TryParse(mod_custid_textbox.Text, out ignore);
                if (successfully_parsed)
                {
                    if (crm.CustIDDoesNotExist(int.Parse(mod_custid_textbox.Text)) == false)
                    {
                        error_string += "\nCustomer ID is already in the CRM.";
                        errors = true;
                    }
                }
                if (Regex.IsMatch(mod_custid_textbox.Text, "^[0-9]+") == false)
                {
                    error_string += "\nCustomer ID is invalid. Please input a valid integer.";
                    errors = true;
                }
            }
            if (mod_title_textbox.Text == "")
            {
                error_string += "\nTitle is empty. Please input a valid string.";
                errors = true;
            }
            if (mod_fn_textbox.Text == "")
            {
                error_string += "\nFirst name is empty. Please input a valid string.";
                errors = true;
            }
            if (mod_ln_textbox.Text == "")
            {
                error_string += "\nLast name is empty. Please input a valid string.";
                errors = true;
            }
            if (mod_cust_gender_exclaim.Visible)
            {
                error_string += "\nGender is invalid. Please select an option from the drop down list.";
                errors = true;
            }
            if (mod_cust_dob_exclaim.Visible)
            {
                error_string += "\nDate of birth is invalid. Please enter a date in DD/MM/YYYY format.";
                errors = true;
            }

            if (errors == true)
            {
                MessageBox.Show(error_string); // Showing error message that was constructed above
            }

            if (errors == false) // If all data entered is valid, a new customer is constructed
            {
                if (mod_add_cust_groupbox.Text == "Modify Customer") // If in 'modify customer mode'
                {
                    crm.GetCustomer(int.Parse(mod_custid_textbox.Text)).Title = mod_title_textbox.Text;
                    crm.GetCustomer(int.Parse(mod_custid_textbox.Text)).FirstNames = mod_fn_textbox.Text;
                    crm.GetCustomer(int.Parse(mod_custid_textbox.Text)).LastName = mod_ln_textbox.Text;
                    crm.GetCustomer(int.Parse(mod_custid_textbox.Text)).Gender_Property = (Customer.Gender)gender_select.SelectedIndex;
                    crm.GetCustomer(int.Parse(mod_custid_textbox.Text)).DateOfBirth = mod_dob_textbox.Text;
                    PopulateDataGridViewCustomers();


                    customer_combobox.Items.Clear(); // Updating customer combobox based on edited values
                    foreach (Customer cust in crm.GetCustomers())
                    {
                        customer_combobox.Items.Add(cust.CustomerID.ToString() + " - " + cust.FirstNames + " " + cust.LastName);
                    }
                }
                else if (mod_add_cust_groupbox.Text == "Add Customer") // If in 'add customer mode'
                {
                    Customer newCustomer = new Customer(int.Parse(mod_custid_textbox.Text), mod_title_textbox.Text, mod_fn_textbox.Text, mod_ln_textbox.Text, (Customer.Gender)gender_select.SelectedIndex, mod_dob_textbox.Text);
                    crm.AddCustomer(newCustomer);
                    PopulateDataGridViewCustomers();
                    ClearCustomerFields();
                    mod_add_cust_groupbox.Text = ""; // Don't enable groupbox
                    mod_add_cust_groupbox.Enabled = false;
                    HideCustomerWarnings();


                    customer_combobox.Items.Clear(); // Updating customer combobox based on edited values
                    foreach (Customer cust in crm.GetCustomers()) 
                    {
                        customer_combobox.Items.Add(cust.CustomerID.ToString() + " - " + cust.FirstNames + " " + cust.LastName);
                    }
                }
            }
        }

        // Clearing entry boxes when cancel button is pressed
        private void mod_cust_cancel_button_Click(object sender, EventArgs e) 
        { 
            mod_add_cust_groupbox.Text = ""; 
            ClearCustomerFields();
            mod_add_cust_groupbox.Enabled = false;
            HideCustomerWarnings();
        }

        /// ------------------------------------------------------------------------------------------------------------- ///
        ///                                              Rental report tab                                                ///
        /// ------------------------------------------------------------------------------------------------------------- ///
        // Setting up the table headings based on rentalColumns
        private void SetupRentalColumns() 
        {
            dataGridViewRentals.ColumnCount = rentalColumns.Length;
            for (int i = 0; i < rentalColumns.Length; i++)
            {
                dataGridViewRentals.Columns[i].Name = rentalColumns[i];
            }
            dataGridViewRentals.AllowUserToAddRows = false;
        }

        // Setting up the table data based on the Fleet
        private void PopulateDataGridViewRentals() 
        {
            dataGridViewRentals.Rows.Clear();
            double total_charges = 0;

            label10.Text = fleet.GetFleet(true).Count.ToString();

            for (int i = 0; i < fleet.GetFleet(true).Count; i++)
            {
                dataGridViewRentals.Rows.Add(fleet.GetFleet(true)[i].VehicleRego, fleet.RentedBy(fleet.GetFleet(true)[i].VehicleRego), "$" + fleet.GetFleet(true)[i].DailyRate);
                total_charges += fleet.GetFleet(true)[i].DailyRate;
            }
            rental_summary_label.Text = "Total vehicles rented: " + fleet.GetFleet(true).Count + "                  " + "Total daily rental charge: $" + total_charges;
        }

        // Method to return the vehicle
        private void return_vehicle_button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you would like to return this car?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                fleet.ReturnCar(fleet.GetFleet(true)[dataGridViewRentals.CurrentRow.Index].VehicleRego);
                PopulateDataGridViewRentals();
            }
        }


        /// ------------------------------------------------------------------------------------------------------------- ///
        ///                                              Rental search tab                                                ///
        /// ------------------------------------------------------------------------------------------------------------- ///
        // Setting up the table headings based on vehicleColumns
        private void SetupVehicleSearchGridViewColumns() 
        {
            dataGridViewSearch.ColumnCount = vehicleColumns.Length;
            for (int i = 0; i < vehicleColumns.Length; i++)
            {
                dataGridViewSearch.Columns[i].Name = vehicleColumns[i];
            }
            dataGridViewSearch.AllowUserToAddRows = false;
        }

        // Displaying all available vehicles when 'show all' button is pressed
        private void show_all_button_Click(object sender, EventArgs e)
        {
            results_groupbox.Enabled = true;
            dataGridViewSearch.Rows.Clear();

            if (min_price_search.Value == 0 && max_price_search.Value == 0) // If $$ fields are not entered
            {
                if (fleet.Query().Count > 0)
                {
                    SetupVehicleSearchGridViewColumns();
                    for (int i = 0; i < fleet.Query().Count; i++) // Display all vehicles 
                    {
                        dataGridViewSearch.Rows.Add(
                            fleet.Query()[i].VehicleRego,
                            fleet.Query()[i].Make,
                            fleet.Query()[i].Model,
                            fleet.Query()[i].Year,
                            fleet.Query()[i].VehicleClass_Property,
                            fleet.Query()[i].NumSeats,
                            fleet.Query()[i].Transmission_Property,
                            fleet.Query()[i].Fuel_Property,
                            fleet.Query()[i].GPS_Property,
                            fleet.Query()[i].SunRoof,
                            fleet.Query()[i].DailyRate,
                            fleet.Query()[i].Colour);
                    }
                }
                else
                {
                    MessageBox.Show("No matching vehicles found. Please try another search.");
                }

            }
            
            if (max_price_search.Value != 0) // If $$ are entered
            {
                if (fleet.Query((double)min_price_search.Value, (double)max_price_search.Value).Count > 0)
                {
                    SetupVehicleSearchGridViewColumns();
                    for (int i = 0; i < fleet.Query((double)min_price_search.Value, (double)max_price_search.Value).Count; i++) // Display all vehicles within price range
                    {
                        dataGridViewSearch.Rows.Add(
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].VehicleRego,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].Make,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].Model,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].Year,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].VehicleClass_Property,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].NumSeats,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].Transmission_Property,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].Fuel_Property,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].GPS_Property,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].SunRoof,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].DailyRate,
                            fleet.Query((double)min_price_search.Value, (double)max_price_search.Value)[i].Colour);
                    }
                }
                else
                {
                    MessageBox.Show("No matching vehicles found. Please try another search.");
                }     
            }
        }

        // Displaying vehicles after search button is clicked (using string input)
        private void search_button_Click(object sender, EventArgs e)
        {
            SetupVehicleSearchGridViewColumns();
            results_groupbox.Enabled = true;
            dataGridViewSearch.Rows.Clear();

            if (max_price_search.Value == 0) // if $$ is not entered
            {
                if (fleet.Query(search_input_textbox.Text).Count > 0)
                {
                    SetupVehicleSearchGridViewColumns();
                    for (int i = 0; i < fleet.Query(search_input_textbox.Text).Count; i++)
                    {
                        dataGridViewSearch.Rows.Add(
                            fleet.Query(search_input_textbox.Text)[i].VehicleRego,
                            fleet.Query(search_input_textbox.Text)[i].Make,
                            fleet.Query(search_input_textbox.Text)[i].Model,
                            fleet.Query(search_input_textbox.Text)[i].Year,
                            fleet.Query(search_input_textbox.Text)[i].VehicleClass_Property,
                            fleet.Query(search_input_textbox.Text)[i].NumSeats,
                            fleet.Query(search_input_textbox.Text)[i].Transmission_Property,
                            fleet.Query(search_input_textbox.Text)[i].Fuel_Property,
                            fleet.Query(search_input_textbox.Text)[i].GPS_Property,
                            fleet.Query(search_input_textbox.Text)[i].SunRoof,
                            fleet.Query(search_input_textbox.Text)[i].DailyRate,
                            fleet.Query(search_input_textbox.Text)[i].Colour);
                    }
                }
                else
                {
                    MessageBox.Show("No matching vehicles found. Please try another search.");
                }
            }

                if (max_price_search.Value != 0) // if $$ values are used in search
            {
                if (fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value).Count > 0)
                {
                    SetupVehicleSearchGridViewColumns();
                    for (int i = 0; i < fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value).Count; i++)
                    {
                        dataGridViewSearch.Rows.Add(
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].VehicleRego,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].Make,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].Model,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].Year,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].VehicleClass_Property,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].NumSeats,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].Transmission_Property,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].Fuel_Property,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].GPS_Property,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].SunRoof,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].DailyRate,
                            fleet.Query(search_input_textbox.Text, (double)min_price_search.Value, (double)max_price_search.Value)[i].Colour);
                    }
                }
                else
                {
                    dataGridViewSearch.Rows.Clear();
                    MessageBox.Show("No matching vehicles found. Please try another search.");
                }   
            }
        }

        // Enabling search button if there is text in the text field
        private void search_input_textbox_TextChanged(object sender, EventArgs e)
        {
            if (search_input_textbox.Text == "")
            {
                search_button.Enabled = false;
            }
            if (search_input_textbox.Text != "")
            {
                search_button.Enabled = true;
            }
        }

        // Enabling error labels when combobox input is incorrect
        private void customer_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fleet.IsRenting(crm.GetCustomers()[customer_combobox.SelectedIndex].CustomerID) == true)
            {
                cust_renting_exclaim.Show();
            }
            else
            {
                cust_renting_exclaim.Hide();
            }
            if (customer_combobox.SelectedItem == null)
            {
                cust_renting_exclaim.Show();
            }
            else
            {
                cust_renting_exclaim.Hide();
            }
        }

        // Method for pressing the final rent button. This checks for errors in the customer combobox and no. of days NumericUpDown
        private void rent_confirm_button_Click(object sender, EventArgs e)
        {
            string error_string = "Error: "; // Compiling error statement
            bool errors = false;
            if (customer_combobox.SelectedItem != null)
            {
                if (fleet.IsRenting(crm.GetCustomers()[customer_combobox.SelectedIndex].CustomerID) == true)
                {
                    error_string += "\nCustomer is already renting a vehicle. Please make another selection";
                    errors = true;
                }
            }
            if (customer_combobox.SelectedItem == null)
            {
                error_string += "\nPlease select an available customer to rent this vehicle.";
                errors = true;
            }
            if (days_renting_exclaim.Visible)
            {
                error_string += "\nPlease indicate the number of days for this rental.";
                errors = true;
            }
            if (errors)
            {
                MessageBox.Show(error_string);
            }
            else if (errors == false) // If there are no errors
            {
                string selectedRego = dataGridViewSearch.SelectedRows[0].Cells[ID_COL].Value.ToString();
                selectedVehicle = fleet.GetVehicle(selectedRego);

                if (MessageBox.Show("Do you want to rent vehicle " + fleet.GetVehicle(selectedRego).VehicleRego.ToString() + " to customer " + customer_combobox.Text + 
                    " for a total cost of "+ ((int)fleet.GetVehicle(selectedRego).DailyRate * (int)rental_days_select.Value).ToString() + "?", "Rental confirmation", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    fleet.RentCar(fleet.GetVehicle(selectedRego).VehicleRego.ToString(), crm.GetCustomers()[customer_combobox.SelectedIndex].CustomerID);
                    dataGridViewSearch.Rows.Clear();

                    results_groupbox.Enabled = false;
                    dataGridViewRentals.Rows.Clear();
                    PopulateDataGridViewRentals();
                    rental_days_select.Value = 0;
                    days_renting_exclaim.Show();
                    cust_renting_exclaim.Show();
                }
            }
    }

        // Method that handles the warning label for no. of days and the label that tells the user the total rental cost
        private void rental_days_select_ValueChanged(object sender, EventArgs e)
        {
            if ((int)rental_days_select.Value == 0)
            {
                total_cost_label.Hide();
                days_renting_exclaim.Show();
            }

            else if ((int)rental_days_select.Value != 0)
            {
                total_cost_label.Show();
                string selectedRego = dataGridViewSearch.SelectedRows[0].Cells[ID_COL].Value.ToString();
                selectedVehicle = fleet.GetVehicle(selectedRego);
                days_renting_exclaim.Hide();

                total_cost_label.Text = "Total cost: $"  + ((int)fleet.GetVehicle(selectedRego).DailyRate * (int)rental_days_select.Value).ToString();
               
            }
        }
    }
}

