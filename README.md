# Agri-Energy Connect 

## Overview

This prototype web application, "Agri-Energy Connect," is designed to connect the agricultural sector with green energy technology providers. It provides a platform for farmers and employees to manage agricultural products and farmer information.

## Features

* **User Roles:**
    * **Farmer:** Can add, view, edit, and delete their own products.
    * **Employee:** Can manage farmer profiles, view all products, and filter products.
* **Farmer Management:** Employees can add, view, edit, and delete farmer profiles.
* **Product Management:**
    * Farmers can add, view, edit, and delete products associated with their profiles.
    * Employees can view, filter, and search all products.
* **Authentication and Authorization:** Secure login functionality with role-based access control.
* **Data Management:** Product and farmer data is stored in a relational database (SQLite).
* **User Interface:**
    * Responsive design for accessibility across devices.
    * Intuitive navigation and clear data presentation.
    * Bootstrap 5 styling for a modern look.
* **Data Validation:** Server-side and client-side validation to ensure data accuracy.
* **Error Handling:** Robust error handling to prevent system crashes and data corruption.
* **Reporting (Basic):** Employees can filter products by various criteria.
* **Charting (Basic):** Home page displays a product category distribution chart.

## Setup Instructions

1.  **Prerequisites:**
    * Visual Studio (with ASP.NET Core development tools)
    * .NET Core SDK
2.  **Clone the Repository:**
    ```bash
    git clone <https://github.com/MogamatTayyibDawood/Mogamat-Tayyib-Dawood-ST10132915PROG7311-PART2.git>
    cd Agri-Energy-Connect
    ```
3.  **Database Setup:**
    * The application uses SQLite, so no separate database server setup is required. The database file (`AgriEnergyConnect.db`) will be created automatically.
4.  **Build and Run:**
    * Open the project in Visual Studio.
    * Build the solution.
    * Run the application (e.g., by pressing F5).
5.  **Initial Setup:**
    * The database is seeded with initial data, including an admin user (`admin@agrienergy.com`, `Admin@123`), a farmer user (`farmer@example.com`, `Farmer@123`), and sample farmer and product data.
    * To register new users, navigate to the register page. The first registered user will need to be assigned the 'Employee' role.

## Functionality Overview

### User Roles

* **Farmer:**
    * Login to the system.
    * Navigate to the "Products" section.
    * Add new products with details like name, category, and production date.
    * View, edit, and delete their own product listings.
* **Employee:**
    * Login to the system.
    * Navigate to the "Farmers" section to manage farmer profiles.
    * Navigate to the "Products" section to view and filter products.
    * Use the filter functionality to search products by farmer, category, and date range.

### Key Features

* **Dashboard:** Provides a summary of key data, including farmer and product counts, and a product category distribution chart (for employees).
* **Farmer Management:** Employees can create, view, edit, and delete farmer profiles. Farmer details include name, email, and contact number.
* **Product Management:**
    * Farmers can add, edit, and delete products. Product details include name, category, production date, and associated farmer.
    * Employees can view a list of all products, filter products by farmer, category, and date range, and search for products.
* **Filtering and Sorting:** Employees can filter products based on various criteria and sort product lists by name, category, or production date.
* **Registration:** New users can register with either "Farmer" or "Employee" roles. Farmers are required to provide additional information (name, contact number).

## Known Issues

* Activity tracking is not fully implemented.
* Advanced reporting features are not included in this prototype.

## Future Improvements

* Implement a more comprehensive reporting module.
* Add functionality for managing green energy solutions.
* Enhance user interface with more advanced features.

## Testing

* **User Authentication:**
    * Verify that users can register with both "Farmer" and "Employee" roles.
    * Ensure that farmers are prompted for their name and contact number during registration.
    * Test login functionality with valid and invalid credentials.
    * Confirm that users are redirected to the appropriate pages after login.
    * Test logout functionality.
* **Farmer Management (Employee Role):**
    * Test the creation of new farmer profiles.
    * Verify that farmer details can be viewed, edited, and deleted.
    * Ensure that validation is enforced for farmer data (e.g., email format, contact number format).
    * Test the farmer search functionality.
* **Product Management:**
    * **Farmer Role:**
        * Test the creation of new products.
        * Verify that product details can be viewed, edited, and deleted.
        * Ensure that farmers can only manage products associated with their profiles.
    * **Employee Role:**
        * Test the viewing of all products.
        * Verify the filtering of products by farmer, category, and date range.
        * Test the product search functionality.
* **User Interface:**
    * Test the responsiveness of the application on different devices.
    * Verify that the navigation is intuitive and user-friendly.
    * Ensure that data is displayed clearly and accurately.
* **Data Validation:**
    * Test all form fields with valid and invalid data to ensure validation is working correctly.
    * Verify that appropriate error messages are displayed for invalid input.
* **Error Handling:**
    * Test scenarios that might cause errors (e.g., database connection issues, invalid input) and verify that the application handles them gracefully.
    * Ensure that user-friendly error messages are displayed.

    ## Refernces
  AI was used to aid in undersatnding the code and exploring certian techniques
    OpenAI (2025). ChatGPT. [online] ChatGPT. Available at: https://chatgpt.com/.

Andy Runciman (2011). Part 4 - Creating the add product form 1 of 2. [online] YouTube. Available at: https://www.youtube.com/watch?v=qZP8Nqw2_qU [Accessed 14 May 2025].

‌Raza, R. (2020). Design a Beautiful Flat Dashboard UI UX for Desktop Application in C# WinForm’s using Visual Studio. [online] Medium. Available at: https://medium.com/@riazraza0/designing-a-beautiful-flat-dashboard-ui-ux-for-desktop-application-in-c-winforms-using-visual-408f4dcb4d81 [Accessed 14 May 2025].

‌C-sharpcorner.com. (2020). Insert Data Into The Database In Windows Form Using C#. [online] Available at: https://www.c-sharpcorner.com/UploadFile/009464/insert-data-into-database-in-window-form-using-C-Sharp/.

‌SQLitebrowser (2019). DB Browser for SQLite. [online] Sqlitebrowser.org. Available at: https://sqlitebrowser.org/.

‌TechEngineerSchool (2024). How to Use DB Browser for SQLite (Absolute Beginners Guide). [online] YouTube. Available at: https://www.youtube.com/watch?v=fPWiZhVjvIU.

‌
‌

## Conclusion

This prototype provides a solid foundation for the Agri-Energy Connect platform. It demonstrates the core functionality for managing farmers and products and provides a user-friendly interface. Further development can build upon this foundation to create a comprehensive solution for connecting the agricultural sector with green energy solutions.
