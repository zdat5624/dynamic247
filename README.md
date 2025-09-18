# Dynamic247 - Online News System

## Features
- User:
  - Register/Login with email, OTP, and JWT authentication.
  - Manage profile information and reset password.
  - View, search, filter, and save articles.
  - Comment and reply under articles.
  - Follow favorite topics for personalized news feed.

- Editor:
  - Create, edit, delete, and manage articles (text, images, videos).
  - Manage reader comments.
  - Save drafts and submit articles for approval.

- Administrator:
  - Approve, reject, or hide articles.
  - Manage user and editor accounts.
  - View statistics: total views, comments, editor performance, user activity.

## How to Run Backend Server
1. Install prerequisites:
   - .NET 8 SDK
   - Microsoft SQL Server
   - Node.js (if running frontend separately)

2. Clone the project:
git clone <repository-url>
cd Dynamic247/Backend


3. Configure the database:
- Update the connection string in `appsettings.json` with your SQL Server credentials.
- Run database migrations:
  ```
  dotnet ef database update
  ```

4. Run the backend server:
dotnet run
