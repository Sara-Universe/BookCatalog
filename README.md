# Book Catalog BackEnd

## Project Overview
The **Book Catalog BackEnd** is a RESTful API built with **.NET Core Web API** that manages a catalog of books.  
The application loads book data from a **CSV file** at startup, allows querying and manipulation using **LINQ**, and persists changes (add/update/delete) back to the CSV file.

## Features
- **Get all books**
- **Filter books** by author, genre, or published year
- **Sort books** by price or year
- **Pagination support** for large datasets
- **Add new books**
- **Update existing books**
- **Delete books**
- **Input validation** using DTOs and Data Annotations


## Algorithm / Workflow

1. **Startup**
   - Read data from `book.csv` into memory.
   - Map CSV records into `Book` model objects.

2. **Data Access & Processing**
   - Store all books in an in-memory list (`List<Book>`).
   - Use **LINQ** for filtering, sorting, and grouping.

3. **Validation**
   - Incoming requests use `BookDTO` objects with validation attributes (`[Required]`, `[Range]`, etc.).
   - If validation fails → return error response.
   - If validation succeeds → map DTO → `Book` model.

4. **CRUD Operations**
   - **GET** `/books`
     - Retrieve all books with optional filters (author, genre, year).
     - Apply pagination (`pageNumber`, `pageSize`).
   - **GET** `/books/{id}`
     - Retrieve a book by its unique ID.
   - **POST** `/books`
     - Validate request body (DTO).
     - Auto-generate new `BookID`.
     - Add to in-memory list and append to CSV.
   - **PUT** `/books/{id}`
     - Find existing book by ID.
     - Validate new data via DTO.
     - Replace old data → update in-memory list.
     - Rewrite CSV file with updated list.
   - **DELETE** `/books/{id}`
     - Remove book from in-memory list.
     - Rewrite CSV file without the deleted book.

5. **Persistence**
   - **CsvHelper** library is used for reading/writing the CSV file.
   - For updates/deletes → rewrite the entire file from the updated list.
   - For insert → append new record.


## 🛠️ Tools & Technologies
- **.NET Core Web API**
- **LINQ** for queries
- **CsvHelper (v33.1)** for CSV file reading/writing
- **AutoMapper** for mapping between `Book` (model) and `BookDTO` (DTO with validation)
- **Swagger/OpenAPI** for API documentation & testing

## 📂 Example Endpoints
- `GET /api/books?pageNumber=1&pageSize=10`
- `GET /api/books?author=Orwell&genre=Dystopian`
- `GET /api/books/5`
- `POST /api/books`
- `PUT /api/books/5`
- `DELETE /api/books/5`


## API Flow Example
```plaintext
Client Request → Controller → Service → LINQ Query → Validation → Mapping → Update In-Memory List → Rewrite/Append CSV → Response
