This Project is All about Reddit clone .

Features
1. User Authentication and Authorization
Description: Implemented user authentication and authorization using JSON Web Tokens (JWT) and OAuth 2.0. Users can sign up, log in, and access protected routes based on their role (e.g., admin, regular user).
Technologies Used:
JSON Web Tokens (JWT)
OAuth 2.0
ASP.NET Core Identity
Endpoints:
![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/c8d363b9-a8fa-4fb5-95cc-68a3432c8e97)

2. CRUD Operations on Blog Posts
Description: Implemented CRUD (Create, Read, Update, Delete) operations for managing blog posts. Users can create new blog posts, view existing posts, update their own posts, and delete posts.
Technologies Used:
ASP.NET Core Web API
Entity Framework Core
SQL Server (or any other database provider supported by Entity Framework Core)
Endpoints:
![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/abb948f7-a0c1-4542-bde0-ea0700944b38)
![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/e10ad2f1-6ec8-41c3-af35-fbb60c5ff347)
3. CRUD Operations on Comments
Description: Implemented CRUD operations for managing comments on blog posts. Users can add new comments, edit their own comments, delete comments, and view comments associated with specific blog posts.
Technologies Used:
ASP.NET Core Web API
Entity Framework Core
SQL Server (or any other database provider supported by Entity Framework Core)
Endpoints:

![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/af663722-c272-4346-8634-daa2cac9121f)

4. Reaction System
Description: Implemented a reaction system where users can react to blog posts and comments. Supported reactions include upvotes and downvotes.
Technologies Used:
ASP.NET Core Web API
Entity Framework Core
SQL Server (or any other database provider supported by Entity Framework Core)
Endpoints:
![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/3a2b2110-0e9a-4981-9353-1c9f1f343ea5)
![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/e356e152-977c-414e-9d81-1f065901cdec)
5. User Profile Management
Description: Implemented user profile management functionalities. Users can view their profile information, update their profile details, and upload a profile picture.
Technologies Used:
ASP.NET Core Web API
Entity Framework Core
SQL Server (or any other database provider supported by Entity Framework Core)
Endpoints:

![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/08ce49f5-8f74-4e98-8cdc-c2ed6fdabe7f)

6. Real-Time Push Notifications
Description: Implemented real-time push notifications using SignalR to provide instant updates to users without the need for manual page refresh. Users receive notifications when new blog posts are created, comments are added, or reactions are made.
Technologies Used:
SignalR
ASP.NET Core Web API
Functionality:
Blog Post Notifications: Users receive notifications in real-time when new blog posts are created.
Comment Notifications: Users are notified instantly when new comments are added to blog posts.
Reaction Notifications: Users receive immediate updates when reactions (upvotes/downvotes) are made on blog posts or comments.
Implementation Details:
SignalR Hub: A SignalR hub is implemented on the server-side to manage connections and broadcast notifications to connected clients.
Client Integration: The client-side application establishes a SignalR connection and listens for notifications. When a notification is received, it updates the UI dynamically without requiring a page reload.
Endpoints:
N/A (SignalR operates over WebSocket connections and does not rely on traditional HTTP endpoints)

![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/33a62238-3ece-408d-8b90-b812876edc2d)
7. Dashboard FOr admin

Description:Admin can see all comment count all reaction and  blog post count.In this dasboard admin can see all the details of blog post on the basis of month and year of total count too,

![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/2b4390d6-6225-4b5d-828b-73ee2934d770)

![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/7c1dacd4-bc76-4e8e-b033-52834c70ade0)

![image](https://github.com/bhandarimanoj612/BisleriumAspDotnet-CleanArchitectureWebApi/assets/105379940/4262cda9-f58d-4312-81f3-d16655f5ad75)

