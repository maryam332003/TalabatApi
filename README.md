##Description:
A RESTful API built with .NET Core for managing an online food ordering and delivery system, providing functionalities for users, restaurants, and orders.

##Technologies Used:
.NET Core
Entity Framework Core
SQL Server
AutoMapper
JWT Authentication
Swagger
SignalR

##Key Features:
###User Management:

####Registration and Authentication: Users can register and authenticate using JWT tokens.
Profile Management: Users can view and update their profiles.
Roles and Permissions: Implementation of user roles such as customer, restaurant owner, and admin with specific permissions for each role.
Restaurant Management:

####Restaurant CRUD Operations: Restaurant owners can create, read, update, and delete their restaurant profiles.
Menu Management: Restaurant owners can manage their menus, including adding, updating, and removing items.
Order Management:

####Order Placement: Customers can place orders from different restaurants.
Order Tracking: Real-time tracking of order status and notifications using SignalR.
Order History: Users can view their past orders and reorder items.
Real-time Notifications:

####Order Status Updates: Real-time notifications to customers about their order status changes using SignalR.
Email Notifications: Automatic email notifications to customers upon order status changes.
Search and Filtering:

####Restaurant Search: Users can search for restaurants based on various criteria such as location, cuisine, and ratings.
Menu Item Search: Users can search for specific menu items across different restaurants.
API Documentation:

####Swagger Integration: Comprehensive API documentation and testing using Swagger UI.
Contributions and Impact:

Developed a scalable and maintainable API following RESTful principles and best practices.
Implemented robust authentication and authorization mechanisms to ensure secure access to resources.
Enhanced user experience with real-time notifications and updates using SignalR.
Streamlined order management process by integrating comprehensive CRUD operations for restaurants and orders.
Facilitated seamless API testing and integration with well-documented endpoints using Swagger.
