using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Users;
using Tickets;
using DataStructs;
using System.Xml.Linq;

public class Program
{
    static HttpClient client = new HttpClient();

    static Manager manager = new Manager();
    static Employee employee = new Employee();
    public static void Main(string[] args)
    {
        RunAsync().GetAwaiter().GetResult();
    }

    static async Task RunAsync()
    {
        client.BaseAddress = new Uri("https://localhost:7206/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            //Start Running the Client
            Console.WriteLine("--- STARTING PROJECT 1: CLIENT ---");
            await Task.Delay(2000);
            int layer = 0;
            bool running = true;
            while (running)
            {
                //Top Layer (Log In, Create Account, Exit Program)
                if (layer == 0)
                {
                    Console.Clear();
                    Console.WriteLine("--- HOME PAGE ---");
                    Console.WriteLine("[1] Log In\n[2] Create Account\n[3] Exit Program\n");
                    int input = await getUserInputOption(1, 3);
                    if (input == 1)
                    {
                        //Log in user, could be either a manager or an employee
                        Console.Clear();
                        Console.WriteLine("--- SIGNING IN EXISTING USER ---");
                        Console.WriteLine("Please enter your email.");
                        string email = await getUserInputString("Email: ");
                        Console.WriteLine();
                        Console.WriteLine("Please enter your password.");
                        string password = await getUserInputString("Password: ");
                        manager = await LoginManager(email, password);
                        employee = await LoginEmployee(email, password);
                        while (employee.Id == -1 && manager.Id == -1)
                        {
                            Console.WriteLine("Failed to Sign In. Your email and/or password may be incorrect.".ToUpper());
                            Console.WriteLine("Please enter your email.");
                            email = await getUserInputString("Email: ");
                            Console.WriteLine();
                            Console.WriteLine("Please enter your password.");
                            password = await getUserInputStringSecure("Password: ");
                            manager = await LoginManager(email, password);
                            employee = await LoginEmployee(email, password);
                        }

                        //Check if they signed in as a manager
                        bool IsEmployee = true;
                        if (manager.Id > 0)
                        {
                            //They are a manager
                            IsEmployee = false;
                        }
                        
                        //Goto Layer
                        if (IsEmployee)
                        {
                            //Employee Layer
                            layer = 1;
                        }
                        else
                        {
                            //Manager Layer
                            layer = 2;
                        }

                    }
                    else if (input == 2)
                    {
                        //Create a new employee
                        Console.Clear();
                        Console.WriteLine("--- CREATING A NEW USER ---");
                        Console.WriteLine("Please enter an email.");
                        string email = await getUserInputString("Email: ");
                        Console.WriteLine();
                        Console.WriteLine("Please enter a password.");
                        string password = await getUserInputString("Password: ");
                        Console.WriteLine("Please re-enter your password.");
                        string repassword = await getUserInputStringSecure("Password: ");
                        while (password != repassword)
                        {
                            Console.WriteLine("PASSWORDS DID NOT MATCH. REDO PASSWORDS.");
                            Console.WriteLine("Please enter a password.");
                            password = await getUserInputString("Password: ");
                            Console.WriteLine("Please re-enter your password.");
                            repassword = await getUserInputStringSecure("Password: ");
                        }
                        employee = await CreateEmployee(email, password);
                        input = 0;
                        //Check if success
                        if (employee.Id < 1)
                        {
                            //failed
                            Console.WriteLine("FAILED TO CREATE AN ACCOUNT. THE USERNAME IS ALREADY TAKEN.");
                            Console.WriteLine("[1] Retry to create an account\n[2] Return to main menu\n");
                            input = await getUserInputOption(1, 2);
                        }
                        while (input == 1 && employee.Id < 1)
                        {
                            Console.Clear();
                            Console.WriteLine("--- CREATING A NEW USER ---");
                            Console.WriteLine("Please enter an email.");
                            email = await getUserInputString("Email: ");
                            Console.WriteLine();
                            Console.WriteLine("Please enter a password.");
                            password = await getUserInputString("Password: ");
                            Console.WriteLine("Please re-enter your password.");
                            repassword = await getUserInputStringSecure("Password: ");
                            while (password != repassword)
                            {
                                Console.WriteLine("PASSWORDS DID NOT MATCH. REDO PASSWORDS.");
                                Console.WriteLine("Please enter a password.");
                                password = await getUserInputStringSecure("Password: ");
                                Console.WriteLine("Please re-enter your password.");
                                repassword = await getUserInputStringSecure("Password: ");
                            }
                            employee = await CreateEmployee(email, password);
                            input = 0;
                            //Check if success
                            if (employee.Id < 1 )
                            {
                                //failed
                                Console.WriteLine("FAILED TO CREATE AN ACCOUNT. THE USERNAME IS ALREADY TAKEN.");
                                Console.WriteLine("[1] Retry to create an account\n[2] Return to main menu\n");
                                input = await getUserInputOption(1, 2);
                            }
                        }
                        if (employee.Id >= 1)
                        {
                            Console.WriteLine("ACCOUNT WAS CREATED SUCCESSFULLY");
                            layer = 1;
                            await Task.Delay(2000);
                        }
                    }
                    else
                    {
                        Console.WriteLine("--- CLOSING PROJECT 1: CLIENT ---");
                        running = false;
                    }
                }
                //Employee Layer (View Ticket Options, Create Ticket, Log Out)
                if (layer == 1)
                {
                    Console.Clear();
                    Console.WriteLine("--- EMPLOYEE PAGE ---");
                    Console.WriteLine("Employee: {0}", employee.Name);
                    Console.WriteLine("[1] View Tickets\n[2] Create a Ticket\n[3] Log Out\n");
                    int input = await getUserInputOption(1, 3);
                    if (input == 1)
                    {
                        layer = 2;
                        bool filter = false;
                        string filterString = "";
                        while (layer == 2)
                        {
                            Console.Clear();
                            Console.WriteLine("--- EMPLOYEE PAGE: VIEW TICKETS ---");
                            Console.WriteLine("Employee: {0}", employee.Name);
                            if (!filter)
                            {
                                Console.WriteLine("Showing All Types of Tickets.");
                            } else
                            {
                                Console.WriteLine("Showing only {0} Tickets.", filterString);
                            }
                            Console.WriteLine("[1] View All Tickets\n[2] View Pending Tickets\n[3] View Resolved Tickets\n[4] Select Filter Type\n[5] Exit");
                            input = await getUserInputOption(1, 5);
                            if (input == 1)
                            {
                                //Display All Tickets
                                Console.Clear();
                                List<Ticket> list = await GetAllEmployeeTickets(employee.Id);
                                if (filter)
                                {
                                    List<Ticket> tmp = new List<Ticket>();
                                    foreach (Ticket ticket in list)
                                    {
                                        if (ticket.Type == filterString)
                                        {
                                            tmp.Add(ticket);
                                        }
                                    }
                                    list = tmp;
                                }
                                Console.WriteLine("--- EMPLOYEE PAGE: ALL TICKETS ---");
                                Console.WriteLine("TICKETS: {0}", list.Count);
                                int i = 1;
                                foreach (Ticket t in list)
                                {
                                    Console.WriteLine("[{0}]----------------------", i);
                                    Console.WriteLine("Date Created: {0}, Last Edited: {1}", t.DoC.ToString("MM/dd/yyyy"), t.DoE.ToString("MM/dd/yyyy"));
                                    Console.WriteLine("Approving Manager: {0}", await GetManagerName(employee.Manager_ID));
                                    Console.WriteLine("Amount: ${0}", t.Amount);
                                    Console.WriteLine("Type: {0}", t.Type);
                                    if (t.Pending)
                                    {
                                        Console.WriteLine("Status: PENDING");
                                    }
                                    else if (t.Approved)
                                    {
                                        Console.WriteLine("Status: APPROVED");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Status: DENIED");
                                    }
                                    Console.WriteLine("Description:\n" + t.Description);
                                    Console.WriteLine();
                                    i++;
                                }
                                Console.WriteLine("Press [ENTER] to return back to the EMPLOYEE PAGE: VIEW TICKETS");
                                bool cont = false;
                                while (!cont)
                                {
                                    var key = Console.ReadKey(true);
                                    if (key.Key == ConsoleKey.Enter)
                                    {
                                        cont = true;
                                    }
                                }
                            }
                            else if (input == 2)
                            {
                                //Display Only Pending Tickets
                                Console.Clear();
                                List<Ticket> list = await GetPendingEmployeeTickets(employee.Id);
                                if (filter)
                                {
                                    List<Ticket> tmp = new List<Ticket>();
                                    foreach (Ticket ticket in list)
                                    {
                                        if (ticket.Type == filterString)
                                        {
                                            tmp.Add(ticket);
                                        }
                                    }
                                    list = tmp;
                                }
                                Console.WriteLine("--- EMPLOYEE PAGE: PENDING TICKETS ---");
                                Console.WriteLine("TICKETS: {0}", list.Count);
                                int i = 1;
                                foreach (Ticket t in list)
                                {
                                    Console.WriteLine("[{0}]----------------------", i);
                                    Console.WriteLine("Date Created: {0}, Last Edited: {1}", t.DoC.ToString("MM/dd/yyyy"), t.DoE.ToString("MM/dd/yyyy"));
                                    Console.WriteLine("Approving Manager: {0}", await GetManagerName(employee.Manager_ID));
                                    Console.WriteLine("Amount: ${0}", t.Amount);
                                    Console.WriteLine("Type: {0}", t.Type);
                                    Console.WriteLine("Status: PENDING");
                                    Console.WriteLine("Description:\n" + t.Description);
                                    Console.WriteLine();
                                    i++;
                                }
                                Console.WriteLine("Press [ENTER] to return back to the EMPLOYEE PAGE: VIEW TICKETS");
                                bool cont = false;
                                while (!cont)
                                {
                                    var key = Console.ReadKey(true);
                                    if (key.Key == ConsoleKey.Enter)
                                    {
                                        cont = true;
                                    }
                                }
                            }
                            else if (input == 3)
                            {
                                //Display Only Resolved Tickets
                                Console.Clear();
                                List<Ticket> list = await GetResolvedEmployeeTickets(employee.Id);
                                if (filter)
                                {
                                    List<Ticket> tmp = new List<Ticket>();
                                    foreach (Ticket ticket in list)
                                    {
                                        if (ticket.Type == filterString)
                                        {
                                            tmp.Add(ticket);
                                        }
                                    }
                                    list = tmp;
                                }
                                Console.WriteLine("--- EMPLOYEE PAGE: RESOLVED TICKETS ---");
                                Console.WriteLine("TICKETS: {0}", list.Count);
                                int i = 1;
                                foreach (Ticket t in list)
                                {
                                    Console.WriteLine("[{0}]----------------------", i);
                                    Console.WriteLine("Date Created: {0}, Last Edited: {1}", t.DoC.ToString("MM/dd/yyyy"), t.DoE.ToString("MM/dd/yyyy"));
                                    Console.WriteLine("Approving Manager: {0}", await GetManagerName(employee.Manager_ID));
                                    Console.WriteLine("Amount: ${0}", t.Amount);
                                    Console.WriteLine("Type: {0}", t.Type);
                                    if (t.Approved)
                                    {
                                        Console.WriteLine("Status: APPROVED");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Status: DENIED");
                                    }
                                    Console.WriteLine("Description:\n" + t.Description);
                                    Console.WriteLine();
                                    i++;
                                }
                                Console.WriteLine("Press [ENTER] to return back to the EMPLOYEE PAGE: VIEW TICKETS");
                                bool cont = false;
                                while (!cont)
                                {
                                    var key = Console.ReadKey(true);
                                    if (key.Key == ConsoleKey.Enter)
                                    {
                                        cont = true;
                                    }
                                }
                            }
                            else if (input == 4)
                            {
                                //Select the filter type
                                Console.Clear();
                                Console.WriteLine("--- EMPLOYEE PAGE: FILTER ---");
                                Console.WriteLine("Select the Ticket Type to filter for.");
                                Console.WriteLine("[1] Travel\n[2] Lodging\n[3] Food\n[4] Other\n[5] NONE\n[6] Back");
                                input = await getUserInputOption(1, 6);
                                if (input == 1)
                                {
                                    filterString = "Travel";
                                    filter = true;
                                }
                                else if (input == 2)
                                {
                                    filterString = "Lodging";
                                    filter = true;
                                }
                                else if (input == 3)
                                {
                                    filterString = "Food";
                                    filter = true;
                                }
                                else if (input == 4)
                                {
                                    filterString = "Other";
                                    filter = true;
                                }
                                else if (input == 5)
                                {
                                    filter = false;
                                }
                            }
                            else
                            {
                                //Return back to the Employee Page Home
                                layer = 1;
                            }
                        }
                    }
                    else if (input == 2)
                    {
                        //Create a Ticket
                        Console.Clear();
                        Console.WriteLine("--- EMPLOYEE PAGE: CREATE TICKET ---");
                        Console.WriteLine("Employee: {0}", employee.Name);
                        Console.WriteLine("Enter the Option Number of the Type of Ticket you want to create.");
                        Console.WriteLine("[1] Travel\n[2] Lodging\n[3] Food\n[4] Other\n[5] Quit Creating Ticket");
                        input = await getUserInputOption(1, 5);
                        string type = "";
                        if (input == 1)
                        {
                            type = "Travel";
                        } 
                        else if (input == 2)
                        {
                            type = "Lodging";
                        }
                        else if (input == 3)
                        {
                            type = "Food";
                        }
                        else if (input == 4)
                        {
                            type = "Other";
                        }
                        else
                        {
                            continue;
                        }

                        Console.WriteLine("Enter the Amount to be Reimbursed");
                        double amount = await getUserInputAmount("Amount: $");

                        Console.WriteLine("Enter a Description for your Ticket. (Min 1 Character, Max 500 Characters)");
                        string description = await getUserInputString("Description:\n");
                        while (description.Length == 0 || description.Length > 500)
                        {
                            if (description.Length == 0)
                            {
                                Console.WriteLine("ERROR: DESCRIPTION TOO SHORT");
                                Console.WriteLine("Enter a Description for your Ticket. (Min 1 Character, Max 500 Characters)");
                                description = await getUserInputString("Description:\n");
                            }
                            else
                            {
                                Console.WriteLine("ERROR: DESCRIPTION TOO LONG");
                                Console.WriteLine("Enter a Description for your Ticket. (Min 1 Character, Max 500 Characters)");
                                description = await getUserInputString("Description:\n");
                            }
                        }

                        //Preview Ticket before submission
                        Console.Clear();
                        Console.WriteLine("--- EMPLOYEE PAGE: CREATE TICKET ---");
                        Console.WriteLine("Employee: {0}", employee.Name);
                        Console.WriteLine("--- TICKET CREATION PREVIEW ---");
                        Console.WriteLine("Type: {0}", type);
                        Console.WriteLine("Amount: ${0}", amount);
                        Console.WriteLine("Description:\n{0}", description);
                        Console.WriteLine();
                        Console.WriteLine("Does this look correct?");
                        Console.WriteLine("[1] Yes, Submit Ticket\n[2] No, Exit without submitting");
                        input = await getUserInputOption(1, 2);
                        
                        if (input == 2)
                        {
                            //DO NOT SUBMIT
                            continue;
                        }
                        
                        //Submit 
                        TicketStruct data = new TicketStruct(employee.Id, employee.Manager_ID, type, amount, description);
                        SubmitTicket(data);
                        Console.WriteLine("--- TICKET CREATED ---");
                        Console.WriteLine("Press [ENTER] to continue");
                        bool cont = false;
                        while (!cont)
                        {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Enter)
                            {
                                cont = true;
                            }
                        }
                    }
                    else
                    {
                        //Log Out the Employee
                        layer = 0;
                    }
                }
                //Manager Layer (Ticket Options, View Employees, Log Out)
                if (layer == 2)
                {
                    Console.Clear();
                    Console.WriteLine("--- MANAGER PAGE ---");
                    Console.WriteLine("Manager: {0}", manager.Name);
                    Console.WriteLine("[1] Ticket Options\n[2] Log Out\n");
                    int input = await getUserInputOption(1, 2);
                    if (input == 1)
                    {
                        //Manager Ticket Layer (View Pending Tickets, Resolve Tickets, Exit)
                        layer = 3;
                        while (layer == 3)
                        {
                            Console.Clear();
                            Console.WriteLine("--- MANAGER PAGE: TICKET OPTIONS ---");
                            Console.WriteLine("Manager: {0}", manager.Name);
                            Console.WriteLine("[1] View Pending Tickets\n[2] Resolve Tickets\n[3] Exit\n");
                            input = await getUserInputOption(1, 3);
                            if (input == 1)
                            {
                                Console.Clear();
                                List<Ticket> list = await GetPendingManagerTickets(manager.Id);
                                Console.WriteLine("--- MANAGER PAGE: PENDING TICKETS ---");
                                Console.WriteLine("TICKETS: {0}", list.Count);
                                int i = 1;
                                foreach (Ticket t in list)
                                {
                                    Console.WriteLine("[{0}]----------------------", i);
                                    Console.WriteLine("Date Created: {0}, Last Edited: {1}", t.DoC.ToString("MM/dd/yyyy"), t.DoE.ToString("MM/dd/yyyy"));
                                    string eName = await GetEmployeeName(t.EmployeeId);
                                    Console.WriteLine("Employee: {0}", eName);
                                    Console.WriteLine("Type: {0}", t.Type);
                                    Console.WriteLine("Amount: ${0}", t.Amount);
                                    Console.WriteLine("Description:\n" + t.Description);
                                    Console.WriteLine();
                                    i++;
                                }
                                Console.WriteLine("Press [ENTER] to return back to the MANAGER PAGE: TICKET OPTIONS");
                                bool cont = false;
                                while (!cont)
                                {
                                    var key = Console.ReadKey(true);
                                    if (key.Key == ConsoleKey.Enter)
                                    {
                                        cont = true;
                                    }
                                }
                            }
                            else if (input == 2)
                            {
                                layer = 4;
                                Dictionary<int, string> setTickets = new Dictionary<int, string>();
                                while (layer == 4)
                                {
                                    Console.Clear();
                                    List<Ticket> list = await GetPendingManagerTickets(manager.Id);
                                    Console.WriteLine("--- MANAGER PAGE: RESOLVING TICKETS ---");
                                    Console.WriteLine("TICKETS: {0}", list.Count);
                                    int i = 1;
                                    foreach (Ticket t in list)
                                    {
                                        Console.WriteLine("[{0}]----------------------", i);
                                        Console.WriteLine(" Date Created: {0}, Last Edited: {1}", t.DoC.ToString("MM/dd/yyyy"), t.DoE.ToString("MM/dd/yyyy"));
                                        string eName = await GetEmployeeName(t.EmployeeId);
                                        Console.WriteLine(" Employee: {0}", eName);
                                        Console.WriteLine(" Type: {0}", t.Type);
                                        Console.WriteLine(" Amount: ${0}", t.Amount);
                                        Console.WriteLine("Description:\n" + t.Description);
                                        Console.WriteLine();
                                        i++;
                                    }
                                    Console.WriteLine("Enter the codes to Approve or Deny tickes following the instructions bellow.");
                                    Console.WriteLine("* To set a Ticket to be Approved, type: [Ticket#,A] (with no spaces)");
                                    Console.WriteLine("* To set a Ticket to be Denied, type: [Ticket#,D] (with no spaces)");
                                    Console.WriteLine("* To view all Tickets currently set, type: View");
                                    Console.WriteLine("* To clear all currently set Tickets, type: Clear");
                                    Console.WriteLine("* To submit all currently set Tickets, type: Submit (this will also exit)");
                                    Console.WriteLine("* To Exit, type: Exit");
                                    string inputStr = await getUserInputString("Code: ");
                                    Console.WriteLine();
                                    if (inputStr.ToLower() == "view")
                                    {
                                        //Display
                                        if (setTickets.Count > 0)
                                        {
                                            foreach(KeyValuePair<int, string> entry in setTickets)
                                            {
                                                Console.WriteLine("Ticket [{0}]: {1}", entry.Key, entry.Value);
                                            }
                                        } 
                                        else
                                        {
                                            Console.WriteLine("You have no set tickets.");
                                        }
                                    }
                                    else if (inputStr.ToLower() == "clear")
                                    {
                                        //Clear
                                        setTickets.Clear();
                                        Console.WriteLine("Tickets cleared.");
                                    }
                                    else if (inputStr.ToLower() == "exit")
                                    {
                                        //Exit
                                        layer = 3;
                                        Console.WriteLine("Exiting.");
                                    }
                                    else if (inputStr.ToLower() == "submit")
                                    {
                                        //Submit
                                        foreach (KeyValuePair<int, string> entry in setTickets)
                                        {
                                            if (entry.Value == "APPROVE")
                                            {
                                                int tNum = 1;
                                                foreach (Ticket t in list)
                                                {
                                                    if (tNum == entry.Key)
                                                    {
                                                        ApproveTicket(t);
                                                        break;
                                                    }
                                                    tNum++;
                                                }
                                            }
                                            else
                                            {
                                                int tNum = 1;
                                                foreach (Ticket t in list)
                                                {
                                                    if (tNum == entry.Key)
                                                    {
                                                        DenyTicket(t);
                                                        break;
                                                    }
                                                    tNum++;
                                                }
                                            }
                                        }
                                        layer = 3;
                                        Console.WriteLine("Tickets Submitted. Exiting.");
                                    } 
                                    else if (inputStr.Contains("[") && inputStr.Contains("]"))
                                    {
                                        string tmp = inputStr.ToLower();
                                        char[] chars = tmp.ToCharArray();
                                        for (int c = 0; c < chars.Length; c++)
                                        {
                                            if (chars[c] == '[')
                                            {
                                                int num = chars[c+1] - '0';
                                                if (chars[c+3] == 'a')
                                                {
                                                    if (!setTickets.ContainsKey(num))
                                                    {
                                                        setTickets.Add(num, "APPROVE");
                                                    }
                                                }
                                                else if (chars[c+3] == 'd')
                                                {
                                                    if (!setTickets.ContainsKey(num))
                                                    {
                                                        setTickets.Add(num, "DENY");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Unrecognized command");
                                    }
                                    Console.WriteLine("Press [ENTER] to continue");
                                    bool cont = false;
                                    while (!cont)
                                    {
                                        var key = Console.ReadKey(true);
                                        if (key.Key == ConsoleKey.Enter)
                                        {
                                            cont = true;
                                        }
                                    }
                                }
                                

                            }
                            else
                            {
                                //Go back to Manager page.
                                layer = 2;
                            }
                        }
                    }
                    else
                    {
                        //Log out Manager
                        layer = 0;
                    }
                }
            }
            

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static async Task<int> getUserInputOption(int min, int max)
    {
        string? inputStr = "";
        int? inputInt = 0;

        try
        {
            Console.Write("Please enter an Option Number between {0} and {1}.\nOption: ", min, max);
            inputStr = Console.ReadLine();
            inputInt = Int32.Parse(inputStr);
            while (inputInt == null || (inputInt < min || inputInt > max))
            {
                Console.Write("Please enter an Option Number between {0} and {1}.\nOption: ", min, max);
                inputStr = Console.ReadLine();
                inputInt = Int32.Parse(inputStr);
            }
            return inputInt.Value;
        }
        catch (Exception e)
        {
            return await getUserInputOption(min, max);
        }
    }
    static async Task<string> getUserInputString(string prompt)
    {
        string? input = "";
        Console.Write(prompt);
        input = Console.ReadLine();
        if (input == null)
        {
            return await getUserInputString(prompt);
        }
        return input;
    }
    static async Task<string> getUserInputStringSecure(string prompt)
    {
        string? input = "";
        Console.Write(prompt);
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            input += key.KeyChar;
        }
        Console.WriteLine();

        if (input == null)
        {
            return await getUserInputString(prompt);
        }

        return input;
    }
    static async Task<double> getUserInputAmount(string prompt)
    {
        try
        {
            double? amount = 0;
            Console.Write(prompt);
            string? input = Console.ReadLine();
            while (input == null)
            {
                Console.WriteLine("INVALID AMOUNT: ENTER A POSITIVE NUMBER WITH NO ALPHABETICAL CHARACTERS");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            amount = Double.Parse(input);
            while (amount == null || amount < 0)
            {
                Console.WriteLine("INVALID AMOUNT: ENTER A POSITIVE NUMBER WITH NO ALPHABETICAL CHARACTERS");
                return await getUserInputAmount(prompt);
            }
            return amount.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine("INVALID AMOUNT: ENTER A POSITIVE NUMBER WITH NO ALPHABETICAL CHARACTERS");
            return await getUserInputAmount(prompt);
        }
        
    }
    static async Task<Manager> LoginManager(string name, string password)
    {
        Manager manager = new Manager();
        LoginStruct data = new LoginStruct(name, password);
        HttpResponseMessage response = await client.PostAsJsonAsync("manager/login", data);
        manager = await response.Content.ReadAsAsync<Manager>();
        return manager;
    }
    static async Task<Employee> LoginEmployee(string name, string password)
    {
        Employee employee = new Employee();
        LoginStruct data = new LoginStruct(name, password);
        HttpResponseMessage response = await client.PostAsJsonAsync("employee/login", data);
        employee = await response.Content.ReadAsAsync<Employee>();
        return employee;
    }
    static async Task<Employee> CreateEmployee(string name, string password)
    {
        Employee employee = new Employee();
        EmployeeStruct data = new EmployeeStruct(name, password, 1);
        HttpResponseMessage response = await client.PostAsJsonAsync("employee/create", data);
        employee = await response.Content.ReadAsAsync<Employee>();
        return employee;
    }
    static async Task<List<Ticket>> GetPendingManagerTickets(int id)
    {
        List<Ticket> ticketList = new List<Ticket>();
        HttpResponseMessage response = await client.GetAsync("manager/tickets/pending/" + id);
        if (response.IsSuccessStatusCode)
        {
            ticketList = await response.Content.ReadAsAsync<List<Ticket>>();
        }
        return ticketList;
    }
    static async Task<List<Ticket>> GetResolvedEmployeeTickets(int id)
    {
        List<Ticket> ticketList = new List<Ticket>();
        HttpResponseMessage response = await client.GetAsync("employee/tickets/resolved/" + id);
        if (response.IsSuccessStatusCode)
        {
            ticketList = await response.Content.ReadAsAsync<List<Ticket>>();
        }
        return ticketList;
    }
    static async Task<List<Ticket>> GetPendingEmployeeTickets(int id)
    {
        List<Ticket> ticketList = new List<Ticket>();
        HttpResponseMessage response = await client.GetAsync("employee/tickets/pending/" + id);
        if (response.IsSuccessStatusCode)
        {
            ticketList = await response.Content.ReadAsAsync<List<Ticket>>();
        }
        return ticketList;
    }
    static async Task<List<Ticket>> GetAllEmployeeTickets(int id)
    {
        List<Ticket> ticketList = new List<Ticket>();
        HttpResponseMessage response = await client.GetAsync("employee/tickets/all/" + id);
        if (response.IsSuccessStatusCode)
        {
            ticketList = await response.Content.ReadAsAsync<List<Ticket>>();
        }
        return ticketList;
    }
    static async Task<string> GetManagerName(int id)
    {
        string name = "";
        HttpResponseMessage response = await client.GetAsync("manager/name/"+id);
        if (response.IsSuccessStatusCode)
        {
            name = await response.Content.ReadAsStringAsync();
        }
        return name;
    }
    static async Task<string> GetEmployeeName(int id)
    {
        string name = "";
        HttpResponseMessage response = await client.GetAsync("employee/name/" + id);
        if (response.IsSuccessStatusCode)
        {
            name = await response.Content.ReadAsStringAsync();
        }
        return name;
    }
    static async void SubmitTicket(TicketStruct data)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("employee/tickets/create", data);
    }
    static async void ApproveTicket(Ticket t)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("manager/tickets/approve/", t);
    }
    static async void DenyTicket(Ticket t)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("manager/tickets/deny/", t);
    }
}
